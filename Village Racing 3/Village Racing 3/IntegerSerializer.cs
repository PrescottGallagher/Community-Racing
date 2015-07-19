using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Village_Racing_3
{
    public static class IntegerSerializer<T>
    {
        public static byte[] serialize(UnlimitedMatrix<T> data)
        {
            List<byte[]> contents = new List<byte[]>();
            ulong size = 0;
            long value;
            long current_value = 0;
            List<long[]> current_stack = new List<long[]>();
            long[] previous = new long[] { 64, 64 };
            foreach (long[] coords in data)
            {
                value = convert_from(data[coords[0], coords[1]]);
                if (value == current_value)
                {
                    current_stack.Add(coords);
                    continue;
                }
                if (current_value != 0) write_stack(contents, ref size, current_stack, current_value, ref previous);
                current_value = value;
                current_stack = new List<long[]>();
                current_stack.Add(coords);
            }
            if (current_value != 0) write_stack(contents, ref size, current_stack, current_value, ref previous);
            current_stack = null;
            byte[] result = new byte[size + 1];
            ulong current, pointer = 0;
            for (current = 0; current < (ulong)contents.Count; current++)
            {
                Array.Copy(contents[(int)current], 0L, result, (long)pointer, contents[(int)current].LongLength);
                pointer += (ulong)contents[(int)current].LongLength;
            }
            result[pointer] = 0;
            return result;
        }


        private static void write_stack(List<byte[]> contents, ref ulong size, List<long[]> stack, long value, ref long[] prev)
        {
            if (stack.Count < 4)
                write_stack_single(contents, ref size, stack, value, ref prev);
            else
                write_stack_run(contents, ref size, stack, value, ref prev);
        }

        private static void write_stack_single(List<byte[]> contents, ref ulong size, List<long[]> stack, long value, ref long[] prev)
        {
            byte[] serialized_value = encode_number(value, false);
            foreach (long[] coords in stack)
            {
                push(contents, ref size, serialized_value);
                push_coordinates(contents, ref size, coords, ref prev);
            }
        }

        private static void write_stack_run(List<byte[]> contents, ref ulong size, List<long[]> stack, long value, ref long[] prev)
        {
            push(contents, ref size, encode_number(value, true));
            foreach (long[] coords in stack) push_coordinates(contents, ref size, coords, ref prev);
            push(contents, ref size, new byte[] { 0x40, 0x40 });
        }

        private static void push_coordinates(List<byte[]> contents, ref ulong size, long[] coordinates, ref long[] previous)
        {
            push(contents, ref size, create_coordinate(coordinates[0], ref previous[0]));
            push(contents, ref size, create_coordinate(coordinates[1], ref previous[1]));
        }

        private static byte[] create_coordinate(long coordinate, ref long reference)
        {
            byte[] absolute = encode_number(coordinate);
            byte[] relative = encode_number(coordinate, reference);
            reference = coordinate;
            if (relative.Length < absolute.Length) return relative;
            return absolute;
        }

        private static void push(List<byte[]> contents, ref ulong size, byte[] item)
        {
            contents.Add(item);
            size += (ulong)item.LongLength;
        }

        public static UnlimitedMatrix<T> unserialize(byte[] data, ref ulong pointer)
        {
            UnlimitedMatrix<T> result = new UnlimitedMatrix<T>();
            long[] previous_coordinates = new long[] { 64, 64 };
            while (data[pointer] != 0)
                if ((data[pointer] & 0x40) != 0)
                    read_run(result, data, ref pointer, ref previous_coordinates);
                else
                    read_single(result, data, ref pointer, ref previous_coordinates);
            pointer++;
            return result;
        }

        private static void read_single(UnlimitedMatrix<T> dst, byte[] src, ref ulong pointer, ref long[] previous)
        {
            bool kind;
            int advance;
            long value = decode_number(src, pointer, out kind, out advance);
            if (kind) throw new InvalidOperationException();
            pointer += (ulong)advance;
            long[] coordinates = read_coordinates(src, ref pointer, ref previous);
            dst[coordinates[0], coordinates[1]] = convert_to(value);
        }

        private static void read_run(UnlimitedMatrix<T> dst, byte[] src, ref ulong pointer, ref long[] previous)
        {
            bool kind;
            int advance;
            long value = decode_number(src, pointer, out kind, out advance);
            T converted = convert_to(value);
            if (!kind) throw new InvalidOperationException();
            pointer += (ulong)advance;
            long[] coordinates;
            while ((src[pointer] != 0x40) || (src[pointer + 1] != 0x40))
            {
                coordinates = read_coordinates(src, ref pointer, ref previous);
                dst[coordinates[0], coordinates[1]] = converted;
            }
            pointer += 2;
        }

        private static long[] read_coordinates(byte[] src, ref ulong pointer, ref long[] previous)
        {
            long[] result = new long[2];
            result[0] = read_coordinate(src, ref pointer, ref previous[0]);
            result[1] = read_coordinate(src, ref pointer, ref previous[1]);
            return result;
        }

        private static long read_coordinate(byte[] src, ref ulong pointer, ref long previous)
        {
            bool relative;
            int advance;
            long coordinate = decode_number(src, pointer, out relative, out advance);
            pointer += (ulong)advance;
            if (relative) coordinate += previous;
            previous = coordinate;
            return coordinate;
        }

        public static UnlimitedMatrix<T> unserialize(byte[] data, ulong pointer)
        {
            return unserialize(data, ref pointer);
        }

        public static UnlimitedMatrix<T> unserialize(byte[] data)
        {
            ulong pointer = 0;
            return unserialize(data, ref pointer);
        }

        private static byte[] encode_number(long number, bool kind)
        {
            bool sign = number < 0;
            if (sign) number = ~number;
            int length;
            if (number < 0x20L)
                length = 1;
            else if (number < 0x1000L)
                length = 2;
            else if (number < 0x80000L)
                length = 3;
            else if (number < 0x4000000L)
                length = 4;
            else if (number < 0x200000000L)
                length = 5;
            else if (number < 0x10000000000L)
                length = 6;
            else if (number < 0x800000000000L)
                length = 7;
            else
                length = 9;
            byte[] result = new byte[length];
            long available_bits = 0x887777775L;
            int current, bits;
            for (current = 0; current < length; current++)
            {
                bits = (int)(available_bits & 15);
                available_bits >>= 4;
                result[current] = (byte)((((bits == 8) || (length == (current + 1))) ? 0 : 0x80) | ((byte)number & ((1 << bits) - 1)));
                number >>= bits;
            }
            if (kind) result[0] |= 0x40;
            if (sign) result[0] |= 0x20;
            return result;
        }

        private static byte[] encode_number(long number, long reference)
        {
            return encode_number(number - reference, true);
        }

        private static byte[] encode_number(long number)
        {
            return encode_number(number, false);
        }

        private static long decode_number(byte[] data, ulong position, out bool kind, out int length)
        {
            for (length = 0; length < 7; length++) if ((data[position + (ulong)length] & 0x80) == 0) break;
            if (length == 7)
                length = 9;
            else
                length++;
            if ((ulong)data.Length < (position + (ulong)length)) throw new IndexOutOfRangeException();
            long available_bits = 0x887777775L;
            int current, bits, shift_state = 0;
            long result = 0;
            for (current = 0; current < length; current++)
            {
                bits = (int)(available_bits & 15);
                available_bits >>= 4;
                result |= (((long)data[position + (ulong)current]) & ((1L << bits) - 1)) << shift_state;
                shift_state += bits;
            }
            kind = (data[position] & 0x40) != 0;
            if ((data[position] & 0x20) != 0) result = ~result;
            return result;
        }

        private static long decode_number(byte[] data, ulong position, long reference, out int length)
        {
            bool relative;
            long rv = decode_number(data, position, out relative, out length);
            if (relative) rv += reference;
            return rv;
        }

        private static long convert_from(T value)
        {
            return (long)Convert.ChangeType(value, typeof(long));
        }

        private static T convert_to(long value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}

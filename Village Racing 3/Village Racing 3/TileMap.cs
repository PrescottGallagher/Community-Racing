using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;


namespace Village_Racing_3
{
    class TileMap
    {
        int offset;
        int scrollTile = 1;
        int maxScrollTile = 20;
        int minScrollTile = 0;
        Texture2D tileSheet;
        enum Levels {Ship, Outside};
        public UnlimitedMatrix<int> levelOne = new UnlimitedMatrix<int>();
        public UnlimitedMatrix<Block> blocks = new UnlimitedMatrix<Block>();
        public Vector2 StartingPoint;
        public Vector2 EndingPoint;
        MouseState lastState;
        Camera camera;
        Texture2D selector;
        MouseState MS;


        public TileMap(Texture2D tiles, Camera camera, Texture2D selector)
        {
            tileSheet = tiles;
            this.camera = camera;
            this.selector = selector;
        }

        public void SetTile(int x, int y, int type)
        {
            levelOne[x, y] = type;
            blocks[x, y] = new Block(tileSheet, new Vector2(0 + (65 * type), 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if(type == 2)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(65, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 3)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(130, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 4)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(195, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 5)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(260, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 6)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(325, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 7)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(390, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 8)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(455, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 9)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(520, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 10)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(585, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 11)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(650, 0), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 12)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(0, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 13)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(65, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 14)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(130, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 15)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(195, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 16)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(260, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 17)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(325, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 18)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(390, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 19)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(455, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 20)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(520, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //if (type == 21)
            //    blocks[x, y] = new Block(tileSheet, new Vector2(585, 65), new Vector2(x * Block.BLOCKWIDTH, y * Block.BLOCKHEIGHT));
            //blocks[x, y] = new Rectangle(x * 64, y * 64, 64, 64);
        }

        public void toLevel()
        {
            using (StreamReader streamReader = new StreamReader("level.txt"))
            {
                for(int Y = 0; Y != File.ReadLines("level.txt").Count(); Y++)
                {
                    string line = streamReader.ReadLine();
                    string[] numbers = line.Split(',');

                    for (int X = 0; X != numbers.Length; X++)
                    {
                        int tile = int.Parse(numbers[X]);
                        SetTile(X, Y, tile);
                    }
                }
            }
        }

        public void Update()
        {
            if ((Mouse.GetState().X / Block.BLOCKWIDTH) < 0 || (Mouse.GetState().Y / Block.BLOCKHEIGHT) < 0)
                offset = Block.BLOCKWIDTH;
            else
                offset = 0;

            StartingPoint = new Vector2(camera.Position.X - Block.BLOCKWIDTH, camera.Position.Y - Block.BLOCKHEIGHT);
            EndingPoint = new Vector2(StartingPoint.X + camera._viewport.Width + (Block.BLOCKWIDTH * 3), StartingPoint.Y + camera._viewport.Height + (Block.BLOCKHEIGHT * 3));
            EndingPoint /= Block.BLOCKHEIGHT;
            StartingPoint /= Block.BLOCKHEIGHT;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed /* && lastState.LeftButton == ButtonState.Pressed*/)
            {
                SetTile((int)((Mouse.GetState().X + camera.Position.X + offset) / Block.BLOCKWIDTH), (int)((Mouse.GetState().Y + camera.Position.Y + offset) / Block.BLOCKHEIGHT), scrollTile);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed /*&& lastState.RightButton == ButtonState.Pressed*/)
            {
                SetTile((int)((Mouse.GetState().X + camera.Position.X) / Block.BLOCKWIDTH), (int)((Mouse.GetState().Y + camera.Position.Y) / Block.BLOCKHEIGHT), 0);
            }

            if(lastState.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                if (scrollTile < maxScrollTile)
                {
                    scrollTile++;
                }
            if (lastState.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                if (scrollTile > minScrollTile)
                {
                    scrollTile--;
                }


            lastState = Mouse.GetState();

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = (int)StartingPoint.Y; y != (int)EndingPoint.Y; y++)
            {
                for (int x = (int)StartingPoint.X; x != (int)EndingPoint.X; x++)
                {
                    try
                    {
                        if (levelOne[x, y] > 0)
                        {
                            //spriteBatch.Draw(Metal, new Rectangle((x * 64) + 0, y * 64, 64, 64), Color.White);
                            if (blocks[x, y] != null)
                                blocks[x, y].Draw(spriteBatch);
                        }
                    }
                    catch
                    {
                        //Nothing?
                    }
                }
            }
            
        }

        public void DrawForeGround(SpriteBatch sb)
        {
            Block myBlock;
            Vector2 position;
            sb.Draw(selector, new Rectangle(camera._viewport.Width - 200, 0, 200, 200), Color.White);
            if(scrollTile != minScrollTile)
                new Block(tileSheet, new Vector2(0 + (65 * (scrollTile - 1)), 0), new Vector2(camera._viewport.Width - 100, 0)).DrawLarge(sb);
            new Block(tileSheet, new Vector2(0 + (65 * (scrollTile)), 0), new Vector2(camera._viewport.Width - 100, 66)).DrawLarge(sb);
            if(scrollTile != maxScrollTile)
            new Block(tileSheet, new Vector2(0 + (65 * (scrollTile + 1)), 0), new Vector2(camera._viewport.Width - 100, 131)).DrawLarge(sb);
        }
    }
}

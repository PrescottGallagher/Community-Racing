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
        MouseState downState;
        bool isFirstFrame = false;


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
        }

        public void tileEmpty(int x, int y, string type)
        {
 
        }

        public void toLevel()
        {
            //using (StreamReader streamReader = new StreamReader("level.dat"))
            //{
            //    try
            //    {
            //        for (int Y = 0; Y != File.ReadLines("level.txt").Count(); Y++)
            //        {
            //            string line = streamReader.ReadLine();
            //            string[] numbers = line.Split(',');

            //            for (int X = 0; X != numbers.Length; X++)
            //            {
            //                int tile = int.Parse(numbers[X]);
            //                SetTile(X, Y, tile);
            //            }
            //        }
            //    }
            //    catch
            //    {
 
            //    }
            //}

            string filename = "level.dat";
              try {
                byte[] data = File.ReadAllBytes(filename);
                levelOne = IntegerSerializer<int>.unserialize(data);
                foreach (long[] coords in levelOne)
                  SetTile((int)coords[0], (int)coords[1], (int)levelOne[coords[0], (int)coords[1]]);
              } catch (Exception e) {
                // some error message here
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
                if(Mouse.GetState().X == downState.X && Mouse.GetState().Y == downState.Y)
                    SetTile((int)((Mouse.GetState().X + camera.Position.X) / Block.BLOCKWIDTH), (int)((Mouse.GetState().Y + camera.Position.Y) / Block.BLOCKHEIGHT), scrollTile);
                else
                    if(levelOne[(int)((Mouse.GetState().X + camera.Position.X) / Block.BLOCKWIDTH), (int)(Mouse.GetState().Y + camera.Position.Y) / Block.BLOCKHEIGHT] < 1)
                        SetTile((int)((Mouse.GetState().X + camera.Position.X) / Block.BLOCKWIDTH), (int)((Mouse.GetState().Y + camera.Position.Y) / Block.BLOCKHEIGHT), scrollTile);
            }
            else if (Mouse.GetState().RightButton == ButtonState.Pressed /*&& lastState.RightButton == ButtonState.Pressed*/)
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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                if (isFirstFrame)
                {
                    downState = Mouse.GetState();
                    isFirstFrame = false;
                }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                isFirstFrame = true;


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

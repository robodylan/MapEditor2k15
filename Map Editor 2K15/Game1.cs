using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Map_Editor_2K15
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D mapTexture;
        Texture2D originTexture;
        Texture2D cursorTexture;
        Texture2D overlayTexture;
        int currentID;
        List<Block> map;
        Vector2 lastRightClick;
        Vector2 offset;
        SpriteFont font;
        int tileSize = 16;
        int tileScale = 5;
        bool toggleDown = false;
        bool tabDown = false;
        bool scaleDownMinus = false;
        bool scaleDownPlus = false;
        bool showControls = true;

        string spriteSheetName;
        System.Windows.Forms.SaveFileDialog saveFileDialog; 
        System.Windows.Forms.OpenFileDialog openFileDialog;

        int lastScrollValue;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "Choose where to export map";
            saveFileDialog.FileName = "export.map";
            saveFileDialog.Filter = "Map |";
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "Choose spritesheet to open";
            graphics.PreferredBackBufferHeight = ((int)(GraphicsDevice.Adapter.CurrentDisplayMode.Height * 0.75f));
            graphics.PreferredBackBufferWidth = ((int)(GraphicsDevice.Adapter.CurrentDisplayMode.Width * 0.75f));
            Window.AllowAltF4 = true;
            Window.Position = new Point(0,0);
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.ApplyChanges();
            IsMouseVisible = false;
            map = new List<Block>();
            lastRightClick = new Vector2(-10,-10);
            offset = new Vector2(0, 0);
            base.Initialize();
            lastScrollValue = 0;
        }


        protected override void LoadContent()
        {
            cursorTexture = Content.Load<Texture2D>("cursor");
            mapTexture = Content.Load<Texture2D>("spritesheet");
            originTexture = Content.Load<Texture2D>("origin");
            overlayTexture = Content.Load<Texture2D>("overlay");
            font = Content.Load<SpriteFont>("font");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float scrollSpeed = 5f;
            if (Keyboard.GetState().IsKeyDown(Keys.I)) Import();
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) scrollSpeed = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && offset.Y < 0) offset.Y += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) offset.Y -= scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.A) && offset.X < 0) offset.X += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) offset.X -= scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) Save("test.map");
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !toggleDown) showControls = !showControls; toggleDown = true;
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) && tileScale < 16 && !scaleDownPlus) tileScale++; scaleDownPlus = true;
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) && tileScale > 1 && !scaleDownMinus) tileScale--; scaleDownMinus = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && !tabDown)
            {
                tabDown = true;
                int tileSizeNum = (int)Math.Sqrt(tileSize);
                tileSizeNum++;
                tileSize = (int)Math.Pow(2, tileSizeNum);
            }
            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / (tileSize * tileScale), ((int)(Mouse.GetState().Y - offset.Y) / (tileScale * tileSize)) - 1);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                bool canPlace = true;
                foreach(Block block in map)
                {
                    if (block.getPosition() == mouseClickPosition) canPlace = false;
                }
                if (canPlace && mouseClickPosition.X >= 0 && mouseClickPosition.Y >= 0)
                {
                    map.Add(new Block((int)mouseClickPosition.X, (int)mouseClickPosition.Y, currentID, true));
                }
            }
            if(Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                lastRightClick = mouseClickPosition;
               
            }

            if(Keyboard.GetState().IsKeyDown(Keys.O))
            {
                openFileDialog.ShowDialog();
                string filepath = openFileDialog.FileName;
                spriteSheetName = filepath;
                mapTexture = Texture2D.FromStream(GraphicsDevice,new StreamReader(filepath).BaseStream);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.H)) toggleDown = false;
            if (Keyboard.GetState().IsKeyUp(Keys.Tab)) tabDown = false;
            if (Keyboard.GetState().IsKeyUp(Keys.OemMinus)) scaleDownMinus = false;
            if (Keyboard.GetState().IsKeyUp(Keys.OemPlus)) scaleDownPlus = false;
            base.Update(gameTime);
            int currentScrollValue = Mouse.GetState().ScrollWheelValue;
            if (lastScrollValue < currentScrollValue)
            {
                currentID++;
            }
            if (lastScrollValue > currentScrollValue)
            {
                currentID--;
            }
            lastScrollValue = currentScrollValue;
            if(currentID < 0)
            {
                currentID = (mapTexture.Width * mapTexture.Height) / (tileScale * tileSize);
            }
            if(currentID > (mapTexture.Width * mapTexture.Height) / (tileScale * tileSize) && mapTexture != null)
            {
                currentID = 0;
            }
            if(tileSize > 64 || tileSize > mapTexture.Width || tileSize > mapTexture.Height)
            {
                tileSize = 16;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateTranslation(offset.X,offset.Y,0));
            Block blockToRemove = new Block(0, 0, 0, true);
            foreach(Block block in map)
            {
                if (block.getPosition() == lastRightClick)
                {
                    blockToRemove = block;
                }
                if (mapTexture != null)
                {
                    spriteBatch.Draw(mapTexture, new Vector2((int)block.getPosition().X * (tileSize * tileScale), (int)block.getPosition().Y * (tileScale * tileSize)), new Rectangle((block.getID() % (mapTexture.Width / tileSize)) * tileSize, block.getID() / (mapTexture.Height / tileSize) * tileSize, tileSize, tileSize), Color.White, 0, new Vector2(0, 0), tileScale, SpriteEffects.None, 1);
                }
            }
            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / (tileScale * tileSize), (int)((Mouse.GetState().Y - offset.Y) / (tileSize * tileScale)) - 1);
            spriteBatch.Draw(originTexture, new Vector2(0,0), null, new Color(255, 255, 255, 128), 0, new Vector2(0,0), tileScale * (tileSize / 16), SpriteEffects.None, 1);
            spriteBatch.Draw(cursorTexture, new Vector2(mouseClickPosition.X * (tileSize * tileScale), mouseClickPosition.Y * (tileSize * tileScale)), null, new Color(255,255,255,128), 0, new Vector2(0,0), tileScale * (tileSize / 16), SpriteEffects.None, 1);
            spriteBatch.Draw(overlayTexture, new Vector2((GraphicsDevice.DisplayMode.Width * 0.11f) - offset.X, 0 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Current Tile: ", new Vector2(((GraphicsDevice.DisplayMode.Width * 0.11f) + 325) - offset.X, 30 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Block ID: " + currentID, new Vector2(((GraphicsDevice.DisplayMode.Width * 0.11f) + 325) - offset.X, 60 - offset.Y), new Color(255, 255, 255, 128));
            if (showControls) spriteBatch.DrawString(font, "Controls\n\nE = Export map\nI = Import Map\nO = Open spritesheet\nN = New map\nW = Pan Up\nA = Pan Left\nS = Pan Down\nD = Pan Right\nH = Hide Controls\nTab = Change Tile Dimensions\n- = Increase Map Scale\n+ = Decrease Map Scale", new Vector2(15 - offset.X, (GraphicsDevice.DisplayMode.Height / 4) - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Tile Dimensions: " + tileSize + "x" + tileSize + "\nNumber Of Blocks: " + map.Count + "\nMap Scale: " + tileScale + "x", new Vector2(((GraphicsDevice.DisplayMode.Width * 0.75f) - 256) - offset.X, 15 - offset.Y), Color.Purple);
            spriteBatch.Draw(mapTexture, new Vector2((((GraphicsDevice.DisplayMode.Width * 0.11f)) + 420) - offset.X, 10 - offset.Y), new Rectangle((currentID % (mapTexture.Height / tileSize)) * tileSize, currentID / (mapTexture.Height / tileSize) * tileSize, tileSize, tileSize), new Color(255, 255, 255, 128), 0, new Vector2(0, 0), 4 / (tileSize / 16), SpriteEffects.None, 1);
            map.Remove(blockToRemove);
            lastRightClick = new Vector2(-10,-10);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Save(string filename)
        {
            saveFileDialog.ShowDialog();
            string[] mapExportData = new string[map.Count + 1];
            int i = 0;
            mapExportData[0] = spriteSheetName;
            foreach(Block b in map)
            {
                i++;
                mapExportData[i] = b.getPosition().X + "," + b.getPosition().Y + "," + b.getID();
            }
            File.WriteAllLines(saveFileDialog.FileName, mapExportData);
        }

        public void Import()
        {
            openFileDialog.Title = "Choose map to import";
            openFileDialog.ShowDialog();
            string[] importMapData = File.ReadAllLines(openFileDialog.FileName);
            if (importMapData[0] != "")
            {
                mapTexture = Texture2D.FromStream(GraphicsDevice, new StreamReader(importMapData[0]).BaseStream);
            }
            map.Clear();
            foreach (string blockData in importMapData)
            {
                try
                {
                    string[] properties = blockData.Split(',');
                    int x = Convert.ToInt32(properties[0]);
                    int y = Convert.ToInt32(properties[1]);
                    int ID = Convert.ToInt32(properties[2]);
                    map.Add(new Block(x,y,ID,true));
                }
                catch
                {

                }
            }
        }
    }
}

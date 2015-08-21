﻿using System;
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
            openFileDialog.Title = "Choose spritesheet file";
            openFileDialog.ShowDialog();
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
            string filename = openFileDialog.FileName;
            mapTexture = Texture2D.FromStream(GraphicsDevice, new StreamReader(filename).BaseStream);
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
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) scrollSpeed = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && offset.Y < 0) offset.Y += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) offset.Y -= scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.A) && offset.X < 0) offset.X += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) offset.X -= scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) Save("test.map");
            Console.WriteLine(offset.ToString());
            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / 64, ((int)(Mouse.GetState().Y - offset.Y) / 64) - 1);

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
                currentID = 63;
            }
            if(currentID > (mapTexture.Width * mapTexture.Height) / 64)
            {
                currentID = 0;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateTranslation(offset.X,offset.Y,0));
            Block blockToRemove = new Block(0,0,0,true);
            foreach(Block block in map)
            {
                if (block.getPosition() == lastRightClick)
                {
                    blockToRemove = block;
                }
                spriteBatch.Draw(mapTexture, new Vector2((int)block.getPosition().X * 64, (int)block.getPosition().Y * 64), new Rectangle((block.getID() % (mapTexture.Width / 16)) * 16, block.getID() / (mapTexture.Height / 16) * 16,16,16), Color.White, 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            }
            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / 64, (int)((Mouse.GetState().Y - offset.Y) / 64) - 1);
            spriteBatch.Draw(originTexture, new Vector2(0,0), null, new Color(255, 255, 255, 128), 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            spriteBatch.Draw(cursorTexture, new Vector2(mouseClickPosition.X * 64, mouseClickPosition.Y * 64), null, new Color(255,255,255,128), 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            spriteBatch.Draw(overlayTexture, new Vector2((GraphicsDevice.DisplayMode.Width * 0.11f) - offset.X, 0 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Current Tile: ", new Vector2(((GraphicsDevice.DisplayMode.Width * 0.11f) + 325) - offset.X, 30 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Block ID: " + currentID, new Vector2(((GraphicsDevice.DisplayMode.Width * 0.11f) + 325) - offset.X, 60 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.Draw(mapTexture, new Vector2((((GraphicsDevice.DisplayMode.Width * 0.11f)) + 420) - offset.X, 10 - offset.Y), new Rectangle((currentID % (mapTexture.Height / 16)) * 16, currentID / (mapTexture.Height / 16) * 16, 16, 16), new Color(255, 255, 255, 128), 0, new Vector2(0, 0), 4, SpriteEffects.None, 1);
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
            foreach(Block b in map)
            {
                i++;
                mapExportData[i - 1] = b.getPosition().X + "," + b.getPosition().Y + "," + b.getID();
            }
            File.WriteAllLines(saveFileDialog.FileName, mapExportData);
        }
    }
}

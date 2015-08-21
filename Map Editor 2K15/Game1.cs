using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        int lastScrollValue;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = ((756 / 64) * 64);
            graphics.PreferredBackBufferWidth = ((1024 / 64) * 64);
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.ApplyChanges();
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            map = new List<Block>();
            lastRightClick = new Vector2(-10,-10);
            offset = new Vector2(0, (756 / 64) * 64);
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
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) scrollSpeed = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) offset.Y += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S) && offset.Y > 0 + ((756 / 64) * 64)) offset.Y -= scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.A) && offset.X < 0) offset.X += scrollSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) offset.X -= scrollSpeed;

            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / 64, ((int)(Mouse.GetState().Y - offset.Y) / 64) - 1);

            Console.WriteLine(offset.ToString());

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                bool canPlace = true;
                foreach(Block block in map)
                {
                    if (block.getPosition() == mouseClickPosition) canPlace = false;
                }
                if (canPlace)
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
            if(currentID > 63)
            {
                currentID = 0;
            }
            Console.WriteLine(currentID);
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
                spriteBatch.Draw(mapTexture, new Vector2((int)block.getPosition().X * 64, (int)block.getPosition().Y * 64), new Rectangle((block.getID() % 8) * 16, block.getID() / 8 * 16,16,16), Color.White, 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            }
            Vector2 mouseClickPosition = new Vector2((int)(Mouse.GetState().X - offset.X) / 64, (int)((Mouse.GetState().Y - offset.Y) / 64) - 1);
            spriteBatch.Draw(originTexture, new Vector2(0,-64), null, new Color(255, 255, 255, 128), 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            spriteBatch.Draw(cursorTexture, new Vector2(mouseClickPosition.X * 64, mouseClickPosition.Y * 64), null, new Color(255,255,255,128), 0, new Vector2(0,0), 4, SpriteEffects.None, 1);
            spriteBatch.Draw(overlayTexture, new Vector2(0 - offset.X, 0 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.DrawString(font, "Current Tile: ", new Vector2(325 - offset.X, 30 - offset.Y), new Color(255, 255, 255, 128));
            spriteBatch.Draw(mapTexture, new Vector2(420 - offset.X, 10 - offset.Y), new Rectangle((currentID % 8) * 16, currentID / 8 * 16, 16, 16), new Color(255, 255, 255, 128), 0, new Vector2(0, 0), 4, SpriteEffects.None, 1);
            map.Remove(blockToRemove);
            lastRightClick = new Vector2(-10,-10);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Save(string filename)
        {

        }
    }
}

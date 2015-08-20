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
        int currentID;
        List<Block> map;
        Vector2 lastRightClick;
        Vector2 offset;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            map = new List<Block>();
            lastRightClick = new Vector2(-10,-10);

            base.Initialize();
            for(int i = 0; i < 64; i++)
            {
                map.Add(new Block(i, 0, i, true));
            }
        }


        protected override void LoadContent()
        {
            mapTexture = Content.Load<Texture2D>("spritesheet.bmp");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector2 mouseClickPosition = new Vector2(Mouse.GetState().X / 64, Mouse.GetState().Y / 64);
                bool canPlace = true;
                foreach(Block block in map)
                {
                    if (block.getPosition() == mouseClickPosition) canPlace = false;
                }
                if (canPlace)
                {
                    map.Add(new Block(Mouse.GetState().Position.X / 64, Mouse.GetState().Position.Y / 64, currentID, true));
                }
            }
            if(Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                lastRightClick = new Vector2((int)(Mouse.GetState().Position.X / 64), (int)(Mouse.GetState().Position.Y / 64));
            }

            base.Update(gameTime);
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

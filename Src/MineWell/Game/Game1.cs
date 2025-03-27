using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D target;
        GameStateManager gsm;

        public static int width = 9 * 16;
        public static int height = 16 * 16;
        public static bool debug = false;
        private static float scale = 3;
        
        public static KeyboardState currKeyboard;
        KeyboardState prevKeyboard;
        MouseState mouseState;

        public static bool quitGame = false;
        private static bool changeScale = false;
        private static float newScale = 3;
        private static bool fullScreen = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "MineWell";

            SetWindowSize(scale, false);

            currKeyboard = new KeyboardState();
            prevKeyboard = new KeyboardState();
            mouseState = new MouseState();

            InputManager.SetInput("left", Keys.Left);
            InputManager.SetInput("right", Keys.Right);
            InputManager.SetInput("up", Keys.Up);
            InputManager.SetInput("down", Keys.Down);

            ResourceManager.SetUpLooper();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // initialization logic
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourceManager.SetSpriteBatch(spriteBatch);
            target = new RenderTarget2D(GraphicsDevice, width, height);
            gsm = new GameStateManager(this, spriteBatch);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // use this.Content to load game content

            //Songs
            LoadSong("MinerKey");

            //SFX
            LoadSFX("Win");
            LoadSFX("Death");

            LoadSFX("BlockBreak");
            LoadSFX("Swing");
            LoadSFX("Tink");
            LoadSFX("GemGet");
            LoadSFX("Charge");
            LoadSFX("Thunk");
            LoadSFX("Explosion");

            //Sprites
            LoadTexture("Miner");
            LoadTexture("Bug");
            LoadTexture("Spikes");
            LoadTexture("Crack");
            LoadTexture("Diamond");
            LoadTexture("Emerald");
            LoadTexture("Ruby");
            LoadTexture("DiamondOre");
            LoadTexture("DiamondOre1");
            LoadTexture("DiamondOre2");
            LoadTexture("DiamondOre3");
            LoadTexture("DiamondOre4");
            LoadTexture("DiamondOre5");
            LoadTexture("DiamondOre6");
            LoadTexture("EmeraldOre");
            LoadTexture("EmeraldOre1");
            LoadTexture("EmeraldOre2");
            LoadTexture("EmeraldOre3");
            LoadTexture("EmeraldOre4");
            LoadTexture("EmeraldOre5");
            LoadTexture("EmeraldOre6");
            LoadTexture("RubyOre");
            LoadTexture("RubyOre1");
            LoadTexture("RubyOre2");
            LoadTexture("RubyOre3");
            LoadTexture("RubyOre4");
            LoadTexture("RubyOre5");
            LoadTexture("Pixel");
            LoadTexture("HUD");
            LoadTexture("HUDNumbers");
            LoadTexture("DrillMeter");
            LoadTexture("DynamitePickup");
            LoadTexture("YouWin");
            LoadTexture("ScoreDisplay");

            //TileSets
            LoadTexture("LD48Tiles");

            gsm.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ResourceManager.Update();
            currKeyboard = Keyboard.GetState();
            InputManager.UpdateInput(currKeyboard);
            foreach(Keys k in currKeyboard.GetPressedKeys())
            {
                if(prevKeyboard.IsKeyUp(k))
                {
                    gsm.KeyPressed(k);
                }
            }

            foreach (Keys k in prevKeyboard.GetPressedKeys())
            {
                if (currKeyboard.IsKeyUp(k))
                {
                    gsm.KeyReleased(k);
                }
            }

            prevKeyboard = currKeyboard;
            mouseState = Mouse.GetState();

            // update logic
            gsm.Update(gameTime);

            base.Update(gameTime);

            if (quitGame) this.Exit();
            if (changeScale)
            {
                changeScale = false;
                SetWindowSize(newScale, fullScreen);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetRenderTarget(target);

            // drawing code
            gsm.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            
            int xOffset = 0;
            int yOffset = 0;
            /*
            if(graphics.IsFullScreen)
            {
                xOffset = (int) Math.Max((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - scale * width) / 4, 0);
                yOffset = (int) Math.Max((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - scale * height) / 4, 0);
            }
            */

            spriteBatch.Draw(target, new Rectangle(xOffset, yOffset, (int)(width * scale), (int)(height * scale)), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadTexture(String name, String path = "")
        {
            ResourceManager.AddTexture(name, Content.Load<Texture2D>(path + name));
        }

        public void LoadSong(String name)
        {
            ResourceManager.AddSong(name, Content.Load<Song>(name));
        }

        public void LoadSFX(String name)
        {
            ResourceManager.AddSFX(name, Content.Load<SoundEffect>(name));
        }

        private void SetWindowSize(float scale, bool full)
        {
            Game1.scale = full ? GetFullScreenScale() : scale;
            graphics.IsFullScreen = full;
            graphics.PreferredBackBufferWidth = (int)(width * Game1.scale);
            graphics.PreferredBackBufferHeight = (int)(height * Game1.scale);
            graphics.ApplyChanges();
        }

        private float GetFullScreenScale()
        {
            float wScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / width;
            float hScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / height;
            return (float)Math.Min(wScale, hScale);
        }

        public static void SetScale(float scale)
        {
            newScale = scale;
            fullScreen = false;
            changeScale = true;
        }

        public static void SetFullscreen(bool full)
        {
            fullScreen = full;
            changeScale = true;
        }

        public static float GetScale()
        {
            return fullScreen ? 5 : newScale;
        }

        public static bool GetFullscreen()
        {
            return fullScreen;
        }
    }
}

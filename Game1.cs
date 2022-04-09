using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace Woods
{    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World world;
        private Camera cam;
        private Player player;

        public static readonly int GridSize = 16;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            world = new World(16);

            ToggleBorderless();
            
            cam = new Camera(GraphicsDevice.Viewport);

            player = new Player(new Vector2(9, 9), world);

            cam.Track(player);

            world.entities.Add(player);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Player.LoadSpriteSheet(LoadTex2D(Content, "player"));

            Player.craftingRecipes = Content.Load<CraftingRecipe[]>("crafting");

            world.SetContent(LoadTex2D(Content, "blocks"), LoadTex2D(Content, "bases"), Content.Load<Block[]>("block-data"));
            Tile.SetContent(LoadTex2D(Content, "damage"));

            player.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();             

            if (gameTime.IsRunningSlowly)
                Console.WriteLine("Slow");

            player.Update(cam.Transform, gameTime);

            cam.UpdateCamera(GraphicsDevice.Viewport);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: cam.Transform, samplerState: SamplerState.PointClamp);
            //world.LightUpdate();
            world.Draw(_spriteBatch, cam.AdjustedVisibleArea);

            foreach (Entity e in world.entities) {

                Vector2 entityPosition = e.GetPosition();

                e.Draw(_spriteBatch, cam.AdjustedVisibleArea, world.GetTile((int)entityPosition.X, (int)entityPosition.Y).lightLevel);
            }

            player.DrawCarrying(_spriteBatch);

            _spriteBatch.Draw(Content.Load<Texture2D>("cursor"), player.cursorPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            _spriteBatch.End();    

            base.Draw(gameTime);
        }

        private static Texture2D LoadTex2D(ContentManager c, string assetName) {
            return c.Load<Texture2D>(assetName);
        } 

        bool _isFullscreen = false;
        bool _isBorderless = false;
        int _width = 0;
        int _height = 0;

        public void ToggleFullscreen() {
            bool oldIsFullscreen = _isFullscreen;

            if (_isBorderless) {
                _isBorderless = false;
            } else {
                _isFullscreen = !_isFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }
        public void ToggleBorderless() {
            bool oldIsFullscreen = _isFullscreen;

            _isBorderless = !_isBorderless;
            _isFullscreen = _isBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }

        private void ApplyFullscreenChange(bool oldIsFullscreen) {
            if (_isFullscreen) {
                if (oldIsFullscreen) {
                    ApplyHardwareMode();
                } else {
                    SetFullscreen();
                }
            } else {
                UnsetFullscreen();
            }
        }
        private void ApplyHardwareMode() {
            _graphics.HardwareModeSwitch = !_isBorderless;
            _graphics.ApplyChanges();
        }
        private void SetFullscreen() {
            _width = Window.ClientBounds.Width;
            _height = Window.ClientBounds.Height;

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = !_isBorderless;

            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }
        private void UnsetFullscreen() {
            _graphics.PreferredBackBufferWidth = _width;
            _graphics.PreferredBackBufferHeight = _height;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }
    }
}

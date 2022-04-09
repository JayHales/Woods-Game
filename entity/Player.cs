using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Woods {
    class Player : Entity {

        private float moveSpeed = 0.1f;
        public Vector2 cursorPosition { get; private set; }
        public Vector2 selectedPosition { get; private set; }
        public Player(Vector2 startPosition, World world) : base(startPosition, world) { }
        private float hitCooldown = 0f;
        public Block carrying = null;
        private bool cWasUp = true;
        private bool dWasUp = true;

        private float interactCooldown = 0f;

        public static CraftingRecipe[] craftingRecipes;

        public void Start() {

            Vector2 position = GetPosition();

            world.SetTile((int)position.X, (int)position.Y, new Tile(world.GetBlockByID(0)));
        }

        public void Update(Matrix cameraTransform, GameTime gameTime) {

            hitCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            interactCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState kbs = Keyboard.GetState();

            if (kbs.IsKeyDown(Keys.W)) {
                SetPosition(GetPosition() - Vector2.UnitY * moveSpeed);
            }

            if (kbs.IsKeyDown(Keys.S)) {
                SetPosition(GetPosition() + Vector2.UnitY * moveSpeed);
            }

            if (kbs.IsKeyDown(Keys.A)) {
                SetPosition(GetPosition() - Vector2.UnitX * moveSpeed);
            }            

            if (kbs.IsKeyDown(Keys.D)) {
                SetPosition(GetPosition() + Vector2.UnitX * moveSpeed);
            }

            MouseState mss = Mouse.GetState();

            Vector2 worldPosition = Vector2.Transform(new Vector2(mss.Position.X, mss.Position.Y), Matrix.Invert(cameraTransform));

            cursorPosition = new Vector2(RoundToNearsest(worldPosition.X + Game1.GridSize / 2, Game1.GridSize) - Game1.GridSize / 2, RoundToNearsest(worldPosition.Y + Game1.GridSize / 2, Game1.GridSize) - Game1.GridSize / 2);
            selectedPosition = new Vector2((worldPosition.X + Game1.GridSize / 2) / Game1.GridSize, (worldPosition.Y + Game1.GridSize / 2) / Game1.GridSize);

            if ((selectedPosition - GetPosition()).Length() < 2) { // If selected in range
                if (mss.LeftButton == ButtonState.Pressed && hitCooldown < 0f && carrying == null) {
                    carrying = world.HitBlock((int)selectedPosition.X, (int)selectedPosition.Y, 1);
                    hitCooldown = 0.2f;
                }

                if (kbs.IsKeyDown(Keys.C) && cWasUp) {
                    Block blockAtPosition = world.GetTile((int)selectedPosition.X, (int)selectedPosition.Y).blockType;

                    foreach (CraftingRecipe c in Player.craftingRecipes) {
                        
                        if (c.requiredStation == blockAtPosition.id && c.requiredBlock == carrying.id) {
                            carrying = world.GetBlockByID(c.result);
                            break;
                        }

                    }

                    cWasUp = false;
                }

                if (!kbs.IsKeyDown(Keys.C) && !cWasUp) {
                    // C was just released
                    cWasUp = true;
                }
                
                //========

                if (kbs.IsKeyDown(Keys.X) && dWasUp) {
                    carrying = null;
                }

                if (!kbs.IsKeyDown(Keys.X) && !dWasUp) {
                    // C was just released
                    dWasUp = true;
                }

                if (mss.RightButton == ButtonState.Pressed) {

                    if (carrying != null) {
                        bool result = world.PlaceBlock((int)selectedPosition.X, (int)selectedPosition.Y, carrying);
                        if (result) {
                            carrying = null;
                        }
                    }

                    else {

                        if (interactCooldown < 0) {
                            if (world.GetTile((int)selectedPosition.X, (int)selectedPosition.Y).blockType.id == (int)BlockIDs.DoorFull) {
                                world.SetTile((int)selectedPosition.X, (int)selectedPosition.Y, new Tile(world.GetBlockByID(BlockIDs.DoorHalf)));
                                interactCooldown = 0.1f;
                            } else if (world.GetTile((int)selectedPosition.X, (int)selectedPosition.Y).blockType.id == (int)BlockIDs.DoorHalf) {
                                world.SetTile((int)selectedPosition.X, (int)selectedPosition.Y, new Tile(world.GetBlockByID(BlockIDs.DoorFull)));
                                interactCooldown = 0.2f;

                            }
                        }

                        
                    }                    
                     
                }
            }            
        }

        public void DrawCarrying(SpriteBatch spriteBatch) {
            if (carrying != null) {

                Vector2 pos = GetPosition();

                world.DrawDynamicBlock(pos.X, pos.Y, carrying, spriteBatch);
            }
        }

        private static float RoundToNearsest(float a, int near) {

            return (int)(a / near) * near;

        }   
    }
}
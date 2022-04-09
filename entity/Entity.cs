using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Woods {
    public class Entity {
        private Vector2 position;
        private bool collides;
        private static SquareColumnSpritesheet spritesheet;
        protected World world;

        public Entity(Vector2 startPosition, World world, bool doesCollide = true) {
            position = startPosition;
            collides = doesCollide;
            this.world = world;
        }

        public static void LoadSpriteSheet(Texture2D tex) {
            spritesheet = new SquareColumnSpritesheet(tex);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleArea, int lightIntensity) {
            if (!visibleArea.Contains(position.X, position.Y)) 
                return;

            spriteBatch.Draw(
                spritesheet.texture,
                position * Game1.GridSize,
                spritesheet.RectangleFor(2), // Change this line (param 2) when making rotation to get different sprites from sheet
                Color.White * (lightIntensity / (float)World.LightingEngine.MaximumLightIntensity),
                0,
                Vector2.One * (spritesheet.spriteWidth / 2),
                Vector2.One,
                SpriteEffects.None,
                0
            );
        }

        public Vector2 GetPosition() {
            return position;
        }

        public Vector2[] GetVertices(Vector2? pos) {

            if (pos == null)
                pos = GetPosition();

            Vector2 testPos = (Vector2)pos;

            return new Vector2[] { 
                testPos,
                testPos + Vector2.UnitX * 0.9f,
                testPos + Vector2.UnitY * 0.9f,
                testPos + Vector2.One * 0.9f
            };
        }

        public bool SetPosition(Vector2 pos) {

            if (!collides) {
                position = pos;
                return true;
            }

            Vector2[] vertices = GetVertices(pos);

            foreach (Vector2 vertex in vertices) {

                if (vertex.X < 0 || vertex.X > world.dimension || vertex.Y < 0 || vertex.Y > world.dimension)
                    return false;

                int x = (int)vertex.X;
                int y = (int)vertex.Y;

                if (world.GetTile(x, y).blockType.solid)
                    return false;
            }           
                
            position = pos;
            return true;
        }
    }
}


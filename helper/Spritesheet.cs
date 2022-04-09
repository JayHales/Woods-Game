using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Woods {
    public class SquareColumnSpritesheet {
        public readonly Texture2D texture;
        public readonly int spriteWidth;
        public SquareColumnSpritesheet(Texture2D texture) {
            this.texture = texture;
            this.spriteWidth = texture.Width;
        }

        public Rectangle RectangleFor(int spriteRow) {
            return new Rectangle(0, spriteWidth * spriteRow, spriteWidth, spriteWidth);
        }
    }
}

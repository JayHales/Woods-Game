using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Woods {
    public partial class World {
        protected readonly Tile[,] map;
        private readonly LightingEngine le;

        private SquareColumnSpritesheet baseSpritesheet;
        private SquareColumnSpritesheet blockSpritesheet;        
        public readonly int dimension;    
        public List<Entity> entities = new List<Entity>();

        private Block[] blockData;

        public World(int dimension) {

            this.dimension = dimension;

            map = new Tile[this.dimension, this.dimension];

            le = new LightingEngine(this);
        }

        private void Generate() {

            Vector2 center = new Vector2(dimension / 2, dimension / 2);

            Random rnd = new Random();

            for (int y = 0; y < this.dimension; y++) {
                for (int x = 0; x < this.dimension; x++) {

                    if ((center - new Vector2(x, y)).Length() < 4) {
                        map[y,x] = new Tile(GetBlockByID(rnd.Next(0, 10) == 0 ? BlockIDs.Rock : BlockIDs.Air));
                    } else {
                        map[y,x] = new Tile(GetBlockByID(rnd.Next(0, 5) == 0 ? BlockIDs.Air : BlockIDs.Tree));
                    }

                    if (center == new Vector2(x, y)) {
                        map[y,x] = new Tile(GetBlockByID(BlockIDs.Campfire));
                    }
                }
            }
        }

        public void SetContent(Texture2D blocks, Texture2D bases, Block[] blockData) {
            baseSpritesheet = new SquareColumnSpritesheet(bases);
            blockSpritesheet = new SquareColumnSpritesheet(blocks);
            this.blockData = blockData;

            Generate();

            le.Light();
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleArea) {

            for (int y = 0; y < this.dimension; y++) {
                for (int x = 0; x < this.dimension; x++) {

                    if (!visibleArea.Contains(x, y))
                        continue;

                    Color c = Color.White * (map[y, x].lightLevel / (float)LightingEngine.MaximumLightIntensity);
                    Vector2 p = new Vector2(x, y) * Game1.GridSize;

                    spriteBatch.Draw(
                        baseSpritesheet.texture,
                        p,
                        baseSpritesheet.RectangleFor((int)map[y, x].baseType),
                        c,
                        MathF.PI * map[y, x].baseRotation * 0.5f,
                        Vector2.One * (baseSpritesheet.spriteWidth / 2),
                        Vector2.One,
                        SpriteEffects.None,
                        0
                    );      

                    spriteBatch.Draw(
                        blockSpritesheet.texture,
                        p,
                        blockSpritesheet.RectangleFor((int)map[y, x].blockType.id),
                        c,
                        0f,
                        Vector2.One * (blockSpritesheet.spriteWidth / 2),
                        Vector2.One,
                        SpriteEffects.None,
                        0                        
                    );   

                    if (map[y, x].health != map[y, x].blockType.maxHealth) {

                        int damageSpriteNumber = (int)(Tile.numberOfDamageStages - (map[y, x].health / (float)map[y, x].blockType.maxHealth) * Tile.numberOfDamageStages);


                        spriteBatch.Draw(
                            Tile.damageSpritesheet.texture,
                            p,
                            Tile.damageSpritesheet.RectangleFor( damageSpriteNumber ),
                            c,
                            0f,
                            Vector2.One * (blockSpritesheet.spriteWidth / 2),
                            Vector2.One,
                            SpriteEffects.None,
                            0                        
                        ); 
                    }   
                }
            }
        }

        public Tile GetTile(int x, int y) {
            return map[y, x];
        }   

        public void SetTile(int x, int y, Tile t) {
            map[y, x] = t;
            le.Light();
        }

        public Block HitBlock(int x, int y, int damage) {
            map[y, x].health -= damage;

            if (map[y, x].health <= 0) {

                int give = map[y, x].blockType.giveOnBreak;

                SetTile(x, y, new Tile(GetBlockByID(map[y, x].blockType.produceOnBreak))); 

                if (give == -1) {
                    return null;
                }

                return GetBlockByID(give); 
            }


            return null;
        }

        public void DrawDynamicBlock(float x, float y, Block blockType, SpriteBatch spriteBatch) {

            Color c = Color.White * (map[(int)y, (int)x].lightLevel / (float)LightingEngine.MaximumLightIntensity);

            spriteBatch.Draw(
                    blockSpritesheet.texture,
                    new Vector2(x, y) * Game1.GridSize,
                    blockSpritesheet.RectangleFor( blockType.id ),
                    Color.White,
                    0f,
                    Vector2.One * (blockSpritesheet.spriteWidth / 2),
                    Vector2.One / 2,
                    SpriteEffects.None,
                    0                        
                ); 
        }

        public bool PlaceBlock(int x, int y, Block blockType) {

            if (map[y, x].blockType.id != 0)
                return false;
            

            foreach (Entity e in entities) {
                foreach (Vector2 vertex in e.GetVertices(null)) {

                    int xx = (int)vertex.X;
                    int yy = (int)vertex.Y;                

                    if (xx == x && yy == y)
                        return false;
                    }
            }

            SetTile(x, y, new Tile(blockType));
            return true;

        }
        
        public Block GetBlockByID(BlockIDs id) {
            return GetBlockByID((int)id);
        }

        public Block GetBlockByID(int id) {
            return blockData[id];
        }
    }

    public class Tile {
        public BaseType baseType;
        public Block blockType;
        public int health;
        public int lightLevel;
        public int baseRotation;
        public static SquareColumnSpritesheet damageSpritesheet;
        public static int numberOfDamageStages {get; private set; }
        public static void SetContent(Texture2D damageTextures) {
            damageSpritesheet = new SquareColumnSpritesheet(damageTextures);
            numberOfDamageStages = damageSpritesheet.texture.Height / damageSpritesheet.texture.Width;

        }

        public Tile(Block block) {
            blockType = block;
            health = block.maxHealth;

            baseType = BaseType.Grass;
        }
    }

    public enum BaseType {
        Grass,
        Stone,
        Dirt,
        Water           
    }

    public enum ItemIDs {
        Sand,
        Coal
    }
    
}
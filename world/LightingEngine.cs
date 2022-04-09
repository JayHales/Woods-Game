using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Woods {
    public partial class World {        
        public class LightingEngine {
            public static int MaximumLightIntensity = 16;
            private readonly World world;
            private static Vector2[] cardinalDirections = { Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY };

            public LightingEngine(World world) {
                this.world = world;
            }

            public void Light() {

                List<Vector2> updateRequired = new List<Vector2>();

                for (int y = 0; y < world.dimension; y++) {
                    for (int x = 0; x < world.dimension; x++) {
                        world.map[y, x].lightLevel = 0;
                        if (world.map[y, x].blockType.lightIntensity > 0) {
                            updateRequired.Add(new Vector2(x, y));
                            world.map[y, x].lightLevel = (int)(MaximumLightIntensity * world.map[y, x].blockType.lightIntensity);
                        }
                    }
                }

                Vector2[] updating;

                while (updateRequired.Count > 0) {

                    updating = updateRequired.ToArray();
                    updateRequired.Clear();

                    foreach (Vector2 xy in updating) {

                        int x = (int)xy.X;
                        int y = (int)xy.Y;

                        int selfLightLevel = world.map[y, x].lightLevel;

                        int newLightLevel = (int)(selfLightLevel - MaximumLightIntensity * world.map[y, x].blockType.opacity);

                        foreach (Vector2 direction in cardinalDirections) {

                            int xx = (int)direction.Y;
                            int yy = (int)direction.X;
                            if (x + xx < 0 || y + yy < 0 || x + xx >= world.dimension || y + yy >= world.dimension)
                                continue;

                            if (world.map[y + yy, x + xx].lightLevel < newLightLevel) {
                                world.map[y + yy, x + xx].lightLevel = newLightLevel;

                                updateRequired.Add(new Vector2(x + xx, y + yy));
                            }
                        }
                    }
                }
            }
        }
    }
    

}
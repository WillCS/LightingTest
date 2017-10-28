using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    class LevelBuilder {
        public static World BuildFrom(Map map) {
            World world = new World(Vector2.Zero);
            for(int x = 0; x < map.Width; x++) {
                for(int y = 0; y < map.Height; y++) {
                    if(map[x, y]) {
                        if(!map[x + 1, y]) {
                            Body edge = BodyFactory.CreateEdge(world, new Vector2(x + 1, y), new Vector2(x + 1, y + 1));
                            edge.IsStatic = true;
                            edge.CollisionGroup = 1;
                        }

                        if(!map[x - 1, y]) {
                            Body edge = BodyFactory.CreateEdge(world, new Vector2(x, y), new Vector2(x, y + 1));
                            edge.IsStatic = true;
                            edge.CollisionGroup = 1;
                        }

                        if(!map[x, y + 1]) {
                            Body edge = BodyFactory.CreateEdge(world, new Vector2(x, y + 1), new Vector2(x + 1, y + 1));
                            edge.IsStatic = true;
                            edge.CollisionGroup = 1;
                        }

                        if(!map[x, y - 1]) {
                            Body edge = BodyFactory.CreateEdge(world, new Vector2(x, y), new Vector2(x + 1, y));
                            edge.IsStatic = true;
                            edge.CollisionGroup = 1;
                        }
                    }
                }
            }
            return world;
        }
    }
}

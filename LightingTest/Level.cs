using FarseerPhysics.Collision;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LightingTest {
    class Level {
        private World _world;

        public World World {
            get {
                return this._world;
            }
        }

        public Level(Map map) {
            this._world = LevelBuilder.BuildFrom(map);
            Entity entity = new TestEntity(this);
            entity.Position = new Vector2(10, 10);
            entity.PhysicsBody.ApplyForce(new Vector2(10, 10));
        }

        public DebugViewXNA GetDebugView(GraphicsDevice device, ContentManager content) {
            DebugViewXNA debugView = new DebugViewXNA(this._world);
            debugView.LoadContent(device, content);
            return debugView;
        }

        public void Step(float dt) {
            this._world.Step(dt);

            foreach(Body body in this._world.BodyList) {
                if(body.UserData is Entity) {
                    ((Entity)body.UserData).Update();
                }
            }   
        }

        public List<Entity> QueryAABB(AABB boundingBox) {
            List<Entity> list = new List<Entity>();
            this._world.QueryAABB((fixture) => {
                if(fixture.UserData is Entity) {
                    list.Add((Entity)fixture.UserData);
                }
                return true;
            }, ref boundingBox);

            return list;
        }

        public List<Entity> GetEntities() {
            List<Entity> list = new List<Entity>();
            foreach(Body body in this._world.BodyList) {
                if(body.UserData is Entity) {
                    list.Add((Entity)body.UserData);
                }
            }

            return list;
        }

        public void Draw(DebugViewXNA debugView, Matrix projectionMatrix) {
            debugView.RenderDebugData(ref projectionMatrix);
        }
    }
}

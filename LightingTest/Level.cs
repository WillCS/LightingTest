using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightingTest {
    class Level {
        private World world;
        private DebugViewXNA debugView;

        public Level(Map map) {
            this.world = LevelBuilder.BuildFrom(map);
            this.debugView = new DebugViewXNA(this.world);
        }

        public void InitDebugView(GraphicsDevice device, ContentManager content) {
            this.debugView.LoadContent(device, content);
        }

        public void Step(float dt) {
            this.world.Step(dt);
        }

        public void Draw(Matrix projection) {
            this.debugView.RenderDebugData(ref projection);
        }
    }
}

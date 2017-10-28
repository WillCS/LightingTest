using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightingTest {
    public class LightingTest : Game {
        public GraphicsDeviceManager GraphicsDeviceManager;
        private const int tickRate = 60;
        private const float dt = 1.0F / tickRate;
        private double accumulator = 0.0D;

        private Level level;
        private Map map;

        private Matrix projectionMatrix;

        public LightingTest() {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            base.Initialize();
            this.map = Map.GetTestMap();
            this.level = new Level(this.map);
            this.level.InitDebugView(this.GraphicsDevice, this.Content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(20);
            this.projectionMatrix = Matrix.CreateOrthographicOffCenter(0F,
                ConvertUnits.ToSimUnits(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                0F, 0F, 1F);
        }

        protected override void LoadContent() {
            base.LoadContent();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            this.accumulator += gameTime.ElapsedGameTime.TotalSeconds;
            if(this.accumulator >= dt * 4) {
                this.accumulator = dt * 4;
            }

            while(this.accumulator >= dt) {
                this.accumulator -= dt;
                this.level.Step(dt);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this.level.Draw(this.projectionMatrix);
            base.Draw(gameTime);
        }
    }
}

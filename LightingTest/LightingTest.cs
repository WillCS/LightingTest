using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.DebugView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LightingTest {
    public class LightingTest : Game {
        public GraphicsDeviceManager GraphicsDeviceManager;
        private const int tickRate = 60;
        private const float dt = 1.0F / tickRate;
        private double accumulator = 0.0D;
        
        private Vector2 origin;

        private Vector2 mouseClickLocation;
        private Vector2 mouseDiff;
        private bool mouseClicked;

        private Level level;
        private Map map;

        private Matrix projectionMatrix;
        private DebugViewXNA debugView;

        private BasicEffect effect;

        public static Texture2D cross;

        public LightingTest() {
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            this.GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            this.GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            this.IsMouseVisible = true;
            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            base.Initialize();
            this.map = Map.GetTestMap();
            this.level = new Level(this.map);
            this.debugView = this.level.GetDebugView(this.GraphicsDevice, this.Content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(40);
            this.effect = new BasicEffect(this.GraphicsDevice);
            this.effect.World = Matrix.CreateTranslation(Vector3.Zero);
            this.effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.effect.VertexColorEnabled = true;
        }

        protected override void LoadContent() {
            cross = this.Content.Load<Texture2D>("point");
            base.LoadContent();
        }

        protected override void UnloadContent() {
            this.debugView.Dispose();
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

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && !this.mouseClicked) {
                this.mouseClicked = true;
                this.mouseClickLocation = Mouse.GetState().Position.ToVector2();
            }

            if(Mouse.GetState().LeftButton == ButtonState.Released && this.mouseClicked) {
                this.mouseClicked = false;
                this.origin += this.mouseDiff;
            }

            if(mouseClicked) {
                this.mouseDiff = this.mouseClickLocation - Mouse.GetState().Position.ToVector2();
            } else {
                this.mouseDiff = Vector2.Zero;
            }

            this.Window.Title = "FPS: " + 1.0F / gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector2 localOrigin = this.origin + this.mouseDiff;
            this.projectionMatrix = Matrix.CreateOrthographicOffCenter(ConvertUnits.ToSimUnits(localOrigin.X),
                ConvertUnits.ToSimUnits(localOrigin.X + this.GraphicsDeviceManager.GraphicsDevice.Viewport.Width),
                ConvertUnits.ToSimUnits(localOrigin.Y + this.GraphicsDeviceManager.GraphicsDevice.Viewport.Height),
                ConvertUnits.ToSimUnits(localOrigin.Y), 0F, 1F);
            this.level.Draw(this.debugView, this.projectionMatrix);
            base.Draw(gameTime);

            this.effect.Projection = projectionMatrix;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach(EffectPass pass in this.effect.CurrentTechnique.Passes) {
                pass.Apply();
                foreach(Entity entity in this.level.GetEntities()) {
                    entity.Draw(this.GraphicsDevice);
                }
            }
        }
    }
}

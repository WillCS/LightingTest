using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    class TestEntity : Entity {
        public TestEntity(Level level) : base(level, 1, 1) {
            this.components.Add(new EntityComponentLight(this, this.GetLightColour, this.GetLightRadius));
        }

        public override void Update() {
            base.Update();
        }

        public Color GetLightColour() {
            return Color.White;
        }

        public float GetLightRadius() {
            return 10.0F;
        }

        public override void Draw(GraphicsDevice graphicsDevice) {
            List<Vector2> contacts = ((EntityComponentLight)this.components[0]).LightBounds;
            int size = contacts.Count;

            IndexBuffer indices = new IndexBuffer(graphicsDevice, typeof(short), size + 1, BufferUsage.WriteOnly);
            short[] indexList = new short[size + 1];
            for(short i = 0; i < size; i++) {
                indexList[i] = i;
            }
            indexList[size] = 0;
            indices.SetData(indexList);

            VertexBuffer vertices = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), size, BufferUsage.WriteOnly);
            VertexPositionColor[] vertexList = new VertexPositionColor[contacts.Count];
            for(int i = 0; i < contacts.Count; i++) {
                vertexList[i] = new VertexPositionColor(new Vector3(0.025F * ConvertUnits.ToDisplayUnits(contacts[i]), 0.2F), Color.Black);
            }
            vertices.SetData(vertexList);

            graphicsDevice.SetVertexBuffer(vertices);
            graphicsDevice.Indices = indices;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, 0, size);

            SpriteBatch batch = new SpriteBatch(graphicsDevice);
            batch.Begin();

            /*for(int i = 0; i < contacts.Count; i++) {
                Vector2 pos = contacts[i];
                batch.Draw(LightingTest.cross, ConvertUnits.ToDisplayUnits(pos) - new Vector2(LightingTest.cross.Width / 2, LightingTest.cross.Height / 2), Color.White);
            }*/

            /*foreach(Vector2 pos in contacts) {
                batch.Draw(LightingTest.cross, ConvertUnits.ToDisplayUnits(pos) - new Vector2(LightingTest.cross.Width / 2, LightingTest.cross.Height / 2), Color.White);
            }*/

            batch.End();
            batch.Dispose(); 
        }
    }
}

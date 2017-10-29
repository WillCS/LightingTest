using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    class EntityComponentLight : EntityComponent {
        private Fixture sensor;
        private List<Vector2> _lightBounds;

        public List<Vector2> LightBounds {
            get {
                return this._lightBounds;
            }
        }

        private Func<Color> _getLightColour;

        private Func<float> _getLightRadius;

        public EntityComponentLight(Entity entity, Func<Color> color, Func<float> radius) : base(entity) {
            this._getLightColour = color;
            this._getLightRadius = radius;
            this._lightBounds = new List<Vector2>();

            //this.sensor = FixtureFactory.AttachCircle(this.GetLightRadius(), 0.0F, this.entity.PhysicsBody);
            //this.sensor.IsSensor = true;
            //this.sensor.OnCollision += this.OnContact;
            //this.sensor.OnSeparation += this.OnSeparate;
        }

        public Color GetLightColour() {
            return this._getLightColour();
        }

        public float GetLightRadius() {
            return this._getLightRadius();
        }

        public override void Update() {
            //((CircleShape)this.sensor.Shape).Radius = this._getLightRadius();
            this._lightBounds.Clear();

            for(int i = 0; i < 100; i++) {
                Vector2 rayPos = this.entity.PhysicsBody.WorldCenter + this.GetLightRadius() * new Vector2(
                    (float) Math.Cos(MathHelper.TwoPi * (i / 99.0F)), 
                    (float) Math.Sin(MathHelper.TwoPi * (i / 99.0F)));

                Vector2 foundPos = rayPos;
                float length = 1.0f;

                this.entity.Level.World.RayCast((fixture, point, normal, fraction) => {
                    if(fraction < length) {
                        length = fraction;
                        foundPos = point;
                    }
                    return 1;
                }, this.entity.PhysicsBody.WorldCenter, rayPos);

                this._lightBounds.Add(foundPos);
            }
        }

        public bool OnContact(Fixture fixtureA, Fixture fixtureB, Contact contact) {
            contact.GetWorldManifold(out Vector2 normal, out FixedArray2<Vector2> points);

            return false;
        }

        public void OnSeparate(Fixture fixtureA, Fixture fixtureB) {

        }
    }
}

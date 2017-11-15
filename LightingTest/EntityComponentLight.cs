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

            int fidelity = 100;

            List<Vector2> radialPoints = new List<Vector2>(fidelity);

            for(int i = 0; i < fidelity; i++) {
                radialPoints.Add(this.entity.PhysicsBody.WorldCenter + this.GetLightRadius() * new Vector2(
                    (float) Math.Cos(MathHelper.TwoPi * ((float) i / (fidelity - 1))), 
                    (float) Math.Sin(MathHelper.TwoPi * ((float) i / (fidelity - 1)))));
            }

            this._lightBounds.AddRange(
                this.RayCastCutOff(this.entity.PhysicsBody.WorldCenter, radialPoints, out List<Fixture> fixtures));

            foreach(Fixture fixture in fixtures) {
                List<Vector2> vertices = null;
                switch(fixture.Shape.ShapeType) {
                    case ShapeType.Polygon:
                        PolygonShape poly = (PolygonShape)fixture.Shape;
                        vertices = poly.Vertices;
                        break;
                    case ShapeType.Edge:
                        EdgeShape edge = (EdgeShape)fixture.Shape;
                        vertices = new List<Vector2> {
                            edge.Vertex1,
                            edge.Vertex2
                        };
                        break;
                }

                if(vertices != null) {
                    List<Vector2> corners = RayCastHit(this.entity.PhysicsBody.WorldCenter, vertices);
                    this._lightBounds.AddRange(corners);
                    this._lightBounds.AddRange(RayCastCorner(this.entity.PhysicsBody.WorldCenter, corners, fixture));
                }
            }

            this._lightBounds.Sort(new AngleComparer(this.entity.PhysicsBody.WorldCenter));
        }

        private List<Vector2> RayCastCutOff(Vector2 centre, List<Vector2> targets, out List<Fixture> hitFixtures) {
            List<Fixture> fixtures = new List<Fixture>();
            List<Vector2> hits = new List<Vector2>();
            foreach(Vector2 vector in targets) {
                Vector2 foundPos = vector;
                float length = 1.0f;

                this.entity.Level.World.RayCast((fixture, point, normal, fraction) => {
                    if(!fixtures.Contains(fixture)) {
                        fixtures.Add(fixture);
                    }
                    if(fraction < length) {
                        length = fraction;
                        foundPos = point;
                    }
                    return 1;
                }, centre, vector);

                hits.Add(foundPos);
            }
            hitFixtures = fixtures;
            return hits;
        }

        private List<Vector2> RayCastHit(Vector2 centre, List<Vector2> targets) {
            List<Vector2> hits = new List<Vector2>();
            foreach(Vector2 vertex in targets) {
                if((vertex - this.entity.PhysicsBody.WorldCenter).LengthSquared() <= Math.Pow(this.GetLightRadius(), 2)) {
                    bool okay = true;

                    this.entity.Level.World.RayCast((f, point, normal, fraction) => {
                        if(fraction < 1.0F) {
                            okay = false;
                            return 0;
                        }
                        return 1;
                    }, centre, vertex);

                    if(okay) {
                        hits.Add(vertex);
                    }
                }
            }
            return hits;
        }

        private List<Vector2> RayCastCorner(Vector2 centre, List<Vector2> targets, Fixture fixture) {
            List<Vector2> hits = new List<Vector2>();
            foreach(Vector2 vertex in targets) {
                if((vertex - this.entity.PhysicsBody.WorldCenter).LengthSquared() <= Math.Pow(this.GetLightRadius(), 2)) {
                    List<Tuple<Fixture, Vector2, float>> collisions = new List<Tuple<Fixture, Vector2, float>>();

                    this.entity.Level.World.RayCast((f, point, normal, fraction) => {
                        collisions.Add(new Tuple<Fixture, Vector2, float>(f, point, fraction));
                        return 1;
                    }, centre, vertex);

                    collisions.Add(new Tuple<Fixture, Vector2, float>(null, vertex, 1.0F));

                    switch(collisions.Count) {
                        case 0:
                            hits.Add(vertex);
                            break;
                        case 1:
                            if(collisions[0].Item1 == fixture) {
                                hits.Add(vertex);
                            }
                            break;
                        default:
                            Fixture closest = null;
                            float closestDist = 1.5F;
                            Vector2 closestPoint = Vector2.Zero;
                            Fixture secondClosest = null;
                            float secondClosestDist = 1.5F;
                            Vector2 secondClosestPoint = Vector2.Zero;

                            foreach(Tuple<Fixture, Vector2, float> collision in collisions) {
                                if(collision.Item3 < closestDist) {
                                    secondClosestDist = closestDist;
                                    secondClosest = closest;
                                    secondClosestPoint = closestPoint;
                                    closest = collision.Item1;
                                    closestPoint = collision.Item2;
                                    closestDist = collision.Item3;
                                } else if(collision.Item3 < secondClosestDist) {
                                    secondClosest = collision.Item1;
                                    secondClosestPoint = collision.Item2;
                                    secondClosestDist = collision.Item3;
                                }
                            }

                            if(secondClosest != null) {
                                hits.Add(closestPoint);
                            } else {
                                hits.Add(secondClosestPoint);
                            }

                            break;
                    }
                }
            }
            return hits;
        }

        public bool OnContact(Fixture fixtureA, Fixture fixtureB, Contact contact) {
            contact.GetWorldManifold(out Vector2 normal, out FixedArray2<Vector2> points);

            return false;
        }

        public void OnSeparate(Fixture fixtureA, Fixture fixtureB) {

        }
    }

    class AngleComparer : IComparer<Vector2> {
        private Vector2 centrePoint;
        public AngleComparer(Vector2 centrePoint) {
            this.centrePoint = centrePoint;
        }

        public int Compare(Vector2 x, Vector2 y) {
            float diff = (x.X - this.centrePoint.X) * (y.Y - centrePoint.Y) - (y.X - centrePoint.X) * (x.Y - centrePoint.Y);

            if(Math.Abs(diff) < 0.001F) {
                return Math.Sign(x.LengthSquared() - y.LengthSquared());
            } else {
                return -Math.Sign(diff);
            }
        }
    }
}

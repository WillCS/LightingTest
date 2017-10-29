using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    abstract class Entity {
        protected Body _physicsBody;
        protected Level _level;
        protected List<EntityComponent> components;

        public Vector2 Position {
            get {
                return this._physicsBody.Position;
            }

            set {
                this._physicsBody.Position = value;
            }
        }
        
        public Body PhysicsBody {
            get {
                return this._physicsBody;
            }
        }

        public Level Level {
            get {
                return this._level;
            }
        }

        protected Entity(Level level, float width, float height) {
            this._level = level;
            this._physicsBody = BodyFactory.CreateRectangle(level.World, width, height, 1, this);
            this._physicsBody.IsStatic = false;
            this.components = new List<EntityComponent>();
        }

        public virtual void Update() {
            foreach(EntityComponent component in this.components) {
                component.Update();
            }
        }

        public abstract void Draw(GraphicsDevice graphicsDevice);
    }

    abstract class EntityComponent {
        protected Entity entity;
        public EntityComponent(Entity entity) {
            this.entity = entity;
        }

        public abstract void Update();
    }
}

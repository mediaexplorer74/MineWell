using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Effects
{
    class Gib : Entity
    {
        public Gib(Vector2 possition, Color color, LevelState levelstate, float angleRange = (float)Math.PI / 4, float minVel = 1, float maxVel = 1.5f, float angleOffset = 0) : base(possition, 1, 1, null, levelstate, solid: false)
        {
            float angle = (float)(ResourceManager.random.NextDouble() * angleRange - Math.PI / 2 - angleRange / 2f) + angleOffset;
            float vel = (float)(ResourceManager.random.NextDouble() * (maxVel - minVel) + minVel);
            xVel = vel * (float)Math.Cos(angle);
            yVel = vel * (float)Math.Sin(angle);

            this.color = color;
        }

        Color color;

        public override void UpdateMovement()
        {
            base.UpdateMovement();
            if (!IsOnScreen()) removeMe = true;
        }

        public override void OnDeath()
        {
            throw new NotImplementedException();
        }

        public override void OnGround()
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera cam, float r = 255, float g = 255, float b = 255, float a = 1, float rotation = 0)
        {
            ResourceManager.DrawRect(new Rect(collisionBox.GetLeftEdge() - cam.GetDrawPosition().X, collisionBox.GetTopEdge() - cam.GetDrawPosition().Y, collisionBox.GetWidth(), collisionBox.GetHeight()), color);
        }
    }
}

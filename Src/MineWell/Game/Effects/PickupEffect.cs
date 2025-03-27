using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell.Effects
{
    abstract class PickupEffect : Entity
    {
        public PickupEffect(Vector2 possition, AnimationManager ani, LevelState levelstate, float angleRange = (float)Math.PI / 4, float minVel = 1, float maxVel = 1.5f, float endVel = .5f) : base(possition, 16, 16, ani, levelstate, .15f, false)
        {
            float angle = (float)(ResourceManager.random.NextDouble() * angleRange - Math.PI / 2 - angleRange / 2f);
            float vel = (float)(ResourceManager.random.NextDouble() * (maxVel - minVel) + minVel);
            xVel = (float)(vel * Math.Cos(angle));
            yVel = (float)(vel * Math.Sin(angle));

            this.endVel = endVel;
        }

        float endVel;

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (yVel > endVel)
            {
                OnPickup();
                removeMe = true;
            }
        }

        protected abstract void OnPickup();

        public override void OnDeath()
        {

        }

        public override void OnGround()
        {

        }
    }
}

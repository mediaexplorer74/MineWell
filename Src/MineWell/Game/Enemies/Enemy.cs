using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell.Enemies
{
    abstract class Enemy : Entity
    {
        public Enemy(Vector2 possition, float width, float height, AnimationManager ani, LevelState levelstate, float gravity = 0.15F, bool solid = true, int facing = 1, int yfacing = -1, float maxHealth = 1) : base(possition, width, height, ani, levelstate, gravity, solid, facing, yfacing, maxHealth)
        {
            target = GetPosition();
            prevPos = GetPosition();
        }

        bool jumping = false;
        int jumpTimer = 0;
        Vector2 target, prevPos;

        public override void UpdateLogic()
        {
            if (levelstate.tick) Tick();

            if (jumping)
            {
                LerpTo(target, .2f, 1);
                if (GetPosition() == target)
                {
                    jumping = false;
                    ani.ResetAndSet(0);
                }
                jumpTimer++;
            }

            if(collisionBox.Intersects(levelstate.playa.GetCollisionBox()))
            {
                levelstate.playa.Hurt(collisionBox, 1);
                if(!levelstate.playa.IsDead())Kill();
            }

            base.UpdateLogic();
        }

        public abstract void Tick();

        protected void TurnForWall()
        {
            if (levelstate.map.IsSolid(GetPosition() + new Vector2(16 * collisionBox.facing, 0))) collisionBox.facing = -collisionBox.facing;
        }

        protected void JumpForward()
        {
            JumpTo(GetPosition() + new Vector2(16 * collisionBox.facing, 0));
            SquashAndStretch(9, .3f, true);
        }

        protected void JumpTo(Vector2 newTarget)
        {
            if (!levelstate.map.IsSolid(newTarget))
            {
                target = newTarget;
                prevPos = GetPosition();
                jumping = true;
                jumpTimer = 0;
            }
        }

        public override void OnDeath()
        {
            removeMe = true;
            Gib();
        }

        public override void OnGround()
        {

        }
    }
}

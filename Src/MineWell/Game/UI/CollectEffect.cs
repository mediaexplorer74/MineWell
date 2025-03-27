using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.UI
{
    abstract class CollectEffect : UIEffect
    {
        public CollectEffect(Vector2 position, AnimationManager ani, int targetX, LevelState levelstate) : base(position, ani, levelstate)
        {
            angle = (float)Math.Atan2(8 - position.Y, targetX - position.X);
        }

        float angle, vel = 0, acc = .2f;

        public override void Update()
        {
            vel += acc;
            position = new Vector2(position.X + vel * (float)Math.Cos(angle), position.Y + vel * (float)Math.Sin(angle));
            if(position.Y < 0)
            {
                OnCollect();
                removeMe = true;
            }
        }

        protected abstract void OnCollect();
    }
}

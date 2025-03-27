using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.UI
{
    abstract class UIEffect
    {
        public UIEffect(Vector2 position, AnimationManager ani, LevelState levelstate)
        {
            this.position = position;
            this.ani = ani;
            this.levelstate = levelstate;
        }

        protected Vector2 position;
        protected AnimationManager ani;
        protected LevelState levelstate;
        public bool removeMe = false;

        abstract public void Update();

        public void Draw()
        {
            ani.Draw(position);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    abstract class GameState
    {
        protected GameStateManager gsm;

        public GameState(GameStateManager gsm)
        {
            this.gsm = gsm;
        }

        public abstract void Initialize();

        public abstract void KeyPressed(Keys key);

        public abstract void KeyReleased(Keys key);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}

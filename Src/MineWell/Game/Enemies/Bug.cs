using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Enemies
{
    class Bug : Enemy
    {
        public Bug(Vector2 possition, int facing, LevelState levelstate) : base(possition, 12, 8, new AnimationManager("Bug", 16, new int[] { 14, 14 }), levelstate, 0, facing: facing)
        {
        }

        public override void Tick()
        {
            TurnForWall();
            JumpForward();
            ani.ResetAndSet(1);
        }
    }
}

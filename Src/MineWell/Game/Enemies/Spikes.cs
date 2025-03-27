using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Enemies
{
    class Spikes : Enemy
    {
        public Spikes(Vector2 possition, LevelState levelstate) : base(possition, 12, 8, new AnimationManager("Spikes", 16, new int[] { 14 }), levelstate, 0)
        {

        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!levelstate.map.IsSolid(GetPosition() + new Vector2(0, 16))) Kill();
        }

        public override void Tick()
        {

        }
    }
}

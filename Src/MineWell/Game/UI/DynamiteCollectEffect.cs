using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.UI
{
    class DynamiteCollectEffect : CollectEffect
    {
        public DynamiteCollectEffect(Vector2 position, LevelState levelstate) : base(position, new AnimationManager("DynamitePickup", 16, new int[] { 7 }), 101, levelstate)
        {

        }

        protected override void OnCollect()
        {
            levelstate.AddBomb();
        }
    }
}

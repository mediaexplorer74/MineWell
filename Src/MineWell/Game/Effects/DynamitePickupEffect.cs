using MineWell.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell.Effects
{
    class DynamitePickupEffect : PickupEffect
    {
        public DynamitePickupEffect(Vector2 possition, LevelState levelstate) : base(possition, new AnimationManager("DynamitePickup", 16, new int[] { 7 }), levelstate, (float)(Math.PI / 8f), 2.5f, 3f)
        {

        }

        protected override void OnPickup()
        {
            levelstate.UIEffects.Add(new DynamiteCollectEffect(GetPosition() - levelstate.cam.GetDrawPosition(), levelstate));
        }
    }
}

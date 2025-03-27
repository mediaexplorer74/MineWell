using GameManager.Effects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Pickups
{
    class DynamitePickup : Pickup
    {
        public DynamitePickup(Vector2 possition, LevelState levelstate) : base(possition, "DynamitePickup", levelstate)
        {

        }

        protected override void OnPickup()
        {
            levelstate.entities["effects"].Add(new DynamitePickupEffect(GetPosition(), levelstate));
        }
    }
}

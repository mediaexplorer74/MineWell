using GameManager.Effects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Pickups
{
    class Gem : Pickup
    {
        public Gem(Vector2 possition, string name, int score, LevelState levelstate) : base(possition, name, levelstate)
        {
            this.score = score;
        }

        int score;

        protected override void OnPickup()
        {
            levelstate.entities["effects"].Add(new ScorePickupEffect(GetPosition(), ani, score, levelstate, (float)(Math.PI / 8f), 2.5f, 3f));
            ResourceManager.PlaySFX("GemGet");
        }
    }
}

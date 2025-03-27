using GameManager.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Effects
{
    class ScorePickupEffect : PickupEffect
    {
        public ScorePickupEffect(Vector2 possition, AnimationManager ani, int score, LevelState levelstate, float angleRange = 0.7853982F, float minVel = 1, float maxVel = 1.5F, float endVel = .5f) : base(possition, ani, levelstate, angleRange, minVel, maxVel, endVel)
        {
            this.score = score;
        }

        int score;

        protected override void OnPickup()
        {
            levelstate.UIEffects.Add(new ScoreCollectEffect(GetPosition() - levelstate.cam.GetDrawPosition(), ani, score, levelstate));
        }
    }
}

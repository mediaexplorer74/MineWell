using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell.UI
{
    class ScoreCollectEffect : CollectEffect
    {
        public ScoreCollectEffect(Vector2 position, AnimationManager ani, int score, LevelState levelstate) : base(position, ani, 10, levelstate)
        {
            this.score = score;
        }

        int score;

        protected override void OnCollect()
        {
            levelstate.AddScore(score);
        }
    }
}

using GameManager.Effects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Pickups
{
    class Ore : Entity
    {
        public Ore(Vector2 possition, string name, int pieces, int score, LevelState levelstate) : base(possition, 16, 16, new AnimationManager(name + "Ore", 16, new int[] { 7 }), levelstate, 0, false)
        {
            this.name = name;
            this.pieces = pieces;
            this.score = score;
        }

        string name;
        int pieces, score;

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if(!levelstate.map.IsSolid(GetPosition()))
            {
                for(int i = 1; i <= pieces; i++)
                {
                    levelstate.entities["effects"].Add(new ScorePickupEffect(GetPosition(), new AnimationManager(name + "Ore" + i, 16, new int[] { 7 }), score, levelstate, 
                        angleRange: (float)(Math.PI / 3f), minVel: 2, maxVel: 2.5f, endVel: 1));
                }
                removeMe = true;
                ResourceManager.PlaySFX("GemGet");
            }
        }

        public override void OnDeath()
        {

        }

        public override void OnGround()
        {

        }
    }
}

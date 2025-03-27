using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.GameStates
{
    class WinState : LevelState
    {
        public WinState(GameStateManager gsm, string tileName) : base(gsm, "WinMenu", tileName)
        {
            menu = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            startedTransition = false;
        }

        bool startedTransition = false;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!gsm.IsTransitioning() && !startedTransition)
            {
                if (InputManager.IsPressed("left") || InputManager.IsPressed("right") || InputManager.IsPressed("up") || InputManager.IsPressed("down"))
                {
                    startedTransition = true;
                    gsm.TransitionOut();
                    ResourceManager.PlaySFX("GemGet");
                }
            }
            if(startedTransition)
            {
                if(gsm.IsBlackScreen())
                {
                    gsm.score = 0;
                    gsm.SetLevel(1);
                    gsm.TransitionIn();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ResourceManager.DrawTexture("YouWin", new Vector2(Game1.width / 2, Game1.height / 2 - 16));
            ResourceManager.DrawTexture("ScoreDisplay", new Vector2(Game1.width / 2, Game1.height / 2 + 32));

            string scoreString = displayScore.ToString();
            for (int i = 0; i < scoreString.Length; i++)
            {
                int digit = scoreString.ToCharArray()[scoreString.Length - 1 - i] - 48;
                ResourceManager.DrawSubTexture("HUDNumbers", new Vector2(Game1.width / 2 + 33 - i * 8, Game1.height / 2 + 31), 0, digit * 9, 7, 9);
            }

        }
    }
}

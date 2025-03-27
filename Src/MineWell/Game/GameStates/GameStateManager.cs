using GameManager.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    class GameStateManager
    {
        public Game baseGame;
        public SpriteBatch spriteBatch;

        Dictionary<String, GameState> gameStates;
        string currState = "";
        string defaultState = "level1";

        int curLevel = 1;
        int numLevels = 3;

        float[] transitionBars;
        float[] transitionVels;
        int transitionTimer = 0;
        int transitionTimerPerBar = 10;
        float transitionAcc = .15f;
        bool transitionIn = false;
        bool transitionOut = false;
        bool blackScreen = false;

        public int score = 0;

        public GameStateManager(Game baseGame, SpriteBatch spriteBatch)
        {
            this.baseGame = baseGame;
            this.spriteBatch = spriteBatch;
            ResetTrantionBars();
            gameStates = new Dictionary<String, GameState>();
        }

        public void Initialize()
        {
            gameStates["win"] = new WinState(this, "LD48Tiles");
            gameStates["level1"] = new LevelState(this, "Level1", "LD48Tiles");
            gameStates["level2"] = new LevelState(this, "Level2", "LD48Tiles");
            gameStates["level3"] = new LevelState(this, "Level3", "LD48Tiles");
            gameStates["level4"] = new LevelState(this, "Level4", "LD48Tiles");
            SetState(defaultState);
        }

        public void SetState(string state)
        {
            if (state == currState) return;
            if (!gameStates.ContainsKey(state)) return;

            gameStates[state].Initialize();
            currState = state;
        }

        public void NextLevel()
        {
            curLevel++;
            if (curLevel > numLevels) SetState("win");
            else SetState("level" + curLevel);
        }

        public void SetLevel(int level)
        {
            curLevel = level;
            if (curLevel > numLevels) curLevel = 1;
            SetState("level" + curLevel);
        }

        public int GetCurLevel()
        {
            return curLevel;
        }

        private void ResetTrantionBars()
        {
            transitionBars = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            transitionVels = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            transitionTimer = 0;
        }

        public void TransitionIn()
        {
            ResetTrantionBars();
            transitionIn = true;
            transitionOut = false;
            blackScreen = false;
        }

        public void TransitionOut()
        {
            ResetTrantionBars();
            transitionIn = false;
            transitionOut = true;
            blackScreen = false;
        }

        public void CutToBlack()
        {
            transitionIn = false;
            transitionOut = false;
            blackScreen = true;
        }

        public void CutIn()
        {
            transitionIn = false;
            transitionOut = false;
            blackScreen = false;
        }

        public bool IsTransitioning()
        {
            return transitionIn || transitionOut;
        }

        public bool IsTransitionedIn()
        {
            return !IsTransitioning() && !blackScreen;
        }

        public bool IsBlackScreen()
        {
            return blackScreen;
        }

        public void KeyPressed(Keys k)
        {
            gameStates[currState].KeyPressed(k);
        }

        public void KeyReleased(Keys k)
        {
            gameStates[currState].KeyReleased(k);
        }

        public void Update(GameTime gameTime)
        {
            if(transitionIn || transitionOut)
            {
                bool stillTransitioning = false;
                for(int i = 0; i <= transitionTimer / transitionTimerPerBar && i < 9; i++)
                {
                    if (transitionBars[i] >= Game1.height) continue;
                    transitionVels[i] += transitionAcc;
                    transitionBars[i] += transitionVels[i];
                    if (transitionBars[i] >= Game1.height) continue;
                    stillTransitioning = true;
                }

                if (!stillTransitioning)
                {
                    if (transitionOut) blackScreen = true;
                    transitionIn = false;
                    transitionOut = false;
                }
                else
                {
                    transitionTimer++;
                }
            }

            gameStates[currState].Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            gameStates[currState].Draw(gameTime);

            if(transitionIn)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (transitionBars[i] > Game1.height) continue;
                    ResourceManager.DrawRect(new Rect(i * 16, transitionBars[i], 16, Game1.height - transitionBars[i] + 1), Color.Black);
                }
            }
            else if (transitionOut)
            {
                for (int i = 0; i < 9; i++)
                {
                    ResourceManager.DrawRect(new Rect(i * 16, 0, 16, transitionBars[i]), Color.Black);
                }
            }
            else if (blackScreen)
            {
                ResourceManager.DrawRect(new Rect(0, 0, Game1.width, Game1.height), Color.Black);
            }

            spriteBatch.End();
        }
    }
}

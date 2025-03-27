using GameManager.UI;
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
    class LevelState: GameState
    {
        public Map map;
        public Player playa;
        public Camera cam;
        public Dictionary<String, List<Entity>> entities;
        public List<UIEffect> UIEffects;
        public Vector2 spawnPoint;

        String levelName;
        String tileName;

        protected int displayScore = 0;
        int score = 0;

        int bombs = 0;

        float drillMeter = 0;
        float drillMeterRate = .002f;
        AnimationManager drillAni;

        public bool tick = false;

        int deathTimer = 0;
        public bool won = false;
        int winTimer = 0;

        protected bool menu = false;

        public LevelState(GameStateManager gsm, String levelName, String tileName) : base(gsm)
        {
            this.levelName = levelName;
            this.tileName = tileName;
        }

        public override void Initialize()
        {
            entities = new Dictionary<String, List<Entity>>();
            InitializeEntityLists();

            spawnPoint = new Vector2(0, 0);

            map = new Map(levelName, tileName, 16, 16, this);
            map.LoadEntities();

            cam = new Camera(0, 0, map);
            cam.CenterOn(spawnPoint);

            drillAni = new AnimationManager("DrillMeter", 12, new int[] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 21 });

            UIEffects = new List<UIEffect>();

            deathTimer = 0;

            won = false;
            winTimer = 0;

            score = gsm.score;
            displayScore = score;

            Spawn();
        }

        public override void KeyPressed(Keys key)
        {
            /*
            if(key == Keys.I)
            {

            }
            */
        }

        public override void KeyReleased(Keys key)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if(playa.IsDead())
            {
                if (deathTimer == 0)
                {
                    ResourceManager.StopSong();
                    ResourceManager.PlaySFX("Death");
                }
                deathTimer++;
                if(deathTimer == 30)
                {
                    gsm.TransitionOut();
                }
                else if(deathTimer > 60 &&!gsm.IsTransitioning())
                {
                    Initialize();
                    gsm.TransitionIn();
                }
            }

            else if(won)
            {
                if (winTimer == 0)
                {
                    ResourceManager.StopSong();
                    ResourceManager.PlaySFX("Win");
                }
                winTimer++;
                if (winTimer == 60)
                {
                    gsm.TransitionOut();
                }
                else if (winTimer > 60 && !gsm.IsTransitioning())
                {
                    gsm.score = score;
                    gsm.NextLevel();
                    gsm.TransitionIn();
                    return;
                }
            }

            else if(!playa.active && !menu)
            {
                if (!gsm.IsTransitioning()) playa.active = true;
            }

            playa.UpdateInput();

            cam.LerpTo(playa.GetLastGroundedPosition());

            cam.Update();

            HashSet<Entity> MovementUpdated = new HashSet<Entity>();
            HashSet<Entity> LogicUpdated = new HashSet<Entity>();

            //Updating Logic
            playa.UpdateLogic();
            LogicUpdated.Add(playa);

            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                for (int i = pair.Value.Count - 1; i >= 0; i--)
                {
                    Entity e = pair.Value[i];
                    if (!LogicUpdated.Contains(e))
                    {
                        e.UpdateLogic();
                        LogicUpdated.Add(e);
                    }
                }
            }

            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                pair.Value.RemoveAll(e => e.removeMe);
            }

            //Updating Movement
            foreach (Entity e in entities["walls"])
            {
                e.UpdateMovement();
                MovementUpdated.Add(e);
            }

            foreach (Entity e in entities["platforms"])
            {
                e.UpdateMovement();
                MovementUpdated.Add(e);
            }

            playa.UpdateMovement();
            MovementUpdated.Add(playa);

            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                if (pair.Key == "platforms" || pair.Key == "walls") continue;
                for (int i = pair.Value.Count - 1; i >= 0; i--)
                {
                    Entity e = pair.Value[i];
                    if (!MovementUpdated.Contains(e))
                    {
                        e.UpdateMovement();
                        MovementUpdated.Add(e);
                    }
                }
            }

            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                pair.Value.RemoveAll(e => e.removeMe);
            }

            map.Update();

            for(int i = UIEffects.Count - 1; i >= 0; i--)
            {
                UIEffects[i].Update();
                if (UIEffects[i].removeMe) UIEffects.RemoveAt(i);
            }

            if (displayScore > score) displayScore = score;

            if (displayScore < score)
            {
                displayScore += 5;
                if (displayScore > score) displayScore = score;
                else if (displayScore > 9999999) displayScore = 9999999;
            }

            if(drillMeter < 1)
            {
                drillMeter += drillMeterRate;
                if (drillMeter > 1) drillMeter = 1;
            }

            if (tick) tick = false;
        }

        public void Tick()
        {
            tick = true;
        }

        public void Spawn()
        {
            playa = new Player(spawnPoint, this);
            cam.CenterOn(spawnPoint);
        }

        public void Clear()
        {
            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                pair.Value.Clear();
            }
        }

        public void InitializeEntityLists()
        {
            AddEntityList("walls");
            AddEntityList("platforms");

            AddEntityList("pickups");

            AddEntityList("enemies");

            AddEntityList("effects");
        }

        public void AddEntityList(String name)
        {
            entities[name] = new List<Entity>();
        }

        public void AddScore(int toAdd)
        {
            score += toAdd;
        }

        public void AddBomb()
        {
            if(bombs < 9) bombs++;
        }

        public void RemoveBomb()
        {
            if(bombs > 0) bombs--;
        }
        
        public bool HasBombs()
        {
            return bombs > 0;
        }

        public override void Draw(GameTime gameTime)
        {
            map.Draw(cam);

            HashSet<Entity> drawn = new HashSet<Entity>();
            foreach (KeyValuePair<String, List<Entity>> pair in entities)
            {
                foreach (Entity e in pair.Value)
                {
                    if (!drawn.Contains(e))
                    {
                        e.Draw(cam);
                        drawn.Add(e);
                    }
                }
            }

            playa.Draw(cam);

            foreach (UIEffect e in UIEffects)
            {
                e.Draw();
            }

            if (!menu)
            {

                ResourceManager.DrawTexture("HUD", new Vector2(Game1.width / 2, 8));

                string scoreString = displayScore.ToString();
                for (int i = 0; i < scoreString.Length; i++)
                {
                    int digit = scoreString.ToCharArray()[scoreString.Length - 1 - i] - 48;
                    ResourceManager.DrawSubTexture("HUDNumbers", new Vector2(74 - i * 8, 7), 0, digit * 9, 7, 9);
                }

                ResourceManager.DrawSubTexture("HUDNumbers", new Vector2(136, 7), 0, gsm.GetCurLevel() * 9, 7, 9);
            }
        }
    }
}

using GameManager.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    abstract class Entity
    {
        //Basic Properties
        private Vector2 position;
        private Vector2 prevPos;
        protected Vector2 startPosition;
        protected Hitbox collisionBox;
        protected float gravity, defaultGravity;
        protected float gravityMultiplier = 1;
        protected bool solid;
        protected AnimationManager ani;
        public enum Side { left, right, top, bottom };
        protected List<Vector2> collisionPoints;
        protected LevelState levelstate;
        protected int removeTime = -1;
        protected int defaultAni = 0;
        protected int defaultFacing;
        protected int defaultYFacing;

        //Movement
        protected float xVel = 0, yVel = 0;
        protected bool ignoreSemiSolid = false;
        protected bool ignoreWallEntities = false;
        protected bool grounded = true;
        protected float speed = .5f;
        protected float speedBoost = 1;
        protected float acceleration = .1f;
        protected float deacceleration = .07f;
        Entity platform = null;
        protected float stunModifier = 0;
        private float xDif = 0;
        private float yDif = 0;
        protected bool StandOnAble = true;

        //Health and Damage
        protected bool dead = false;
        protected float health;
        protected float maxHealth;
        protected int iFrames = 0;
        protected int maxIFrames = 10;
        protected bool invulnerable = false;
        protected float xKnockbackMultiplier = 1;
        protected float yKnockbackMultiplier = 1;
        protected bool stunable = true;
        protected int stunCount = 0;
        protected int maxStun = 15;
        protected bool noFlash = false;
        protected bool selfOrientKnockback = false;
        public bool removeMe = false;

        protected Hitbox spriteBox;

        //Draw Effects
        public float drawShake = 4;

        int squashFrames = 0;
        float squashMin = 0;
        float squashMax = 0;
        int totalSquashFrames = 0;
        bool squashToGround = false;

        protected bool visible = true;

        public bool debug = false;

        protected bool conserveXMomentum = false;
        protected bool conserveYMomentum = false;

        protected float spriteOriginXOffset = 0;
        protected float spriteOriginYOffset = 0;

        protected float drawXOffset = 0;
        protected float drawYOffset = 0;

        public Entity(Vector2 possition, float width, float height, AnimationManager ani, LevelState levelstate,
            float gravity = .15f, bool solid = true, int facing = 1, int yfacing = -1, float maxHealth = 3)
        {
            this.levelstate = levelstate;
            this.position = new Vector2(possition.X, possition.Y);
            startPosition = new Vector2(possition.X, possition.Y);
            prevPos = new Vector2(possition.X, possition.Y);
            collisionBox = new Hitbox(possition.X, possition.Y, 0, 0, width, height, this, facing, yfacing);
            int aniWidth = (int)width;
            int aniHeight = (int)height;
            if(ani != null)
            {
                aniWidth = ani.GetWidth();
                aniHeight = ani.GetHeight();
            }
            if (aniWidth % 2 == 1) aniWidth++;
            if (aniHeight % 2 == 1) aniHeight++;
            spriteBox = new Hitbox(possition.X, possition.Y, 0, 0, aniWidth, aniHeight, this, facing, yfacing);
            this.gravity = gravity;
            defaultGravity = gravity;
            this.solid = solid;
            this.ani = ani;
            this.maxHealth = maxHealth;
            health = maxHealth;
            defaultFacing = facing;
            defaultYFacing = yfacing;
            collisionPoints = new List<Vector2>();
        }

        public virtual void Reset()
        {
            xDif = 0;
            yDif = 0;
            dead = false;
            health = maxHealth;
            gravity = defaultGravity;
            removeMe = false;
            stunCount = 0;
            removeTime = -1;
            iFrames = 0;
            ResetPossition();
            platform = null;
            speedBoost = 1;
            if (ani != null)
            {
                ani.SetCurrAni(defaultAni);
                ani.Reset();
            }
            collisionBox.facing = defaultFacing;
            collisionBox.yfacing = defaultYFacing;
            collisionBox.Update(position.X, position.Y);
            spriteBox.Update(position.X, position.Y);
            grounded = true;
            collisionPoints = new List<Vector2>();
        }

        //Resets the entity to its default possition
        public void ResetPossition()
        {
            SetPossition(startPosition);
        }

        //Returns the entity's default possition
        public Vector2 GetStartPossition()
        {
            return new Vector2(startPosition.X, startPosition.Y);
        }

        //Steps the entity's logic, called one per frame
        public virtual void UpdateLogic()
        {
            //Updating frame counts
            if (iFrames > 0) iFrames--;
            if (stunCount > 0) stunCount--;
            if (removeTime == 0) removeMe = true;
            else if (removeTime > 0) removeTime--;

            //Chacking for platform/wall entities
            platform = null;
            if (solid)
            {
                //SemiSolid platform check
                if (!ignoreSemiSolid)
                {
                    foreach (Entity e in levelstate.entities["platforms"])
                    {
                        if (e == this || e.dead || !e.StandOnAble) continue;
                        if (collisionBox.XOverlap(e.GetCollisionBox()) &&
                            Math.Abs(e.GetCollisionBox().GetTopEdge() - collisionBox.GetBottomEdge()) <= .1f)
                        {
                            SetPlatform(e);
                        }
                    }
                }

                //Wall check
                if (!ignoreWallEntities)
                {
                    foreach (Entity e in levelstate.entities["walls"])
                    {
                        if (e == this || e.dead) continue;
                        if (collisionBox.XOverlap(e.GetCollisionBox()) &&
                            Math.Abs(e.GetCollisionBox().GetTopEdge() - collisionBox.GetBottomEdge()) <= .1f)
                        {
                            SetPlatform(e);
                        }
                    }
                }
            }

        }

        //Steps the entity's movement, called once per frame
        public virtual void UpdateMovement()
        {
            //Resetting list of collision points
            collisionPoints = new List<Vector2>();

            //Adding gravity
            yVel += gravity * gravityMultiplier;

            //Platform movement
            if (platform != null)
            {
                bool temp = grounded;
                grounded = true;
                Move(platform.GetXDif(), platform.GetYDif());
                grounded = temp;
            }

            //Normal movement
            Move(xVel, yVel);

            //Updating differnce
            Vector2 posDif = GetPosition() - prevPos;
            xDif = posDif.X;
            yDif = posDif.Y;
            prevPos = GetPosition();
        }

        //Moves the entity to a set possition
        public void MoveTo(float x, float y)
        {
            Move(x - GetX(), y - GetY());
        }

        //Moves the entity by a set distance
        public void Move(float xDif, float yDif)
        {
            MoveX(xDif);
            MoveY(yDif);
        }

        //Moves the entity along the x axis by a set distance
        public virtual void MoveX(float xMove)
        {
            if (xMove == 0) return;
            bool possitive = xMove > 0;

            //Creating rectangle representing the pathe the entity will move
            Rect movePath;
            if(possitive)
            {
                movePath = new Rect(collisionBox.GetRightEdge(), collisionBox.GetTopEdge(), 
                    xMove, collisionBox.GetHeight());
            }
            else
            {
                movePath = new Rect(collisionBox.GetLeftEdge() + xMove, collisionBox.GetTopEdge(),
                    -xMove, collisionBox.GetHeight());
            }

            //Checking for collision with obstacles in the move path
            List<Vector2> points = new List<Vector2>();
            movePath.CheckCollision(levelstate.map, true, points);
            float moveMin = xMove;
            if (solid)
            {
                //static terrain check
                foreach (Vector2 curPoint in points)
                {
                    if (possitive)
                    {
                        float dif = curPoint.X - collisionBox.GetRightEdge();
                        if (dif < moveMin) moveMin = dif;
                    }
                    else
                    {
                        float dif = curPoint.X + levelstate.map.GetTileWidth() - collisionBox.GetLeftEdge();
                        if (dif > moveMin) moveMin = dif;
                    }
                }

                //Wall entity check
                if (!ignoreWallEntities)
                {
                    foreach (Entity e in levelstate.entities["walls"])
                    {
                        if (e == this || e.dead) continue;
                        if (!collisionBox.Intersects(e.GetCollisionBox()) && e.GetCollisionBox().Intersects(movePath))
                        {
                            if (possitive)
                            {
                                float dif = e.GetCollisionBox().GetLeftEdge() - collisionBox.GetRightEdge();
                                if (dif < moveMin) moveMin = dif;
                            }
                            else
                            {
                                float dif = e.collisionBox.GetRightEdge() - collisionBox.GetLeftEdge();
                                if (dif > moveMin) moveMin = dif;
                            }
                        }
                    }
                }
            }

            //If this is a wall, checking for entities which should be moved by this entity
            if (levelstate.entities["walls"].Contains(this) && !dead)
            {
                if (possitive)
                {
                    movePath = new Rect(collisionBox.GetRightEdge(), collisionBox.GetTopEdge(),
                        xMove, collisionBox.GetHeight());
                }
                else
                {
                    movePath = new Rect(collisionBox.GetLeftEdge() + xMove, collisionBox.GetTopEdge(),
                        -xMove, collisionBox.GetHeight());
                }
                foreach (KeyValuePair<String, List<Entity>> pair in levelstate.entities)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        Entity e = pair.Value[i];
                        if (this == e) continue;
                        if (e.platform != this &&
                            e.IsSolid() && !e.ignoreWallEntities &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                        {
                            if (possitive)
                            {
                                e.MoveTo(
                                    collisionBox.GetRightEdge() + xMove + e.collisionBox.GetWidth() / 2,
                                    e.GetY());
                            }
                            else
                            {
                                e.MoveTo(
                                    collisionBox.GetLeftEdge() + xMove - e.collisionBox.GetWidth() / 2,
                                    e.GetY());
                            }
                        }
                    }
                }
                if (levelstate.playa != this)
                {
                    Entity e = levelstate.playa;
                    if (e.platform != this &&
                            e.IsSolid() && !e.ignoreWallEntities &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                    {
                        if (possitive)
                        {
                            e.MoveTo(
                                collisionBox.GetRightEdge() + xMove + e.collisionBox.GetWidth() / 2,
                                e.GetY());
                        }
                        else
                        {
                            e.MoveTo(
                                collisionBox.GetLeftEdge() + xMove - e.collisionBox.GetWidth() / 2,
                                e.GetY());
                        }
                    }
                }
            }

            //Moving the entity
            SetPossition(new Vector2(GetX() + moveMin, GetY()));
            if (xMove != moveMin)
                OnCollision(points, possitive ? Side.right : Side.left);

            //Adding new collision points to the list
            foreach(Vector2 v in points) collisionPoints.Add(v);
        }

        //Moves the entity along the y axis by a set distance
        public virtual void MoveY(float yMove)
        {
            if (yMove == 0) return;
            bool possitive = yMove > 0;

            //Creating rectangle representing the pathe the entity will move
            Rect movePath;
            if (possitive)
            {
                movePath = new Rect(collisionBox.GetLeftEdge(), collisionBox.GetBottomEdge(),
                    collisionBox.GetWidth(), yMove);
            }
            else
            {
                movePath = new Rect(collisionBox.GetLeftEdge(), collisionBox.GetTopEdge() + yMove,
                    collisionBox.GetWidth(), -yMove);
            }

            //Checking for collision with obstacles in the move path
            List<Vector2> points = new List<Vector2>();
            movePath.CheckCollision(levelstate.map, ignoreSemiSolid || !possitive, points, yMove);
            float moveMin = yMove;
            if (solid)
            {
                //Static terrain check
                foreach (Vector2 curPoint in points)
                {
                    if (possitive)
                    {
                        float dif = curPoint.Y - collisionBox.GetBottomEdge();
                        if (dif < moveMin) moveMin = dif;
                    }
                    else
                    {
                        float dif = curPoint.Y + levelstate.map.GetTileHeight() - collisionBox.GetTopEdge();
                        if (dif > moveMin) moveMin = dif;
                    }
                }

                //Wall entity check
                if (!ignoreWallEntities)
                {
                    foreach (Entity e in levelstate.entities["walls"])
                    {
                        if (e == this || e.dead) continue;
                        if (!collisionBox.Intersects(e.GetCollisionBox()) && e.GetCollisionBox().Intersects(movePath))
                        {
                            if (possitive)
                            {
                                float dif = e.GetCollisionBox().GetTopEdge() - collisionBox.GetBottomEdge();
                                if (dif < moveMin) moveMin = dif;
                            }
                            else
                            {
                                float dif = e.collisionBox.GetBottomEdge() - collisionBox.GetTopEdge();
                                if (dif > moveMin) moveMin = dif;
                            }
                        }
                    }
                }

                //SemiSolid platform entity check
                if (possitive && !ignoreSemiSolid)
                {
                    foreach (Entity e in levelstate.entities["platforms"])
                    {
                        if (e == this || e.dead) continue;
                        if (!collisionBox.Intersects(e.GetCollisionBox()) && e.GetCollisionBox().Intersects(movePath) && e.StandOnAble)
                        {
                            float dif = e.GetCollisionBox().GetTopEdge() - collisionBox.GetBottomEdge();
                            if(dif < moveMin)
                            {
                                moveMin = dif;
                            }
                        }
                    }
                }
            }

            //If this is a SemiSolid platform, checking for entities which should now be standing on this
            if (levelstate.entities["platforms"].Contains(this) && !possitive && !dead && StandOnAble)
            {
                movePath = new Rect(collisionBox.GetLeftEdge(), collisionBox.GetTopEdge() + yMove,
                    collisionBox.GetWidth(), -yMove);
                foreach (KeyValuePair<String, List<Entity>> pair in levelstate.entities)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        Entity e = pair.Value[i];
                        if (this == e) continue;
                        if (e.platform != this &&
                            e.IsSolid() && !e.GetIgnoreSemiSolid() &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                        {
                            e.SetPlatform(this);
                        }
                    }
                }
                if(levelstate.playa != this)
                {
                    Entity e = levelstate.playa;
                    if (e.platform != this &&
                            e.IsSolid() && !e.GetIgnoreSemiSolid() &&
                        e.IsSolid() && !e.GetIgnoreSemiSolid() &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                    {
                        e.SetPlatform(this);
                    }
                }
            }

            //If this is a wall, checking for entities which should be moved by this entity
            if (levelstate.entities["walls"].Contains(this) && !dead)
            {
                if (possitive)
                {
                    movePath = new Rect(collisionBox.GetLeftEdge(), collisionBox.GetBottomEdge(),
                        collisionBox.GetWidth(), yMove);
                }
                else
                {
                    movePath = new Rect(collisionBox.GetLeftEdge(), collisionBox.GetTopEdge() + yMove,
                        collisionBox.GetWidth(), -yMove);
                }
                foreach (KeyValuePair<String, List<Entity>> pair in levelstate.entities)
                {
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        Entity e = pair.Value[i];
                        if (this == e) continue;
                        if (e.platform != this &&
                            e.IsSolid() && !e.ignoreWallEntities &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                        {
                            if (possitive)
                            {
                                e.MoveY((collisionBox.GetBottomEdge() + yMove + e.GetCollisionBox().GetHeight() / 2) - e.GetPosition().Y);
                            }
                            else
                            {
                                e.SetPlatform(this);
                            }
                        }
                    }
                }
                if (levelstate.playa != this)
                {
                    Entity e = levelstate.playa;
                    if (e.platform != this &&
                            e.IsSolid() && !e.ignoreWallEntities &&
                            e.GetCollisionBox().Intersects(movePath) &&
                            !collisionBox.Intersects(e.GetCollisionBox()))
                    {
                        if (possitive)
                        {
                            e.MoveY((collisionBox.GetBottomEdge() + yMove + e.GetCollisionBox().GetHeight() / 2) - e.GetPosition().Y);
                        }
                        else
                        {
                            e.SetPlatform(this);
                        }
                    }
                }
            }

            //Moving the entity
            SetPossition(new Vector2(GetX(), GetY() + moveMin));
            if (yMove != moveMin)
            {
                if(possitive)
                {
                    if (!grounded) OnGround();
                    grounded = true;
                }
                OnCollision(points, possitive ? Side.bottom : Side.top);
            }
            else
            {
                grounded = false;
            }
            
            //Adding new collision points to the list
            foreach (Vector2 v in points) collisionPoints.Add(v);
        }

        //Called when the entity becomes grounded
        public abstract void OnGround();

        public virtual void OnCollision(List<Vector2> points, Side side, bool reset = true)
        {
            if(solid)
            {
                if((reset && !conserveYMomentum) && (side == Side.top || side == Side.bottom))
                {
                    yVel = 0;
                }
                else if((reset && !conserveXMomentum) && (side == Side.left || side == Side.right))
                {
                    xVel = 0;
                }
            }

            if (side == Side.bottom && stunCount == -1) stunCount = 0;

            foreach(Vector2 v in points)
            {
                collisionPoints.Add(v);
            }
        }

        public Hitbox GetCollisionBox()
        {
            return collisionBox;
        }

        public Hitbox GetSpriteBox()
        {
            return spriteBox;
        }

        public bool IsDead()
        {
            return dead;
        }

        public virtual void Heal(float heal)
        {
            health += heal;
            if (health > maxHealth) health = maxHealth;
        }

        protected virtual bool ShouldDie()
        {
            return health <= 0;
        }

        public virtual bool Hurt(Hitbox damager, float damageTaken = -1, bool ignoreInvulnerability = false)
        {
            if (!ignoreInvulnerability && (iFrames != 0 || invulnerable) || dead) return false;
            if (damageTaken < 0)
            {
                damageTaken = damager.GetDamage();
                if (damageTaken < 0) return false;
            }

            health -= damageTaken;
            if (health < 0) health = 0;
            if(ShouldDie())
            {
                Kill();
            }
            else
            {
                iFrames = maxIFrames;
                Knockback(damager);
                OnHurt(damager, damageTaken);
            }
            return true;
        }

        public void Kill()
        {
            if (dead) return;
            health = 0;
            dead = true;
            OnDeath();
        }

        public abstract void OnDeath();

        public void Knockback(Hitbox damager)
        {
            if (damager.GetXKnockback() != 0)
            {
                xVel = damager.GetXKnockback() * xKnockbackMultiplier *
                    (selfOrientKnockback ? -collisionBox.facing : damager.facing);
            }
            if (damager.GetYKnockback() != 0)
            {
                yVel = damager.GetYKnockback() * yKnockbackMultiplier;
            }
            if (stunable)
            {
                if (damager.GetStun() < 0) stunCount = -1;
                else if (damager.GetStun() > maxStun) stunCount = damager.GetStun();
                else stunCount = maxStun;
            }
        }

        public virtual void OnHurt(Hitbox damager, float damageTaken)
        {

        }

        public void SetXToSpeed(float speed = -1, bool applyBoost = true, float stunModifier = -1)
        {
            if (speed < 0) speed = this.speed;
            if (stunModifier < 0) stunModifier = this.stunModifier;
            if ((stunCount > 0 && stunModifier == 0) || dead) return;
            xVel = speed * collisionBox.facing * (applyBoost ? speedBoost : 1) * (stunCount > 0 ? stunModifier: 1);
        }

        public void AccelerateXToSpeed(bool applyBoost = true, float stunModifier = -1)
        {
            if (stunModifier < 0) stunModifier = this.stunModifier;
            if ((stunCount > 0 && stunModifier == 0) || dead) return;
            AccelerateX(speed * collisionBox.facing * (applyBoost ? speedBoost : 1) * (stunCount > 0 ? stunModifier : 1));
        }

        public void AccelerateX(float target, float acceleration = -1, float deacceleration = -1)
        {
            if (xVel != target)
            {
                bool possitive = xVel < target;
                int dir = collisionBox.facing;
                if (target - xVel > 0 == dir > 0)
                {
                    if (acceleration < 0) acceleration = this.acceleration;
                    xVel += acceleration * dir;
                    if (possitive != xVel < target)
                    {
                        xVel = target;
                    }
                }
                else
                {
                    if (deacceleration < 0) deacceleration = this.deacceleration;
                    xVel += deacceleration * -dir;
                    if (possitive != xVel < target)
                    {
                        xVel = target;
                    }
                }
            }
        }

        public void AccelerateY(float target, float acceleration = -1, float deacceleration = -1)
        {
            if (yVel != target)
            {
                bool possitive = yVel < target;
                int dir = possitive ? 1 : -1;
                if (target - yVel > 0 == dir > 0)
                {
                    if (acceleration < 0) acceleration = this.acceleration;
                    yVel += acceleration * dir;
                    if (possitive != yVel < target)
                    {
                        yVel = target;
                    }
                }
                else
                {
                    if (deacceleration < 0) deacceleration = this.deacceleration;
                    yVel += deacceleration * -dir;
                    if (possitive != yVel < target)
                    {
                        yVel = target;
                    }
                }
            }
        }

        public bool MoveTowards(Vector2 target, float speed = -1)
        {
            Vector2 possition = this.position;
            if (speed < 0) speed = this.speed;
            bool reached = false;
            if (Math.Abs(target.X - possition.X) < .1f && Math.Abs(target.Y - possition.Y) <= .1f) reached = true;
            else if (target.X != possition.X && target.Y != possition.Y)
            {
                float yDif = target.Y - possition.Y;
                float xDif = target.X - possition.X;
                float angle = (float)(Math.Atan(Math.Abs(yDif)
                        / Math.Abs(xDif)));
                int xDir = xDif > 0 ? 1 : -1;
                int yDir = yDif > 0 ? 1 : -1;
                float xMove = (float)(Math.Cos(angle) * speed * xDir);
                if (Math.Abs(xMove) >= Math.Abs(xDif))
                {
                    possition.X = target.X;
                }
                else possition.X += xMove;
                float yMove = (float)(Math.Sin(angle) * speed * yDir);
                if (Math.Abs(yMove) >= Math.Abs(yDif))
                {
                    possition.Y = (int)target.Y;
                }
                else possition.Y += yMove;
            }
            else if (Math.Abs(target.X - possition.X) < .1f)
            {
                float yDif = target.Y - possition.Y;
                if (Math.Abs(yDif) <= speed)
                {
                    possition.Y = target.Y;
                }
                else if (yDif > 0) possition.Y += speed;
                else possition.Y -= speed;
            }
            else
            {
                float xDif = target.X - possition.X;
                if (Math.Abs(xDif) <= speed)
                {
                    possition.X = target.X;
                }
                else if (xDif > 0) possition.X += speed;
                else possition.X -= speed;
            }
            if (Math.Abs(target.X - possition.X) < .1f && Math.Abs(target.Y - possition.Y) <= .1f)
            {
                reached = true;
            }
            if(reached)
            {
                possition.X = target.X;
                possition.Y = target.Y;
            }

            SetPossition(possition);
            return reached;
        }

        public bool AccelerateTowards(Vector2 target, float speed = -1, float acceleration = -1, bool useModifier = false)
        {
            Vector2 possition = this.position;
            if (speed < 0) speed = this.speed;
            if (acceleration < 0) acceleration = this.acceleration;
            if (useModifier)
            {
                float modifier = (1 - ((float)Math.Sqrt(xVel * xVel + yVel * yVel) / speed)) * .1f;
                acceleration -= modifier;
            }
            bool reached = false;
            if (Math.Abs(target.X - possition.X) < .1f && Math.Abs(target.Y - possition.Y) <= .1f) reached = true;
            else if (target.X != possition.X && target.Y != possition.Y)
            {
                float yDif = target.Y - possition.Y;
                float xDif = target.X - possition.X;
                float angle = (float)(Math.Atan(Math.Abs(yDif)
                        / Math.Abs(xDif)));
                int xDir = xDif > 0 ? 1 : -1;
                int yDir = yDif > 0 ? 1 : -1;
                float xMove = (float)(Math.Cos(angle) * acceleration * xDir);
                xVel += xMove;
                float yMove = (float)(Math.Sin(angle) * acceleration * yDir);
                yVel += yMove;
                float curVel = (float) Math.Sqrt(xVel * xVel + yVel * yVel);
                if(curVel > speed && ((xVel > 0) == (xDir > 0)) && ((yVel > 0) == (yDir > 0)))
                {
                    xVel = (float) Math.Cos(angle) * speed * xDir;
                    yVel = (float) Math.Sin(angle) * speed * yDir;
                }
                if(Math.Abs(yDif) < Math.Abs(yVel) && Math.Abs(xDif) < Math.Abs(xVel))
                {
                    reached = true;
                }
            }
            else if (Math.Abs(target.X - possition.X) < .1f)
            {
                float yDif = target.Y - possition.Y;
                if (yDif > 0)
                {
                    yVel += acceleration;
                }
                else
                {
                    yVel -= acceleration;
                }
                if (Math.Abs(yDif) < Math.Abs(yVel))
                {
                    reached = true;
                }
                else if (yVel > speed)
                {
                    yVel = speed;
                }
                else if (yVel < -speed)
                {
                    yVel = -speed;
                }
            }
            else
            {
                float xDif = target.X - possition.X;
                if (xDif > 0) xVel += acceleration;
                else xVel -= acceleration;
                if (Math.Abs(xDif) < Math.Abs(xVel))
                {
                    reached = true;
                }
                else if (xVel > speed) xVel = speed;
                else if (xVel < -speed) xVel = -speed;
            }
            if (Math.Abs(target.X - possition.X) <= 1 && Math.Abs(target.Y - possition.Y) <= 1)
            {
                reached = true;
            }
            if (reached)
            {
                SetPossition(target);
                xVel = 0; yVel = 0;
            }

            return reached;
        }

        public void LerpTo(Vector2 target, float lerpFactor = .1f, float endRange = .5f)
        {
            Vector2 dif = (target - position);
            if (dif.Length() >= endRange)
            {
                float angle = (float)Math.Atan2(dif.Y, dif.X);
                MoveX((float)Math.Cos(angle) * dif.Length() * lerpFactor);
                MoveY((float)Math.Sin(angle) * dif.Length() * lerpFactor);

                if (dif.Length() < endRange) SetPossition(target);
            }
            else
            {
                SetPossition(target);
            }
        }

        public float GetX()
        {
            return position.X;
        }

        public float GetY()
        {
            return position.Y;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(position.X, position.Y);
        }

        public virtual void SetPossition(Vector2 newPos)
        {
            position = new Vector2(newPos.X, newPos.Y);
            collisionBox.Update(position.X, position.Y);
            spriteBox.Update(position.X, position.Y);
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(int newMaxHealth)
        {
            maxHealth = newMaxHealth;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

        public void SquashAndStretch(int frames, float magnitude, 
            bool toGround = false, float min = 0, bool interrupt = true)
        {
            if(!interrupt && squashFrames > 0)
            if (frames < 0) frames = 0;
            squashFrames = frames;
            totalSquashFrames = frames;
            squashMax = magnitude;
            squashToGround = toGround;
            squashMin = min;
        }

        public void SetXVel(float xVel)
        {
            this.xVel = xVel;
        }

        public void SetYVel(float yVel)
        {
            this.yVel = yVel;
        }

        public float GetXVel()
        {
            return xVel;
        }

        public float GetYVel()
        {
            return yVel;
        }

        public bool IsOnScreen()
        {
            return levelstate.cam.OnScreen(this);
        }

        public float GetXDif()
        {
            return xDif;
        }

        public float GetYDif()
        {
            return yDif;
        }

        public void SetPlatform(Entity e)
        {
            if (platform == e) return;
            platform = e;
            PlaceOnTopOf(e);
        }

        public void PlaceOnTopOf(Entity e)
        {
            SetPossition(new Vector2(GetX(), GetY() + collisionBox.GetBottomEdge() - e.collisionBox.GetTopEdge()));
        }

        /**
        public void PlaceOnTopOf(Rect r)
        {
            SetPossition(new Vector2(GetX(), r.y - collisionBox.GetHeight() / 2 - 1));
        }
        **/

        public bool IsSolid()
        {
            return solid;
        }

        public bool GetIgnoreSemiSolid()
        {
            return ignoreSemiSolid;
        }

        public bool IsGrounded()
        {
            return grounded;
        }

        public void Gib()
        {
            for(int x = 0; x < ani.GetWidth(); x++)
            {
                for(int y = 0; y < ani.GetHeight(); y++)
                {
                    levelstate.entities["effects"].Add(new Gib(GetPosition() + new Vector2(x - ani.GetWidth() / 2f, y - ani.GetHeight() / 2f), ani.GetRGB(x, y), levelstate,
                        (float)(Math.PI / 3), 2f, 3f));
                }
            }
        }

        public virtual void Draw(Camera cam, float r = 255, float g = 255, float b = 255, float a = 1, float rotation = 0)
        {
            if (ani == null || !visible) return;
            if (!noFlash && iFrames % 16 > 7)
            {
                g = 0;
                b = 0;
            }

            Vector2 drawPossition = position - cam.GetDrawPosition() + new Vector2(drawXOffset, drawYOffset);

            float squashAmount = 0;
            if(squashFrames > 0 && spriteBox != null)
            {
                float progress = 1 - (((float) squashFrames) / totalSquashFrames);
                squashAmount = squashMin + (float) Math.Sin(Math.PI * progress) * (squashMax - squashMin);
                if (squashToGround)
                    drawPossition += new Vector2(0, squashAmount * spriteBox.GetHeight() / 2);
                squashFrames--;
            }

            ani.Draw(drawPossition, 
                flipHorizontally: collisionBox.facing == -1, flipVertically: collisionBox.yfacing == 1, 
                shake: stunCount > 0 ? drawShake: 0, r: r, g: g, b: b, a: a, 
                xScale: 1 + squashAmount, yScale: 1 - squashAmount, rotation: rotation, originXOffset: spriteOriginXOffset, originYOffset: spriteOriginYOffset);

            if (debug)
            {
                ResourceManager.DrawRect(new Rect(collisionBox.GetLeftEdge(), collisionBox.GetTopEdge(),
                    collisionBox.GetWidth(), collisionBox.GetHeight()), Color.Orange, cam);
            }
        }
    }
}

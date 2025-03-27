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
    class Player : Entity
    {
        public enum direction {up, down, left, right, none }
        public Player(Vector2 possition, LevelState levelstate, bool skipSpawnAnim = false) :
            base(possition, 8, 14, new AnimationManager("Miner", 24, new int[] { 14, 14, 14, 14, 14, 4 }, startFrames: new int[] { 0, 0, 0, 1, 1, 0 }), levelstate, maxHealth: 1)
        {
            inputBuffer = direction.none;
            curInput = direction.none;
            target = GetPosition();
            prevPos = GetPosition();
            lastGroundedPosition = GetPosition();
            prevGravity = gravity;

            xKnockbackMultiplier = 0;
            yKnockbackMultiplier = 0;

            drawShake = 2;
        }

        direction inputBuffer;
        direction curInput;
        Vector2 target, prevPos, lastGroundedPosition;
        int moveTimer = 0;
        float prevGravity;
        bool swung = false;
        bool heldFront = false;
        int maxChargeTime = 30;

        public bool active = false;
        bool started = false;

        public override void UpdateLogic()
        {
            if (!active) return;
            if (dead == true) return;
            if (levelstate.won) return;

            if (collisionBox.GetTopEdge() > levelstate.map.GetTileHeight() * levelstate.map.GetMapHeight())
            {
                levelstate.won = true;
                return;
            }

            if (curInput == direction.none)
            {
                swung = false;
                moveTimer = 0;
                if (grounded && inputBuffer != direction.none)
                {
                    curInput = inputBuffer;
                    inputBuffer = direction.none;
                    if (curInput == direction.left)
                    {
                        collisionBox.facing = -1;
                        ani.ResetAndSet(1);
                        target = GetPosition() + new Vector2(-16, 0);
                        prevPos = GetPosition();
                        gravity = 0;
                        levelstate.Tick();
                        heldFront = true;
                    }
                    else if (curInput == direction.right)
                    {
                        collisionBox.facing = 1;
                        ani.ResetAndSet(1);
                        target = GetPosition() + new Vector2(16, 0);
                        prevPos = GetPosition();
                        gravity = 0;
                        levelstate.Tick();
                        heldFront = true;
                    }
                    else if (curInput == direction.down)
                    {
                        ani.ResetAndSet(3);
                        levelstate.Tick();
                    }
                    else if (curInput == direction.up)
                    {
                        ani.ResetAndSet(4);
                        levelstate.Tick();
                    }
                }
            }
            else
            {
                if (curInput == direction.left || curInput == direction.right)
                {
                    if (heldFront)
                    {
                        if (curInput == direction.left && !InputManager.IsHeld("left") || curInput == direction.right && !InputManager.IsHeld("right")) heldFront = false;
                        else if (moveTimer >= maxChargeTime)
                        {
                            stunCount = 2;
                            if(moveTimer == maxChargeTime) ResourceManager.PlaySFX("Charge");
                        }
                    }
                    if (!heldFront && !swung && moveTimer >= maxChargeTime)
                    {
                        if (levelstate.map.BreakBlock(GetPosition() + new Vector2(16 * collisionBox.facing, 0), curInput))
                        {
                            if (levelstate.map.BreakBlock(GetPosition() + new Vector2(32 * collisionBox.facing, 0), curInput))
                            {
                                levelstate.map.BreakBlock(GetPosition() + new Vector2(48 * collisionBox.facing, 0), curInput);
                            }
                        }
                        target = new Vector2(GetPosition().X + 48 * collisionBox.facing, GetPosition().Y);
                        if (levelstate.map.IsSolid(GetPosition() + new Vector2(16 * collisionBox.facing, 0))) prevPos = GetPosition();
                        else if (levelstate.map.IsSolid(GetPosition() + new Vector2(32 * collisionBox.facing, 0))) prevPos = GetPosition() + new Vector2(16 * collisionBox.facing, 0);
                        else if (levelstate.map.IsSolid(GetPosition() + new Vector2(48 * collisionBox.facing, 0))) prevPos = GetPosition() + new Vector2(32 * collisionBox.facing, 0);
                        SquashAndStretch(6, .3f, true);
                        ani.SetCurrAni(2);
                        swung = true;
                        ResourceManager.PlaySFX("Swing");
                    }
                    else if (!heldFront && !swung && moveTimer >= 14)
                    {
                        levelstate.map.BreakBlock(GetPosition() + new Vector2(16 * collisionBox.facing, 0), curInput);
                        SquashAndStretch(6, .3f, true);
                        ani.SetCurrAni(2);
                        swung = true;
                        ResourceManager.PlaySFX("Swing");
                    }
                    if (swung) LerpTo(target, lerpFactor: .2f, endRange: 1);
                    if (GetPosition() == target)
                    {
                        ani.ResetAndSet(0);
                        curInput = direction.none;
                        gravity = prevGravity;
                    }
                    Rect test = new Rect(collisionBox.GetLeftEdge() - 1, collisionBox.GetTopEdge(), collisionBox.GetWidth() + 2, collisionBox.GetHeight());
                    if(test.CheckCollision(levelstate.map))
                    {
                        target = prevPos;
                    }
                    /*
                    if (levelstate.map.IsSolid(target.X, target.Y) && moveTimer == 20)
                    {
                        target = prevPos;
                    }
                    */
                }
                else if (curInput == direction.down || curInput == direction.up)
                {
                    if (!swung && ani.GetAnimation(ani.GetCurrAni()).GetFrame() == 1)
                    {
                        levelstate.map.BreakBlock(GetPosition() + new Vector2(0, 16 * (curInput == direction.down ? 1 : -1)), curInput);
                        yVel = -1;
                        SquashAndStretch(6, -.2f, true);
                        swung = true;
                        ResourceManager.PlaySFX("Swing");
                    }
                }
                moveTimer++;
            }
            base.UpdateLogic();
        }

        public override void UpdateMovement()
        {
            if (!active) return;
            if (dead == true) return;
            base.UpdateMovement();
            if(!grounded && curInput == direction.none && ani.GetCurrAni() != 4)
            {
                ani.ResetAndSet(5);
            }
            if(grounded)
            {
                lastGroundedPosition = GetPosition();
            }
        }

        public void UpdateInput()
        {
            if ((curInput == direction.none || swung) && started)
            {
                if (InputManager.IsPressed("up")) inputBuffer = direction.up;
                if (InputManager.IsPressed("down")) inputBuffer = direction.down;
                if (InputManager.IsPressed("left")) inputBuffer = direction.left;
                if (InputManager.IsPressed("right")) inputBuffer = direction.right;
            }
        }

        public Vector2 GetLastGroundedPosition()
        {
            return new Vector2(lastGroundedPosition.X, lastGroundedPosition.Y);
        }

        public override void Draw(Camera cam, float r = 255, float g = 255, float b = 255, float a = 1, float rotation = 0)
        {
            if (dead) return;
            base.Draw(cam, r, g, b, a, rotation);
        }

        public override void OnGround()
        {
            if(curInput == direction.down || curInput == direction.up)
            {
                curInput = direction.none;
                ani.ResetAndSet(0);
            }
            if(ani.GetCurrAni() == 5)
            {
                ani.ResetAndSet(0);
            }
            if(!started)
            {
                started = true;
                ResourceManager.PlaySong("MinerKey", true);
            }
        }

        public override void OnDeath()
        {
            Gib();
            levelstate.cam.ScreenShake();
            ResourceManager.PlaySFX("Explosion");
        }
    }
}

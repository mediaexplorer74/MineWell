using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    class Camera
    {
        Map map;
        Vector2 possition;
        Hitbox screen;
        float lerpFactor = .1f;
        int screenShakeCounter = 0;
        int screenShakeMagnitude = 0;
        private Vector2 drawPosition;

        public Camera(float x, float y, Map map)
        {
            possition = new Vector2(x, y);
            screen = new Hitbox(possition.X, possition.Y, Game1.width / 2, Game1.height / 2, Game1.width, Game1.height, null);
            this.map = map;
            UpdateScreen();
            drawPosition = GetDrawPosition();
        }

        public void Update()
        {
            drawPosition = GetPosition();
            if(screenShakeCounter > 0 && screenShakeMagnitude > 0)
            {
                drawPosition += new Vector2(
                    ((ResourceManager.random.Next(3) - 1) * screenShakeMagnitude),
                    ((ResourceManager.random.Next(3) - 1) * screenShakeMagnitude));
                screenShakeCounter--;
            }
        }

        public void UpdateScreen()
        {
            screen.Update(possition.X, possition.Y);
        }

        public Vector2 GetPosition()
        {
            return new Vector2((int)possition.X, (int)possition.Y);
        }

        public Vector2 GetDrawPosition()
        {
            return new Vector2((int)drawPosition.X, (int)drawPosition.Y);
        }

        public void CenterOn(float x, float y)
        {
            Vector2 center = GetInBoundsCenter(x, y);

            possition.X = center.X - Game1.width / 2;
            possition.Y = center.Y - Game1.height / 2;
            UpdateScreen();
        }

        public void CenterOn(Vector2 point)
        {
            CenterOn(point.X, point.Y);
        }

        public void LerpTo(float x, float y)
        {
            Vector2 point = GetInBoundsCenter(x, y);
            float targetX = (point.X - Game1.width / 2);
            float targetY = (point.Y - Game1.height / 2);
            point.X = (targetX - possition.X) * lerpFactor;
            point.Y = (targetY - possition.Y) * lerpFactor;

            possition += point;
            if (Math.Abs(possition.X - targetX) <= .5f) possition.X = targetX;
            if (Math.Abs(possition.Y - targetY) <= .5f) possition.Y = targetY;
            UpdateScreen();
        }

        public void LerpTo(Vector2 point)
        {
            LerpTo(point.X, point.Y);
        }

        private Vector2 GetInBoundsCenter(float x, float y)
        {
            if (x - Game1.width / 2 < 0) x = Game1.width / 2;
            else if (x + Game1.width / 2 > map.GetMapWidth() * map.GetTileWidth()) x = map.GetMapWidth() * map.GetTileWidth() - Game1.width / 2;

            if (y - Game1.height / 2 < 0) y = Game1.height / 2;
            else if (y + Game1.height / 2 > map.GetMapHeight() * map.GetTileHeight()) y = map.GetMapHeight() * map.GetTileHeight() - Game1.height / 2;

            return new Vector2(x, y);
        }

        public bool OnScreen(Entity e)
        {
            return e.GetSpriteBox().Intersects(screen);
        }

        public void ScreenShake(int time = 5, int magnitude = 2)
        {
            screenShakeCounter = time;
            screenShakeMagnitude = magnitude;
        }
    }
}

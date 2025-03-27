using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager.Pickups
{
    abstract class Pickup : Entity
    {
        public Pickup(Vector2 possition, string name, LevelState levelstate) : base(possition, 16, 16, new AnimationManager(name, 16, new int[] { 7 }), levelstate, 0, false)
        {
            floatTimer = (float)ResourceManager.random.NextDouble();
        }

        float floatTimer = 0;
        float floatRate = .01f;
        float floatAmplitude = 1.9f;

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if(collisionBox.Intersects(levelstate.playa.GetCollisionBox()))
            {
                OnPickup();
                removeMe = true;
            }
        }

        protected abstract void OnPickup();

        public override void OnDeath()
        {

        }

        public override void OnGround()
        {

        }

        public override void Draw(Camera cam, float r = 255, float g = 255, float b = 255, float a = 1, float rotation = 0)
        {
            floatTimer += floatRate;
            floatTimer %= 1;
            Camera temp = new Camera(cam.GetPosition().X, cam.GetPosition().Y + (float)(floatAmplitude * Math.Sin(floatTimer * 2 * Math.PI)), levelstate.map);
            temp.Update();
            base.Draw(temp, r, g, b, a, rotation);
        }
    }
}

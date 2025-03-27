using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    class Hitbox
    {
        float xOff, yOff;
        Rect box;
        Entity owner;
        public int facing;
        public int yfacing;
        float damage = 1;
        float xKnockback = 1;
        float yKnockback = -1;
        int stun = 0;

        public Hitbox(float x, float y, float xOff, float yOff, float width, float height,
            Entity owner, int facing = 1, int yfacing = -1)
        {
            this.xOff = xOff - width/2;
            this.yOff = yOff - height/2;

            box = new Rect(x + this.xOff, y + this.yOff, width, height);

            this.owner = owner;

            this.facing = facing;
            this.yfacing = yfacing;
        }

        public void Update(float x, float y)
        {
            box.MoveTo(x + xOff, y + yOff);
        }

        public bool Intersects(Hitbox other)
        {
            return box.Intersects(other.box);
        }

        public bool XOverlap(Hitbox other)
        {
            return box.XOverlap(other.box);
        }

        public bool YOverlap(Hitbox other)
        {
            return box.YOverlap(other.box);
        }

        public bool Intersects(Rect other)
        {
            return box.Intersects(other);
        }

        public List<Vector2> MapIntersections(Map map)
        {
            return box.MapIntersections(map);
        }

        public bool CheckCollision(Map map, bool ignoreSemi = true, List<Vector2> points = null, float yDif = 0)
        {
            return box.CheckCollision(map, ignoreSemi, points, yDif);
        }

        public float GetWidth()
        {
            return box.width;
        }

        public float GetHeight()
        {
            return box.height;
        }

        public float GetLeftEdge()
        {
            return box.x;
        }

        public float GetRightEdge()
        {
            return box.x + box.width;
        }

        public float GetXFacingEdge()
        {
            return box.x + (facing == 1 ? box.width : 0);
        }

        public float GetTopEdge()
        {
            return box.y;
        }

        public float GetBottomEdge()
        {
            return box.y + box.height;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(box.x + box.width / 2, box.y + box.height / 2);
        }

        public Entity GetOwner()
        {
            return owner;
        }

        public float GetDamage()
        {
            return damage;
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        public float GetXKnockback()
        {
            return xKnockback;
        }

        public void SetXKnockback(float xKnockback)
        {
            this.xKnockback = xKnockback;
        }

        public float GetYKnockback()
        {
            return yKnockback;
        }

        public void SetYKnockback(float yKnockback)
        {
            this.yKnockback = yKnockback;
        }

        public int GetStun()
        {
            return stun;
        }

        public void SetStun(int stun)
        {
            this.stun = stun;
        }
    }
}

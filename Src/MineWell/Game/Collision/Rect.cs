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
    class Rect
    {
        public float x, y, width, height;

        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        struct Bounds
        {
            public Bounds(float left, float right, float top, float bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }

            public float left;
            public float right;
            public float top;
            public float bottom;
        }

        public bool IsEmpty()
        {
            return width <= 0 || height <= 0;
        }

        private Bounds GetBounds()
        {
            return new Bounds(x, x + width, y, y + height);
        }

        public bool Intersects(Rect other)
        {
            if(IsEmpty() || other.IsEmpty()) return false;

            Bounds thisBounds = GetBounds();
            Bounds otherBounds = other.GetBounds();

            return (
                (thisBounds.left < otherBounds.right) &&
                (thisBounds.right > otherBounds.left) &&
                (thisBounds.top < otherBounds.bottom) &&
                (thisBounds.bottom > otherBounds.top));
        }

        public bool XOverlap(Rect other)
        {
            if (IsEmpty() || other.IsEmpty()) return false;

            Bounds thisBounds = GetBounds();
            Bounds otherBounds = other.GetBounds();

            return (
                (thisBounds.left < otherBounds.right) &&
                (thisBounds.right > otherBounds.left));
        }

        public bool YOverlap(Rect other)
        {
            if (IsEmpty() || other.IsEmpty()) return false;

            Bounds thisBounds = GetBounds();
            Bounds otherBounds = other.GetBounds();

            return (
                (thisBounds.top < otherBounds.bottom) &&
                (thisBounds.bottom > otherBounds.top));
        }

        public List<Vector2> MapIntersections(Map map)
        {
            List<Vector2> points = new List<Vector2>();

            for (float x = map.RoundX(this.x); x < this.x + this.width; x += map.GetTileWidth())
            {
                for (float y = map.RoundY(this.y); y < this.y + this.height; y += map.GetTileHeight())
                {
                    points.Add(new Vector2(map.RoundX(x), map.RoundY(y)));
                }
            }

            return points;
        }

        public bool CheckCollision(Map map, bool ignoreSemi = true, List<Vector2> points = null, float yDif = 0)
        {

            for (float x = map.RoundX(this.x); x < this.x + this.width; x += map.GetTileWidth())
            {
                for (float y = map.RoundY(this.y); y < this.y + this.height; y += map.GetTileHeight())
                {
                    if (map.IsSolid(x, y) || !ignoreSemi && map.IsSemiSolid(x, y) && this.y + height - yDif - y <= .1f)
                    {
                        if (points == null) return true;
                        points.Add(new Vector2(map.RoundX(x), map.RoundY(y)));
                    }
                }
            }

            if(points == null || points.Count() == 0) return false;
            return true;
        }

        public void MoveTo(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void MoveBy(float x, float y)
        {
            this.x += x;
            this.y += y;
        }
    }
}

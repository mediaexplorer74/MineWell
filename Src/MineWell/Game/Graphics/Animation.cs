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
    class Animation
    {
        String name;
        int delay;
        int offset;
        int numFrames;
        int width;
        int height;
        int startFrame;
        int currFrame = 0;
        int count = 0;

        public Animation(String name, int delay,
            int offset, int numFrames, int width, int height, int startFrame = 0)
        {
            this.name = name;
            this.delay = delay;
            this.offset = offset;
            this.numFrames = numFrames;
            this.width = width;
            this.height = height;
            this.startFrame = startFrame;
        }

        public void Draw(Vector2 possition,
            float xScale = 1, float yScale = 1, float scale = 1, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0, float shake = 0,
            float r = 255, float g = 255, float b = 255, float a = 255, float originXOffset = 0, float originYOffset = 0)
        {
            if(shake > 0)
            {
                float x = possition.X;
                float y = possition.Y;
                x += (float) ResourceManager.random.NextDouble() * shake - (shake / 2);
                y += (float)ResourceManager.random.NextDouble() * shake - (shake / 2);
                possition = new Vector2(x, y);
            }
            ResourceManager.DrawSubTexture(name, possition, currFrame * width, offset, width, height,
                xScale, yScale, scale, rotation, flipHorizontally, flipVertically, layerDepth, (int)r, (int)g, (int)b, a, originXOffset, originYOffset);
            Update();
        }

        public void Update()
        {
            if (ResourceManager.animationsPaused) return;
            SetCount(count + 1);
        }

        public int GetCount()
        {
            return count;
        }

        public void SetCount(int count)
        {
            this.count = count;
            if (count >= delay)
            {
                SetFrame(currFrame + 1);
            }
        }

        public int GetFrame()
        {
            return currFrame;
        }

        public void SetFrame(int frame)
        {
            count = 0;
            currFrame = frame;
            if(currFrame >= numFrames)
            {
                currFrame = startFrame;
            }
        }

        public void Synch(Animation other)
        {
            SetFrame(other.GetFrame());
            SetCount(other.GetCount());
        }

        public void Reset()
        {
            SetFrame(0);
            SetCount(0);
        }

        public Color GetRGB(int x, int y, int frame = -1)
        {
            if (frame < 0) frame = currFrame;
            return ResourceManager.GetTexturePixelColor(name, frame * width + x, offset + y);
        }
    }
}

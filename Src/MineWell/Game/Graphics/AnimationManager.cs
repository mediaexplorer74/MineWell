using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    class AnimationManager
    {
        String name;
        List<Animation> animations;
        int currAni = 0;
        int width, height;
        static Dictionary<string, int[]> frameNums = new Dictionary<string, int[]>();

        public AnimationManager(String name, int width,
            int[] delay, int[] numFrames = null, int[] startFrames = null)
        {
            
            this.name = name;
            animations = new List<Animation>();
            int numAnimations = delay.Length;
            this.width = width;
            if (name == "null")
            {
                height = 1;
                return;
            }
            height = ResourceManager.GetTexture(name).Height / delay.Length;
            if (frameNums.ContainsKey(name))
            {
                numFrames = frameNums[name];
            }
            int[] curFrameNums = new int[numAnimations];
            for (int i = 0; i < numAnimations; i++)
            {
                int currNumFrames;
                if(numFrames == null)
                {
                    currNumFrames = GetNumFrames(width, height, i * height);
                }
                else
                {
                    currNumFrames = numFrames[i];
                }
                animations.Add(new Animation(name, delay[i], 
                    i * height, currNumFrames, width, height, startFrames == null ? 0 : startFrames[i]));
                curFrameNums[i] = currNumFrames;
            }
            if (!frameNums.ContainsKey(name))
            {
                frameNums[name] = curFrameNums;
            }
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        private int GetNumFrames(int width, int height, int offset)
        {
            int numFrames = 0;
            for (int frame = 0; frame < ResourceManager.GetTexture(name).Width / width; frame++)
            {
                bool found = false;
                for (int x = 0; !found && x < width; x++)
                {
                    for (int y = 0; !found && y < height; y++)
                    {
                        if (ResourceManager.GetTexturePixelColor(name, x + frame * width, y + offset).A != 0)
                        {
                            found = true;
                            numFrames++;
                        }
                    }
                }
                if (!found) break;
            }
            return numFrames;
        }

        public Animation GetAnimation(int index)
        {
            return animations[index];
        }

        public void Synch(int ani1, int ani2)
        {
            animations[ani1].Synch(animations[ani2]);
        }

        public void Reset(int ani)
        {
            animations[ani].Reset();
        }

        public void Reset()
        {
            foreach(Animation ani in animations)
            {
                ani.Reset();
            }
        }

        public int GetCurrAni()
        {
            return currAni;
        }

        public void SetCurrAni(int ani)
        {
            currAni = ani;
        }

        public void SynchAndSet(int ani)
        {
            Synch(ani, currAni);
            currAni = ani;
        }

        public void ResetAndSet(int ani)
        {
            animations[ani].Reset();
            currAni = ani;
        }

        public Color GetRGB(int x, int y, int ani = -1)
        {
            if (ani < 0) ani = currAni;
            return animations[ani].GetRGB(x, y);
        }

        public void Draw(Vector2 possition,
            float xScale = 1, float yScale = 1, float scale = 1, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0, float shake = 0,
            float r = 255, float g = 255, float b = 255, float a = 255, float originXOffset = 0, float originYOffset = 0)
        {
            animations[currAni].Draw(possition, xScale, yScale, scale, rotation,
                flipHorizontally, flipVertically, layerDepth, shake, r, g, b, a, originXOffset, originYOffset);
        }
    }
}

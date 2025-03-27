using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    static class ResourceManager
    {
        //Visuals
        static SpriteBatch spriteBatch;
        static public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        static public Dictionary<string, Color[,]> colorMaps = new Dictionary<string, Color[,]>();
        static public bool animationsPaused = false;

        //Audio
        static public Dictionary<string, Song> songs = new Dictionary<string, Song>();
        static public Dictionary<string, SoundEffect> sfx = new Dictionary<string, SoundEffect>();
        static private Dictionary<string, List<SoundEffectInstance>> sfxInstances = new Dictionary<string, List<SoundEffectInstance>>();
        private const int MAXSOUNDS = 20;
        static private string curSong = "";
        static private float songVolume = 1f;
        static private float sfxVolume = 1f;
        static private float masterVolume = .3f;
        static private float curSongVolume = 1;
        private static bool JustSetSong = false;
        private static bool soundPaused = false;
        private static float songVolumeModifier = 1;
        private static int songDipCount = 0;
        private static float songDipVol = .3f;
        private static string queueName = "";
        private static bool queueLoop = true;
        private static float queueVolume = 1;
        private static bool queueSong = false;

        //Utility
        static public Random random = new Random();

        public static void SetSpriteBatch(SpriteBatch sb)
        {
            spriteBatch = sb;
        }

        public static void AddTexture(String name, Texture2D texture)
        {
            textures[name] = texture;
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(0, new Rectangle(0, 0, texture.Width, texture.Height), 
                colorData, 0, texture.Width * texture.Height);
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colorData[x + y * texture.Width];
                }
            }
            colorMaps[name] = colors2D;
        }

        public static Texture2D GetTexture(String name)
        {
            return textures[name];
        }

        public static Color GetTexturePixelColor(String name, int x, int y)
        {
            return colorMaps[name][x, y];
        }

        public static void DrawTexture(String name, Vector2 possition,
            float xScale = 1, float yScale = 1, float scale = 1, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0,
            int r = 255, int g = 255, int b = 255, float a = 1, float originXOffset = 0, float originYOffset = 0)
        {
            spriteBatch.Draw(textures[name],
                new Rectangle((int)(possition.X + originXOffset), (int)(possition.Y + originYOffset), 
                (int)(textures[name].Width * xScale * scale),(int)(textures[name].Height * yScale * scale)), null,
                new Color(r, g, b) * a, rotation, new Vector2(textures[name].Width / 2 + originXOffset, textures[name].Height / 2 + originYOffset),
                (flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth);
        }

        public static void DrawTextureScaled(String name, Vector2 possition,
            int width, int height, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0,
            int r = 255, int g = 255, int b = 255, float a = 1, float originXOffset = 0, float originYOffset = 0)
        {
            spriteBatch.Draw(textures[name],
                new Rectangle((int)(possition.X + originXOffset), (int)(possition.Y + originYOffset), width, height), null,
                new Color(r, g, b) * a, rotation, new Vector2(textures[name].Width / 2 + originXOffset, textures[name].Height / 2 + originYOffset),
                (flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth);
        }

        public static void DrawSubTexture(String name, Vector2 possition,
            int x, int y, int width, int height,
            float xScale = 1, float yScale = 1, float scale = 1, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0, 
            int r = 255, int g = 255, int b = 255, float a = 1, float originXOffset = 0, float originYOffset = 0)
        {
            spriteBatch.Draw(textures[name],
                new Rectangle((int)(possition.X + originXOffset), (int)(possition.Y + originYOffset),
                (int)(width * xScale * scale), (int)(height * yScale * scale)),
                new Rectangle(x, y, width, height),
                new Color(r, g, b) * a, rotation, new Vector2(width / 2 + originXOffset, height / 2 + originYOffset),
                (flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth);
        }

        public static void DrawRect(Rect rect, Color color, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0)
        {
            spriteBatch.Draw(textures["Pixel"],
                new Rectangle((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height), null,
                color, rotation, new Vector2(textures["Pixel"].Width / 2, textures["Pixel"].Height / 2),
                (flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth);
        }

        public static void DrawRect(Rect rect, Color color, Camera cam, float rotation = 0,
            bool flipHorizontally = false, bool flipVertically = false, float layerDepth = 0)
        {
            spriteBatch.Draw(textures["Pixel"],
                new Rectangle((int)(rect.x - cam.GetDrawPosition().X), (int)(rect.y - cam.GetDrawPosition().Y),
                (int)rect.width, (int)rect.height), null,
                color, rotation, new Vector2(textures["Pixel"].Width / 2, textures["Pixel"].Height / 2),
                (flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth);
        }

        public static void DrawTextCentered(int y, string text, string font, int leading = 2, int r = 255, int g = 255, int b = 255, float a = 1, int xOffset = 0)
        {
            int width = textures[font].Width;
            int x = (int)(Game1.width / 2f - text.Length * width / 2f + width / 2f);
            DrawText(new Vector2(x + xOffset, y), text, font, leading, r, g, b, a);
        }

        public static void DrawText(Vector2 position, string text, string font, int leading = 2, int r = 255, int g = 255, int b = 255, float a = 1)
        {
            string upperText = text.ToUpper();
            char[] textArray = upperText.ToCharArray();
            float xOffset = 0;
            float yOffset = 0;
            int charWidth = textures[font].Width;
            int charHeight = textures[font].Height / 64;
            for (int i = 0; i < textArray.Length; i++)
            {
                if (textArray[i] == '\n')
                {
                    xOffset = 0;
                    yOffset += charHeight + leading;
                }
                else
                {
                    int curChar = textArray[i] - 32;
                    if (curChar < 0 || curChar > 63) curChar = 31;
                    DrawSubTexture(font, position + new Vector2(xOffset, yOffset), 0, charHeight * curChar, charWidth, charHeight, r: r, g: g, b: b, a: a);
                    xOffset += charWidth;
                }
            }
        }

        public static void DrawTextCentered(Vector2 position, string text, string font, int leading = 2, int r = 255, int g = 255, int b = 255, float a = 1)
        {
            int charWidth = textures[font].Width;
            DrawText(position + new Vector2(-(charWidth * (text.Length - 1)) / 2f, 0), text, font, leading, r, g, b, a);
        }

        public static void SetUpLooper()
        {
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        public static void AddSong(string name, Song song)
        {
            songs[name] = song;
        }

        public static void QueueSong(string name, bool loop = false, float volume = 1)
        {
            queueName = name;
            queueLoop = loop;
            queueVolume = volume;
            queueSong = true;
        }

        public static void PlaySong(string name, bool loop = false, float volume = 1)
        {
            if (curSong == name && (MediaPlayer.State == MediaState.Playing || MediaPlayer.State == MediaState.Paused)) return;
            curSongVolume = volume;
            volume *= songVolume * masterVolume;
            StopSong();
            MediaPlayer.IsRepeating = loop && !songs.ContainsKey(name + " Intro");
            MediaPlayer.Volume = volume;
            curSong = name;
            JustSetSong = true;
            MediaPlayer.Play(songs[name]);
        }

        public static void StopSong()
        {
            curSong = "";
            MediaPlayer.Stop();
        }

        static void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            if (soundPaused) return;
            if(!JustSetSong && songs.ContainsKey(curSong + " Loop"))
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(songs[curSong + " Loop"]);
            }

            JustSetSong = false;
        }

        public static bool IsSongPlaying()
        {
            return MediaPlayer.State == MediaState.Playing;
        }

        public static void AddSFX(string name, SoundEffect sound)
        {
            sfx[name] = sound;
            sfxInstances[name] = new List<SoundEffectInstance>();
            for(int i = 0; i < MAXSOUNDS; i++)
            {
                sfxInstances[name].Add(sfx[name].CreateInstance());
            }
        }

        public static void PlaySFX(string name, float volume = 1f, float pitch = 0, float pan = 0, bool jingle = false, bool song = false)
        {
            if (!sfx.ContainsKey(name)) return;
            volume *= (song ? songVolume : sfxVolume) * masterVolume;
            SoundEffectInstance instance = sfxInstances[name][0];
            sfxInstances[name].RemoveAt(0);
            sfxInstances[name].Add(instance);
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.Play();

            if(jingle)
            {
                songDipCount = (int)(sfx[name].Duration.TotalSeconds * 60);
            }
        }

        public static void SetSongVolume(float volume)
        {
            if (volume < 0) volume = 0;
            else if (volume > 1) volume = 1;
            songVolume = volume;
            UpdateSongVolume();
        }

        private static void UpdateSongVolume()
        {
            MediaPlayer.Volume = songVolume * masterVolume * songVolumeModifier;
        }

        public static float GetSongVolume()
        {
            return songVolume;
        }

        public static void SetSFXVolume(float volume)
        {
            if (volume < 0) volume = 0;
            else if (volume > 1) volume = 1;
            sfxVolume = volume;
        }

        public static float GetSFXVolume()
        {
            return sfxVolume;
        }

        public static void SetMasterVolume(float volume)
        {
            if (volume < 0) volume = 0;
            else if (volume > 1) volume = 1;
            masterVolume = volume;
            MediaPlayer.Volume = songVolume * masterVolume;
        }

        public static float GetMasterVolume()
        {
            return masterVolume;
        }

        public static void PauseSound()
        {
            if(!soundPaused)
            {
                soundPaused = true;
                if(MediaPlayer.State == MediaState.Playing) MediaPlayer.Pause();
                foreach(KeyValuePair<string, List<SoundEffectInstance>> pair in sfxInstances)
                {
                    foreach(SoundEffectInstance s in pair.Value)
                    {
                        if (s.State == SoundState.Playing) s.Pause();
                    }
                }
            }
        }

        public static void UnpauseSound()
        {
            if (soundPaused)
            {
                if(MediaPlayer.State == MediaState.Paused)MediaPlayer.Resume();
                foreach (KeyValuePair<string, List<SoundEffectInstance>> pair in sfxInstances)
                {
                    foreach (SoundEffectInstance s in pair.Value)
                    {
                        if (s.State == SoundState.Paused) s.Resume();
                    }
                }
                soundPaused = false;
            }
        }

        public static void Update()
        {
            if (!soundPaused)
            {
                if (songDipCount > 0)
                {
                    songDipCount--;
                    if (songVolumeModifier > songDipVol)
                    {
                        songVolumeModifier -= .05f;
                        if (songVolumeModifier < songDipVol) songVolumeModifier = songDipVol;
                        UpdateSongVolume();
                    }
                }
                else if (songVolumeModifier < 1)
                {
                    songVolumeModifier += .1f;
                    if (songVolumeModifier > 1) songVolumeModifier = 1;
                    UpdateSongVolume();
                }
                if(queueSong)
                {
                    PlaySong(queueName, queueLoop, queueVolume);
                    queueSong = false;
                }
            }
        }
    }
}

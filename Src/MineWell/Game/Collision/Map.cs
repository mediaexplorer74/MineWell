using MineWell.Effects;
using MineWell.Enemies;
using MineWell.Pickups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MineWell
{
    class Map
    {
        int[,] map;
        int[,] dirt;
        int[,] superDirt;
        int[,] crack;
        int mapWidth, mapHeight, tileWidth, tileHeight;
        String tileset;
        LevelState levelstate;
        String name;

        public Map(String name, String tileset, int tileWidth, int tileHeight, LevelState levelstate)
        {
            this.name = name;
            this.levelstate = levelstate;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.tileset = tileset;
            LoadBlockMap(name);
        }

        public void Update()
        {

        }

        public List<int[]> LoadCSV(string name)
        {
            String path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Maps\" + name + ".csv");
            StreamReader reader = new StreamReader(path);
            List<int[]> layers = new List<int[]>();

            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                String[] layerS = line.Split(',');
                int[] layerI = new int[layerS.Length];
                for (int i = 0; i < layerS.Length; i++)
                {
                    layerI[i] = Convert.ToInt32(layerS[i]);
                }
                layers.Add(layerI);
            }

            return layers;
        }

        public void LoadBlockMap(string name)
        {
            List<int[]> blockLayers = LoadCSV(name + "_Blocks");
            List<int[]> dirtLayers = LoadCSV(name + "_Dirt");
            List<int[]> superDirtLayers = LoadCSV(name + "_SuperDirt");

            mapWidth = blockLayers[0].Length;
            mapHeight = blockLayers.Count;
            map = new int[mapWidth, mapHeight];
            dirt = new int[mapWidth, mapHeight];
            superDirt = new int[mapWidth, mapHeight];
            crack = new int[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    superDirt[x, y] = superDirtLayers[y][x];
                    if (superDirt[x, y] == -1) dirt[x, y] = dirtLayers[y][x];
                    else dirt[x, y] = -1;
                    if (superDirt[x, y] == -1 && dirt[x, y] == -1) map[x, y] = blockLayers[y][x];
                    else map[x, y] = -1;
                    crack[x, y] = -1;
                }
            }
        }

        public void LoadEntities()
        {
            List<int[]> layers = LoadCSV(name + "_Entities");
            
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (layers[y][x] != -1)
                    {
                        int entityID = layers[y][x];
                        int entityX = x * tileWidth + tileWidth / 2;
                        int entityY = y * tileHeight + tileHeight / 2;

                        //Load Entities Here
                        if(entityID == 0)
                        {
                            levelstate.spawnPoint = new Vector2(entityX, entityY + 1 - 64);
                        }

                        if (entityID == 9)
                        {
                            levelstate.entities["enemies"].Add(new Bug(new Vector2(entityX, entityY), 1, levelstate));
                        }
                        if (entityID == 10)
                        {
                            levelstate.entities["enemies"].Add(new Bug(new Vector2(entityX, entityY), -1, levelstate));
                        }

                        if (entityID == 18)
                        {
                            levelstate.entities["pickups"].Add(new Ore(new Vector2(entityX, entityY), "Diamond", 6, 30, levelstate));
                        }
                        if (entityID == 19)
                        {
                            levelstate.entities["pickups"].Add(new Ore(new Vector2(entityX, entityY), "Emerald", 6, 20, levelstate));
                        }
                        if (entityID == 20)
                        {
                            levelstate.entities["pickups"].Add(new Ore(new Vector2(entityX, entityY), "Ruby", 5, 10, levelstate));
                        }

                        if (entityID == 27)
                        {
                            levelstate.entities["pickups"].Add(new Gem(new Vector2(entityX, entityY), "Diamond", 1000, levelstate));
                        }
                        if (entityID == 28)
                        {
                            levelstate.entities["pickups"].Add(new Gem(new Vector2(entityX, entityY), "Emerald", 500, levelstate));
                        }
                        if (entityID == 29)
                        {
                            levelstate.entities["pickups"].Add(new Gem(new Vector2(entityX, entityY), "Ruby", 250, levelstate));
                        }

                        if (entityID == 36)
                        {
                            levelstate.entities["pickups"].Add(new DynamitePickup(new Vector2(entityX, entityY), levelstate));
                        }

                        if (entityID == 45)
                        {
                            levelstate.entities["enemies"].Add(new Spikes(new Vector2(entityX, entityY), levelstate));
                        }
                    }
                }
            }
        }

        public int GetValue(int x, int y)
        {
            return map[x, y];
        }

        public void SetValue(int x, int y, int value)
        {
            map[x, y] = value;
        }

        public void DrawTiles(Camera cam)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] != -1)
                    {
                        ResourceManager.DrawSubTexture(tileset,
                            new Vector2(x * tileWidth + tileWidth / 2, y * tileHeight + tileHeight / 2) - cam.GetDrawPosition(),
                            (map[x, y] % 32) * tileWidth, (map[x, y] / 32) * tileHeight, tileWidth, tileHeight);
                    }
                    if (dirt[x, y] != -1)
                    {
                        ResourceManager.DrawSubTexture(tileset,
                            new Vector2(x * tileWidth + tileWidth / 2, y * tileHeight + tileHeight / 2) - cam.GetDrawPosition(),
                            (dirt[x, y] % 32) * tileWidth, (dirt[x, y] / 32) * tileHeight, tileWidth, tileHeight);
                    }
                    if (superDirt[x, y] != -1)
                    {
                        ResourceManager.DrawSubTexture(tileset,
                            new Vector2(x * tileWidth + tileWidth / 2, y * tileHeight + tileHeight / 2) - cam.GetDrawPosition(),
                            (superDirt[x, y] % 32) * tileWidth, (superDirt[x, y] / 32) * tileHeight, tileWidth, tileHeight);
                    }
                    if (crack[x, y] != -1)
                    {
                        ResourceManager.DrawTexture("Crack", new Vector2(x * tileWidth + tileWidth / 2, y * tileHeight + tileHeight / 2) - cam.GetDrawPosition());
                    }
                }
            }
        }

        public void Draw(Camera cam)
        {
            DrawTiles(cam);
        }

        public bool Inbounds(float x, float y)
        {
            return (x >= 0 && x < mapWidth * tileWidth && y >= 0 && y < mapHeight * tileHeight);
        }

        public bool IsSolid(float x, float y)
        {
            if (!Inbounds(x, y)) return false;
            int value1 = map[(int)(x / tileWidth), (int)(y / tileHeight)];
            int value2 = dirt[(int)(x / tileWidth), (int)(y / tileHeight)];
            int value3 = superDirt[(int)(x / tileWidth), (int)(y / tileHeight)];
            int value4 = crack[(int)(x / tileWidth), (int)(y / tileHeight)];
            return value1 != -1 || value2 != -1 || value3 != -1 || value4 != -1;
        }

        public bool IsSolid(Vector2 v)
        {
            return IsSolid(v.X, v.Y);
        }

        public bool IsSemiSolid(float x, float y)
        {
            if (!Inbounds(x, y)) return false;
            int value = map[(int)(x / tileWidth), (int)(y / tileHeight)];
            return value >= 96 && value < 128;
        }

        public bool IsSemiSolid(Vector2 v)
        {
            return IsSemiSolid(v.X, v.Y);
        }

        public bool IsLadder(float x, float y)
        {
            if (!Inbounds(x, y)) return false;
            int value = map[(int)(x / tileWidth), (int)(y / tileHeight)];
            return value >= 128 && value < 160;
        }

        public bool IsLadder(Vector2 v)
        {
            return IsLadder(v.X, v.Y);
        }

        public int GetTileWidth()
        {
            return tileWidth;
        }

        public int GetTileHeight()
        {
            return tileHeight;
        }

        public int GetMapWidth()
        {
            return mapWidth;
        }

        public int GetMapHeight()
        {
            return mapHeight;
        }

        public float RoundX(float x)
        {
            return (int)(x / tileWidth) * tileWidth;
        }

        public float RoundY(float y)
        {
            return (int)(y / tileHeight) * tileHeight;
        }

        public bool BreakBlock(float x, float y, Player.direction dir = Player.direction.none)
        {
            bool broken = true;
            float angleOffset = 0;
            if (dir == Player.direction.left) angleOffset = (float)(Math.PI / 4);
            if (dir == Player.direction.right) angleOffset = -(float)(Math.PI / 4);

            if (!Inbounds(x, y)) return true;
            int tileX = (int)(x / tileWidth);
            int tileY = (int)(y / tileHeight);
            if (dirt[tileX, tileY] != -1)
            {
                GibDirt(tileX, tileY, angleOffset);
                dirt[tileX, tileY] = -1;
                if (crack[tileX, tileY] != -1)
                {
                    crack[tileX, tileY] = -1;
                }
                ResourceManager.PlaySFX("BlockBreak");
            }
            else if (superDirt[tileX, tileY] != -1)
            {
                dirt[tileX, tileY] = superDirt[tileX, tileY];
                crack[tileX, tileY] = 1;
                superDirt[tileX, tileY] = -1;
                broken = false;
                ResourceManager.PlaySFX("Thunk");
            }
            else if (map[tileX, tileY] != -1)
            {
                ResourceManager.PlaySFX("Tink");
                broken = false;
            }

            return broken;
        }

        private void GibDirt(int tileX, int tileY, float angleOffset = 0)
        {
            if(dirt[tileX, tileY] != -1)
            {
                for(int x = 0; x < tileWidth; x++)
                {
                    for(int y = 0; y < tileHeight; y++)
                    {
                        levelstate.entities["effects"].Add(new Gib(new Vector2(tileX * tileWidth + x, tileY * tileHeight + y), 
                            ResourceManager.GetTexturePixelColor(tileset, (dirt[tileX, tileY] % 32) * tileWidth + x, (dirt[tileX, tileY] / 32) * tileHeight + y), 
                            levelstate, angleOffset: angleOffset));
                    }
                }
            }
        }

        public bool BreakBlock(Vector2 v, Player.direction dir = Player.direction.none)
        {
            return BreakBlock(v.X, v.Y, dir);
        }
    }
}

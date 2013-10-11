using CastleMaster.Entities;
using CastleMaster.MathHelpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster.World
{
    public delegate void TileMethod(Level level, int xTile, int zTile);

    public class LevelBuilder
    {
        private Dictionary<uint, List<TileMethod>> registeredMethods;
        private Level level;
        private uint[] colorMap;
        private int width, height;

        public LevelBuilder(Level level, Texture2D mapTexture)
        {
            this.level = level;
            width = mapTexture.Width;
            height = mapTexture.Height;
            registeredMethods = new Dictionary<uint, List<TileMethod>>();
            colorMap = new uint[mapTexture.Width * mapTexture.Height];
            mapTexture.GetData(colorMap);
        }

        public void AddCustom(uint color, TileMethod tm)
        {
            color = FastFunctions.ToABGR(color);
            if (!registeredMethods.ContainsKey(color))
                registeredMethods.Add(color, new List<TileMethod>());
            registeredMethods[color].Add(tm);
        }

        public void AddTile(uint color, int tileID, byte dataVal = 0)
        {
            TileMethod tm = delegate(Level level, int xTile, int zTile)
            {
                level.SetTile(xTile, zTile, tileID);
                level.SetData(xTile, zTile, dataVal);
            };

            color = FastFunctions.ToABGR(color);
            if (!registeredMethods.ContainsKey(color))
                registeredMethods.Add(color, new List<TileMethod>());
            registeredMethods[color].Add(tm);
        }

        public void AddEntity(uint color, Type entType, float xOffs, float zOffs, params object[] constructorParams)
        {
            if (!typeof(Entity).IsAssignableFrom(entType)) return;

            TileMethod tm = delegate(Level level, int xTile, int zTile)
            {
                Entity ent = (Entity)Activator.CreateInstance(entType, constructorParams);
                level.AddEntity(ent, xTile * Viewport.TILESIZE + xOffs, zTile * Viewport.TILESIZE + zOffs);
            };

            color = FastFunctions.ToABGR(color);
            if (!registeredMethods.ContainsKey(color))
                registeredMethods.Add(color, new List<TileMethod>());
            registeredMethods[color].Add(tm);
        }

        public void BuildLevel()
        {
            for (int i = 0; i < width * height; i++)
            {
                List<TileMethod> tileMethodList;
                if (registeredMethods.TryGetValue(colorMap[i], out tileMethodList))
                    tileMethodList.ForEach(tm => tm.Invoke(level, i % width, i / width));
#if DEBUG
                else Console.WriteLine("No TileMethods assigned to this color: " + colorMap[i].ToString("X"));
#endif
            }
        }
    }
}

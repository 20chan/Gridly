﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Gridly
{
    public class TileMap
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public Vector2 Size { get; private set; }

        public float TileSize { get; private set; }
        public int WidthTileCount { get; private set; }
        public int HeightTileCount { get; private set; }

        private Tile[,] tiles;
        private List<Part> parts;

        public TileMap(List<Part> parts, int width, int height, float tileSize = 128)
        {
            this.parts = parts;
            WidthTileCount = width;
            HeightTileCount = height;
            Width = width * tileSize;
            Height = height * tileSize;
            Size = new Vector2(Width, Height);
            TileSize = tileSize;

            tiles = new Tile[height, width];
            InitTiles();
        }

        private void InitTiles()
        {
            for (int i = 0; i < HeightTileCount; i++)
                for (int j = 0; j < WidthTileCount; j++)
                    tiles[i, j] = new Tile();

            for (int i = 0; i < HeightTileCount; i++)
                for (int j = 0; j < WidthTileCount; j++)
                {
                    tiles[i, j].nearTiles[0] = TileOrNull(i - 1, j - 1);
                    tiles[i, j].nearTiles[1] = TileOrNull(i, j - 1);
                    tiles[i, j].nearTiles[2] = TileOrNull(i + 1, j - 1);
                    tiles[i, j].nearTiles[3] = TileOrNull(i + 1, j);
                    tiles[i, j].nearTiles[4] = TileOrNull(i + 1, j + 1);
                    tiles[i, j].nearTiles[5] = TileOrNull(i, j + 1);
                    tiles[i, j].nearTiles[6] = TileOrNull(i - 1, j + 1);
                    tiles[i, j].nearTiles[7] = TileOrNull(i - 1, j);
                }

            Tile TileOrNull(int i, int j)
            {
                if (i < 0 || i >= HeightTileCount)
                    return null;
                if (j < 0 || j >= WidthTileCount)
                    return null;
                return tiles[i, j];
            }
        }

        public void TileParts()
        {
            foreach (var t in tiles)
                t.Clear();
            foreach (var n in parts)
            {
                var x = MathHelper.Clamp((int)(n.Position.X / TileSize), 0, WidthTileCount - 1);
                var y = MathHelper.Clamp((int)(n.Position.Y / TileSize), 0, HeightTileCount - 1);
                tiles[y, x].Add(n);
            }
        }

        public void UpdatePhysics()
        {
            foreach (var n in parts)
                n.UpdatePhysics();
            foreach (var t in tiles)
                t.UpdatePhysics();
        }


        class Tile
        {
            public Tile[] nearTiles;
            private List<Part> parts;

            public Tile()
            {
                parts = new List<Part>();
                nearTiles = new Tile[8];
            }

            public void Clear()
            {
                parts.Clear();
            }

            public void Add(Part n)
            {
                parts.Add(n);
            }

            public void UpdatePhysics()
            {
                foreach (var n in parts)
                {
                    foreach (var m in parts)
                        ProcessCollision(n, m);
                    foreach (var t in nearTiles)
                        if (t != null)
                            foreach (var m in t.parts)
                                ProcessCollision(n, m);
                }
            }

            private void ProcessCollision(Part a, Part b)
            {
                if (a == b)
                    return;
                var dist = Vector2.Distance(a.Position, b.Position);
                if (dist < 15)
                {
                    // 완전히 붙어버리면 서로 반대방향이 없어져서 합쳐져 버려서
                    // 해결하려 했는데 일단 보류.
                    //if (dist < 0.1f)
                    //{
                    //    var rx = random.Next(60);
                    //    var vec = new Vector2(rx, 60 - rx);
                    //    a.AddForce(vec);
                    //    b.AddForce(-vec);
                    //}
                    a.AddForceTo(b.Position, -0.005f);
                    b.AddForceTo(a.Position, -0.005f);
                }
            }
        }
    }
}

using Iced.Intel;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace MogMod.World
{
    // all of this is dependent on ScalarVector1's StructureHelper mod
    // find more information on how to use it here
    // https://github.com/ScalarVector1/StructureHelper/wiki
    public partial class WorldGeneration : ModSystem
    {
        private void GenCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Building Caves";
            bool generated = false;
            int placementPositionX = Main.maxTilesX;
            int placementPositionY = (int)Main.worldSurface;

            Point placementPoint = new Point(placementPositionX, placementPositionY);
            Point16 caveSize = StructureHelper.API.Generator.GetStructureDimensions("Structures/SmallCave", Mod);
            for (int x = 0; x < (int)(placementPoint.X * .334f); x++)
            {
                for (int y = 100; y < placementPoint.Y; y++)
                {
                    Tile tile = Main.tile[x, y];

                    if (tile.HasTile && tile.TileType == TileID.Grass && NonSolidScanUp(new Point16(x, y), 40))
                    {
                        if (GetElevationDeviation(new Point16(x, y), caveSize.X, 20, 5, true) < 5)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Structures/SmallCave", new Point16(x, y - caveSize.Y + 8), ModContent.GetInstance<MogMod>());
                            generated = true;
                            break;
                        }
                    }
                }

                if (generated)
                    break;
            }
        }

        private void GenGrief(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Verifying Grief";
            Point16 griefSize = StructureHelper.API.Generator.GetStructureDimensions("Structures/Grief", Mod);
            bool generated = false;
            int xCheckArea = 20;

            for (int x = (int)(Main.maxTilesX * 0.15f); x < (int)(Main.maxTilesX * 0.85f) + xCheckArea + griefSize.X; x++)
            {
                int placementPositionY = WorldGen.genRand.Next(Main.UnderworldLayer - 550, Main.UnderworldLayer - 50);
                for (int y = placementPositionY; y < placementPositionY + griefSize.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == TileID.Stone)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Structures/Grief", new Point16(x, y - griefSize.Y + 12), ModContent.GetInstance<MogMod>());
                        generated = true;
                        break;
                    }
                }

                if (generated)
                    break;
            }
        }

        private void GenClubstepMonster(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Verifying Clubstep";
            Point16 clubstepMonsterSize = StructureHelper.API.Generator.GetStructureDimensions("Structures/ClubstepMonster", Mod);
            bool generated = false;
            int placementPositionX = WorldGen.genRand.Next((int)(Main.maxTilesX / 12), (int)(Main.maxTilesX * 0.925));
            int xCheckArea = 10;
            for (int x = placementPositionX; x < placementPositionX + xCheckArea + clubstepMonsterSize.X; x++)
            {
                int placementPositionY = WorldGen.genRand.Next(Main.UnderworldLayer + 70, Main.UnderworldLayer + 85);
                for (int y = placementPositionY; y < placementPositionY + clubstepMonsterSize.Y; y++)
                {
                    Tile tile = Main.tile[x, y];

                    StructureHelper.API.Generator.GenerateStructure("Structures/ClubstepMonster", new Point16(x, y - clubstepMonsterSize.Y + 40), ModContent.GetInstance<MogMod>());
                    generated = true;
                    break;
                }

                if (generated)
                    break;
            }
        }

        #region Utilities
        /// <summary>
        /// Checks that all tiles above the given point are air
        /// </summary>
        /// <param name="start"></param>
        /// <param name="MaxScan"></param>
        /// <returns></returns>
        public static bool AirScanUp(Point16 start, int MaxScan)
        {
            if (start.Y - MaxScan < 0)
                return false;

            for (int k = 1; k <= MaxScan; k++)
            {
                if (Main.tile[start.X, start.Y - k].HasTile)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Checks that all tiles above the given point are non-solid. Like a less strict AirScanUp.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="MaxScan"></param>
        /// <returns></returns>
        public static bool NonSolidScanUp(Point16 start, int maxScan)
        {
            if (start.Y - maxScan < 0)
                return false;

            for (int k = 1; k <= maxScan; k++)
            {
                if (Main.tile[start.X, start.Y - k].HasTile && Main.tileSolid[Main.tile[start.X, start.Y - k].TileType])
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Gets the greatest difference from the start point's surface level, determined
        /// by having the provided amount of air above itself.
        /// </summary>
        /// <param name="start">The coordinate to start looking at. This assumes it has air above it.</param>
        /// <param name="width">The width to scan over</param>
        /// <param name="neededAir">The amount of air above a tile needed for it to count as a surface</param>
        /// <param name="max">The max deviation to check for. If this is exceeded it automatically returns this value</param>
        /// <param name="lenient">Allows non-solid tiles to not count against elevation</param>
        /// <returns>The greatest aboslute value difference between surface tiles across the width, or the provided max</returns>
        public static int GetElevationDeviation(Point16 start, int width, int neededAir, int max, bool lenient)
        {
            int maxDeviation = 0;

            for (int k = 1; k < width; k++)
            {
                int thisMinDeviation = max;

                for (int i = -max; i < max; i++)
                {
                    int thisX = start.X + k;
                    int thisY = start.Y + i;

                    Tile thisTile = Main.tile[thisX, thisY];

                    if (thisTile.HasTile && Main.tileSolid[thisTile.TileType])
                    {
                        bool scan = lenient ? NonSolidScanUp(new Point16(thisX, thisY), neededAir) : AirScanUp(new Point16(thisX, thisY), neededAir);

                        if (scan && Math.Abs(i) < thisMinDeviation)
                            thisMinDeviation = Math.Abs(i);
                    }
                }

                if (thisMinDeviation > maxDeviation)
                    maxDeviation = thisMinDeviation;
            }

            return maxDeviation;
        }
        #endregion
    }
}
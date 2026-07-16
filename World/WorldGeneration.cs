using Microsoft.Xna.Framework;
using MogMod.Tiles.Ores;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MogMod.World
{
    // copied this from calamity mod world gen
    public partial class WorldGeneration : ModSystem
    {
        /// <summary>
        /// Generates clusters of ore across the world based on various requirements and with various strengths/frequencies.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <param name="verticalStartFactor"></param>
        /// <param name="verticalEndFactor"></param>
        /// <param name="strengthMin"></param>
        /// <param name="strengthMax"></param>
        /// <param name="convertibleTiles"></param>
        public static void SpawnOre(int type, double frequency, float verticalStartFactor, float verticalEndFactor, int strengthMin, int strengthMax, params int[] convertibleTiles)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = 0; k < (int)(x * y * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * verticalStartFactor), (int)(y * verticalEndFactor));
                    if (convertibleTiles.Length <= 0 || convertibleTiles.Contains(ParanoidTileRetrieval(tilesX, tilesY).TileType))
                        WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(strengthMin, strengthMax), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }
        public static Tile ParanoidTileRetrieval(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }
        public static void BroadcastLocalizedText(string key, Color? textColor = null)
        {
            // An attempt to bypass the need for a separate method and runtime/compile-time parameter
            // constraints by using nulls for defaults.
            if (!textColor.HasValue)
                textColor = Color.White;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), textColor.Value);
            else if (Main.dedServ)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int SurfaceIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            int HellIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int TrapsIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Traps"));
            int EndIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Tile Cleanup"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(SurfaceIndex + 1, new PassLegacy("MogMod Grief", GenGrief));
                tasks.Insert(SurfaceIndex + 1, new PassLegacy("MogMod Caves", GenCave));
                tasks.Insert(SurfaceIndex + 1, new PassLegacy("MogMod Clubstep Monster", GenClubstepMonster));
            }
        }
    }
    public static class MogModWorld
    {
        public static bool spawnedMendez = false;
        public static bool spawnedPrapor = false;
        public static bool spawnedSolBadguy = false;
        public static bool HasFoundGiantsMaul = false;
        public static bool UnderworldIsFreaky = false;
        public static void Save(List<string> boolTagContainer)
        {
            boolTagContainer.AddWithCondition("HasFoundGiantsMaul", HasFoundGiantsMaul);
            boolTagContainer.AddWithCondition("UnderworldIsFreaky", UnderworldIsFreaky);
        }
        public static void Load(IList<string> boolTagContainer)
        {
            HasFoundGiantsMaul = boolTagContainer.Contains("HasFoundGiantsMaul");
            UnderworldIsFreaky = boolTagContainer.Contains("UnderworldIsFreaky");
        }
        public static void SendData(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = HasFoundGiantsMaul;
            flags[1] = UnderworldIsFreaky;
            writer.Write(flags);
        }
        public static void ReceiveData(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            HasFoundGiantsMaul = flags[0];
            UnderworldIsFreaky = flags[1];
        }
    }
}
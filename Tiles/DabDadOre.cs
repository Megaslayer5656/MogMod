using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MogMod.Tiles
{
    public class DabDadOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 410;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(34, 101, 33), name);

            DustType = 61;
            HitSound = SoundID.Tink;
            MinPick = 200;
        }
    }
    public class DabDadOreSystem : ModSystem
    {
        public static LocalizedText DabDadOrePassMessage { get; private set; }
        public static LocalizedText BlessedWithDabDadOreMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            DabDadOrePassMessage = Mod.GetLocalization($"WorldGen.{nameof(DabDadOrePassMessage)}");
            BlessedWithDabDadOreMessage = Mod.GetLocalization($"WorldGen.{nameof(BlessedWithDabDadOreMessage)}");
        }
        public void BlessWorldWithDabDadOre()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(_ => {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(BlessedWithDabDadOreMessage.Value, 50, 255, 130);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(BlessedWithDabDadOreMessage.ToNetworkText(), new Color(50, 255, 130));
                }
                int splotches = (int)(200 * (Main.maxTilesX / 4200f));
                int highestY = (int)Utils.Lerp(Main.rockLayer, Main.UnderworldLayer, 0.5);
                for (int iteration = 0; iteration < splotches; iteration++)
                {
                    int i = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    int j = WorldGen.genRand.Next(highestY, Main.UnderworldLayer);
                    WorldGen.OreRunner(i, j, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), (ushort)ModContent.TileType<DabDadOre>());
                }
            });
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new DabDadOrePass("DabDad Ore", 237.4298f));
            }
        }
    }
    public class DabDadOrePass : GenPass
    {
        public DabDadOrePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DabDadOreSystem.DabDadOrePassMessage.Value;
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<DabDadOre>());
            }
        }
    }
}
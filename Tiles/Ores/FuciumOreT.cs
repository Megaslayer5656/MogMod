using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MogMod.Tiles.Ores
{
    public class FuciumOreT : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 320;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(232, 140, 160), name);

            DustType = DustID.BubbleBurst_Pink;
            HitSound = SoundID.Tink;
            MinPick = 65;
            MineResist = 2f;
        }
    }
    // FuciumOreSystem contains code related to spawning ExampleOre. It contains both spawning ore during world generation, seen in ModifyWorldGenTasks, and spawning ore after defeating a boss, seen in BlessWorldWithExampleOre and MinionBossBody.OnKill.
    public class FuciumOreSystem : ModSystem
    {
        public static LocalizedText FuciumOrePassMessage { get; private set; }
        public override void SetStaticDefaults()
        {
            FuciumOrePassMessage = Mod.GetLocalization($"WorldGen.{nameof(FuciumOrePassMessage)}");
        }
        // World generation is explained more in https://github.com/tModLoader/tModLoader/wiki/World-Generation
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // Because world generation is like layering several images on top of each other, we need to do some steps between the original world generation steps.

            // Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
            // First, we find out which step "Shinies" is.
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                // Next, we insert our pass directly after the original "Shinies" pass.
                // FuciumOrePass is a class seen bellow
                tasks.Insert(ShiniesIndex + 1, new FuciumOrePass("Fucium Ore", 237.4298f));
            }
        }
    }
    public class FuciumOrePass : GenPass
    {
        public FuciumOrePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // progress.Message is the message shown to the user while the following code is running.
            // Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes.
            progress.Message = FuciumOreSystem.FuciumOrePassMessage.Value;

            // Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
            // "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // should spawn on the dungeon side of the world (its completely random)
                int StartX = 0;
                StartX = GenVars.dungeonX < Main.maxTilesX / 2 ? 25 : (Main.maxTilesX - (Main.maxTilesX / 9)) - 25;

                //set these to be able to easily place things in certain locations, like structures
                int biomeStart = StartX;
                int biomeEdge = biomeStart + (Main.maxTilesX / 9);

                int x = WorldGen.genRand.Next(biomeStart, biomeEdge);

                // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place.
                // Feel free to experiment with strength and step to see the shape they generate.
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 5), ModContent.TileType<FuciumOreT>());
            }
        }
    }
}
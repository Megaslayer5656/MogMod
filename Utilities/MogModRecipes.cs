using Microsoft.Build.Tasks;
using MogMod.Items.Accessories;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Utilities
{
    public class MogModRecipes : ModSystem
    {
        public static int AnyScaleOrTissue, AnyCursedFlameOrIchor, AnyEvilWater, AnyEvilMushroom, AnyEvilMaterial;
        public static int AnyCopperOre, AnySilverOre, AnyGoldOre, AnyEvilOre, AnyCobaltOre, AnyMythrilOre, AnyAdamantiteOre;
        public static int AnyCopperBar, AnySilverBar, AnyGoldBar, AnyEvilBar, AnyCobaltBar, AnyMythrilBar, AnyAdamantiteBar;
        public static int AnyHallowedHelmet, AnyHallowedPlatemail, AnyHallowedGreaves;
        public static int AnyQuiver, AnyTombstone, AnyChest, AnyTorch, AnySquirrel, AnyEmblem, AnyButterfly, AnyFragment, AnyRocket, AnyJavelin;
        public override void AddRecipeGroups()
        {
            #region Recipe Groups
            #region Bars
            // Copper and Tin
            RecipeGroup group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperOre)}",
            [
                ItemID.CopperOre,
                ItemID.TinOre
            ]);
            AnyCopperOre = RecipeGroup.RegisterGroup("AnyCopperOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}",
            [
                ItemID.CopperBar,
                ItemID.TinBar
            ]);
            AnyCopperBar = RecipeGroup.RegisterGroup("AnyCopperBar", group);

            // Silver and Tungsten
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverOre)}",
            [
                ItemID.SilverOre,
                ItemID.TungstenOre
            ]);
            AnySilverOre = RecipeGroup.RegisterGroup("AnySilverOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
            [
                ItemID.SilverBar,
                ItemID.TungstenBar,
            ]);
            AnySilverBar = RecipeGroup.RegisterGroup("AnySilverBar", group);

            // Gold and Platinum
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldOre)}",
            [
                ItemID.GoldOre,
                ItemID.PlatinumOre
            ]);
            AnyGoldOre = RecipeGroup.RegisterGroup("AnyGoldOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}",
            [
                ItemID.GoldBar,
                ItemID.PlatinumBar
            ]);
            AnyGoldBar = RecipeGroup.RegisterGroup("AnyGoldBar", group);

            // Demonite and Crimtane
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.DemoniteOre)}",
            [
                ItemID.DemoniteOre,
                ItemID.CrimtaneOre
            ]);
            AnyEvilOre = RecipeGroup.RegisterGroup("AnyEvilOre", group);

            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyEvilBar"),
            [
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            ]);
            AnyEvilBar = RecipeGroup.RegisterGroup("AnyEvilBar", group);

            // Cobalt and Palladium
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltOre)}",
            [
                ItemID.CobaltOre,
                ItemID.PalladiumOre
            ]);
            AnyCobaltOre = RecipeGroup.RegisterGroup("AnyCobaltOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltBar)}",
            [
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            ]);
            AnyCobaltBar = RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            // Mythril and Orichalcum
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilOre)}",
            [
                ItemID.MythrilOre,
                ItemID.OrichalcumOre
            ]);
            AnyMythrilOre = RecipeGroup.RegisterGroup("AnyMythrilOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilBar)}",
            [
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            ]);
            AnyMythrilBar = RecipeGroup.RegisterGroup("AnyMythrilBar", group);

            // Adamantite and Titanium
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteOre)}",
            [
                ItemID.AdamantiteOre,
                ItemID.TitaniumOre
            ]);
            AnyAdamantiteOre = RecipeGroup.RegisterGroup("AnyAdamantiteOre", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteBar)}",
            [
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            ]);
            AnyAdamantiteBar = RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);
            #endregion

            #region Evil Materials
            // Shadow Scale and Tissue Sample
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyScaleOrTissue"),
            [
                ItemID.ShadowScale,
                ItemID.TissueSample
            ]);
            AnyScaleOrTissue = RecipeGroup.RegisterGroup("AnyScaleOrTissue", group);

            // Cursed Flame and Ichor
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyCursedFlameOrIchor"),
            [
                ItemID.CursedFlame,
                ItemID.Ichor
            ]);
            AnyCursedFlameOrIchor = RecipeGroup.RegisterGroup("AnyCursedFlameOrIchor", group);

            // Unholy Water and Blood Water
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyEvilWater"),
            [
                ItemID.UnholyWater,
                ItemID.BloodWater
            ]);
            AnyEvilWater = RecipeGroup.RegisterGroup("AnyEvilWater", group);

            // Vile and Vicious Mushrooms
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyEvilMushroom"),
            [
                ItemID.VileMushroom,
                ItemID.ViciousMushroom
            ]);
            AnyEvilMushroom = RecipeGroup.RegisterGroup("AnyEvilMushroom", group);

            // Unholy Water and Blood Water
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyEvilMaterial"),
            [
                ItemID.Vertebrae,
                ItemID.RottenChunk
            ]);
            AnyEvilMaterial = RecipeGroup.RegisterGroup("AnyEvilMaterial", group);
            #endregion

            #region Misc
            // Magic Quivers
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyQuiver"),
            [
                ItemID.MagicQuiver,
                ItemID.MoltenQuiver,
                ItemID.StalkersQuiver
            ]);
            AnyQuiver = RecipeGroup.RegisterGroup("AnyQuiver", group);

            // Tombstones
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Tombstone)}",
            [
                ItemID.Tombstone,
                ItemID.GraveMarker,
                ItemID.CrossGraveMarker,
                ItemID.Headstone,
                ItemID.Gravestone,
                ItemID.Obelisk,
                ItemID.RichGravestone1,
                ItemID.RichGravestone2,
                ItemID.RichGravestone3,
                ItemID.RichGravestone4,
                ItemID.RichGravestone5
            ]);
            AnyTombstone = RecipeGroup.RegisterGroup("AnyTombstone", group);

            // Class Emblems
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyEmblem"),
            [
                ItemID.WarriorEmblem,
                ItemID.RangerEmblem,
                ItemID.SorcererEmblem,
                ItemID.SummonerEmblem
            ]);
            AnyEmblem = RecipeGroup.RegisterGroup("AnyEmblem", group);

            // Hallowed Helmets
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedHelmet)}",
            [
                ItemID.HallowedHelmet,
                ItemID.HallowedHeadgear,
                ItemID.HallowedMask,
                ItemID.HallowedHood,
                ItemID.AncientHallowedHelmet,
                ItemID.AncientHallowedHeadgear,
                ItemID.AncientHallowedMask,
                ItemID.AncientHallowedHood
            ]);
            AnyHallowedHelmet = RecipeGroup.RegisterGroup("AnyHallowedHelmet", group);

            // Hallowed Plate Mails
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedPlateMail)}",
            [
                ItemID.HallowedPlateMail,
                ItemID.AncientHallowedPlateMail
            ]);
            AnyHallowedPlatemail = RecipeGroup.RegisterGroup("AnyHallowedPlatemail", group);

            // Hallowed Greaves
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.HallowedGreaves)}",
            [
                ItemID.HallowedGreaves,
                ItemID.AncientHallowedGreaves
            ]);
            AnyHallowedGreaves = RecipeGroup.RegisterGroup("AnyHallowedGreaves", group);

            // Chests
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Chest)}",
            [
                ItemID.Chest,
                ItemID.AshWoodChest,
                ItemID.BalloonChest,
                ItemID.BambooChest,
                ItemID.BlueDungeonChest,
                ItemID.BoneChest,
                ItemID.BorealWoodChest,
                ItemID.CactusChest,
                ItemID.CapricornChestplate, 
                ItemID.CoralChest,
                ItemID.CorruptionChest,
                ItemID.CrimsonChest,
                ItemID.CrystalChest,
                ItemID.DeadMansChest,
                ItemID.DesertChest,
                ItemID.DungeonDesertChest,
                ItemID.DynastyChest,
                ItemID.EbonwoodChest,
                ItemID.FleshChest,
                ItemID.FrozenChest,
                ItemID.GlassChest,
                ItemID.GoldChest,
                ItemID.GoldenChest,
                ItemID.GolfChest,
                ItemID.GraniteChest,
                ItemID.GreenDungeonChest,
                ItemID.HallowedChest,
                ItemID.HoneyChest,
                ItemID.IceChest,
                ItemID.IvyChest,
                ItemID.JungleChest,
                ItemID.LesionChest,
                ItemID.LihzahrdChest,
                ItemID.LivingWoodChest,
                ItemID.MarbleChest,
                ItemID.MartianChest,
                ItemID.MeteoriteChest,
                ItemID.MushroomChest,
                ItemID.NebulaChest,
                ItemID.ObsidianChest,
                ItemID.PalmWoodChest,
                ItemID.PearlwoodChest,
                ItemID.PinkDungeonChest,
                ItemID.PumpkinChest,
                ItemID.RichMahoganyChest,
                ItemID.ShadewoodChest,
                ItemID.ShadowChest,
                ItemID.SkywareChest,
                ItemID.SlimeChest,
                ItemID.SolarChest,
                ItemID.SpiderChest,
                ItemID.SpookyChest,
                ItemID.StardustChest,
                ItemID.SteampunkChest,
                ItemID.VortexChest,
                ItemID.WaterChest,
                ItemID.WebCoveredChest
            ]);
            AnyChest = RecipeGroup.RegisterGroup("AnyChest", group);

            // Torches
            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Torch)}",
            [
                ItemID.Torch,
                ItemID.BlueTorch,
                ItemID.BoneTorch,
                ItemID.CoralTorch,
                ItemID.CorruptTorch,
                ItemID.CrimsonTorch,
                ItemID.CursedTorch,
                ItemID.DemonTorch,
                ItemID.DesertTorch,
                ItemID.GreenTorch,
                ItemID.HallowedTorch,
                ItemID.IceTorch,
                ItemID.IchorTorch,
                ItemID.JungleTorch,
                ItemID.MushroomTorch,
                ItemID.OrangeTorch,
                ItemID.PinkTorch,
                ItemID.PurpleTorch,
                ItemID.RainbowTorch,
                ItemID.RedTorch,
                ItemID.ShimmerTorch,
                ItemID.TikiTorch, // has torch in the name
                ItemID.UltrabrightTorch,
                ItemID.WhiteTorch,
                ItemID.YellowTorch
            ]);
            AnyTorch = RecipeGroup.RegisterGroup("AnyTorch", group);

            // Butterflies
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyButterfly"),
            [
                ItemID.MonarchButterfly, 
                ItemID.HellButterfly, 
                ItemID.JuliaButterfly, 
                ItemID.GoldButterfly, 
                ItemID.PurpleEmperorButterfly, 
                ItemID.RedAdmiralButterfly, 
                ItemID.SulphurButterfly, 
                ItemID.TreeNymphButterfly, 
                ItemID.UlyssesButterfly, 
                ItemID.ZebraSwallowtailButterfly
            ]);
            AnyButterfly = RecipeGroup.RegisterGroup("AnyButterfly", group);

            // Class Emblems
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyFragment"),
            [
                ItemID.FragmentSolar, 
                ItemID.FragmentStardust, 
                ItemID.FragmentNebula, 
                ItemID.FragmentVortex
            ]);
            AnyFragment = RecipeGroup.RegisterGroup("AnyFragment", group);

            // Class Emblems
            group = new(() => MiscUtils.GetTextValue("Common.RecipeGroup.AnyRocket"),
            [
                ItemID.RocketI, 
                ItemID.RocketII, 
                ItemID.RocketIII, 
                ItemID.RocketIV, 
                ItemID.ClusterRocketI, 
                ItemID.ClusterRocketII, 
                ItemID.DryRocket, 
                ItemID.HoneyRocket, 
                ItemID.LavaRocket, 
                ItemID.WetRocket, 
                ItemID.MiniNukeI, 
                ItemID.MiniNukeII
            ]);
            AnyRocket = RecipeGroup.RegisterGroup("AnyRocket", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Squirrel)}",
            [
                ItemID.Squirrel,
                ItemID.SquirrelRed, 
                ItemID.SquirrelGold, 
                ItemID.GemSquirrelAmber, 
                ItemID.GemSquirrelAmethyst, 
                ItemID.GemSquirrelDiamond, 
                ItemID.GemSquirrelEmerald, 
                ItemID.GemSquirrelRuby, 
                ItemID.GemSquirrelSapphire, 
                ItemID.GemSquirrelTopaz
            ]);
            AnySquirrel = RecipeGroup.RegisterGroup("AnySquirrel", group);

            group = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Javelin)}",
            [
                ItemID.Javelin,
                ItemID.BoneJavelin
            ]);
            AnyJavelin = RecipeGroup.RegisterGroup("AnyJavelin", group);
            #endregion
            #endregion
        }

        public override void AddRecipes()
        {
            #region Vanilla Item Recipes
            // In addition to these methods, there are also methods relating to shimmer decrafting. See ShimmerShowcase.cs for that.
            #region Weapons
            // magic missile
            Recipe magicMissileRecipe = Recipe.Create(ItemID.MagicMissile);
            magicMissileRecipe.AddIngredient(ItemID.DiamondStaff)
                .AddIngredient(ItemID.Bone, 40)
                .AddIngredient<HealingLotus>(3)
                .AddTile(TileID.Anvils)
                .Register();
            // anchor weapon
            Recipe anchorRecipe = Recipe.Create(ItemID.Anchor);
            anchorRecipe.AddIngredient(ItemID.Rope, 100)
                .AddRecipeGroup("IronBar", 25)
                .AddIngredient(ItemID.PirateMap)
                .AddTile(TileID.Anvils)
                .Register();
            // tsunami
            Recipe tsunami = Recipe.Create(ItemID.Tsunami);
            tsunami.AddIngredient<TidalWave>()
                .AddIngredient<BrinyRind>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            // north pole
            Recipe northPoleRecipe = Recipe.Create(ItemID.NorthPole);
            northPoleRecipe.AddIngredient<FrozenSpear>()
                .AddIngredient(ItemID.IceQueenTrophy)
                .AddIngredient<FrostEssence>(20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            // elf melter
            Recipe elfMelter = Recipe.Create(ItemID.ElfMelter);
            elfMelter.AddIngredient(ItemID.Flamethrower)
                .AddIngredient(ItemID.SantaNK1Trophy)
                .AddIngredient<FrostEssence>(20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            #endregion
            #region Armor
            // eskimo helmet
            Recipe hoodRecipe = Recipe.Create(ItemID.EskimoHood);
            hoodRecipe.AddIngredient(ItemID.Silk, 10)
                .AddIngredient(ItemID.FlinxFur, 3)
                .AddTile(TileID.Loom)
                .Register();
            // skull vanity helmet
            Recipe skullRecipe = Recipe.Create(ItemID.Skull);
            skullRecipe.AddIngredient(ItemID.Bone, 50)
                .AddRecipeGroup("AnySilverBar", 8)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
            // wizard hat
            Recipe wizardRecipe = Recipe.Create(ItemID.WizardHat);
            wizardRecipe.AddRecipeGroup("AnyScaleOrTissue", 12)
                .AddIngredient(ItemID.Leather, 7)
                .AddIngredient<ManaEssence>(3)
                .AddTile(TileID.Loom)
                .DisableDecraft()
                .Register();
            #endregion
            #region Accessories
            // band of starpower
            Recipe manaBandRecipe = Recipe.Create(ItemID.BandofStarpower);
            manaBandRecipe.AddIngredient(ItemID.Shackle)
                .AddIngredient<FrigidShard>(5)
                .AddIngredient<ManaEssence>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            // magic quiver
            Recipe magicQuiver = Recipe.Create(ItemID.MagicQuiver);
            magicQuiver.AddIngredient<ElvenQuiver>()
                .AddIngredient(ItemID.PixieDust, 15)
                .AddIngredient<PointBooster>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            #endregion
            #region Misc
            // golem power cell summon
            Recipe powerCellRecipe = Recipe.Create(ItemID.LihzahrdPowerCell, 3);
            powerCellRecipe.AddIngredient(ItemID.LunarTabletFragment, 4)
                .AddIngredient(ItemID.Ectoplasm, 2)
                .AddIngredient<UltimateOrb>()
                .AddTile(TileID.LihzahrdFurnace)
                .Register();
            // tnt barrel
            Recipe tntBarrel = Recipe.Create(ItemID.TNTBarrel, 3);
            tntBarrel.AddIngredient(ItemID.ExplosivePowder, 3)
                .AddIngredient(ItemID.Barrel, 3)
                .DisableDecraft()
                .Register();
            // jester arrow
            Recipe jesterArrow = Recipe.Create(ItemID.JestersArrow, 150);
            jesterArrow.AddIngredient(ItemID.WoodenArrow, 150)
                .AddIngredient<ManaEssence>()
                .AddTile(TileID.Anvils)
                .Register();
            // broken hero sword
            Recipe brokenHeroSword = Recipe.Create(ItemID.BrokenHeroSword);
            brokenHeroSword.AddIngredient<BrokenHeroShard>(5)
                .AddIngredient<CraftingRecipe>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            // terra toilet (gun)
            Recipe terraToiletGun = Recipe.Create(ItemID.TerraToilet);
            terraToiletGun.AddIngredient(ItemID.Toilet)
                .AddIngredient<BrokenHeroGun>()
                .AddTile(TileID.Anvils)
                .Register();
            // terra toilet (staff)
            Recipe terraToiletStaff = Recipe.Create(ItemID.TerraToilet);
            terraToiletStaff.AddIngredient(ItemID.Toilet)
                .AddIngredient<BrokenHeroStaff>()
                .AddTile(TileID.Anvils)
                .Register();
            #endregion
            #endregion
        }
    }
}
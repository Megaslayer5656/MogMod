using MogMod.Items.Consumables;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Content
{
    // This class contains thoughtful examples of item recipe creation.
    // Recipes are explained in detail on the https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes and https://github.com/tModLoader/tModLoader/wiki/Intermediate-Recipes wiki pages. Please visit the wiki to learn more about recipes if anything is unclear.
    public class MogModRecipes : ModSystem
    {
        // A place to store the recipe group so we can easily use it later
        public static RecipeGroup ExampleRecipeGroup;

        public override void Unload()
        {
            ExampleRecipeGroup = null;
        }

        public override void AddRecipeGroups()
        {
            #region Summary
            // Create a recipe group and store it
            // Language.GetTextValue("LegacyMisc.37") is the word "Any" in English, and the corresponding word in other languages

            //ExampleRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<Items.ExampleItem>())}",
            //    ModContent.ItemType<Items.ExampleItem>(), ModContent.ItemType<Items.ExampleDataItem>());

            // To avoid name collisions, when a modded items is the iconic or 1st item in a recipe group, name the recipe group: ModName:ItemName
            //RecipeGroup.RegisterGroup("ExampleMod:ExampleItem", ExampleRecipeGroup);

            // Add an item to an existing Terraria recipeGroup. ExampleCritterItem isn't gold but it serves as an example for this.
            //RecipeGroup.recipeGroups[RecipeGroupID.GoldenCritter].ValidItems.Add(ModContent.ItemType<ExampleCritterItem>());

            // We also add ExampleSand to the Sand group, which is used in the Glass and Magic Sand Dropper recipes
            //RecipeGroup.recipeGroups[RecipeGroupID.Sand].ValidItems.Add(ModContent.ItemType<ExampleSandBlock>());

            // We can also add ExamplePressurePlate to the pressure plate group, allowing it to be used to craft weighted pressure plates and pressure plate track. Since ExamplePressurePlate is a weighed pressure plate, we'll leave this commented out.
            //RecipeGroup.recipeGroups[RecipeGroupID.PressurePlate].ValidItems.Add(ModContent.ItemType<ExamplePressurePlate>());

            // While an "IronBar" group exists, "SilverBar" does not. tModLoader will merge recipe groups registered with the same name,
            // so if you are registering a recipe group with a vanilla item as the 1st item,
            // you can register it using just the internal item name if you anticipate other mods wanting to use this recipe group for the same concept.
            // By doing this, multiple mods can add to the same group without extra effort.
            // In this case we are adding a SilverBar group. Don't store the RecipeGroup instance, it might not be used,
            // use the same nameof(ItemID.ItemName) or RecipeGroupID returned from RegisterGroup when using Recipe.AddRecipeGroup instead.

            // AddRecipeGroup(RecipeGroupID.Wood, 1). <-- example of vanilla
            // AddRecipeGroup("SilverBar", 1). <-- example of modded
            #endregion

            #region Recipe Groups
            RecipeGroup SilverBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
            ItemID.SilverBar, ItemID.TungstenBar); // , ModContent.ItemType<Items.Placeable.ExampleBar>() // if you want to add a modded bar
            RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar), SilverBarRecipeGroup);

            RecipeGroup GoldBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.GoldBar), GoldBarRecipeGroup);

            RecipeGroup WorldEvilBossRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.TissueSample)}", ItemID.TissueSample, ItemID.ShadowScale);
            RecipeGroup.RegisterGroup(nameof(ItemID.TissueSample), WorldEvilBossRecipeGroup);

            RecipeGroup EvilBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CrimtaneBar)}", ItemID.CrimtaneBar, ItemID.DemoniteBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.CrimtaneBar), EvilBarRecipeGroup);

            RecipeGroup CobaltBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CobaltBar)}", ItemID.CobaltBar, ItemID.PalladiumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.CobaltBar), CobaltBarRecipeGroup);

            RecipeGroup MythrilBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.MythrilBar)}", ItemID.MythrilBar, ItemID.OrichalcumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.MythrilBar), MythrilBarRecipeGroup);

            RecipeGroup AdamantiteBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.AdamantiteBar)}", ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.AdamantiteBar), AdamantiteBarRecipeGroup);

            RecipeGroup HardmodeEvilMaterialGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Ichor)}", ItemID.Ichor, ItemID.CursedFlame);
            RecipeGroup.RegisterGroup(nameof(ItemID.Ichor), HardmodeEvilMaterialGroup);

            RecipeGroup DamageClassEmblemGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.WarriorEmblem)}", ItemID.WarriorEmblem, ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem);
            RecipeGroup.RegisterGroup(nameof(ItemID.WarriorEmblem), DamageClassEmblemGroup);

            RecipeGroup EvilMushroomRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.VileMushroom)}", ItemID.VileMushroom, ItemID.ViciousMushroom);
            RecipeGroup.RegisterGroup(nameof(ItemID.VileMushroom), EvilMushroomRecipeGroup);
            #endregion
        }

        public override void AddRecipes()
        {
            #region Vanilla Item Recipes
            // In addition to these methods, there are also methods relating to shimmer decrafting. See ShimmerShowcase.cs for that.

            // eskimo helmet
            Recipe hoodRecipe = Recipe.Create(ItemID.EskimoHood, 1);
            hoodRecipe.AddIngredient(ItemID.Silk, 10)
                .AddIngredient(ItemID.FlinxFur, 3)
                .AddTile(TileID.Loom)
                .Register();
            // skull vanity helmet
            Recipe skullRecipe = Recipe.Create(ItemID.Skull, 1);
            skullRecipe.AddIngredient(ItemID.Bone, 50)
                .AddRecipeGroup("SilverBar", 8)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
            // golem power cell summon
            Recipe powerCellRecipe = Recipe.Create(ItemID.LihzahrdPowerCell, 3);
            powerCellRecipe.AddIngredient(ItemID.LunarTabletFragment, 4)
                .AddIngredient(ItemID.Ectoplasm, 2)
                .AddIngredient<UltimateOrb>(1)
                .AddTile(TileID.LihzahrdFurnace)
                .Register();
            // anchor weapon
            Recipe anchorRecipe = Recipe.Create(ItemID.Anchor, 1);
            anchorRecipe.AddIngredient(ItemID.Rope, 100)
                .AddRecipeGroup("IronBar", 25)
                .AddIngredient(ItemID.PirateMap, 1)
                .AddTile(TileID.Anvils)
                .Register();
            // magic missle weapon
            Recipe magicMissileRecipe = Recipe.Create(ItemID.MagicMissile, 1);
            magicMissileRecipe.AddIngredient(ItemID.DiamondStaff, 1)
                .AddIngredient(ItemID.Bone, 40)
                .AddIngredient<HealingLotus>(3)
                .AddTile(TileID.Anvils)
                .Register();
            #endregion
        }
    }
}
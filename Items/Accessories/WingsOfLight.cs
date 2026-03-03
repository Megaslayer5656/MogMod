using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class WingsOfLight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 38;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWingsOfLight = true;
            mogPlayer.wingsOfLightVisual = !hideVisual;
            player.moveSpeed += 0.1f;
            player.lavaImmune = true;
            player.noFallDmg = true;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GhostWings, 1).
                AddIngredient(ItemID.GarlandHat, 1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Hardmode Evil Material"}", 9).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class EyeOfSkadi : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 25;
            player.GetDamage(DamageClass.Generic) += .10f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingEyeOfSkadi = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EyeoftheGolem, 1).
                AddIngredient(ItemID.ShroomiteBar, 7).
                AddIngredient(ItemID.SpectreBar, 7).
                AddIngredient<UltimateOrb>(2).
                AddIngredient<PointBooster>(1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

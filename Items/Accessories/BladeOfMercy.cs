using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class BladeOfMercy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 26;
            Item.height = 38;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.mercyBladeEquipped = true;
            mogPlayer.exultationEquipped = true;
            player.GetAttackSpeed(DamageClass.Generic) += .1f;
            player.GetDamage(DamageClass.Generic) -= .15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LordOfBloodsExultation>(1).
                AddRecipeGroup("CobaltBar", 8).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class FlayersBota : NeutralItem
    {
        public const float BloodMult = 0.3f;
        public const float AttackSpeedBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AttackSpeedBoost.ToPercent(), BloodMult.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 30;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFlayersBota = true;
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeedBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilMaterial", 15).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient<HellfireEssence>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
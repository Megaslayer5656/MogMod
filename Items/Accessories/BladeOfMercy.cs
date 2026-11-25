using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class BladeOfMercy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float AttackSpeedBoost = 0.1f;
        public const float DamageReduction = 0.15f;
        public const float BloodMult = 0.35f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AttackSpeedBoost.ToPercent(), DamageReduction.ToPercent(), BloodMult.ToPercent());
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
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.mercyBladeEquipped = true;
            mogPlayer.exultationEquipped = true;
            mogPlayer.bloodMult += BloodMult;
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeedBoost;
            player.GetDamage<GenericDamageClass>() -= DamageReduction;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LordOfBloodsExultation>().
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient<PointBooster>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
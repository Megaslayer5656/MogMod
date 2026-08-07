using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class OversizedAnchor : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int BuffTime = 180;
        public const float DefenseReductionBoost = 0.10f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffTime.FramesToSeconds(), DefenseReductionBoost.ToPercent());
        public override int ProjectileType => ModContent.ProjectileType<OversizedAnchorHoldout>();
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 100;

            Item.damage = 76;
            Item.knockBack = 12f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 32;
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool AltFunctionUse(Player player)
        {
            player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum = 2;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Anchor, 1).
                AddIngredient(ItemID.SharkFin, 5).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class ChaosBlade : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const float UltraCritChance = 0.17f;
        public const float CritMult = 2.7f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(UltraCritChance.ToPercent(), CritMult);
        public override int ProjectileType => ModContent.ProjectileType<ChaosBladeHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 48;
            Item.height = 50;

            Item.damage = 77;
            Item.crit = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 40;
            Item.knockBack = 8f;
            Item.autoReuse = true;

            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 20).
                AddRecipeGroup("AnyEvilBar", 15).
                AddRecipeGroup("AnyScaleOrTissue", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
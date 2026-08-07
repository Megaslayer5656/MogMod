using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Sange : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<SangeHoldout>();
        public const int BloodDamage = 110;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 40;
            Item.height = 48;

            Item.damage = 78;
            Item.crit = 56;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 7f;
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient<FrigidCrystal>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
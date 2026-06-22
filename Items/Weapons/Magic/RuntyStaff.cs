using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class RuntyStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 40;

            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;

            Item.useTime = 12;
            Item.useAnimation = 24;
            Item.reuseDelay = 12;
            Item.useLimitPerAnimation = 2;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RuntyStaffProj>();
            Item.shootSpeed = 11f;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(10).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
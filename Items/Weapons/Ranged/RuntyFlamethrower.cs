using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class RuntyFlamethrower : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 16;

            Item.damage = 18;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;

            Item.useTime = 12;
            Item.useAnimation = 48;
            Item.reuseDelay = 12;
            Item.useLimitPerAnimation = 4;

            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<RuntyFlamethrowerProj>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(8).
                AddIngredient(ItemID.Gel, 6).
                AddRecipeGroup("AnyTorch").
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class VortexOfConflagration : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.damage = 85;
            Item.mana = 24;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 50;
            Item.UseSound = SoundID.Item84;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<VoCProj>();
            Item.shootSpeed = 20f;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 splinterVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7);
                Projectile.NewProjectile(source, position, splinterVelocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RazorbladeTyphoon).
                AddIngredient<InfernoMaelstrom>().
                AddIngredient<BrinyRind>(12).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
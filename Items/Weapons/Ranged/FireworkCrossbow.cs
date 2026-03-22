using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class FireworkCrossbow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 16;
            Item.shootSpeed = 8f;
            Item.knockBack = 2f;
            Item.width = 56;
            Item.height = 26;
            Item.damage = 52;
            Item.shoot = ModContent.ProjectileType<FireworkCrossbowProj>();
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.ArmorPenetration = 10;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity,  ModContent.ProjectileType<FireworkCrossbowProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes() //TODO: Make this recipe cooler and make more sense
        {
            CreateRecipe().
                AddIngredient<UltimateOrb>(1).
                AddIngredient(ItemID.Wood, 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

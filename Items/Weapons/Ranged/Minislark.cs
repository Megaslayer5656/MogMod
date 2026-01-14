using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Minislark : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 36;
            Item.damage = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Bullet;
            Item.rare = ItemRarityID.Orange;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 1;
        public override Vector2? HoldoutOffset() => new Vector2(0, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + Main.rand.Next(-5, 6) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-5, 6) * 0.05f;
            if (Main.rand.NextBool(7))
            {
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<MinislarkProj>(), damage, knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 35)
                return false;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Minishark, 1).
                AddIngredient<HydrakanLatch>(1).
                AddIngredient(ItemID.SharkToothNecklace, 1).
                AddIngredient(ItemID.Bone, 30). // we need a boss same tier as skeletron (slardar maybe??)
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
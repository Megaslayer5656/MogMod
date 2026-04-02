using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class LAS13Trident : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 18;
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item91;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.ArmorPenetration = 15;
            Item.useAmmo = AmmoID.Gel;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bulletAmt = 6;
            for (int index = 0; index < bulletAmt; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-25, 26) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-25, 26) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<LAS13Proj>(), damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AdamantiteBar", 13).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient(ItemID.GolfCupFlagBlue, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class TidalWave : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 50;

            Item.damage = 37;
            Item.knockBack = 1.5f;
            Item.useTime = Item.useAnimation = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawn = player.RotatedRelativePoint(player.MountedCenter, true);
            int projAmt = 3;
            float piOver10 = MathHelper.Pi * 0.1f;
            velocity.Normalize();
            velocity *= 15f;
            Vector2 spread = velocity * 2.5f;
            bool canHit = Collision.CanHit(spawn, 0, 0, spawn + spread, 0, 0);
            for (int i = 0; i < 3; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = spread.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= spread;

                Projectile.NewProjectile(source, spawn + offset, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-0f, 0f);
    }
}
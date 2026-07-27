using Microsoft.Xna.Framework;
using MogMod.Common.Config;
using MogMod.Items.Global;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class Mosin : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 22;

            Item.damage = 135;
            Item.knockBack = 9f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/MosinShot")
            {
                Volume = .3f,
                PitchVariance = .02f,
            };
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<MosinLPSProj>();
                damage = (int)(damage * 1.3f);
                knockback *= 1.5f;
            }
        }
        public override Vector2? HoldoutOffset() => new Vector2(0f, 2f);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.velocity += velocity.SafeNormalize(Vector2.UnitX) * -6f;
            return true;
        }
        // code taken from calamity mod rubico prime
        public override void HoldItem(Player player)
        {
            if (MogClientConfig.Instance.GunRecoil)
                player.MogMod().mouseWorldListener = true;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (MogClientConfig.Instance.GunRecoil)
            {
                player.ChangeDir(Math.Sign((player.MogMod().mouseWorld - player.Center).X));
                float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

                Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
                Vector2 itemSize = new Vector2(Item.width, Item.height);
                Vector2 itemOrigin = new Vector2(-5, 6);

                MogModUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
                base.UseStyle(player, heldItemFrame);
            }
        }
        // Recoil + Not having the gun aim downwards
        public override void UseItemFrame(Player player)
        {
            if (MogClientConfig.Instance.GunRecoil)
            {
                player.ChangeDir(Math.Sign((player.MogMod().mouseWorld - player.Center).X));
                float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
                float rotation = (player.Center - player.MogMod().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
                if (animProgress < 0.5)
                    rotation += (player.altFunctionUse == 2 ? -1f : -0.45f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation); //must be here otherwise it will vibrate
                //Reloads the gun 
                if (animProgress > 0.5f)
                {
                    float backArmRotation = rotation + 0.52f * player.direction;

                    Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                    player.SetCompositeArmBack(true, stretch, backArmRotation);
                }
            }
        }
    }
}
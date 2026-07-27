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
    public class AXMC : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public const int BloodDamage = 200;
        public override void SetDefaults()
        {
            Item.width = 194;
            Item.height = 38;

            Item.damage = 2641; // enough to one shot eye of junkmunthulugh
            Item.crit = 71;
            Item.knockBack = 14f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/AXMCShot")
            {
                Volume = 2.25f,
                PitchVariance = .02f,
            };
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Bullet;

            Item.autoReuse = true;
            Item.noMelee = true;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override Vector2? HoldoutOffset() => new Vector2(23.5f, 0.2f);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.velocity += velocity.SafeNormalize(Vector2.UnitX) * (Main.zenithWorld ? -100f: -18f);
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<APLapuaProj>();
        }
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
        public override bool AltFunctionUse(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.scope = true;
                return true;
            }
            return base.AltFunctionUse(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Mosin>().
                AddIngredient(ItemID.SniperRifle).
                AddIngredient(ItemID.VortexBeater).
                AddIngredient(ItemID.LunarBar, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
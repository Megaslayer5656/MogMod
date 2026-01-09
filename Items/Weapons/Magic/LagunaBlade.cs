using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Projectiles.MagicProjectiles;

namespace MogMod.Items.Weapons.Magic
{
    public class LagunaBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 52;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 20;
            Item.mana = 16;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<FierySoulProjectile>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item20;
            Item.rare = ItemRarityID.Cyan;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.UseSound = player.altFunctionUse == 2 ? SoundID.Item43 : SoundID.Item66;
            if (player.altFunctionUse == 2 && player.HasBuff(ModContent.BuffType<Buffs.Cooldowns.LagunaBladeCooldown>()))
            {
                return false;
            }
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? 0.3f : 1f;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 4f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2 && !player.HasBuff(ModContent.BuffType<Buffs.Cooldowns.LagunaBladeCooldown>()))
            {
                type = ModContent.ProjectileType<LagunaBladeProjectile>();
                knockback *= 4f;
                velocity *= 1.2f;
                damage *= 10;
                player.AddBuff(ModContent.BuffType<LagunaBladeCooldown>(), 1800);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int totalProjectiles = 1;
            for (int i = 0; i < totalProjectiles; i++)
            {
                Vector2 waveVelocity = ((MathHelper.TwoPi * i / (float)totalProjectiles) + velocity.ToRotation()).ToRotationVector2() * velocity.Length();
                Projectile.NewProjectile(source, position, waveVelocity, type, damage, knockback, Main.myPlayer);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FierySoul>(1).
                AddIngredient(ItemID.ThunderStaff, 1).
                AddIngredient(ItemID.LivingFireBlock, 20).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

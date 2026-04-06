using Microsoft.Xna.Framework;
using MogMod.Buffs.Cooldowns;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class ThrowingShade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 36;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 20;
            Item.mana = 9;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<ThrowingShadeProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item109;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.ArmorPenetration = 20;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (!player.HasBuff(ModContent.BuffType<ShadowRealmBuff>()))
                {
                    player.AddBuff(ModContent.BuffType<ShadowRealmBuff>(), 300);
                }
                return false;
            }
            else if (player.altFunctionUse != 2 && player.HasBuff(ModContent.BuffType<ShadowRealmBuff>()))
            {
                type = ModContent.ProjectileType<ShadowRealmProj>();
                knockback *= 1.2f;
                velocity *= 1.2f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
            else
                return true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WaterBolt, 1).
                AddIngredient(ItemID.ShadowCandle, 1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
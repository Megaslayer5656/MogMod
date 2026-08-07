using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class DaturaLash : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 34;

            Item.damage = 46;
            Item.knockBack = 1.5f;
            Item.shootSpeed = 24f;
            Item.useAnimation = Item.useTime = 20;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<DaturaLashProj>();
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, num);
            return false;
        }
        /* dropped by spiders in HM
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChainKnife).
                AddIngredient(ItemID.SpiderFang, 7).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        */
    }
}
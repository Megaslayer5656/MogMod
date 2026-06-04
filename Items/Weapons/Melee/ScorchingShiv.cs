using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class ScorchingShiv : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 48;

            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 5f;
            Item.knockBack = 7f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            SoundEngine.PlaySound(SoundID.Item1, player.Center);
            Projectile.NewProjectile(source, position, velocity * 0.75f, ModContent.ProjectileType<ScorchingShivProj>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GriefBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
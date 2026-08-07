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
    public class Nativitas : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 46;

            Item.damage = 55;
            Item.knockBack = 3f;
            Item.shootSpeed = 24f;
            Item.useAnimation = Item.useTime = 16;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<NativitasProj>();
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float upHigh = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            float downLow = (Main.rand.NextFloat() - 0.25f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, upHigh);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, downLow);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrostEssence>(18).
                AddIngredient(ItemID.ShroomiteBar, 12).
                AddIngredient(ItemID.FrostCore).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
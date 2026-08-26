using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Glintstone
{
    public class ShardSpiral : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 50;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 16;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.shoot = ModContent.ProjectileType<ShardSpiralProj>();
            Item.shootSpeed = 3f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PixieDust, 15).
                AddRecipeGroup("AnyMythrilBar", 6).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
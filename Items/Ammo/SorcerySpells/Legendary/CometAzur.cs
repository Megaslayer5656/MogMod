using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Legendary
{
    public class CometAzur : SorcerySpell
    {
        public override int ManaCost => 4;
        public override bool Channeled => true;
        public override bool OnlyOneActive => true;
        public override SoundStyle UseSound => SoundID.Item9;
        public override bool SwordStyle => Main.zenithWorld;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 30;
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<CometAzurLaser>();
            Item.shootSpeed = 30f;
            SorceryClass = SorceryID.Legendary;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient<SoulOfMogMod>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.zenithWorld)
            {
                int numSplits = 6;
                float angleVariance = MathHelper.TwoPi / numSplits;
                Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.ToRadians(45));
                for (int i = 0; i < numSplits; ++i)
                {
                    projVec = projVec.RotatedBy(angleVariance);
                    Projectile.NewProjectile(source, player.Center, projVec, type, damage, player.whoAmI);
                }
                return false;
            }
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, player.whoAmI);
            return false;
        }
    }
}
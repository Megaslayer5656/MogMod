using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class Rancorcall : SorcerySpell
    {
        public override int ManaCost => 14;
        public override int AttackSpeed => 50;
        public override SoundStyle UseSound => SoundID.Item104;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 25;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<GhostflameHomingProj>();
            Item.shootSpeed = 3f;
            SorceryClass = SorceryID.Death;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Vector2 randVelocity = velocity.RotatedByRandom(MathHelper.PiOver2 * 0.5);
                Projectile.NewProjectile(source, position, randVelocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(8).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
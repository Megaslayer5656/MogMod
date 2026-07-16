using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class BladesOfStone : SorcerySpell
    {
        public override int ManaCost => 26;
        public override int AttackSpeed => 39; // 52
        public override int NumberOfAttacks => 3;
        public override int AttackDelay => 13;
        public override SoundStyle UseSound => SoundID.Item101;
        public override bool OnlyOneActive => Main.zenithWorld;
        public override void SetStaticDefaults()
        {
            if (Main.zenithWorld)
            {
                ItemID.Sets.LockOnIgnoresCollision[Type] = true;
                ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 42;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<BladesOfStoneProj>();
            Item.shootSpeed = 10f;
            SorceryClass = SorceryID.Gravity;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 15).
                AddIngredient<PointBooster>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // sentry placement
            if (Main.zenithWorld)
            {
                player.FindSentryRestingSpot(type, out int XPosition, out int YPosition, out int YOffset);
                YOffset -= 6;
                position = new Vector2((float)XPosition, (float)(YPosition - YOffset));
                for (int x = 0; x < 5; x++)
                    Projectile.NewProjectile(source, new(position.X + (float)Main.rand.Next(-50, 50), position.Y - 50f), Vector2.Zero, type, damage, knockback, player.whoAmI);
                return false;
            }
            // mouse position
            Vector2 mousePosition = player.MogMod().mouseWorld;
            Projectile.NewProjectile(source, new(mousePosition.X + (float)Main.rand.Next(-10, 10), mousePosition.Y - 30f), Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
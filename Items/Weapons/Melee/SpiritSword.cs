using MogMod.Items.Global;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using MogMod.Projectiles.Melee;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class SpiritSword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 52;

            Item.damage = 28;
            Item.scale = 1.15f;
            Item.knockBack = 4.5f;
            Item.useTime = Item.useAnimation = 23;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpiritSparkle>();
            Item.shootSpeed = 12f;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), 1f, 1f, 1f);

            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.SilverCoin);
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            float randDirX = Main.rand.Next(-5, 6);
            float randDirY = Main.rand.Next(-5, 6);
            Vector2 velocity = new(randDirX * 5, randDirY * 5);
            Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<SpiritSwordProj>(), (int)(Item.damage * 0.5f), 1f, player.whoAmI);
        }
        public override void AddRecipes() // simple recipies like this keep me hard at night
        {
            CreateRecipe().
            AddIngredient<SpiritShard>(8).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}
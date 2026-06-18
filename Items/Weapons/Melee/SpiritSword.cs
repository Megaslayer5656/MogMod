using MogMod.Items.Global;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class SpiritSword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 52;
            Item.damage = 38;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 23;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.scale = 1.5f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), 1f, 1f, 1f);

            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.SilverCoin);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            float randDirX = random.Next(-5, 5);
            float randDirY = random.Next(-5, 5);
            Vector2 velocity = new Vector2(randDirX * 5, randDirY * 5);
            Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<SpiritSwordProj>(), Convert.ToInt32(Item.damage * .5f), 1f, player.whoAmI);
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
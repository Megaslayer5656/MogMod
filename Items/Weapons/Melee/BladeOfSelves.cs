using Microsoft.Build.Execution;
using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class BladeOfSelves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        int numHits = 0;
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 82;
            Item.DamageType = DamageClass.Melee;
            Item.reuseDelay = 20;
            Item.useAnimation = 20;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useLimitPerAnimation = 2;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.scale = 1.75f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            numHits++;
            if (numHits == 2)
            {
                MogModUtils.ProjectileBarrage(target.GetSource_FromAI(), target.Center, target.Center, Main.rand.NextBool(), 150f, 150f, -150f, 150f, 10f, ModContent.ProjectileType<SelvesProj1>(), Convert.ToInt32(Item.damage * 0.95), 0f, player.whoAmI, false, 0f);
                numHits = 0;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EchoSabre>(1).
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
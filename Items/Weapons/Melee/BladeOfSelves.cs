using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using MogMod.Projectiles.MeleeProjectiles;
using Microsoft.Build.Execution;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class BladeOfSelves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 64;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
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
            MogModUtils.ProjectileBarrage(target.GetSource_FromAI(), target.Center, target.Center, Main.rand.NextBool(), 150f, 150f, -150f, 150f, 7f, ModContent.ProjectileType<SelvesProj1>(), Convert.ToInt32(Item.damage * 0.95), 0f, player.whoAmI, false, 0f);
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
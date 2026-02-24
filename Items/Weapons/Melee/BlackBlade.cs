using System;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class BlackBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 97;
            Item.height = 96;
            Item.damage = 115;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 13f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Red;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 300);
        }
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<BlackBladeDebuff>(), 300);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.life >= Convert.ToInt32(target.lifeMax * .9f))
            {
                modifiers.FinalDamage *= 1.5f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BreakerBlade, 1).
                AddIngredient(ItemID.ShadowFlameKnife, 1).
                AddIngredient(ItemID.HallowedBar, 15).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

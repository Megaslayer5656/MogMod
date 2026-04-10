using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Sange : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 48;
            Item.damage = 72;
            Item.scale = 2f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 66;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // for SOME REASON player has a default of 70 lifesteal
            int heal = 1;
            heal *= Convert.ToInt32(player.lifeSteal * 0.015);
            player.statLife += heal;
            player.HealEffect(heal);
            // so we dont go over max life
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Crystalys>(1).
                AddRecipeGroup("AdamantiteBar", 12).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient(ItemID.SoulofNight, 7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

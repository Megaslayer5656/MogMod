using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Items.Other;
using System;

namespace MogMod.Items.Weapons.Melee
{
    public class Sange : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 59;
            Item.damage = 58;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 65, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 66;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // for SOME REASON player has a default of 70 lifesteal
            int heal = 1;
            heal *= Convert.ToInt32(player.lifeSteal * 0.035);
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
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

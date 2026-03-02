using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class KayaAndSange : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSange = true;

            player.statLifeMax2 += 30;
            player.lifeRegen += 4;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.GetDamage(DamageClass.Magic) += .12f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.statManaMax2 += 70;
            player.lifeSteal *= 1.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kaya>(1).
                AddIngredient<Sange>(1).
                AddIngredient<GriefBar>(7).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddIngredient<PointBooster>(1).
                AddIngredient<VitalityBooster>(1).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

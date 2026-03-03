using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class YashaAndKaya : ModItem, ILocalizedModType
    {
        // delete this item
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();

            player.GetAttackSpeed(DamageClass.Generic) += .24f;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.GetDamage(DamageClass.Magic) += .12f;
            player.moveSpeed += .24f;
            player.accRunSpeed += player.accRunSpeed * .24f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.statManaMax2 += 50;

            // fish slop 1
            mogPlayer.wearingFishSlop1 = true;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accFishFinder = true;
            player.accLavaFishing = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kaya>(1).
                AddIngredient<Yasha>(1).
                AddIngredient<Fishrael>(1).
                AddIngredient(ItemID.FishingPotion, 15).
                AddIngredient(ItemID.CratePotion, 15).
                AddIngredient(ItemID.SonarPotion, 15).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddIngredient<FrigidCrystal>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
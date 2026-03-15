﻿using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ATGMissile : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PlasmaShrimp>();
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 52;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) += .05f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.atgActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient(ItemID.HighVelocityBullet, 100).
            AddIngredient(ItemID.ChlorophyteBar, 15).
            AddIngredient<GriefBar>(10).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}

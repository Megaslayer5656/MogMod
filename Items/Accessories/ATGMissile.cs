﻿using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using MogMod.Items.Other;

namespace MogMod.Items.Accessories
{
    public class ATGMissile : ModItem //TODO: Make this shimmerable into plasma shrimp
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
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
            AddIngredient(ItemID.ChlorophyteBar, 15).
            AddIngredient<GriefBar>(10).
            AddIngredient(ItemID.HighVelocityBullet, 100).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}

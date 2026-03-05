﻿using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ATGMissile : ModItem
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
            player.GetAttackSpeed(DamageClass.Generic) += .1f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.atgActive = true;
        }

        public override void AddRecipes()
        {
            //TODO: Add recipe and decide place in progression, then balance accordingly
        }
    }
}

﻿using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class Polylute : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.SparkleGuitar;
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 50;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 7f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.polyluteActive = true;
            mogPlayer.polyluteVisual = !hideVisual;
        }
    }
}
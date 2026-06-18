﻿using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class PlasmaShrimp : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ATGMissile>();
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 67;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        // cant be equipped with atg or icbm
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<ATGMissile>() || equippedItem.type == ModContent.ItemType<ICBM>())
            {
                return false;
            }
            return true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 5f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.plasmaActive = true;
            mogPlayer.plasmaVisual = !hideVisual;
        }
    }
}
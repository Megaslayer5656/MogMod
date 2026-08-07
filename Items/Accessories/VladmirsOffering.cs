using MogMod.Buffs.AccessoryAuras;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class VladmirsOffering : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxSentries = 2;
        // auras
        public const int DefenseBoost = 4;
        public const float FlatDamageBoost = 3f;
        public const int ManaRegenBoost = 4;
        public const float LifeStealBoost = 0.20f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxSentries, DefenseBoost, FlatDamageBoost, ManaRegenBoost.ToRegenPerSecond(), LifeStealBoost.ToPercent());
        int teamBuff = ModContent.BuffType<VladmirsBuff>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingVladimirs = true;
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(teamBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(teamBuff, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ManaRegenerationBand).
                AddIngredient<BladesOfAttack>().
                AddIngredient(ItemID.Silk, 12).
                AddRecipeGroup("AnyScaleOrTissue", 7).
                AddIngredient<ManaEssence>(3).
                AddIngredient(ItemID.Skull).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
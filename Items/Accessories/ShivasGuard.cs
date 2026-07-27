using MogMod.Buffs.AccessoryAuras;
using MogMod.Common.Classes;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ShivasGuard : ModItem, ILocalizedModType
    {
        // auras
        public const int DefenseBoost = 10;
        public const float RangedDamageBoost = 0.10f;
        // accessory buffs
        public const float SorceryDamageBoost = 0.12f;
        public const float AttackDamageBoost = 0.10f;
        public const float AttackSpeedBoost = 0.10f;
        public const int LifeRegenBoost = 8;
        public const int LifeBoost = 50;
        public const int ManaBoost = 50;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SorceryDamageBoost.ToPercent(), AttackDamageBoost.ToPercent(), LifeRegenBoost.ToRegenPerSecond(), LifeBoost, DefenseBoost, RangedDamageBoost.ToPercent());
        public int cooldownTimer = 0;
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ShivasKeybind);
        ModKeybind keybindActive = null;
        int teamBuff = ModContent.BuffType<ShivasGuardBuff>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.defense = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingShivasGuard = true;
            player.GetDamage<SorceryDamageClass>() += SorceryDamageBoost;
            player.GetDamage<GenericDamageClass>() += AttackDamageBoost;
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeedBoost;
            player.statLifeMax2 += LifeBoost;
            player.statManaMax2 += ManaBoost;
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
                AddIngredient<VeilOfDiscord>().
                AddIngredient<FrostEssence>(20).
                AddIngredient(ItemID.SpectreBar, 18).
                AddIngredient<ManaCore>().
                AddIngredient(ItemID.FrostCore).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
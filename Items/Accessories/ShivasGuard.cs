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
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ShivasGuard : ModItem, ILocalizedModType
    {
        public int cooldownTimer = 0;
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ShivasKeybind);
        ModKeybind keybindActive = null;
        int teamBuff = ModContent.BuffType<Buffs.AccessoryAuras.ShivasGuardBuff>();
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
            player.GetDamage<SorceryDamageClass>() += 0.12f;
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.GetAttackSpeed<GenericDamageClass>() += 0.10f;
            player.lifeRegen += 4;
            player.statManaMax2 += 50;
            player.statLifeMax2 += 50;
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
                AddIngredient(ItemID.SpectreBar, 18).
                AddIngredient<FrostEssence>(8).
                AddIngredient<ManaCore>().
                AddIngredient(ItemID.FrostCore).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
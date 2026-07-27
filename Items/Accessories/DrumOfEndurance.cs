using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace MogMod.Items.Accessories
{
    public class DrumOfEndurance : ModItem, ILocalizedModType
    {
        // aura buffs
        public const float MovementSpeedBoost = 0.30f;
        public const float MeleeSpeedBoost = 0.10f;
        public const float WhipSpeedBoost = 0.10f;
        // accessory buffs
        public const int MaxMinions = 1;
        public const float SummonDamageBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent(), MaxMinions, MovementSpeedBoost.ToPercent(), MeleeSpeedBoost.ToPercent());
        int teamBuff = ModContent.BuffType<Buffs.AccessoryAuras.DrumOfEnduranceBuff>();
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            player.GetDamage(DamageClass.Summon) += SummonDamageBoost;
            player.maxMinions += MaxMinions;
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
                AddIngredient<BeltOfStrength>(1).
                AddIngredient(ItemID.Robe, 1).
                AddIngredient(ItemID.AnkletoftheWind, 1).
                AddIngredient(ItemID.RichMahogany, 15).
                AddIngredient(ItemID.JungleSpores, 7).
                AddIngredient(ItemID.SoulofLight, 5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
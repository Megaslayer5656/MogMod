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
    // TODO: give this an active that does something cool
    public class WraithPact : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxSentries = 5;
        // auras
        public const int DefenseBoost = 7;
        public const float AttackDamageBoost = 0.10f;
        public const int ManaRegenBoost = 6;
        public const float LifeStealBoost = 0.50f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxSentries, DefenseBoost, AttackDamageBoost.ToPercent(), ManaRegenBoost.ToRegenPerSecond(), LifeStealBoost.ToPercent());
        int wraithBuff = ModContent.BuffType<WraithPactAura>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWraithPact = true;
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(wraithBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(wraithBuff, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VladmirsOffering>().
                AddIngredient(ItemID.AvengerEmblem).
                AddIngredient<SpookyEssence>(20).
                AddIngredient<PointBooster>().
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
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
    public class CrimsonGuard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int DefenseBoost = 9;
        public const int MaxLifeBoost = 50;
        public const int LifeRegenBoost = 8;
        public const int AggroBoost = 750;
        public const float DamageBlockChance = 0.25f;
        public const int SelfDamageReduction = 100;
        public const float MinHealthReq = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxLifeBoost, LifeRegenBoost.ToRegenPerSecond(), AggroBoost, DamageBlockChance.ToPercent(), SelfDamageReduction, MinHealthReq.ToPercent());
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.defense = DefenseBoost;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingCrimsonGuard = true;
            player.statLifeMax2 += MaxLifeBoost;
            player.lifeRegen += LifeRegenBoost;
            player.noKnockback = true;
            player.aggro += AggroBoost;
            if (player.statLife > player.statLifeMax2 * MinHealthReq)
            {
                player.hasPaladinShield = true;
                if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                {
                    int myPlayer = Main.myPlayer;
                    if (Main.player[myPlayer].team == player.team && player.team != 0)
                    {
                        float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                        float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                        if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                            Main.player[myPlayer].AddBuff(BuffID.PaladinsShield, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Vanguard>(1).
                AddIngredient<HelmOfIronWill>(1).
                AddIngredient(ItemID.PaladinsShield, 1).
                AddIngredient(ItemID.HallowedBar, 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
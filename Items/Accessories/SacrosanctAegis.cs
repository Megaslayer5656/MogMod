using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
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
    [AutoloadEquip(EquipType.Shield)]
    public class SacrosanctAegis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int DashCooldown = 200;
        public const int MaxLifeBoost = 200;
        public const int LifeRegenBoost = 20;
        public const float DamageReductionBoost = 0.1f;
        public const int AggroBoost = 1500;
        public const float MinHealthReq = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DashCooldown.FramesToSeconds(), MaxLifeBoost, LifeRegenBoost.ToRegenPerSecond(), DamageReductionBoost.ToPercent(), AggroBoost, MinHealthReq.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 38;
            Item.defense = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSacrosanctAegis = true;
            player.statLifeMax2 += MaxLifeBoost;
            player.aggro += AggroBoost;
            player.lifeRegen += LifeRegenBoost;
            player.lifeRegenTime += LifeRegenBoost;
            player.endurance += DamageReductionBoost;
            player.noKnockback = true;

            // complete immunity to everything
            player.fireWalk = true;
            foreach (int debuff in player.buffType)
            {
                if (MogModBuffSets.IsDebuff[debuff])
                    player.buffImmune[debuff] = true;
            }
            if (player.statLife > player.statLifeMax2 * 0.25f)
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
                AddIngredient<CrimsonGuard>().
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
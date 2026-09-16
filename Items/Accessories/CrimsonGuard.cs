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
        public const int DefenseBoost = 10;
        public const int LifeRegenBoost = 10;
        public const float DamageReductionBoost = 0.1f;
        public const int AggroBoost = 750;
        public const float DamageBlockChance = 0.25f;
        public const int SelfDamageReduction = 100;
        public const float MinHealthReq = 0.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRegenBoost.ToRegenPerSecond(), DamageReductionBoost.ToPercent(), AggroBoost, DamageBlockChance.ToPercent(), SelfDamageReduction, MinHealthReq.ToPercent());
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
            player.lifeRegen += LifeRegenBoost;
            player.endurance += DamageReductionBoost;
            player.noKnockback = true;
            player.aggro += AggroBoost;

            // ankh shield immunity
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Stoned] = true;

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
                AddIngredient<Vanguard>().
                AddIngredient(ItemID.AnkhCharm).
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.PaladinsShield).
                AddIngredient(ItemID.HallowedBar, 10).
                AddTile(TileID.TinkerersWorkbench).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.AnkhShield).
                AddIngredient<HelmOfIronWill>().
                AddIngredient(ItemID.PaladinsShield).
                AddIngredient(ItemID.HallowedBar, 15).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
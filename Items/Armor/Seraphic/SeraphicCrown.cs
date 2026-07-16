using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Seraphic
{
    [AutoloadEquip(EquipType.Head)]
    public class SeraphicCrown : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const int ReviveTime = SeraphicBreastplate.ReviveDuration;
        public const int ReviveCooldown = SeraphicBreastplate.ReviveCooldown;
        public const float SummonDamageBoost = 0.3f;
        public const float WhipSpeed = 0.2f;
        public const float WhipRange = 0.2f;
        public const int MaxMinions = 5;
        public const int MaxSentries = 5;
        public static Color AbilityBriefColor = Color.LightGoldenrodYellow;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent(), WhipRange.ToPercent(), WhipSpeed.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 26;

            Item.defense = 10; // idk yet, gonna make it post ML though

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SeraphicBreastplate>() && 
                legs.type == ModContent.ItemType<SeraphicGreaves>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSeraphic = true;
            player.maxMinions += MaxMinions;
            player.maxTurrets += MaxSentries;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
            player.GetAttackSpeed<SummonMeleeSpeedDamageClass>() += WhipSpeed;
            player.whipRangeMultiplier += WhipRange;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<SeraphicCrown>() && player.armor[1].type == ItemType<SeraphicBreastplate>() && player.armor[2].type == ItemType<SeraphicGreaves>();
        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    if (Main.keyState.PressingShift())
                    {
                        setBonusIndex++;
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<SeraphicCrown>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), MaxMinions, MaxSentries, ReviveTime.FramesToSeconds(), ReviveCooldown.FramesToMinutes()));
                        tooltips.Insert(setBonusIndex, briefDescription);
                    }
                    else
                    {
                        setBonusIndex++;
                        TooltipLine holdShiftIndicator = new(item.Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextValue("UI.ShiftToExpand"));
                        holdShiftIndicator.OverrideColor = IHoldShiftTooltipItem.DefaultExtensionIndicatorColor;
                        tooltips.Insert(setBonusIndex, holdShiftIndicator);
                    }
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => ModifySetTooltips(this, tooltips);
        #endregion
        #region Visuals
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawShadowLokis = true;
        }
        public override void UpdateVanitySet(Player player)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 156 : DustID.GoldCoin, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.65f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.3f;
                }
            }
        }
        #endregion
        #region Recipe(s)
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedHood).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.AncientHallowedHood).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
        #endregion
    }
}
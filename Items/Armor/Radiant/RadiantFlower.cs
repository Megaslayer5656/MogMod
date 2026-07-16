using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Head)]
    public class RadiantFlower : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const int ManaRegenBonus = 16;
        public const int ManaBoost = 100;
        public const float ManaReduction = 0.83f;
        public const float MagicDamageBoost = 0.13f;
        public const int MagicCritBoost = 13;
        public static Color AbilityBriefColor = Color.LightSkyBlue;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, ManaReduction.ToReversedPercent(), MagicDamageBoost.ToPercent(), MagicCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // so the players hair can be seen with the armor equipped
            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.defense = 18;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        // what armor is needed for a set bonus
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<RadiantTop>() && 
                legs.type == ModContent.ItemType<RadiantBottom>();
        }
        #endregion
        #region Armor Stat Changes
        // set bonus
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRadiantArmor = true;
            player.manaRegenBonus += ManaRegenBonus;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        // armor stat buffs
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += ManaBoost;
            player.manaCost *= ManaReduction;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<RadiantFlower>() && player.armor[1].type == ItemType<RadiantTop>() && player.armor[2].type == ItemType<RadiantBottom>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<RadiantFlower>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), ManaRegenBonus.ToRegenPerSecond()));
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
        // visual effects
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
            player.armorEffectDrawShadow = true;
        }
        #endregion
        #region Recipe(s)
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreHood, 1).
                AddIngredient(ItemID.ObsidianRose, 1).
                AddIngredient<FaeBar>(9).
                AddIngredient<ManaCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SpectreMask, 1).
                AddIngredient(ItemID.ObsidianRose, 1).
                AddIngredient<FaeBar>(9).
                AddIngredient<ManaCore>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        #endregion
    }
}
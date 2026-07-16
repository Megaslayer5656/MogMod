using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Head)]
    public class TankyRizzlerHelmet : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const int AggroBoost = 1300;
        public const int LifeBoost = 100;
        public const float DamageReduction = 0.1f;
        public const float MeleeDamageBoost = 0.08f;
        public const int LifeRegen = 8;
        public static Color AbilityBriefColor = new(163, 30, 0);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBoost.ToPercent(), LifeRegen.ToRegenPerSecond());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.defense = 26;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TankyRizzlerChestplate>() && 
                legs.type == ModContent.ItemType<TankyRizzlerLeggings>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingTankyRizzler = true;

            player.statLifeMax2 += LifeBoost;
            player.endurance += DamageReduction;
            player.aggro += AggroBoost;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.noKnockback = true;
            player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;
            player.lifeRegen += LifeRegen;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<TankyRizzlerHelmet>() && player.armor[1].type == ItemType<TankyRizzlerChestplate>() && player.armor[2].type == ItemType<TankyRizzlerLeggings>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<TankyRizzlerHelmet>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), AggroBoost, DamageReduction.ToPercent(), LifeBoost));
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
            player.armorEffectDrawShadowLokis = true;
        }
        #endregion
        #region Recipe(s)
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleHelmet, 1).
                AddIngredient(ItemID.MartianConduitPlating, 150).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<BlockOfCheese>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        #endregion
    }
}
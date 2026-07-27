using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Armor.Hellfire;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Nihilum
{
    [AutoloadEquip(EquipType.Head)]
    public class NihilumHeadgear : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float RangedDamageBoost = 0.3f;
        public const float AmmoReduction = 0.7f;
        public static Color AbilityBriefColor = Color.Orchid;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent(), AmmoReduction.ToReversedPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;

            Item.defense = 30; // idk yet, gonna make it post ML though

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<NihilumChestplate>() && 
                legs.type == ModContent.ItemType<NihilumLeggings>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingNihilum = true;
            mogPlayer.wearingNihilumRanged = true;
            player.aggro -= 1000;
            if (Main.zenithWorld)
                mogPlayer.nulledDebuff = true;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.ammoCost *= AmmoReduction;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<NihilumHeadgear>() && player.armor[1].type == ItemType<NihilumChestplate>() && player.armor[2].type == ItemType<NihilumLeggings>();
        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.NulledKeybind.TooltipHotkeyString();
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    if (Main.keyState.PressingShift())
                    {
                        if (Main.zenithWorld)
                        {
                            setBonusIndex++;
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<NihilumHeadgear>("SetBonusGFB").Format(AbilityBriefColor.Hex3()));
                            tooltips.Insert(setBonusIndex, briefDescription);
                        }
                        else
                        {
                            setBonusIndex++;
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<NihilumHeadgear>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), Hotkey));
                            tooltips.Insert(setBonusIndex, briefDescription);
                        }
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
        ModKeybind keybindActive = null;
        #endregion
        #region Visuals
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
            player.armorEffectDrawOutlinesForbidden = true;
        }
        #endregion
        #region Recipe(s)
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteHelmet).
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
        #endregion
    }
}
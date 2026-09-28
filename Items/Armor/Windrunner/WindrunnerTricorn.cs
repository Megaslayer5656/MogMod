using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Windrunner
{
    [AutoloadEquip(EquipType.Head)]
    public class WindrunnerTricorn : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float DamageCap = 1f;

        public const int ThornsMult = 7;
        public const float ThornsBoost = 0.2f;
        public const float MeleeDamageBoost = 0.07f;
        public const int MeleeCritBoost = 7;
        public static Color AbilityBriefColor = Color.FloralWhite;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 16;
            Item.defense = 9;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WindrunnerTop>() &&
                legs.type == ModContent.ItemType<WindrunnerTights>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWindrunner = true;
            player.aggro += 550;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;
            player.GetCritChance<MeleeDamageClass>() += MeleeCritBoost;
            player.thorns += ThornsBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<WindrunnerTricorn>() && player.armor[1].type == ItemType<WindrunnerTop>() && player.armor[2].type == ItemType<WindrunnerTights>();
        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.ArmorSetBonusKeybind.TooltipHotkeyString();
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    if (Main.keyState.PressingShift())
                    {
                        setBonusIndex++;
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<WindrunnerTricorn>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), ThornsMult, Hotkey));
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
        #region Recipe(s)
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FalconBlade).
                AddIngredient(ItemID.Spike, 25).
                AddIngredient(ItemID.Bone, 30).
                AddTile(TileID.Anvils).
                Register();
        }
        #endregion
    }
}
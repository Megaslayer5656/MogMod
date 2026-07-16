using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Armor.Hellfire;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Damascus
{
    [AutoloadEquip(EquipType.Head)]
    public class DamascusHelm : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float CritMult = 0.1f;
        public const float GFBCritMult = 0.5f;
        public const int CritBoost = 6;
        public static Color AbilityBriefColor = new(255, 255, 97);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, CritMult.ToPercent(), GFBCritMult.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 22;
            Item.defense = 6;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DamascusMail>() && 
                legs.type == ModContent.ItemType<DamascusGreaves>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingDamascus1 = true;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<DamascusHelm>() && player.armor[1].type == ItemType<DamascusMail>() && player.armor[2].type == ItemType<DamascusGreaves>();
        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
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
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<DamascusHelm>("SetBonusGFB").Format(AbilityBriefColor.Hex3(), GFBCritMult.ToPercent()));
                            tooltips.Insert(setBonusIndex, briefDescription);
                        }
                        else
                        {
                            setBonusIndex++;
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<DamascusHelm>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), CritMult.ToPercent()));
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
        #endregion
        #region Visuals
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        #endregion
        #region Recipe(s)
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(12).
                AddTile(TileID.Anvils).
                Register();
        }
        #endregion
    }
}
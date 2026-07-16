using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.WhiteMaskSet
{
    [AutoloadEquip(EquipType.Head)]
    public class WhiteMask : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float BloodMult = 0.2f;
        public const float AttackSpeed = 0.1f;
        public const int CritBoost = 8;
        public static Color AbilityBriefColor = Color.IndianRed;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AttackSpeed.ToPercent(), CritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.defense = 8;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WhiteCloak>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWhiteArmor = true;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeed;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<WhiteMask>() && player.armor[1].type == ItemType<WhiteCloak>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<WhiteMask>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), BloodMult.ToPercent()));
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
                AddIngredient(ItemID.Silk, 8).
                AddIngredient(ItemID.SoulofFright, 6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        #endregion
    }
}
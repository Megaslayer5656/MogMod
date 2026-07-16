using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
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

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostMaidenMagic : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public static int ShardMax = 50;
        public static double ShardDamage = 0.25D;
        public static int ShardCap = 30;
        public const float SorceryDamageBoost = 0.12f;
        public const int ManaRegenBonus = 4;

        public const int ManaBoost = 40;
        public const int SorceryCritBoost = 10;
        public static Color AbilityBriefColor = Color.LightSkyBlue;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, SorceryCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 26;
            Item.defense = 6;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FrostMaidenRobe>() && 
                legs.type == ModContent.ItemType<FrostMaidenPants>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFrostArmor = true;
            mogPlayer.wearingFrostMagic = true;
            player.manaRegenBonus += ManaRegenBonus;
            player.GetDamage<SorceryDamageClass>() += SorceryDamageBoost;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += ManaBoost;
            player.GetCritChance<SorceryDamageClass>() += SorceryCritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<FrostMaidenMagic>() && player.armor[1].type == ItemType<FrostMaidenRobe>() && player.armor[2].type == ItemType<FrostMaidenPants>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<FrostMaidenMagic>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), SorceryDamageBoost.ToPercent(), ManaRegenBonus.ToRegenPerSecond()));
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
                AddIngredient(ItemID.Bone, 20).
                AddIngredient<FrigidShard>(5).
                AddIngredient(ItemID.FlinxFur, 3).
                AddTile(TileID.Anvils).
                Register();
        }
        #endregion
    }
}
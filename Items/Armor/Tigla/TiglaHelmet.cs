using Microsoft.Xna.Framework;
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

namespace MogMod.Items.Armor.Tigla
{
    [AutoloadEquip(EquipType.Head)]
    public class TiglaHelmet : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float RangedDamageBoost = 0.2f;
        public const int RangedCritBoost = 8;
        public static Color AbilityBriefColor = Color.LimeGreen;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent(), RangedCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // so the players hair can be seen with the armor equipped
            ArmorIDs.Head.Sets.DrawHatHair[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.defense = 16;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TiglaVest>() && 
                legs.type == ModContent.ItemType<TiglaPants>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            // hunter and ammo potion effects
            player.detectCreature = true;
            player.ammoPotion = true;

            // knockback immunity
            player.noKnockback = true;

            // rifle scope effect
            player.scope = true;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<TiglaHelmet>() && player.armor[1].type == ItemType<TiglaVest>() && player.armor[2].type == ItemType<TiglaPants>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<TiglaHelmet>("SetBonusNormal").Format(AbilityBriefColor.Hex3()));
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
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteMask, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<DabDadBar>(8).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteHeadgear, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<DabDadBar>(8).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteHelmet, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<DabDadBar>(8).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        #endregion

    }
}
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

namespace MogMod.Items.Armor.Spirited
{
    [AutoloadEquip(EquipType.Head)]
    public class SpiritedHelmet : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeAndMagicDamageBoost = 0.08f;
        public const int ManaBoost = 80;
        public const int MeleeAndMagicCritBoost = 6;
        public static Color AbilityBriefColor = Color.GhostWhite;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeAndMagicCritBoost);
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
            Item.height = 26;
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SpiritedBreastplate>() && 
                legs.type == ModContent.ItemType<SpiritedLeggings>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSpiritArmor = true;
            player.GetJumpState<SpiritJump>().Enable();
            player.statManaMax2 += ManaBoost;
            player.GetDamage<MeleeDamageClass>() += MeleeAndMagicDamageBoost;
            player.GetDamage<MagicDamageClass>() += MeleeAndMagicDamageBoost;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<MeleeDamageClass>() += MeleeAndMagicCritBoost;
            player.GetCritChance<MagicDamageClass>() += MeleeAndMagicCritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<SpiritedHelmet>() && player.armor[1].type == ItemType<SpiritedBreastplate>() && player.armor[2].type == ItemType<SpiritedLeggings>();
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
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<SpiritedHelmet>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), MeleeAndMagicDamageBoost.ToPercent(), ManaBoost));
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
        public override void UpdateVanitySet(Player player)
        {
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? DustID.PlatinumCoin : DustID.SilverCoin, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
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
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritShard>(6).
                AddIngredient<ManaEssence>(3).
                AddTile(TileID.Anvils).
                Register();
        }
        #endregion
    }
}
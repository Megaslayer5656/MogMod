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

namespace MogMod.Items.Armor.Kaminari
{
    [AutoloadEquip(EquipType.Head)]
    public class KaminariHat : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        // default equip
        public const int ManaBoost = 200; // {0}
        public const float ManaReduction = 1f - 0.3f; // {1}
        public const float MagicDamageBoost = 0.3f; // {2}
        public const int MagicCritBoost = 30; // {2}
        // set bonus
        public const int ZipDamage = 250;
        public const int ZipCost = 30;
        public const int ManaRegenBonus = 30; // {1}
        public static Color AbilityBriefColor = Color.SkyBlue; // {0}
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, ManaReduction.ToReversedPercent(), MagicDamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.defense = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<KaminariGarb>() &&
                legs.type == ModContent.ItemType<KaminariPants>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingKaminari = true;
            player.manaRegenBonus += ManaRegenBonus;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += ManaBoost;
            player.manaCost *= ManaReduction;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<KaminariHat>() && player.armor[1].type == ItemType<KaminariGarb>() && player.armor[2].type == ItemType<KaminariPants>();
        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            var Hotkey = KeybindSystem.ArmorSetBonusKeybind.TooltipHotkeyString();
            var SlowHotkey = KeybindSystem.ZipSlowdownKeybind.TooltipHotkeyString();
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    if (Main.keyState.PressingShift())
                    {
                        setBonusIndex++;
                        TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<KaminariHat>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), ManaRegenBonus.ToRegenPerSecond(), Hotkey, SlowHotkey));
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
            player.armorEffectDrawOutlinesForbidden = true;
        }
        public override void UpdateVanitySet(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? DustID.Electric : 160, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.65f;
                Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.03f;
                Main.dust[dust].fadeIn = Main.rand.NextBool(4) ? Main.rand.NextFloat(0f, 0.5f) : 0f;
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
                AddIngredient(ItemID.RoninHat).
                AddIngredient(ItemID.MartianConduitPlating, 150).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
        #endregion
    }
}
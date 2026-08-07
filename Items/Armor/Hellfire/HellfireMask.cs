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

namespace MogMod.Items.Armor.Hellfire
{
    [AutoloadEquip(EquipType.Head)]
    public class HellfireMask : ModItem, ILocalizedModType
    {
        #region Setup
        public new string LocalizationCategory => "Items.Armor";
        public static double DamageMult = 4D;
        public const float DamageBoost = 0.08f;
        public const int DamageCap = 1000;
        public const int DamageCheck = 100;
        public const int BoomCooldown = 360;
        public static Color AbilityBriefColor = Color.OrangeRed;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), DamageCap);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;

            Item.defense = 13; // 45

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HellfireBreastplate>() && 
                legs.type == ModContent.ItemType<HellfireGreaves>();
        }
        #endregion
        #region Armor Stat Changes
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingHellfireArmor = true;
            player.lavaImmune = true;

            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3());
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
        }
        #endregion
        #region Tooltips
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<HellfireMask>() && player.armor[1].type == ItemType<HellfireBreastplate>() && player.armor[2].type == ItemType<HellfireGreaves>();
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
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<HellfireMask>("SetBonusGFB").Format(AbilityBriefColor.Hex3(), DamageCheck, BoomCooldown.FramesToSeconds()));
                            tooltips.Insert(setBonusIndex, briefDescription);
                        }
                        else
                        {
                            setBonusIndex++;
                            TooltipLine briefDescription = new(item.Mod, "MogMod:SetBonus1", MiscUtils.GetTextFromModItem<HellfireMask>("SetBonusNormal").Format(AbilityBriefColor.Hex3(), DamageCap, DamageCheck, BoomCooldown.FramesToSeconds()));
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
            player.armorEffectDrawOutlinesForbidden = true;
        }
        public override void UpdateVanitySet(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(player.position - new Vector2(2f), player.width + 4, player.height + 4, Main.rand.NextBool(3) ? DustID.Lava : 174, player.velocity.X * 0.04f, player.velocity.Y * 0.04f, 100, default, 1f);
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
                AddIngredient(ItemID.MoltenHelmet).
                AddIngredient<HellfireBar>(10).
                AddIngredient<ScorchedCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        #endregion
    }
}
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Windrunner
{
    [AutoloadEquip(EquipType.Body)]
    public class WindrunnerTop : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int FlatRangedDamageBoost = 4;
        public const float ArrowDamageBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlatRangedDamageBoost, ArrowDamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            //ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            //ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.defense = 6;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>().Flat += FlatRangedDamageBoost;
            player.arrowDamage += ArrowDamageBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            WindrunnerTricorn.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.ArmorSetBonusKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WoodlandFabric>(15).
                AddIngredient<FuciumBar>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
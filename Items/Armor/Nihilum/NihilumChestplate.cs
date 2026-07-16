using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Nihilum
{
    [AutoloadEquip(EquipType.Body)]
    public class NihilumChestplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int RangedCritBoost = 32;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;

            Item.defense = 28;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            NihilumHeadgear.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.NulledKeybind);
        }
        ModKeybind keybindActive = null;
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
        }
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophytePlateMail).
                AddIngredient(ItemID.LunarBar, 16).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
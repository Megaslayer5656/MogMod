using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Kaminari
{
    [AutoloadEquip(EquipType.Body)]
    public class KaminariGarb : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MagicDamageBoost = 0.2f;
        public const int MagicCritBoost = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicDamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.defense = 26;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            KaminariHat.ModifySetTooltips(this, tooltips);
            tooltips.IntegrateHotkey(KeybindSystem.ArmorSetBonusKeybind);
            tooltips.IntegrateHotkey(KeybindSystem.ZipSlowdownKeybind);
        }
        ModKeybind keybindActive = null;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RoninShirt).
                AddIngredient(ItemID.MartianConduitPlating, 250).
                AddIngredient(ItemID.LunarBar, 16).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
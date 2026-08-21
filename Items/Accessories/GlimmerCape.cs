using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class GlimmerCape : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        // accessory buffs
        public const int ManaBoost = 50;
        public const int AggroBoost = 400;
        // effect buffs
        public const float MovementSpeedBoost = 0.25f;
        public const int ManaRegenBoost = 4;
        public const int BuffAggroBoost = 750;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, AggroBoost);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.GlimmerCapeKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += ManaBoost;
            player.aggro -= AggroBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.isWearingGlimmerCape = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe, 1).
                AddIngredient<ShadowAmulet>(1).
                AddIngredient<FrigidShard>(3).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
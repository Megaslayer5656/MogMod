using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.TankyRizzler
{
    [AutoloadEquip(EquipType.Body)]
    public class TankyRizzlerChestplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MeleeDamageBoost = 0.08f;
        public const int MeleeCritBoost = 8;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBoost.ToPercent(), MeleeCritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.defense = 38;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;
            player.GetCritChance<MeleeDamageClass>() += MeleeCritBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => TankyRizzlerHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeetleShell, 1).
                AddIngredient(ItemID.MartianConduitPlating, 125).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
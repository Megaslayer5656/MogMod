using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Radiant
{
    [AutoloadEquip(EquipType.Body)]
    public class RadiantTop : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MagicDamageBoost = 0.09f;
        public const int MagicCritBoost = 9;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicCritBoost);
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
            Item.height = 20;
            Item.defense = 17;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => RadiantFlower.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreRobe, 1).
                AddIngredient<FaeBar>(15).
                AddIngredient<ManaCore>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
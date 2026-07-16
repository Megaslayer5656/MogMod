using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Runty
{
    [AutoloadEquip(EquipType.Body)]
    public class RuntyBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int CritBoost = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost);
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = false;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.defense = 2;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => RuntyHelmet.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(16).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
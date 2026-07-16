using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Undying
{
    [AutoloadEquip(EquipType.Body)]
    public class UndyingBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float DamageBoost = 0.1f;
        public const int CritBoost = 12;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), CritBoost);
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
            Item.width = 30;
            Item.height = 20;

            Item.defense = 24;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => UndyingHelm.ModifySetTooltips(this, tooltips);
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrinyRind>(10).
                AddIngredient<UltimateOrb>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
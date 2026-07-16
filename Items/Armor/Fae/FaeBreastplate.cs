using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Fae
{
    [AutoloadEquip(EquipType.Body)]
    public class FaeBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float DamageBoost = 0.15f;
        public const float AmmoReduction = 0.8f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), AmmoReduction.ToReversedPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = false;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;

            Item.defense = 20;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.ammoCost *= AmmoReduction;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => FaeMask.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(15).
                AddIngredient(ItemID.CrystalNinjaChestplate, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
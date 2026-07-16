using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Damascus
{
    [AutoloadEquip(EquipType.Legs)]
    public class DamascusGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int CritBoost = 4;
        public const float MovementBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, MovementBoost.ToPercent());
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.defense = 6;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<DamascusHelm>())
                DamascusHelm.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<DamascusMask>())
                DamascusMask.ModifySetTooltips(this, tooltips);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.moveSpeed += MovementBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
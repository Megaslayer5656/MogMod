using MogMod.Items.Global;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Legs)]
    public class BoneGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float MovementSpeedBoost = 0.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 5;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MovementSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<BoneHelm>())
                BoneHelm.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<BoneMask>())
                BoneMask.ModifySetTooltips(this, tooltips);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 15).
                AddIngredient(ItemID.FossilOre, 15).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Bone
{
    [AutoloadEquip(EquipType.Body)]
    public class BoneMail : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int CritBoost = 4;
        public const float DamageBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, DamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.GetDamage<GenericDamageClass>() += DamageBoost;
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
                AddIngredient(ItemID.Silk, 20).
                AddIngredient(ItemID.FossilOre, 20).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
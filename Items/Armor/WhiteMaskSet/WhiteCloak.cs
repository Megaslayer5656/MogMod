using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.WhiteMaskSet
{
    [AutoloadEquip(EquipType.Body)]
    public class WhiteCloak : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float AttackSpeedBoost = 0.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AttackSpeedBoost.ToPercent());
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            EquipLoader.AddEquipTexture(Mod, "MogMod/Items/Armor/WhiteMaskSet/WhiteCloak_Legs", EquipType.Legs, this);
        }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = false;
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.defense = 14;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => WhiteMask.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 14).
                AddIngredient(ItemID.SoulofFright, 8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

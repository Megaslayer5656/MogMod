using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class Vanguard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int DefenseBoost = 2;
        public const int LifeRegenBoost = 4;
        public const float DamageBlockChance = 0.20f;
        public const int SelfDamageReduction = 40;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeRegenBoost.ToRegenPerSecond(), DamageBlockChance.ToPercent(), SelfDamageReduction);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.defense = DefenseBoost;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.MogMod().wearingVanguard = true;
            player.lifeRegen += LifeRegenBoost;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CobaltShield, 1).
                AddIngredient(ItemID.BandofRegeneration, 1).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddIngredient<VitalityBooster>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

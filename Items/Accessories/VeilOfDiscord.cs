using MogMod.Common.Classes;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class VeilOfDiscord : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float SorceryDamageBoost = 0.07f;
        public const float AttackDamageAndSpeedBoost = 0.05f;
        public const int LifeRegenBoost = 4;
        public const int MaxLifeAndManaBoost = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SorceryDamageBoost.ToPercent(), AttackDamageAndSpeedBoost.ToPercent(), LifeRegenBoost.ToRegenPerSecond(), MaxLifeAndManaBoost);
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<SorceryDamageClass>() += SorceryDamageBoost;
            player.GetDamage<GenericDamageClass>() += AttackDamageAndSpeedBoost;
            player.GetAttackSpeed<GenericDamageClass>() += AttackDamageAndSpeedBoost;
            player.lifeRegen += LifeRegenBoost;
            player.statManaMax2 += MaxLifeAndManaBoost;
            player.statLifeMax2 += MaxLifeAndManaBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>(1).
                AddIngredient<Crown>(1).
                AddIngredient(ItemID.Bone, 40).
                AddIngredient<FuciumBar>(8).
                AddIngredient(ItemID.LargeAmethyst, 1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
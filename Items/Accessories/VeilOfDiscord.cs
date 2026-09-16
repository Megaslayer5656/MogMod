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
        public const int MaxManaBoost = 30;
        public const float DamageReductionBoost = 0.03f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SorceryDamageBoost.ToPercent(), AttackDamageAndSpeedBoost.ToPercent(), DamageReductionBoost.ToPercent(), MaxManaBoost);
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
            player.statManaMax2 += MaxManaBoost;
            player.endurance += DamageReductionBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>().
                AddIngredient<Crown>().
                AddIngredient(ItemID.Bone, 40).
                AddIngredient<FuciumBar>(8).
                AddIngredient(ItemID.LargeAmethyst).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
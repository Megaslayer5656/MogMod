using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class DivineRapier : NeutralItem
    {
        public const float DamageBoost = 0.35f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Expert;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.expert = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife >= (player.statLifeMax2 * 1))
                player.GetDamage(DamageClass.Generic) += DamageBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShinyStone).
                AddIngredient(ItemID.HallowedBar, 10).
                AddIngredient<BrokenHeroShard>(5).
                AddIngredient(ItemID.HallowedKey).
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
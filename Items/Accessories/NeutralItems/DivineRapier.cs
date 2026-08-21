using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class DivineRapier : NeutralItem
    {
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
                player.GetDamage(DamageClass.Generic) += 0.35f;
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
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class PartisanBrand : NeutralItem
    {
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WhisperOfTheDread>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 28;
            Item.height = 34;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .08f;
            player.GetDamage(DamageClass.Summon) += .08f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                DisableDecraft().
                AddIngredient(ItemID.AshWood, 40).
                AddIngredient(ItemID.HellstoneBar, 8).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
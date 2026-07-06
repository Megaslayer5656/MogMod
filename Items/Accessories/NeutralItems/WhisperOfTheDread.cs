using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class WhisperOfTheDread : NeutralItem
    {
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PartisanBrand>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .15f;
            player.GetDamage(DamageClass.Summon) += .15f;

            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWhisperDread = true;
        }
        /* Changed to be shimmered from Partisan Brand
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofNight, 12).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        */
    }
}
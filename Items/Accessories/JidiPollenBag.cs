using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class JidiPollenBag : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingJidiPollenBag = true;

            player.GetDamage(DamageClass.Summon) += .08f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 12).
                AddIngredient(ItemID.SpiderFang, 7).
                AddIngredient(ItemID.WhoopieCushion, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class DragonLance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += .07f;
            player.GetDamage(DamageClass.Ranged) += .05f;
            Player.tileRangeX = Player.tileRangeY += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BeltOfStrength>(1).
                AddIngredient<FuciumBar>(7).
                AddIngredient(ItemID.Ruby, 5).
                AddIngredient(ItemID.AntlionMandible, 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

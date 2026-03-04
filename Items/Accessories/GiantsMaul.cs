using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class GiantsMaul : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float sizeMult = 1.3f;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGiantsMaul = true;

            player.GetAttackSpeed(DamageClass.Melee) -= .20f;
        }
        public static float GiantsMaulWeaponSize(MogPlayer mogPlayer)
        {
            return sizeMult;
        }
        public override void AddRecipes()
        {
            // give this another item to make the recipe more interesting
            CreateRecipe().
                AddIngredient<SkullBasher>(1).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

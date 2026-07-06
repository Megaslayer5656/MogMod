using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class GiantsMaul : NeutralItem
    {
        public const float sizeMult = 1.3f;
        public static double DamageMult = 2D;
        public const int DamageCap = 100;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGiantsMaul = true;

            player.GetAttackSpeed<MeleeDamageClass>() -= .20f;

            if (Main.netMode != NetmodeID.MultiplayerClient && !MogModWorld.HasFoundGiantsMaul)
            {
                MogModWorld.HasFoundGiantsMaul = true;
                MogModNetcode.SyncWorld();
            }
        }
        public static float GiantsMaulWeaponSize(MogPlayer mogPlayer)
        {
            return sizeMult;
        }
        /* Moved to be guaranteed in a custom structure chest
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkullBasher>(1).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
        */
    }
}
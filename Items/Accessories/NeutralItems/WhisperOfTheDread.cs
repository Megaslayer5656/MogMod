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
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += .15f;
            player.GetDamage(DamageClass.Summon) += .15f;

            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWhisperDread = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofNight, 12).
                AddIngredient<FrigidCrystal>(3).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
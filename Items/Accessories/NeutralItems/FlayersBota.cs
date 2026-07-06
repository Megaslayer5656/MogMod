using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class FlayersBota : NeutralItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float BloodMult = 1.3f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 30;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFlayersBota = true;
            player.GetAttackSpeed<GenericDamageClass>() += 0.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Material"}", 15).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient<HellfireEssence>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
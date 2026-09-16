using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class JidiPollenBag : NeutralItem
    {
        public const float SummonDamageBoost = 0.08f;
        public const float ProcChance = 0.1f;
        public const int ArmorReduction = 20;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SummonDamageBoost.ToPercent(), ProcChance.ToPercent(), ArmorReduction);
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
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingJidiPollenBag = true;

            player.GetDamage(DamageClass.Summon) += SummonDamageBoost;
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
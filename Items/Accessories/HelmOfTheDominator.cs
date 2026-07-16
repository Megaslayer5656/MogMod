using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HelmOfTheDominator : ModItem, ILocalizedModType
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
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Summon) += .10f;
            player.statManaMax2 += 50;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.dominatorMinion = true;
            mogPlayer.diademMinion = true;
            mogPlayer.wearingHelmOfDominator = true;
            player.spiderMinion = true; // temp slop
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HelmOfIronWill>(1).
                AddIngredient<Diadem>(1).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Emblem"}", 1).
                AddRecipeGroup("CobaltBar", 8).
                AddIngredient(ItemID.Topaz, 2).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
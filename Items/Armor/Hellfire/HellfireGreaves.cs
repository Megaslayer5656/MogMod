using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Hellfire
{
    [AutoloadEquip(EquipType.Legs)]
    public class HellfireGreaves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;

            Item.defense = 14;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 10;
            player.moveSpeed += .12f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenGreaves).
                AddIngredient<GriefBar>(12).
                AddIngredient<ScorchedCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
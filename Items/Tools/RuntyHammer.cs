using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class RuntyHammer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = Item.height = 40;

            Item.damage = 10;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;

            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.hammer = 45;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; // melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RuntyBar>(8)
                .AddRecipeGroup(RecipeGroupID.Wood, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

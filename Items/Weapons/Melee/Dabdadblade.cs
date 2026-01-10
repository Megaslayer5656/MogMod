using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Weapons.Melee
{
	public class Dabdadblade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6.5f;
			Item.value = 10000;
			Item.rare = 4;
			Item.UseSound = SoundID.Item7;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.CursedFlameFriendly;
			Item.shootSpeed = 10f;
		}
		public override void AddRecipes()
		{
			// ooo look at me im soooooo rare only i use Recipe recipe = CreateRecipe() oooo im soooooooo rare~~~
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Emerald, 15);
			recipe.AddRecipeGroup("CobaltBar", 15);
			recipe.AddIngredient(ItemID.CursedFlame, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
    }
}
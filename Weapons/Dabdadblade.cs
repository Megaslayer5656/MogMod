using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Weapons
{
	public class Dabdadblade : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.MogMod.hjson file.

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
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Emerald, 15);
			recipe.AddIngredient(ItemID.PalladiumBar, 15);
			recipe.AddIngredient(ItemID.CursedFlame, 20);
			recipe.AddIngredient(ItemID.SoulofSight, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ItemID.Emerald, 15);
			recipe2.AddIngredient(ItemID.CobaltBar, 15);
			recipe2.AddIngredient(ItemID.CursedFlame, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe2.AddTile(TileID.MythrilAnvil);
			recipe2.Register();
		}
    }
}
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class RuntyDrill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetStaticDefaults() {
			// As mentioned in the documentation, IsDrill and IsChainsaw automatically reduce useTime and useAnimation to 60% of what is set in SetDefaults and decrease tileBoost by 1, but only for vanilla items.
			// We set it here despite it doing nothing because it is likely to be used by other mods to provide special effects to drill or chainsaw items globally.
			ItemID.Sets.IsDrill[Type] = true;
		}
		public override void SetDefaults() {
			Item.width = 48;
			Item.height = 22;
			// IsDrill/IsChainsaw effects must be applied manually, so 60% or 0.6 times the time of the corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
			// If you decide to copy values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.
			Item.damage = 7;
			Item.DamageType = DamageClass.MeleeNoSpeed; // ignores melee speed bonuses. There's no need for drill animations to play faster, nor drills to dig faster with melee speed.
			Item.useTime = 5;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.5f;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;

            Item.UseSound = SoundID.Item23;
			Item.shoot = ModContent.ProjectileType<RuntyDrillProj>();
			Item.shootSpeed = 32f; // Adjusts how far away from the player to hold the projectile
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;

			// tileBoost changes the range of tiles that the item can reach.
			// To match Drills, we set this to -1
			Item.tileBoost = -1;
			Item.pick = 40; // How strong the drill is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<RuntyBar>(10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
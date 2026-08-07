using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class GreatswordOfSouls : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 86;

            Item.damage = 165;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 13f;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<GreatswordOfSoulsHoldout>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override bool MeleePrefix() => true;
        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<SoulFragment>(5).
                AddIngredient<GriefBar>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
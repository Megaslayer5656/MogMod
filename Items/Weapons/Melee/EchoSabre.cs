using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class EchoSabre : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 60;

            Item.damage = 71;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 60;
            Item.knockBack = 10f;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<EchoSabreHoldout>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override bool MeleePrefix() => true;
        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyCobaltBar", 22).
                AddIngredient(ItemID.SoulofNight, 6).
                AddIngredient<FrigidCrystal>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
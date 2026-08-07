using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class BladeOfSelves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 50;

            Item.damage = 94;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 60;
            Item.knockBack = 12f;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<BladeOfSelvesHoldout>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool MeleePrefix() => true;
        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EchoSabre>().
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient<UltimateOrb>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
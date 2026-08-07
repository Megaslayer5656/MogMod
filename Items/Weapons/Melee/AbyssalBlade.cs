using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // TODO: resprite
    public class AbyssalBlade : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<AbyssalBladeHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 66;
            Item.height = 66;

            Item.damage = 166;
            Item.crit = 56;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 28;
            Item.knockBack = 12f;
            Item.shootSpeed = 12f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkullBasher>().
                AddIngredient<Sange>().
                AddIngredient(ItemID.VampireKnives).
                AddRecipeGroup("AnyAdamantiteBar", 15).
                AddIngredient<GriefBar>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
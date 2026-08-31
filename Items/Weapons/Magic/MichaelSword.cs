using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class MichaelSword : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override int ProjectileType => ModContent.ProjectileType<MichaelSwordHoldout>();
        public const float ExplosionExpandFactor = 1.013f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 86;
            Item.height = 92;
            
            Item.mana = 24;
            Item.damage = 110;
            Item.knockBack = 7f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 52;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.autoReuse = true;
            
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(15).
                AddIngredient(ItemID.FireFeather).
                AddIngredient(ItemID.IceFeather).
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<ManaCore>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
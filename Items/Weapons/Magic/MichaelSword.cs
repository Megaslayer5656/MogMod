using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class MichaelSword : ModItem, ILocalizedModType
    {
        // code taken from example swing sword
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const float ExplosionExpandFactor = 1.013f;
        public static readonly SoundStyle SwingSound = SoundID.Item94;
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 92;
            Item.damage = 110;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.mana = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.shoot = ModContent.ProjectileType<MichaelSwordHoldout>();
            Item.shootSpeed = 1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FaeBar>(15).
                AddIngredient(ItemID.FireFeather, 1).
                AddIngredient(ItemID.IceFeather, 1).
                AddIngredient(ItemID.BrokenHeroSword, 1).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
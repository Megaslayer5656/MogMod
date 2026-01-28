using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class DagonOne : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DagonOneProj>();
            Item.shootSpeed = 11f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FlareGun, 1).
                AddIngredient(ItemID.WandofSparking, 1).
                AddIngredient(ItemID.Torch, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
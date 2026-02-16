using Terraria;
using Terraria.ID;
using MogMod.Items.Ammo;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic //So yeah I'm gonna make a glintstone sorcery inspired magic weapon line, with shard spiral, glintstone stars, and star shower from elden ring. Will have cool vfx. Placeholder file for now until I get home and work on it.
    // making glintstone pebble -> glintstone staff that fires ammo types and gonna make glintstone pebble a non-consumable ammo type that fires glintstone pebble proj;

{
    public class GlintstoneStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // fires glintstone ammo types;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DiamondStaff, 1)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.ManaCrystal, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

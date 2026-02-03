using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic //So yeah I'm gonna make a glintstone sorcery inspired magic weapon line, with shard spiral, glintstone stars, and star shower from elden ring. Will have cool vfx. Placeholder file for now until I get home and work on it.
{
    public class GlintstonePebble : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 7, 30, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            //Item.shoot = ModContent.ProjectileType<GlintstonePebbleProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
        }
    }
}

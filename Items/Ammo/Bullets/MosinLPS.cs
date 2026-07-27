using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo.Bullets
{
    public class MosinLPS : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 16;

            Item.damage = 31;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.shoot = ModContent.ProjectileType<MosinLPSProj>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Bullet;

            Item.value = Item.sellPrice(copper: 3);
            Item.rare = ItemRarityID.Green;
        }
    }
}   
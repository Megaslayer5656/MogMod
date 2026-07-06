using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class MosinLPS : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 10;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(copper: 3);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<MosinLPSProj>();
            Item.shootSpeed = 15f;
            Item.ammo = Item.type;
        }
    }
}   
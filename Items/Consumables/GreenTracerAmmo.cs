using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Projectiles;

namespace MogMod.Items.Consumables
{
    public class GreenTracerAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 58; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<GreenTracerProj>();
            Item.shootSpeed = 4.5f;
            Item.ammo = Item.type;
        }
    }
}

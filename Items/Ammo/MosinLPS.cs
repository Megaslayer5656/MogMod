using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Ammo
{
    public class MosinLPS : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 150;
            Item.height = 94;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 0, 1, 3);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<MosinLPSProj>();
            Item.shootSpeed = 15f;
            Item.ammo = Item.type;
        }
    }
}

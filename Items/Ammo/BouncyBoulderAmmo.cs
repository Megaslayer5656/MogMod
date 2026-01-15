using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Common.Classes;
using Terraria.ID;

namespace MogMod.Items.Ammo
{
    public class BouncyBoulderAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<BoulderClass>();
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.BouncyBoulder;
            Item.shootSpeed = 4.5f;
            Item.ammo = Item.type;
        }
    }
}

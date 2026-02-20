using MogMod.Projectiles.RangedProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace MogMod.Items.Ammo
{
    public class SplinterAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 15;
            Item.height = 187;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 0, 1, 3);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<SplinterProjectile>();
            Item.shootSpeed = 1f;
            Item.ammo = Item.type;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient(ItemID.Wood, 2).
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}

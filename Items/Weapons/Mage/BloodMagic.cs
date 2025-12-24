using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Projectiles;
using Terraria.ID;

namespace MogMod.Items.Weapons.Mage
{
    public class BloodMagic : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 7, 30, 50);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodMagicProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
              AddIngredient(ItemID.Book, 1).
              AddRecipeGroup("CrimtaneBar", 15).
              AddRecipeGroup("TissueSample", 10).
              AddTile(TileID.Anvils).
              Register();
        }
    }
}

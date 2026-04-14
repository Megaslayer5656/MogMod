using MogMod.Projectiles.RangedProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MogMod.Items.Ammo
{
    // unobtainable until it does something different
    public class EnergyBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<EnergyBulletProj>();
            Item.shootSpeed = 5f;
            Item.ammo = AmmoID.Bullet;
        }
        //public override void AddRecipes()
        //{
        //    CreateRecipe(70).
        //        AddIngredient(ItemID.MusketBall, 70).
        //        AddIngredient(ItemID.Ectoplasm, 1).
        //        AddTile(TileID.Anvils).
        //        Register();
        //}
    }
}

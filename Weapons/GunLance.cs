using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social.WeGame;

namespace MogMod.Weapons
{
    public class GunLance : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 10;
            Item.height = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 4.5f;
            Item.value = 10000;
            Item.rare = 3;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            else
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.damage = 40;
                Item.DamageType = DamageClass.Melee;
                Item.width = 10;
                Item.height = 10;
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.useStyle = 3;
                Item.knockBack = 6.5f;
                Item.value = 10000;
                Item.rare = 3;
                Item.UseSound = SoundID.Item1;
                Item.autoReuse = true;
            }
            else 
            {
                Item.damage = 35;
                Item.DamageType = DamageClass.Ranged;
                Item.width = 10;
                Item.height = 10;
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.useStyle = 5;
                Item.knockBack = 4.5f;
                Item.value = 10000;
                Item.rare = 3;
                Item.UseSound = SoundID.Item11;
                Item.autoReuse = true;
                Item.shoot = ProjectileID.Bullet;
                Item.shootSpeed = 20f;
            }
            return true;
        }
    }
}

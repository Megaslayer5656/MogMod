using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Items.Consumables;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Ranged
{
    public class R8Revolver :ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool rotated = false;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 100;
            Item.height = 19;
            Item.scale = .15f;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/R8RevolverShot")
            {
                Volume = .75f,
                PitchVariance = .02f,
            };
            Item.value = Item.buyPrice(0, 32, 82, 5);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = Item.useAnimation = 12;
                rotated = true;
            } else
            {
                Item.useTime = Item.useAnimation = 45;
                rotated = false;
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (rotated)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15f, -.5f);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
               AddIngredient(ItemID.Revolver, 1).
               AddRecipeGroup("IronBar", 20).
               AddRecipeGroup("GoldBar", 15).
               AddTile(TileID.Anvils).
               Register();

        }
    }
}

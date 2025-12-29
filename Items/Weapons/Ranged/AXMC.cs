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
using MogMod.Projectiles;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons.Ranged
{
    public class AXMC : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 1150; //Damage of ap lapua in tarkov * 10
            Item.DamageType = DamageClass.Ranged;
            Item.width = 193;
            Item.height = 37;
            Item.scale = .5f;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/AXMCShot")
            {
                Volume = 2.25f,
                PitchVariance = .02f,
            };
            Item.value = Item.buyPrice(0, 32, 82, 5);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 200f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 71;
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25f, -.5f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<APLapua>();
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            Vector2 leftOffset = new Vector2(67f, .5f); //That number isn't even a joke, it's somehow the offset that looks the best
            position += leftOffset;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.scope = true;
                return true;
            }
            return base.AltFunctionUse(player);
        }
        //TODO: Add way to craft the AXMC
        // make it use 8 vortex frags and TileID.LunarCraftingStation
    }
}

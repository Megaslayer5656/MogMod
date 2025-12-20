using MogMod.Common.Classes;
using MogMod.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons.Boulder
{
    public class BouncerSwitch : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<BoulderClass>();
            Item.width = 50;
            Item.height = 34;
            Item.scale = .5f;
            Item.useTime = 3; //Accurate to Glock 18c firerate (at least in Tarkov)
            Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2")
            {
                Volume = .2f,
                PitchVariance = .02f,
            };
            Item.value = 10000;
            Item.rare = 3;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BouncyBoulder;
            Item.shootSpeed = 20f;
            Item.useAmmo = ModContent.ItemType<BouncyBoulderAmmo>();
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, ProjectileID.BouncyBoulder, damage, knockback);
            Main.projectile[proj].friendly = true;
            Main.projectile[proj].DamageType = ModContent.GetInstance<BoulderClass>();
            Main.projectile[proj].owner = player.whoAmI;
            Main.projectile[proj].numHits = 20;
            Main.projectile[proj].netUpdate = true;
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            //TODO: Give spread to the shots
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(10f, 1.5f);
        }
    }
}
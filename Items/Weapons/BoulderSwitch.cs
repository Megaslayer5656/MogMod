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
using MogMod.Common.Classes;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons
{
    public class BoulderSwitch : ModItem
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
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = ModContent.ItemType<BoulderAmmo>();
            Item.noMelee = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            //TODO: Give spread to the shots
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            type = ProjectileID.Boulder;
            Main.projectile[type].friendly = true;
            Main.projectile[type].DamageType = ModContent.GetInstance<BoulderClass>();
            Main.projectile[type].owner = player.whoAmI;
            Main.projectile[type].numHits = 20;
            Main.projectile[type].netUpdate = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(10f, 1.5f);
        }
    }
}

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

namespace MogMod.Items.Weapons.Ranged
{
    public class Mosin : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 100;
            Item.height = 19;
            Item.scale = .8f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/MosinShot")
            {
                Volume = .3f,
                PitchVariance = .02f,
            };
            Item.value = Item.buyPrice(0, 32, 82, 5);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = ModContent.ItemType<MosinLPS>(); //TODO: Add another mosin ammo (more pen and damage) and let the mosin use that as well.
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15f, -.5f);
        }


    }
}

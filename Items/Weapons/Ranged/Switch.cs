using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace MogMod.Items.Weapons.Ranged
{
    public class Switch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults() {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 34;
            Item.scale = .5f;
            Item.useTime = 3; //Accurate to Glock 18c firerate (at least in Tarkov)
            Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2") {
                Volume = .2f,
                PitchVariance = .02f,
            };
            Item.value = 10000;
            Item.rare = 3;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<GreenTracerAmmo>();
            Item.noMelee = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));

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
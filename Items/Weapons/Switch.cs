using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Runtime.CompilerServices;
using Terraria.Social.WeGame;
using Mono.Cecil;
namespace MogMod.Items.Weapons
{
    public class Switch : ModItem {
        public override void SetDefaults() {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 34;
            Item.scale = .5f;
            Item.useTime = 3; //Accurate to Glock 18c firerate (at least in Tarkov)
            // set to 15 for burst fire
            Item.useAnimation = 15;
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
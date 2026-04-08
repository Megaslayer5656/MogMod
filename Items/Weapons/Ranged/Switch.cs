using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Rarities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Weapons.Ranged
{
    public class Switch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults() 
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 34;
            Item.scale = .5f;
            Item.knockBack = 2.2f;
            Item.useTime = Item.useAnimation = 3; // Accurate to Glock 18c firerate (at least in Tarkov)
            // values of 4 and below cause it to lose out on speed reforges like unreal
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2") {
                Volume = .2f,
                PitchVariance = .02f,
            };
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 14f;

            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
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
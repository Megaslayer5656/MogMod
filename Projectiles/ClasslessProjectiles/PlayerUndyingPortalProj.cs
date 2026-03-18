using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class PlayerUndyingPortalProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.ClasslessProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 350;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
        }
        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.position.X = player.position.X + (player.width / 2) - (Projectile.width / 2);
                Projectile.position.Y = player.position.Y + (player.height / 2) - (Projectile.height / 2);
            }
            var source = Projectile.GetSource_FromThis();
            if (Projectile.timeLeft <= 300 && Projectile.timeLeft % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

                Random spawnNumb = new Random();
                int[] amount = { 6, 8 };
                int choice = amount[spawnNumb.Next(amount.Length)];

                float offset = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < choice; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / choice) - offset).ToRotationVector2() * (choice / 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UndyingHomingProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 4; i++)
                {
                    int deathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald);
                    Main.dust[deathDust].noGravity = true;
                    Main.dust[deathDust].scale = 1.75f;
                }
            }
        }
    }
}
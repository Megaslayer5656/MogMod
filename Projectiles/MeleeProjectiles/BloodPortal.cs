using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII; // what?

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class BloodPortal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.alpha = 120;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.scale += .2f;
            float width = (float)Projectile.width * Projectile.scale;
            float height = (float)Projectile.height * Projectile.scale;
            Projectile.Center = Main.player[Projectile.owner].Center;

            for (int i = 0; i < 70; i++)
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(width / 2.1f, height / 2.1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + randomOffset, DustID.Blood, Projectile.DirectionFrom(Projectile.Center + Projectile.velocity + randomOffset) * Main.rand.NextFloat(5f, 7f));
                d.fadeIn = .15f;
                d.scale = 1.5f;
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(width / 2f, height / 2f);
                Dust telegraphDust = Dust.NewDustPerfect(Projectile.Center + randPos, DustID.Blood, Projectile.DirectionFrom(Projectile.Center + Projectile.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Projectile.scale, .1f, .1f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LordOfBloodsSpearBloodProj>(), Projectile.damage, 0, Projectile.owner);
        }
    }
}

using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class BloodPortal : ModProjectile, ILocalizedModType
    {
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
            Vector2 dustPos = Projectile.Center - new Vector2(width / 2f, height / 2f);
            Vector2 randomOffset = Main.rand.NextVector2Circular(width / 2f, height / 2f); //Maybe remove this?

            for (int i = 0; i < 100; i++)
            {
                int d = Dust.NewDust(dustPos + randomOffset, Convert.ToInt32(width), Convert.ToInt32(height), DustID.Blood);
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

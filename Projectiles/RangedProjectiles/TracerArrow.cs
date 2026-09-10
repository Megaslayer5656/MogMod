using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class TracerArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public int StuckTime = 60; // seconds
        public bool Collided = false;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 70;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 2;

            Projectile.hide = true;
            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (!Collided || Projectile.ai[0] != 1f) Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.StickyNPCProjAI(StuckTime);
            //Projectile.tileCollide = Projectile.ai[0] != 1f && Projectile.localAI[1]++ > 4f;
            if (Projectile.ai[0] == 1f) Projectile.timeLeft = StuckTime * 60;
            if (Projectile.localAI[1] > 0f)
            {
                Projectile.localAI[1]++;
                if (Collided && Projectile.tileCollide == false && Projectile.localAI[1] > 4f)
                {
                    Projectile.tileCollide = true;
                    Projectile.velocity *= 0.8f;
                    Projectile.velocity.X = 0f;
                }
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 drawPos = Projectile.Center - Vector2.UnitY + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * -0.5f;
                Dust dust = Dust.NewDustPerfect(drawPos + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool(3) ? DustID.Flare : DustID.Torch);
                dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                dust.velocity = -Projectile.velocity * (Projectile.ai[0] == 1f ? 0.02f : 0.7f);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Collided)
            {
                Collided = true;
                Projectile.localAI[1]++;
                Projectile.tileCollide = false;
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(1);
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
        public override bool? CanDamage() => Projectile.ai[0] != 1f;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                dustVelocity.Normalize();
                dustVelocity *= 50;

                int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Flare : DustID.Stone, 0, 0, 100, default, 1f);
                Dust dust = Main.dust[dagonDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
            }
        }
    }
}
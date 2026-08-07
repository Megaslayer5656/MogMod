using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Melee
{
    public class BladeOfSelvesProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public bool canHit = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.7853982f;
            Projectile.alpha = 100;
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 2 == 0) Projectile.alpha += 12;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.alpha += 25;
            if (Projectile.alpha >= 255) Projectile.Kill();
            if (Projectile.ai[2] < 2 && canHit)
            {
                MogModUtils.ProjectileBarrage(Projectile.GetSource_FromThis(), target.Center, target.Center, Projectile.direction == 1, 150f, 150f, -150f, 150f, 10f, ModContent.ProjectileType<BladeOfSelvesProj>(), (int)(Projectile.damage * 0.95), 0f, Projectile.owner, false, 0f, ai2: Projectile.ai[2] + 1);
                canHit = false;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.75f, Pitch = 0.3f, MaxInstances = 3 }, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 1, 1, DustID.ShimmerSpark, 0, 0, 0, default, 1f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            for (int i = 0; i < 25; i++)
            {
                Color auraColor = Color.Lerp(Color.Pink, Color.Goldenrod, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f) with { A = 0 } * 0.15f;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 5;
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset, null, auraColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            return true;
        }
    }
}
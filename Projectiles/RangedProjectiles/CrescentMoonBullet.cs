using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Graphics;
using MogMod.Items.Weapons.Ranged;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class CrescentMoonBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "MogMod/Assets/Textures/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Color Colour = Color.Turquoise;
        public bool HitEnemy = false;
        public float MinCharge = 8f;
        NPC potentialTarget = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (Projectile.ai[2] == 0f) Colour = Color.SeaGreen;
            MinCharge *= Projectile.extraUpdates;
        }
        public override void AI()
        {
            if (!HitEnemy)
            {
                Projectile.rotation = Projectile.velocity.ToRotation(); // important so that trails can be drawn correctly
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 15;
                    if (Projectile.alpha < 0) Projectile.alpha = 0;
                }
            }
            if (HitEnemy)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.6f), 100);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            dust.color = Main.rand.NextBool(3) ? Colour : Colour * 0.5f;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= MinCharge)
            {
                if (Projectile.ai[0] == MinCharge)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        potentialTarget = Projectile.Center.ClosestNPCAt(1500f, true, true);
                        if (potentialTarget != null)
                        {
                            DustEffect();
                            Vector2 randPos = Main.rand.NextVector2CircularEdge(potentialTarget.width * 3f, potentialTarget.height * 3f);
                            Projectile.Center = potentialTarget.Center + randPos;
                            float angularTurnSpeed = MathHelper.ToRadians(360f);
                            float idealDirection = Projectile.AngleTo(potentialTarget.Center);
                            float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                            Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length();
                            DustEffect();
                        }
                        else KillEffect();
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.tileCollide = false;
            }
        }
        public void DustEffect()
        {
            float dustLoopcheck = 16f;
            int dustIncr = 0;
            while (dustIncr < dustLoopcheck)
            {
                Vector2 dustRotate = Vector2.UnitX * 0f;
                dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                dustRotate = dustRotate.RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int bedman = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Colour, 1f);
                Main.dust[bedman].scale = 1.5f;
                Main.dust[bedman].noGravity = true;
                Main.dust[bedman].position = Projectile.Center + dustRotate;
                Main.dust[bedman].velocity = Projectile.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                dustIncr++;
            }
        }
        public override bool? CanDamage() => !HitEnemy && Projectile.ai[0] >= MinCharge;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.owner == Main.myPlayer) modifiers.SourceDamage *= (Owner.MogMod().radiancePower + 1f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            KillEffect();
            HitEnemyEffect(target, hit);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            KillEffect();
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;
            return false;
        }
        public void HitEnemyEffect(NPC target, NPC.HitInfo hit)
        {
            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch,
                    new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
                    Projectile.owner);
            }
            float powerGain = MathHelper.Clamp(hit.Damage * 0.00025f, 0.01f, 1f);
            float powerLoss = CrescentMoon.maxBarrageShots / 100;
            Main.NewText($"{Owner.MogMod().crescentMoonPower}, {powerGain}, {powerLoss}", Colour);
            if (Projectile.owner == Main.myPlayer && Owner.MogMod().crescentMoonPower < 1f && Projectile.ai[1] == 5f) Owner.MogMod().crescentMoonPower += powerGain;
            if (Projectile.owner == Main.myPlayer && Owner.MogMod().crescentMoonPower > 0f && Projectile.ai[1] != 5f) Owner.MogMod().crescentMoonPower -= powerLoss;
        }
        public void KillEffect()
        {
            if (!HitEnemy)
            {
                if (Projectile.ai[0] <= MinCharge) DustEffect();
                Projectile.velocity = Vector2.Zero;
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                for (int i = 0; i < 4; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.FireworksRGB, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, Colour, .5f);
                }
                HitEnemy = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "MagicMissile", Color.White, Colour, minLength: 20, maxLength: 30);

            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(tex, drawPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            Color drawColour = Projectile.GetAlpha(Colour);
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Texture2D starTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/BoltParticle").Value;
                Main.EntitySpriteDraw(starTex, drawPosition, null, drawColour * (0.25f + i), Projectile.rotation + MathHelper.PiOver2, starTex.Size() * 0.5f, Projectile.scale * (0.5f + i), SpriteEffects.None);

                Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Assets/Textures/GlowParticle").Value;
                if (i % 0.5f == 0) Main.EntitySpriteDraw(bloomTex, drawPosition, null, drawColour * (0.75f - i), Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * (0.15f + i), SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        } /*
        public override bool PreDraw(ref Color lightColor)
        {
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);

                if (Projectile.timeLeft <= 1550)
                {
                    // backtrail
                    Vector2 trailOffset = Projectile.oldVelocity * 5f;
                    for (float n = 0; n < 4; n++)
                    {
                        Color newColor = Colour * 0.4f;
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.05f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.05f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.1f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.1f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.25f, SpriteEffects.None);
                    }
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        } */
    }
}
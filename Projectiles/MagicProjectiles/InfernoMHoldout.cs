using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    // code taken from calamity mod burning sea
    public class InfernoMHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MagicProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public static Color Colour => new(255, 107, 28);
        public bool CanExplode = true;
        public bool DoubleDamage = false;
        public bool Released = false;
        public const int AttackSpeed = 15;
        public const float StartScale = 0.0004f;
        public const float EndScale = 10.25f;
        public const float ChargeTime = 180f;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = AttackSpeed;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Timer++;
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * (Projectile.scale * 0.5f));
            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            float dustTimer = 0f;
            if (Timer <= ChargeTime)
                dustTimer++;
            if (Owner.CantUseHoldout() || !canUseMana)
            {
                Released = true;
                Projectile.scale -= 0.2f;
                Projectile.ExpandHitboxBy((int)(Projectile.scale * 50f));
                // swirling dust effect
                for (int i = 0; i < 10; i++)
                {
                    float randomAngle = Main.rand.NextFloat() * MathHelper.TwoPi;
                    float outwardnessFactor = Main.rand.NextFloat();
                    Vector2 spawnPosition = Projectile.Center + randomAngle.ToRotationVector2() / (Utils.Remap(Timer, 0f, ChargeTime, StartScale, EndScale) * 50f);
                    Vector2 velocity = (randomAngle - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (10f + 9f * Main.rand.NextFloat() + 4f * outwardnessFactor);
                    Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, DustID.Flare, new Vector2?(velocity), 0, default, 1.4f);
                    swirlingDust.fadeIn = 0.25f + outwardnessFactor * 0.2f;
                    swirlingDust.noGravity = true;
                }
                if (Projectile.scale <= 0)
                    Projectile.Kill();
            }
            else if (!Released)
            {
                // Follow the player's position.
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 projLocation = Owner.Center;
                    Vector2 mouse = Owner.ClampedMouseWorld();
                    float mouseDist = Vector2.Distance(mouse, projLocation);
                    Vector2 mouseDiff = mouse - projLocation;
                    if (mouseDist > 128f)
                    {
                        mouseDiff.Normalize();
                        mouseDiff *= 128f;
                    }
                    projLocation += mouseDiff;

                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(projLocation - Projectile.Center, 25f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 0.08f);
                    Projectile.netUpdate = true;
                }

                // Slowly increase in size as the maelstrom is charged up
                Projectile.scale = Utils.Remap(Timer, 0f, ChargeTime, StartScale, EndScale);
                Projectile.ExpandHitboxBy((int)(Projectile.scale * 50f));

                // Consume mana periodically
                if (Timer % AttackSpeed == 0f && Timer >= ChargeTime)
                {
                    Owner.CheckMana(Owner.HeldItem, -1, true);
                    CanExplode = true;
                }

                // swirling dust effect
                for (int i = 0; i < 10; i++)
                {
                    float randomAngle = Main.rand.NextFloat() * MathHelper.TwoPi;
                    float outwardnessFactor = Main.rand.NextFloat();
                    Vector2 spawnPosition = Projectile.Center + randomAngle.ToRotationVector2() * MathHelper.Lerp(0f, dustTimer, dustTimer);
                    Vector2 velocity = (randomAngle - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (10f + 9f * Main.rand.NextFloat() + 4f * outwardnessFactor);
                    Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, DustID.Flare, new Vector2?(velocity), 0, default, 1.4f);
                    swirlingDust.fadeIn = 0.25f + outwardnessFactor * 0.2f;
                    swirlingDust.noGravity = true;
                }

                if (Timer > ChargeTime)
                {
                    DoubleDamage = true;
                    // 6 tiny dusts swirling when fully charged
                    for (int n = 0; n < 6; n++)
                    {
                        float swirlRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * n);
                        Vector2 swirlPos = Projectile.Center + Vector2.UnitX.RotatedBy(swirlRotation) * 220f;
                        Vector2 swirlVelocity = Vector2.Normalize(swirlPos - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f;
                        Dust swirlDust = Dust.NewDustPerfect(swirlPos, DustID.CopperCoin, swirlVelocity * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                        swirlDust.noGravity = true;
                    }
                    // fire aura
                    for (int i = 0; i < 50; i++)
                    {
                        float randomAngle = Main.rand.NextFloat() * MathHelper.TwoPi;
                        float outwardnessFactor = Main.rand.NextFloat();
                        Vector2 spawnPosition = Projectile.Center + randomAngle.ToRotationVector2() * MathHelper.Lerp(20f, 140f, outwardnessFactor);
                        Vector2 velocity = (randomAngle - 3f * MathHelper.Pi / 8f).ToRotationVector2() * (10f + 9f * Main.rand.NextFloat() + 4f * outwardnessFactor);
                        Dust swirlingDust = Dust.NewDustPerfect(spawnPosition, DustID.Flare, new Vector2?(velocity), 0, default, 2f);
                        swirlingDust.fadeIn = 0.25f + outwardnessFactor * 0.05f;
                        swirlingDust.noGravity = true;
                    }
                }
            }
            AdjustPlayerValues();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Released && DoubleDamage ? 2f : 1f;
        }
        public void AdjustPlayerValues()
        {
            if (!Released)
            {
                Projectile.spriteDirection = Projectile.direction = Owner.direction;
                Owner.heldProj = Projectile.whoAmI;
                Owner.itemTime = 2;
                Owner.itemAnimation = 2;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            if (Timer > ChargeTime)
            {
                if (CanExplode)
                {
                    var source = Projectile.GetSource_FromThis();
                    Projectile explosion = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireBoom>(), Projectile.damage * 2, 0f, Main.myPlayer, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                    explosion.DamageType = DamageClass.Magic;
                    CanExplode = false;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            if (Timer > ChargeTime)
            {
                if (CanExplode)
                {
                    var source = Projectile.GetSource_FromThis();
                    Projectile explosion = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<HellfireBoom>(), Projectile.damage * 2, 0f, Main.myPlayer, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                    explosion.DamageType = DamageClass.Magic;
                    CanExplode = false;
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.zenithWorld)
            {
                Vector2 truthPosition = Projectile.Center - Main.screenPosition;
                Texture2D truthNova = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/true").Value;
                Main.EntitySpriteDraw(truthNova, truthPosition, null, Colour * 0.85f, Projectile.rotation, truthNova.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None);
                return false;
            }
            // glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
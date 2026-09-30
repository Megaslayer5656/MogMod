using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Systems;
using MogMod.Items.Weapons.Classless;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    // code partially lifted from calamity mod gilded Proboscis
    public class HurricanePikeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public static readonly SoundStyle WeakCharge = new($"{nameof(MogMod)}/Sounds/SE/bowChargeWeak") { Volume = 1.1f, PitchVariance = .2f, MaxInstances = 5 };
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<HurricanePike>()).Item;
        private Player Owner => Main.player[Projectile.owner];
        public override bool UseMeleeSize => false;
        public override int AfterImageLength => 0;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 196;
        public ref float ChargeTimer => ref Projectile.ai[0];
        public bool CanEmpower = true;
        public bool WasEmpowered = true;
        public bool Dashing = false;
        public float MaxCharge = 60f;
        public float EmpowerFrames = HurricanePike.EmpowerFrames;
        public Color Color1 = Color.Goldenrod;
        public Color Color2 = Color.Crimson;
        public override void Defaults()
        {
            Projectile.width = Projectile.height = 92;
            Projectile.extraUpdates = 5; //ExtraUpdates help make the VFX smoother
            Projectile.noEnchantmentVisuals = true;
        }
        public override void Spawn()
        {
            var mogPlayer = Owner.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 20;
            CooldownTime = 21;
            swingTime = 5;
            mogPlayer.swingNum = 0;
            angle = new Vector2(angle.X > 0 ? 1 : -1, 0);
            if (Owner.dashDelay == -1)
            {
                angle = new Vector2(-MathF.Sign(Owner.velocity.X), 0);
            }
            RotateInCooldown = 0;

            UseSound = SoundID.DD2_JavelinThrowersAttack with { Pitch = 1f, PitchVariance = 0.5f };
            RotateInStartup = 0.5f;
            OffsetDistance = 10;
            MaxCharge *= Projectile.extraUpdates;
            EmpowerFrames *= Projectile.extraUpdates;
            CanEmpower = true;
            WasEmpowered = Owner.MogMod().pikeEmpowered;
        }
        public override void AdditionalAI()
        {
            // Fix the sprite rotation
            Projectile.rotation -= MathHelper.PiOver2 * (angle.X > 0 ? 1 : -1);

            // Custom offset distance since this isnt actually a sword
            if (inStartup)
            {
                OffsetDistance = (int)MathHelper.SmoothStep(40, 5, 1 - MathF.Pow(1 - StartupCompletion, 1f));
                if (KeybindSystem.FirstWeaponKeybind.JustPressed && WasEmpowered)
                {
                    if (!Dashing)
                    {
                        Dashing = true;
                        SoundEngine.PlaySound(WeakCharge, Projectile.Center);
                        for (int i = 0; i < 75; i++)
                        {
                            float colorRando = Main.rand.NextFloat(0, 1);
                            float offsetAngle = MathHelper.TwoPi * i / 75f;
                            float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                            float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                            Vector2 vel = new Vector2(unitOffsetX, unitOffsetY) * 2.5f;
                            Dust charged = Dust.NewDustPerfect(Owner.Center, DustID.FireworksRGB, vel);
                            charged.scale = 0.75f;
                            charged.fadeIn = 0.5f;
                            charged.color = Color.Lerp(Color1, Color2, colorRando);
                            charged.noGravity = true;
                        }
                    }
                }
            }
            if (inCooldown)
            {
                OffsetDistance = (int)MathHelper.SmoothStep(90, 40, MathF.Pow(CooldownCompletion, 1));
                if (WasEmpowered && Dashing)
                {
                    // owner visual effects
                    DashingDustEffect();
                }
            }
            if (inSwing)
            {
                OffsetDistance = (int)MathHelper.SmoothStep(5, 90, MathF.Pow(SwingCompletion, 1f));
                Vector2 aimVel = (Owner.Center - Owner.MogMod().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                if (swingTimer % 4 == 0)
                {
                    for (int i2 = -1; i2 <= 1; i2 += 2)
                    {
                        float scale = Main.rand.NextFloat(0.007f, 0.015f);
                        Vector2 velocity = angle.RotatedBy(i2 * -0.15f) * Main.rand.NextFloat(5, 10);
                        Vector2 pos = Owner.Center + angle.RotatedBy(MathHelper.Pi) * (OffsetDistance + Main.rand.NextFloat(30, 100)) * Projectile.scale + new Vector2(0, 10 * i2).RotatedByRandom(MathHelper.Pi - 0.2f);

                        Dust dust2 = Dust.NewDustPerfect(pos, Main.rand.NextBool(3) ? 263 : 247, velocity, Scale: scale);
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool(3) ? Color2 : Color1;
                    }
                }
                
                if (ChargeTimer <= 0f && Owner.MogMod().pikeEmpowered)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        var source = Projectile.GetSource_FromThis();
                        Projectile.NewProjectile(source, Owner.Center, -(aimVel / 4), ModContent.ProjectileType<DragonPhantomProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 5f);
                        Owner.MogMod().pikeEmpowered = false;
                        CanEmpower = false;
                    }
                }

                if (WasEmpowered)
                {
                    if (Dashing)
                    {
                        Vector2 toMouse = Owner.MountedCenter.DirectionTo(Owner.ClampedMouseWorld());
                        Projectile.velocity = toMouse * 30f;
                        Owner.velocity = Projectile.velocity;
                        Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
                        Owner.mount?.Dismount(Owner);
                        Owner.RemoveAllGrapplingHooks();
                        Owner.SetImmuneTimeForAllTypes(swingTime);

                        // owner visual effects
                        DashingDustEffect();
                    }

                    // Dusts positioned to be at the tip of the spear
                    var dustAngle = angle.RotatedBy(MathHelper.Pi);
                    var color = Color.Lerp(Color.Gold, Color.Crimson, SwingCompletion);
                    if (swingTimer % 2 == 0 || (swingTimer == swingTime - 1))
                        for (int j = 0; j < 3; j++) for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 velocity = -dustAngle.RotatedBy(i * -0.3f) * 10f;
                            Vector2 position = Projectile.Center + new Vector2(75, i).RotatedBy(dustAngle.ToRotation());
                            Dust dust2 = Dust.NewDustPerfect(position, Main.rand.NextBool(3) ? 263 : 247, velocity, Scale: 0.3f, newColor: color);
                        }
                }
            }

            // Right click charge the spear
            if (Owner.MogMod().mouseRight)
            {
                if (ChargeTimer <= 0f && !Owner.MogMod().pikeEmpowered && !inCooldown && CanEmpower)
                {
                    SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 1f });
                    ChargeTimer = MaxCharge;
                }
            }
            if (ChargeTimer > 0f) ChargeTimer--;
        }
        public void DashingDustEffect()
        {
            Owner.armorEffectDrawShadowEOCShield = true;
            float dustLoopcheck = 8f;
            int dustIncr = 0;
            while (dustIncr < dustLoopcheck)
            {
                Vector2 dustRotate = Vector2.UnitX * 0f;
                dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                dustRotate = dustRotate.RotatedBy((double)Owner.velocity.ToRotation(), default);
                int bedman = Dust.NewDust(Owner.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightYellow, 1f);
                Main.dust[bedman].scale = 1.5f;
                Main.dust[bedman].noGravity = true;
                Main.dust[bedman].position = Owner.Center + dustRotate;
                Main.dust[bedman].velocity = Owner.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                dustIncr++;
            }
            for (int d = 0; d < 4; d++)
            {
                Dust faeDust = Dust.NewDustPerfect(Owner.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Owner.velocity * 1.2f), DustID.Sandnado, -Owner.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                faeDust.noGravity = faeDust.type != 222;
                faeDust.fadeIn = 0.5f;
                faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(6, 6) - Owner.velocity * 2, DustID.YellowStarDust);
                dust.velocity = -Owner.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                dust.noGravity = true;
            }
        }
        public override float SwingFunction() => 0;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (WasEmpowered)
            {
                modifiers.SourceDamage *= HurricanePike.DamageMult;
                modifiers.Knockback += 1f;
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce with { Pitch = 1f });
                    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { Volume = 0.9f, PitchVariance = 0.15f }, Projectile.Center);

                    int totalDusts = 5;
                    float starAngle = MathHelper.Pi / totalDusts;
                    for (int i = 0; i < totalDusts; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * totalDusts;
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel, 80, Color2, 1.2f);
                        dust2.noGravity = true;
                    }
                }
            }
            else if (!Owner.MogMod().pikeEmpowered && ChargeTimer >= MaxCharge - EmpowerFrames)
            {
                Owner.MogMod().pikeEmpowered = true;
                SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack with { Pitch = -0.5f });
                SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.15f, Pitch = 0.35f });

                int totalDusts = 5;
                float starAngle = MathHelper.Pi / totalDusts;
                for (int i = 0; i < totalDusts; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    Vector2 vel = (MathHelper.TwoPi * i / totalDusts).ToRotationVector2().RotatedBy(starAngle) * totalDusts;
                    Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, vel, 80, Color1, 1.2f);
                    dust2.noGravity = true;
                }
            }
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.25f, Pitch = -1f });
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 rotationPoint = texture.Size() * 0.5f;

            SpriteEffects flip = SpriteEffects.None;

            var ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/HurricanePikeGhost").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 ghostRotationPoint = ghost.Size() * 0.5f;
            float drawRotation = Projectile.rotation;

            float value = ChargeTimer > 0 ? ChargeTimer / MaxCharge : MathHelper.Min(timer, StartupTime) / StartupTime;
            float intensity = 0;
            Color color = Color.White;
            if (SwingCompletion < 1 || ChargeTimer >= MaxCharge - EmpowerFrames) intensity = MathF.Pow(Math.Clamp(value, 0, 1), 2);
            color = (ChargeTimer > 0 || WasEmpowered) ? Color2 : Color1;

            if (Owner.gravDir == 1f)
            {
                if (Projectile.spriteDirection == -1) flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                rotationPoint.Y = texture.Height - rotationPoint.Y;
                ghostRotationPoint.Y = ghost.Height - ghostRotationPoint.Y;

                if (Projectile.spriteDirection == 1) drawRotation += MathHelper.Pi;
                else flip = SpriteEffects.FlipVertically;
            }

            if (intensity > 0) for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                Main.EntitySpriteDraw(ghost, Projectile.Center - Main.screenPosition + new Vector2(2 * intensity, 0).RotatedBy(i), null, color, drawRotation, ghostRotationPoint, Projectile.scale, flip);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale, flip, 0);
            return false;
        }
    }
}
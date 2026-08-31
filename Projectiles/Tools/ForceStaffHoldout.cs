using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Tools;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Tools
{
    public class ForceStaffHoldout : BaseHoldoutProjectile
    {
        public override LocalizedText DisplayName => MiscUtils.GetItemName<ForceStaff>();
        public override string Texture => "MogMod/Items/Tools/ForceStaff";
        public override float RotationOffset => 45;
        public override float HoldoutOffset => Projectile.width / 3;
        public override float TurnSpeed => 0.25f;
        public static readonly SoundStyle ActivateSound = new($"{nameof(MogMod)}/Sounds/SE/ForceStaffActivate") { Volume = 0.2f, PitchVariance = 0.2f, MaxInstances = 1 };
        public ref float Timer => ref Projectile.ai[0];
        public ref float DashTimer => ref Projectile.ai[1];
        public Color Color1 = ForceStaff.MainColor1;
        public Color Color2 = ForceStaff.MainColor2;
        public int minChargeTime = 30;
        public int maxChargeTime = 100;
        public bool fullCharge = false;
        public bool playedSound = false;
        public bool launchedPlayer = false;
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
        }
        public override void HoldoutAI()
        {
            if (Timer < maxChargeTime) Timer++;
            if (Timer == 3) Projectile.alpha = 0;

            if (Owner.CantUseHoldout() || launchedPlayer)
            {
                if (Timer > minChargeTime)
                {
                    launchedPlayer = true;
                    Owner.channel = true;
                    Projectile.timeLeft = 2;

                    DashTimer--;
                    if (!playedSound)
                    {
                        Owner.velocity += Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15f;
                        DashTimer = Timer / 4;
                        Owner.mount?.Dismount(Owner);
                        Owner.RemoveAllGrapplingHooks();

                        SoundEngine.PlaySound(ActivateSound, Owner.Center);
                        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown with { Pitch = (10 - (Timer / 100)) * 0.1f }, Projectile.Center);

                        float dustLoopcheck = 16f;
                        int dustIncr = 0;
                        while (dustIncr < dustLoopcheck)
                        {
                            Vector2 dustRotate = Vector2.UnitX * 0f;
                            dustRotate += -Vector2.UnitY.RotatedBy((double)((float)dustIncr * (6.28318548f / dustLoopcheck)), default) * new Vector2(1f, 4f);
                            dustRotate = dustRotate.RotatedBy((double)Owner.velocity.ToRotation(), default);
                            int bedman = Dust.NewDust(Owner.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, Color.LightGreen, 1f);
                            Main.dust[bedman].scale = 1.5f;
                            Main.dust[bedman].noGravity = true;
                            Main.dust[bedman].position = Owner.Center + dustRotate;
                            Main.dust[bedman].velocity = Owner.velocity * 0f + dustRotate.SafeNormalize(Vector2.UnitY) * 1f;
                            dustIncr++;
                        }
                        playedSound = true;
                    }

                    Owner.armorEffectDrawShadowEOCShield = true;
                    for (int d = 0; d < 4; d++)
                    {
                        Dust faeDust = Dust.NewDustPerfect(Owner.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (Owner.velocity * 1.2f), DustID.Terra, -Owner.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        faeDust.noGravity = Main.rand.NextBool(2);
                        faeDust.fadeIn = 0.5f;
                        faeDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                        faeDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                        Dust dust = Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(6, 6) - Owner.velocity * 2, DustID.GemEmerald);
                        dust.velocity = -Owner.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        dust.noGravity = true;
                    }
                    Owner.velocity *= 1.01f;

                    if (DashTimer <= 0)
                    {
                        Timer = 0;
                        fullCharge = false;
                        playedSound = false;
                        //Owner.velocity *= 0.1f;
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;
                if (Timer >= maxChargeTime)
                {
                    float shakeValue = 0.4f;
                    Vector2 shakePos = new(Main.rand.NextFloat(-shakeValue, shakeValue), Main.rand.NextFloat(-shakeValue, shakeValue));
                    Projectile.position += shakePos;
                    if (!fullCharge)
                    { 
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot with { Pitch = 0.15f }, Projectile.Center);
                        fullCharge = true;
                    }
                    for (int i = 0; i <= 2; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Owner.Center + Projectile.velocity * 62 + Main.rand.NextVector2Circular(6, 6), Main.rand.NextBool(3) ? 263 : 247, (-Projectile.velocity * Main.rand.NextFloat(-0.25f, 0.25f)).RotatedByRandom(0.2f));
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        dust2.color = Main.rand.NextBool(3) ? Color2 : Color1;
                    }
                }
            }
        }
        public override void PreDrawBehind(ref Color lightColor)
        {
            if (Timer < minChargeTime) return;
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Tools/ForceStaffGhost").Value;
            float drawSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * (Timer / 100)) * 0.5f + 0.5f;
            float outlineWidth = Timer / 150;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Color.Lerp(Color1, Color2, drawSpeed),
                    Projectile.rotation,
                    ghost.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
                    0
                );
            }
        }
    }
}
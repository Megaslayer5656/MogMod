using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class EmpyreanHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => MiscUtils.GetItemName<EmpyreanBombardment>();
        public override string Texture => "MogMod/Items/Weapons/Magic/EmpyreanBombardment";
        private Player Owner => Main.player[Projectile.owner];
        public ref float Timer => ref Projectile.ai[0];
        public ref float ChargedTimer => ref Projectile.ai[1];

        public static readonly Color[] colorList =
        [
            new Color(255, 249, 59), // yellow
            new Color(247, 119, 224), // pink
            new Color(40, 105, 240) // blue
        ];
        public int framesBetweenShots = 0;
        public bool fullCharge = false;
        public int fullChargedShots = EmpyreanBombardment.MaxBarrageStars;
        public int windupAnim = 11;
        public int soundTimer = 0;
        public bool discharging = false;
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
        }
        public override void AI()
        {
            Timer++;
            if (Timer == 3) Projectile.alpha = 0;
            if (Timer % 2 == 0) soundTimer++;
            if (Timer > windupAnim && !Owner.CantUseHoldout()) if (windupAnim > 0) windupAnim--;
            if (Owner.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }
            int type = ModContent.ProjectileType<EmpyreanBombardmentProj>();
            float speed = 16f;
            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.85f + new Vector2(0, 3.8f);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;
            if (Owner.CantUseHoldout() || discharging)
            {
                discharging = true;
                if (fullCharge && fullChargedShots > 0)
                {
                    Owner.channel = true;
                    Projectile.timeLeft = 2;
                    if (Timer % 2 == 0)
                    {
                        Owner.SetScreenshake(1.85f);
                        for (int i = 0; i < fullChargedShots; ++i)
                        {
                            float randSpeed = speed * Main.rand.NextFloat(1f, 1.5f);
                            if (Projectile.owner == Main.myPlayer)
                                MogModUtils.ProjectileRain(Projectile.GetSource_FromThis(), Main.MouseWorld, 350f, 120f, 850f, 1200f, randSpeed, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        fullChargedShots--;
                    }
                }
                else
                {
                    if (!canUseMana)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }
            else
            {
                if (!canUseMana) discharging = true;
                if (Timer % 3 == 0f) Owner.CheckMana(Owner.HeldItem, -1, true);
                if (Timer < 90 && soundTimer > (windupAnim + 2))
                {
                    SoundEngine.PlaySound(SoundID.Item6 with { Pitch = (8 - windupAnim) * 0.15f }, Projectile.Center);
                    soundTimer = 0;
                }
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;
                if (Timer > 90)
                {
                    fullCharge = true;
                    if (framesBetweenShots == 0)
                    {
                        for (int i = 0; i <= 2; i++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(Owner.Center - Projectile.velocity * 6, Main.rand.NextBool(3) ? 263 : 247, (Projectile.velocity * Main.rand.NextFloat(4f, 15.5f)).RotatedByRandom(0.2f));
                            dust2.noGravity = true;
                            dust2.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        }
                        for (int i = 0; i < EmpyreanBombardment.MaxStars; ++i)
                        {
                            float randSpeed = speed * Main.rand.NextFloat(0.6f, 1.2f);
                            if (Projectile.owner == Main.myPlayer)
                                MogModUtils.ProjectileRain(Projectile.GetSource_FromThis(), Main.MouseWorld, 400f, 150f, 850f, 1100f, randSpeed, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        framesBetweenShots = 3;
                    }
                    if (framesBetweenShots > 0) framesBetweenShots--;
                }
            }
            UpdatePlayerVisuals();
        }
        private void UpdatePlayerVisuals()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.Center += Projectile.rotation.ToRotationVector2() * 30f;
            Projectile.direction = Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = MiscUtils.WrapAngle90Degrees(Projectile.rotation);
            Projectile.rotation += MathHelper.PiOver4;
            if (Projectile.spriteDirection == -1) Projectile.rotation += MathHelper.PiOver2;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/EmpyreanGhost").Value;
            float drawSpeed = discharging ? MathF.Sin(Main.GlobalTimeWrappedHourly * 8) * 0.5f + 0.5f : MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.5f + 0.5f;
            float outlineWidth = 4;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    MogModUtils.MulticolorLerp(drawSpeed, colorList),
                    Projectile.rotation,
                    ghost.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0
                );
            }
            return true;
        }
    }
}
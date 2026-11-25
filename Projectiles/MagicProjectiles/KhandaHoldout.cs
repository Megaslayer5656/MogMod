using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class KhandaHoldout : BaseHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Khanda>();
        public override float RotationOffset => 45;
        public override float HoldoutOffset => Projectile.width / 2;
        public override float TurnSpeed => 0.1f;
        public override int HoldoutHandling => HoldoutStyle.Rigid;
        public override int SyncTimer => 1;
        public ref float Timer => ref Projectile.ai[0];
        public ref float CanShoot => ref Projectile.ai[2];
        public static Color WeakColor => Khanda.WeakColor;
        public static Color StrongColor => Khanda.StrongColor;
        //public SlotId AudSlot;
        bool coolingDown = false; // cooldown effects
        int MinCharge = 30; // how long before firing proj
        int Cap = 15; // to prevent being unable to fire with enough attack speed
        public override void Defaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
        }
        public override void HoldoutAI()
        {
            var attackSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
            if (attackSpeed > Cap) attackSpeed = Cap;
            if (attackSpeed != 0f) attackSpeed = 1f / attackSpeed;

            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;

            Timer++;
            if (Timer == 3) Projectile.alpha = 0;

            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            if (Owner.CantUseHoldout() || !canUseMana)
            {
                CanShoot = 5f;
                Projectile.Kill();
                return;
            }
            else
            {
                // holdout shake effect
                float shakeValue = 0.4f;
                Vector2 shakePos = new(Main.rand.NextFloat(-shakeValue, shakeValue), Main.rand.NextFloat(-shakeValue, shakeValue));
                Projectile.position += shakePos * MathHelper.Lerp(0f, 1f, Math.Min(Timer, MinCharge) / MinCharge);
                if (Timer >= MinCharge)
                {
                    Projectile.timeLeft = 2; // refresh holdout lifetime

                    // dust effect
                    int dustNum = 1;
                    for (int i = 0; i <= dustNum; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 30f, Main.rand.NextBool(3) ? DustID.AncientLight : DustID.PurpleCrystalShard, -(Projectile.velocity * Main.rand.NextFloat(-1.5f, 1.5f)).RotatedByRandom(1.4f));
                        dust2.noGravity = true;
                        dust2.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        dust2.color = Color.Lerp(WeakColor, StrongColor, (float)Math.Abs(Math.Cos(Main.GlobalTimeWrappedHourly)));
                    }

                    if (Timer % 9 == 0) Owner.CheckMana(Owner.HeldItem, -1, true);
                    if (Timer % 21 == 0) SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.15f, MaxInstances = -1 }, Projectile.Center);
                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<KhandaBeam>();
                    if (Projectile.owner == Main.myPlayer && Owner.ownedProjectileCounts[type] < 1)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                        Vector2 spawnPos = Owner.MountedCenter;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Projectile.velocity, type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner, ai1: Projectile.whoAmI);
                    }
                }
                else if (Timer == 2) SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
            }
        }
        public override void PreDrawBehind(ref Color lightColor)
        {
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/KhandaGhost").Value;
            float outlineWidth = 2 * MathHelper.Lerp(0f, 1f, Math.Min(Timer, MinCharge) / MinCharge);
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Projectile.GetAlpha(Color.Lerp(WeakColor * 0.5f, StrongColor, Timer / MinCharge)),
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
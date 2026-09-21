using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class DagonTwoHoldout : BaseHoldoutProjectile
    {
        public override LocalizedText DisplayName => MiscUtils.GetItemName<DagonTwo>();
        public override string Texture => "MogMod/Items/Weapons/Magic/DagonTwo";
        public override float RotationOffset => 45;
        public override float HoldoutOffset => Projectile.width / 2;
        public override float TurnSpeed => 0.175f;
        public override int HoldoutHandling => HoldoutStyle.Rigid;
        public ref float Timer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public static Color WeakColor => DagonTwo.WeakColor;
        public static Color StrongColor => DagonTwo.StrongColor;
        public SlotId AudSlot;
        bool coolingDown = false; // cooldown effects
        int MinCharge = 30; // how long before firing proj
        int Cap = 15; // to prevent being unable to fire with enough attack speed
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;

            Projectile.netImportant = true;
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
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound)) ChargeSound?.Stop();
                Projectile.Kill();
                return;
            }
            else
            {
                if (Timer >= MinCharge)
                {
                    if (coolingDown && ShootTimer >= 0f)
                    {
                        ShootTimer -= 0.04f;
                        Projectile.position -= shootVelocity * ShootTimer;
                        if (ShootTimer <= 0f) coolingDown = false;
                    }
                    else if (ShootTimer < 1f * attackSpeed) ShootTimer += 0.06f; // increase shoot timer with attack speed in mind
                    Projectile.timeLeft = 2; // refresh holdout lifetime

                    if (!coolingDown)
                    {
                        // dust effect
                        int dustNum = (int)MathHelper.Clamp(ShootTimer * 2f, 1f, 2f);
                        for (int i = 0; i <= dustNum; i++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 20f, Main.rand.NextBool(3) ? DustID.AncientLight : Main.rand.NextBool() ? DustID.Flare : DustID.Torch, -(Projectile.velocity * Main.rand.NextFloat(-2f, 2f)).RotatedByRandom(1.4f));
                            dust2.noGravity = true;
                            dust2.scale = Main.rand.NextFloat(0.9f, 1.6f);
                            dust2.color = Color.Lerp(WeakColor, StrongColor, ShootTimer * 1.5f);
                        }
                    }

                    // holdout shake effect
                    float shakeValue = ShootTimer;
                    Vector2 shakePos = new(Main.rand.NextFloat(-shakeValue, shakeValue), Main.rand.NextFloat(-shakeValue, shakeValue));
                    Projectile.position += shakePos;

                    var source = Projectile.GetSource_FromThis();
                    int type = ModContent.ProjectileType<WeakDagonBolt>();
                    if (ShootTimer >= 1f * attackSpeed && canUseMana && !coolingDown)
                    {
                        Owner.CheckMana(Owner.HeldItem, -1, true);
                        SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(source, Projectile.Center + Projectile.velocity * 20f, shootVelocity, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        coolingDown = true;
                        Projectile.position -= shootVelocity * ShootTimer;
                    }
                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(ShootTimer, 0, 1f, -0.5f, 0f);
                        ChargeSound.Volume = Utils.Remap(ShootTimer, 0, 1f, 0f, 0.85f) * 100;
                    }
                    else if (ShootTimer < MinCharge * attackSpeed) AudSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                }
            }
        }
        public override void PreDrawBehind(ref Color lightColor)
        {
            if (Timer < MinCharge) return;
            Texture2D ghost = ModContent.Request<Texture2D>("MogMod/Assets/Ghosts/DagonTwoGhost").Value;
            float outlineWidth = 4;
            for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
            {
                Main.spriteBatch.Draw(
                    ghost,
                    Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                    null,
                    Projectile.GetAlpha(Color.Lerp(WeakColor, StrongColor, ShootTimer * 1.5f)) * ShootTimer,
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
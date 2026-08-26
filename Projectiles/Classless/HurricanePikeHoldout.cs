using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Weapons.Classless;
using MogMod.Projectiles.BaseProjectiles;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace MogMod.Projectiles.Classless
{
    // code lifted from calamity mod gilded Proboscis
    public class HurricanePikeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<HurricanePike>()).Item;
        private Player Owner => Main.player[Projectile.owner];
        public override bool UseAttackSpeed => false;
        public override bool UseMeleeSize => false;
        //public override int swingWidth => 360;
        public override int AfterImageLength => 0;
        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }
        public override float lineCollisionLength => 196;
        public Color Color1 = Color.Goldenrod;
        public Color Color2 = Color.Crimson;
        int channelCharge = 0;
        int maxCharge = 300;
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
            CooldownTime = 41;
            swingTime -= StartupTime + CooldownTime;
            mogPlayer.swingNum = 0;
            angle = new Vector2(angle.X > 0 ? 1 : -1, 0);
            if (Owner.dashDelay == -1)
            {
                angle = new Vector2(-MathF.Sign(Owner.velocity.X), 0);
            }
            RotateInCooldown = 0;

            UseSound = SoundID.DD2_JavelinThrowersAttack;
            RotateInStartup = 0.5f;
            OffsetDistance = 10;
        }
        public override void AdditionalAI()
        {
            #region Primary Attack
            //When channeling, the internal timer will not progress & the charge timer will go up
            if (Owner.channel)
            {
                if (timer >= StartupTime - 1)
                {
                    timer--;
                    Projectile.timeLeft++;
                }
                if (channelCharge < maxCharge)
                {
                    channelCharge++;
                    if (channelCharge == 300)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BookStaffCast with { Volume = 1f, Pitch = -0.5f });
                    }
                    //else if (channelCharge % 10 == 0 && channelCharge < 2000)
                    //{
                    //    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/ElectricHit") with { Volume = channelCharge / 300f * 0.25f, Pitch = -1 + channelCharge / 300f });
                    //}
                }
            }
            //Once not channeling, set the damage at the end of the startup
            else
            {
                if (timer == StartupTime - 1)
                {
                    Projectile.originalDamage = (int)(Projectile.originalDamage * 0.75 * (channelCharge / 75f)); //scales from 0x to 3x power
                    Projectile.damage = (int)(Projectile.damage * 0.75 * (channelCharge / 75f)); //Both are needed 
                }
            }
            //Make the sprite rotation look right in game
            Projectile.rotation -= (MathHelper.PiOver2) * (angle.X > 0 ? 1 : -1);

            //Adjust the offset distance (how close/far the weapon is held to the player) depending on charge during startup
            //In cooldown or during the swing, these are based on how far completed the corresponding step is
            if (inStartup) OffsetDistance = (int)MathHelper.SmoothStep(40, 5, channelCharge / 300f);
            if (inCooldown) OffsetDistance = (int)MathHelper.SmoothStep(90, 60, MathF.Pow(CooldownCompletion, 1));
            if (inSwing)
            {
                OffsetDistance = (int)MathHelper.Lerp(-40, 90, MathF.Pow(SwingCompletion, 1.7f));
                //Spawn the little sparks
                var veloc = oldPlayerOffset - (Projectile.Center - Owner.Center);
                veloc.Normalize();
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
                //Spawn the large glow V
                var dustAngle = angle.RotatedBy(MathHelper.Pi);
                var color = Color.Lerp(Color.Gold, Color.Crimson, SwingCompletion);
                if (swingTimer % 2 == 0 || (swingTimer == swingTime - 1))
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 velocity = -dustAngle.RotatedBy(i * -0.3f) * 10f;
                        Vector2 position = Projectile.Center + new Vector2(75, i * 22).RotatedBy(dustAngle.ToRotation());
                        Dust dust2 = Dust.NewDustPerfect(position, Main.rand.NextBool(3) ? 263 : 247, velocity, Scale: 0.3f, newColor: color);
                    }
            }
            return;
            #endregion
        }
        public override float SwingFunction()
        {
            if (inStartup && channelCharge >= maxCharge)
            {
                //shake when fully charged
                return Main.rand.NextFloat(-0.025f, 0.025f);
            }
            return 0; //Primary attack should face the mouse direction directly
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.originalDamage = (int)(Projectile.originalDamage * 0.925f);
            Projectile.damage = (int)(Projectile.damage * 0.925f);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = channelCharge / 300f * 0.25f, Pitch = -1 + channelCharge / 300f });
            //if (channelCharge >= maxCharge)
            //{
            //    SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = 1 });
            //}
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type];
            var ghost = ModContent.Request<Texture2D>("MogMod/Projectiles/Classless/HurricanePikeGhost").Value;
            var frame = tex.Frame();

            float intensity = MathF.Pow(Math.Clamp((channelCharge - 150) / 150f, 0, 1), 2);

            if (SwingCompletion >= 1) intensity = 0;

            Color color = inSwing ? Color2 : Color1;
            if (intensity > 0) for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                Main.EntitySpriteDraw(ghost, Projectile.Center - Main.screenPosition + new Vector2(2 * intensity, 0).RotatedBy(i), frame, color, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0);
            }
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0);

            return false;
        }
    }
}
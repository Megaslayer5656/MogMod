using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.NPCs.Global;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.EnemyProjectiles
{
    public class HostileOverloadingOrbProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.EnemyProjectiles";
        public int NumAnimationFrames = 8;
        public int AnimationFrameTime = 4;
        public bool hitPlayer = false;
        public override void SetStaticDefaults() => Main.projFrames[Type] = NumAnimationFrames;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
        }
        public override void AI()
        {
            Vector2 randomOffset = Main.rand.NextVector2Circular(Projectile.width / 1.9f, Projectile.height / 1.9f);
            Dust d = Dust.NewDustPerfect(Projectile.Center + randomOffset, Main.rand.NextBool(3) ? 29 : 137, -Projectile.DirectionFrom(Projectile.Center + Projectile.velocity + randomOffset) * Main.rand.NextFloat(0.5f, 1f));
            if (Main.rand.NextBool(3))
            {
                d.scale *= 1.2f;
                d.fadeIn = 0.3f;
            }
            d.noLight = true;
            d.noGravity = true;
            d.fadeIn = 0.15f;
            d.scale *= 1.05f;
            Player target = Main.player[Main.npc[MogModGlobalNPC.overloadingOwner].target];
            if (Projectile.ai[1] != 0f)
            {
                Projectile.position.X = target.Center.X - (Projectile.width / 2);
                Projectile.position.Y = target.Center.Y - (Projectile.height / 2);
                Projectile.position.X = (int)Projectile.position.X;
                Projectile.position.Y = (int)Projectile.position.Y;
            }

            if (Projectile.frame != 1)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter % AnimationFrameTime == 0)
                    Projectile.frame++;
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter % (100) == 40)
                    Projectile.frame++;
            }

            if (Projectile.frame >= NumAnimationFrames + 1)
                Projectile.Kill();
            if (Projectile.frame == 3)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Projectile.frame <= 3)
            {
                modifiers.Cancel();
                return;
            }
            hitPlayer = true;
        }
        public override bool? CanDamage() => !hitPlayer;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            // projectile animation
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(tex,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
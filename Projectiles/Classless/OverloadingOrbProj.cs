using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Classless
{
    public class OverloadingOrbProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Classless";
        public override string Texture => "MogMod/Projectiles/EnemyProjectiles/HostileOverloadingOrbProj";
        public int NumAnimationFrames = 8;
        public int AnimationFrameTime = 4;
        public override void SetStaticDefaults() => Main.projFrames[Type] = NumAnimationFrames;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -2;
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

            if (Projectile.ai[2] != 0f)
            {
                Player player = Main.player[Projectile.owner];
                Rectangle myRect = Projectile.Hitbox;
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                    {
                        NPC npc = Main.npc[npcIndex];
                        //covers most edge cases like voodoo dolls
                        if (npc.active && !npc.dontTakeDamage &&
                            ((Projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && Projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && Projectile.owner < Main.maxPlayers && player.killClothier))) ||
                            (Projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (Projectile.owner < 0 || npc.immune[Projectile.owner] == 0 || Projectile.maxPenetrate == 1))
                        {
                            if (npc.noTileCollide || !Projectile.ownerHitCheck)
                            {
                                bool stickingToNPC;
                                //Solar Crawltipede tail has special collision
                                if (npc.type == NPCID.SolarCrawltipedeTail)
                                {
                                    Rectangle rect = npc.Hitbox;
                                    int crawltipedeHitboxMod = 8;
                                    rect.X -= crawltipedeHitboxMod;
                                    rect.Y -= crawltipedeHitboxMod;
                                    rect.Width += crawltipedeHitboxMod * 2;
                                    rect.Height += crawltipedeHitboxMod * 2;
                                    stickingToNPC = Projectile.Colliding(myRect, rect);
                                }
                                else
                                    stickingToNPC = Projectile.Colliding(myRect, npc.Hitbox);
                                if (stickingToNPC)
                                {
                                    if (npc.reflectsProjectiles && Projectile.CanBeReflected())
                                    {
                                        npc.ReflectProjectile(Projectile);
                                        return;
                                    }
                                    Projectile.ai[0] = 1f;
                                    Projectile.ai[1] = (float)npcIndex;
                                    Projectile.velocity = (npc.Center - Projectile.Center);
                                    Projectile.netUpdate = true;
                                }
                            }
                        }
                    }
                    if (Projectile.ai[0] == 1f)
                    {
                        int npcIndex = (int)Projectile.ai[1];
                        NPC npc = Main.npc[npcIndex];
                        if (npc.active && !npc.dontTakeDamage)
                        {
                            Projectile.Center = npc.Center - Projectile.velocity;
                            Projectile.gfxOffY = npc.gfxOffY;
                        }
                    }
                }
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
            if (Projectile.frame >= 3 && Projectile.localNPCHitCooldown != -1f)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.localNPCHitCooldown = -1;
            }
        }
        public override bool? CanDamage() => Projectile.frame >= 3;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            // projectile animation
            Texture2D tex = ModContent.Request<Texture2D>("MogMod/Projectiles/EnemyProjectiles/HostileOverloadingOrbProj").Value;
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
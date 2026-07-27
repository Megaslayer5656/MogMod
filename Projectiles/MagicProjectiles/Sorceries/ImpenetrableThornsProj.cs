using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Classes;
using MogMod.Items.Ammo.SorcerySpells.Thorn;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    // code copied from calamity mod thorn blossom proj
    public class ImpenetrableThornsProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public int TotalSegments = 66;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = SorceryDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 10;

            MogModGlobalProjectile mogProj = Projectile.MogMod();
            mogProj.bloodDamage = ImpenetrableThorns.BloodDamage;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha -= 100;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[1] = 1f;

                    // This projectile normally does not move by itself, so this will manually move it one time only
                    // This is only for the first segment
                    if (Projectile.ai[0] == 0f)
                    {
                        Projectile.ai[0]++;
                        Projectile.position += Projectile.velocity;
                    }

                    // Spawn the next segment
                    if (Main.myPlayer == Projectile.owner && Projectile.ai[0] < TotalSegments)
                    {
                        float spread = MathHelper.PiOver4 / 2f;
                        int nextSegment = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity.RotatedByRandom(spread), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, nextSegment);
                    }
                }
            }
            // fade out
            else
            {
                int AlphaPerFrame = 8;
                Projectile.alpha += AlphaPerFrame;
                if (Projectile.alpha == AlphaPerFrame * 21)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Dust thorn = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.GoldCoin : DustID.Shadewood_Tree, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 200, default, 1.3f);
                        thorn.noGravity = true;
                        thorn.velocity *= 0.5f;
                    }
                }

                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.ai[0] = TotalSegments - 2;
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => Projectile.ai[0] = TotalSegments - 2;
        // This is essential for Vilethorn-type projectiles, as velocity is a stored parameter and isn't supposed to actually move the projectile
        public override bool ShouldUpdatePosition() => false;
        // Draw the tip for the final thorn
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            if (Projectile.ai[0] == TotalSegments)
                texture = ModContent.Request<Texture2D>("MogMod/Projectiles/MagicProjectiles/Sorceries/ImpenetrableThornsTip").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, Main.rand.NextBool() ? SpriteEffects.FlipHorizontally: SpriteEffects.None, 0);
            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles
{
    public class EmpyreanBombardmentProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/BaseStarProj";
        public int StarColorType = Main.rand.Next(0, 3);
        public Color StarColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 280;
            Projectile.extraUpdates = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            switch (StarColorType)
            {
                case 0:
                    StarColor = new Color(255, 249, 59);
                    break;
                case 1:
                    StarColor = new Color(247, 119, 224);
                    break;
                case 2:
                    StarColor = new Color(40, 105, 240);
                    break;
            }
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 240) Projectile.tileCollide = true;
            if (Projectile.soundDelay == 0 && Projectile.ai[0] == 0f)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5)) SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            if (Main.rand.NextBool(48) && !Main.dedServ)
            {
                int idx = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, 16, 1f);
                Main.gore[idx].velocity *= 0.66f;
                Main.gore[idx].velocity += Projectile.velocity * 0.3f;
            }
            if (Main.rand.NextBool(10)) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 156, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
            if (Main.rand.NextBool(20) && !Main.dedServ) Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
        }
        public override Color? GetAlpha(Color lightColor) => StarColor with { A = 200 };
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawStarTrail(StarColor, Color.White);
            MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 1)
            {
                var source = Projectile.GetSource_FromThis();
                int type = ModContent.ProjectileType<EmpyreanStarProj>();
                float height = 340f;
                if (Projectile.owner == Main.myPlayer)
                    MogModUtils.ProjectileRain(source, target.Center, 50f, 20f, height, height, Projectile.velocity.Length(), type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: StarColorType);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0) modifiers.SourceDamage *= 0.98f;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position += Projectile.Size;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position -= Projectile.Size;
            for (int i = 0; i < 5; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 1.2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            if (!Main.dedServ) for (int i = 0; i < 3; i++) Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
        }
    }
}
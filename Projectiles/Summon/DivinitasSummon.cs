using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Summons;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Summon
{
    // code copied from calamity mod vengeful sun summon
    // Resprite this to be a biblically accurate angel
    public class DivinitasSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public static Color Colour => new(207, 239, 255);
        public int dust = 3;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 66;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
        }
        public int MinionSlotsToAdd
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            player.AddBuff(ModContent.BuffType<DivinitasSummonBuff>(), 3600);
            Projectile.rotation += player.direction == 1 ? 0.01f : -0.01f;

            #region Add Minion Slots
            if (MinionSlotsToAdd > 0)
            {
                float minionSlotsAvaliable = player.maxMinions;
                foreach (var item in Main.ActiveProjectiles)
                {
                    if (item.owner == Projectile.owner)
                        minionSlotsAvaliable -= item.minionSlots;
                }
                while (minionSlotsAvaliable >= 1 && MinionSlotsToAdd > 0)
                {

                    Projectile.minionSlots++;
                    minionSlotsAvaliable--;
                    MinionSlotsToAdd--;
                    Projectile.netUpdate = true;
                }
                MinionSlotsToAdd = 0;
            }
            #endregion

            #region Checking alive
            bool correctMinion = Projectile.type == ModContent.ProjectileType<DivinitasSummon>();
            if (correctMinion)
            {
                if (player.dead)
                    modPlayer.divinitasMinion = false;
                if (modPlayer.divinitasMinion)
                    Projectile.timeLeft = 2;
            }
            #endregion

            #region Positioning, Lighting, && Dust
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY + player.gravDir * -80f);
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * (Projectile.scale * 0.5f));


            // onspawn dust effect
            dust--;
            if (dust >= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(Projectile.width * 0.1f, Projectile.height * 0.1f), 100, Colour, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                    dust.scale *= 1.15f;
                }
            }
            #endregion


            NPC target = null;
            int targetID = -1;
            Projectile.Minion_FindTargetInRange(2500, ref targetID, false);
            if (targetID < 0)
                return;

            target = Main.npc[targetID];

            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1] -= 1f;
                    return;
                }
                int type = ModContent.ProjectileType<DivinitasBeamProj>();
                int damage = (int)(Projectile.damage * (0.75f + Projectile.minionSlots * 0.25f));
                float shootSpeed = 15f;
                Vector2 source = Projectile.Center;
                var velocity = MogModUtils.CalculatePredictiveAimToTargetMaxUpdates(Projectile.Center, target, shootSpeed, 3);
                float Spread = 0.15f;
                switch (Projectile.ai[2])
                {
                    case 0f:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        break;
                    case 1f:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity.RotatedBy(Spread * .5f), type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity.RotatedBy(-Spread * .5f), type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        break;
                    case 2f:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity.RotatedBy(Spread), type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity.RotatedBy(-Spread), type, damage, Projectile.knockBack, Projectile.owner, ai2: (Projectile.minionSlots - 1) / 6f);
                        break;
                }
                //Main.NewText($"ai 1 == {Projectile.ai[1]}", 200, 255, 200);
                Projectile.ai[2]++;
                Projectile.ai[1] += 60f / (0.75f + Projectile.minionSlots * 0.25f);
                if (Projectile.ai[2] > 2f)
                    Projectile.ai[2] = 0f;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.minionSlots);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.minionSlots = reader.ReadSingle();
        }
        public override bool? CanDamage() => false;
        private static Texture2D AllWhiteVersion = null;
        public static Texture2D GetWhiteTex()
        {
            if (AllWhiteVersion == null)
            {
                var texture = TextureAssets.Projectile[ModContent.ProjectileType<DivinitasSummon>()].Value;
                AllWhiteVersion = new Texture2D(Main.graphics.GraphicsDevice, texture.Width, texture.Height);

                var BaseArray = new Color[AllWhiteVersion.Width * AllWhiteVersion.Height];
                var ColorArray = new Color[AllWhiteVersion.Width * AllWhiteVersion.Height];
                texture.GetData(BaseArray);
                for (var i = 0; i < BaseArray.Length; i++)
                {
                    ColorArray[i] = new Color(255, 255, 255) * (((float)BaseArray[i].A) / 255f);
                }
                AllWhiteVersion.SetData(ColorArray);
            }
            return AllWhiteVersion;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, Colour, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.85f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.1f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
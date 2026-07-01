using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Summons;
using MogMod.Common.MogModPlayer;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Summon
{
    // code copied from calamity mod vengeful sun summon
    public class DivinitasSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
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

            #region Positioning && Lighting
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY + player.gravDir * -80f);
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            #endregion


            NPC target = null;
            int targetID = -1;
            Projectile.Minion_FindTargetInRange(1600, ref targetID, false);
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
                float shootSpeed = 15f;
                Vector2 source = Projectile.Center;
                var velocity = MogModUtils.CalculatePredictiveAimToTargetMaxUpdates(Projectile.Center, target, shootSpeed, 2);
                Projectile beam = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center - velocity, velocity, ModContent.ProjectileType<MichaelSwordBeam>(), (int)(Projectile.damage * (0.75f + Projectile.minionSlots * 0.25f)), Projectile.knockBack, Projectile.owner, ai1: (Projectile.minionSlots - 1) / 6f);
                beam.DamageType = DamageClass.Summon;
                Projectile.ai[1] += 60f / (0.75f + Projectile.minionSlots * 0.25f);
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
    }
}
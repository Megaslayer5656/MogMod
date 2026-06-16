using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.ProjectileEnemies
{
    public class WarlockFireball : ModNPC
    {
        #region Setup
        public Player player => Main.player[NPC.target];
        public ref float AITimer => ref NPC.ai[1];
        public float explodeTimer = 300f;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.ProjectileNPC[NPC.type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 38;

            NPC.damage = 40;
            NPC.lifeMax = 1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = false;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.DeathSound = SoundID.Item14;
        }
        #endregion

        #region AI
        public override void OnSpawn(IEntitySource source) => SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
        //public override void SendExtraAI(BinaryWriter writer)
        //{
        //    writer.Write(NPC.dontTakeDamage);
        //}
        //public override void ReceiveExtraAI(BinaryReader reader)
        //{
        //    NPC.dontTakeDamage = reader.ReadBoolean();
        //}
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.OrangeRed.ToVector3());
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                NPC.TargetClosest(true);
            if (Main.rand.Next(0, 10) == 0)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, NPC.velocity.X * 0.25f, NPC.velocity.Y * 0.25f, 0, default, 1f);
            AITimer++;
            if (NPC.WithinRange(player.Center, player.Size.Length()))
            {
                if (AITimer < explodeTimer)
                    AITimer = explodeTimer;
                NPC.netUpdate = true;
            }
            if (AITimer < explodeTimer)
                AIMovement(player);
            else
                State_Exploding(player);
        }
        public void AIMovement(Player player)
        {
            NPC.immortal = true;
            if (NPC.FindBuffIndex(ModContent.BuffType<WingsOfLightDebuff>()) >= 0)
                AITimer = explodeTimer;
            Vector2 epstein = new Vector2(NPC.Center.X + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
            bool canHitTarget = Collision.CanHit(new Vector2(epstein.X, epstein.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
            Vector2 einstein = Main.player[NPC.target].Center;
            Vector2 velocity = NPC.SafeDirectionTo(einstein) * 8f;

            // Movement calculations
            if (Vector2.Distance(epstein, einstein) > 40f || !canHitTarget)
                NPC.SimpleFlyMovement(velocity, .1f);

            NPC.netUpdate = true;
        }
        public void State_Exploding(Player player)
        {
            NPC.velocity *= 0.9f;
            NPC.dontTakeDamage = true;
            if (AITimer == (explodeTimer + 1f))
                SoundEngine.PlaySound(SoundID.Zombie91, NPC.Center);
            NPC.TargetClosest(true);

            int size = (int)((NPC.ai[1] * .75f) + 30f);
            Vector2 offset = new Vector2(size / 2f);
            for (int i = 0; i < 30; i++)
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(size / 2.1f, size / 2.1f);
                Dust d = Dust.NewDustPerfect(NPC.Center + randomOffset, DustID.Flare, NPC.DirectionFrom(NPC.Center + NPC.velocity + randomOffset) * Main.rand.NextFloat(3f, 5f));
                d.fadeIn = .15f;
                d.scale = .75f;
                d.noGravity = true;
            }
            for (int i = 0; i < 50; i++)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(size / 2f, size / 2f);
                Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, DustID.CopperCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
            }

            // explosive climax
            if (AITimer >= (explodeTimer + 90f))
            {
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
                Vector2 center = NPC.Center;
                NPC.width = NPC.height = 300;
                NPC.Center = center;
                Rectangle myRect = NPC.getRect();

                for (int i = 0; i < 45; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.SolarFlare, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 27; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.Torch, 0f, 0f, 100, default, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.Torch, 0f, 0f, 100, default, 1.6f);
                    Main.dust[dust].velocity *= 3f;
                }
                NPC.immortal = false;
                NPC.StrikeInstantKill();
                foreach (Player target in Main.ActivePlayers)
                {
                    if (player.dead || !NPC.Hitbox.Intersects(player.Hitbox))
                        continue;
                    int direction = NPC.Center.X - target.Center.X < 0 ? -1 : 1;
                    target.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, -direction);
                }
                NPC.active = true;
                //NPC.StrikeInstantKill();
                NPC.netUpdate = true;
            }
        }
        public override bool CanHitNPC(NPC target) => AITimer <= explodeTimer;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AITimer <= explodeTimer;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            int duration = AITimer >= explodeTimer ? 300 : 120;
            target.AddBuff(BuffID.Burning, duration);
        }
        #endregion

        #region Frames && Hit Effects
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += AITimer >= explodeTimer ? 0.3f : 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AITimer < explodeTimer)
                AITimer = explodeTimer;
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
                for (int k = 0; k < 25; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, hit.HitDirection, -1f, 0, default, 1f);
        }
        #endregion
    }
}
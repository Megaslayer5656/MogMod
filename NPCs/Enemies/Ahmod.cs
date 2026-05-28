using Microsoft.Xna.Framework;
using MogMod.Items.Pets;
using MogMod.Items.Placeable.Banners;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Enemies
{
    public class Ahmod : ModNPC
    {
        // copied from ice clasper npc && nova npc from calamity mod
        // sorry emaad i made you a terrorist
        
        #region Setup
        public Player player => Main.player[NPC.target];
        public bool expert = Main.expertMode;
        public enum AhmodAIState
        {
            Shooting,
            Suicide
        }
        public AhmodAIState CurrentState
        {
            get => (AhmodAIState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float ExplosionTimer => ref NPC.ai[1];
        public ref float TimerForShooting => ref NPC.ai[2];
        public ref float AITimer => ref NPC.ai[3];
        public bool isSuicide => (AITimer >= TimeBetweenBurst * .67f && AITimer < TimeBetweenBurst) || CurrentState == AhmodAIState.Suicide;
        public float TimeBetweenBurst = 180f;
        public float ProjectileSpeed = 8f;
        public override void SetStaticDefaults() => Main.npcFrameCount[NPC.type] = 4;
        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = false;
            NPC.damage = 13;
            NPC.width = 40;
            NPC.height = 58;
            NPC.defense = 7;
            NPC.lifeMax = 62;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 7, 0);
            NPC.HitSound = SoundID.NPCHit40;
            NPC.DeathSound = SoundID.NPCDeath42;
            NPC.knockBackResist = 1.2f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AhmodBanner>();
        }
#endregion
        
        #region Bestiary & Spawning
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("MogMod/NPCs/Enemies/Ahmod_Beastiary")
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt ||
                spawnInfo.Player.ZoneCrimson ||
                spawnInfo.Player.ZoneOldOneArmy ||
                spawnInfo.Player.ZoneSkyHeight ||
                spawnInfo.PlayerSafe ||
                !spawnInfo.Player.ZoneDesert ||
                !spawnInfo.Player.ZoneOverworldHeight ||
                Main.eclipse ||
                Main.snowMoon ||
                Main.pumpkinMoon ||
                Main.invasionType != InvasionID.None)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            return 0.1f;
        }
        #endregion
        
        #region AI
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                NPC.TargetClosest(true);
            if (NPC.life <= (int)(NPC.lifeMax * 0.3))
                CurrentState = AhmodAIState.Suicide;
            else
                AIMovement(player);

            if (NPC.velocity.X < 0f)
                NPC.direction = -1;
            else
                NPC.direction = 1;
            NPC.spriteDirection = NPC.direction;
            float distToTarget = NPC.Distance(player.Center) + .1f;

            switch (CurrentState)
            {
                case AhmodAIState.Shooting:
                    State_Shooting(player);
                    break;
                case AhmodAIState.Suicide:
                    State_Suicide(player);
                    break;
            }
        }
        public void AIMovement(Player player)
        {
            Vector2 epstein = new Vector2(NPC.Center.X + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
            bool canHitTarget = Collision.CanHit(new Vector2(epstein.X, epstein.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
            Vector2 einstein = Main.player[NPC.target].Center - Vector2.UnitY * (!canHitTarget ? 0f : 100f);
            Vector2 velocity = NPC.SafeDirectionTo(einstein) * 5f;

            // Movement calculations
            if (Vector2.Distance(epstein, einstein) > 40f || !canHitTarget)
                NPC.SimpleFlyMovement(velocity, .1f);

            NPC.netUpdate = true;
        }
        public void State_Shooting(Player player)
        {
            // Minimum distance so the minion is able to shoot.
            if (NPC.Distance(player.Center) > 800f)
                return;

            AITimer++;

            if (AITimer >= TimeBetweenBurst)
            {
                if (TimerForShooting == 0)
                {
                    Vector2 vecToPlayer = NPC.DirectionTo(player.Center);
                    Vector2 projVelocity = vecToPlayer * ProjectileSpeed;
                    int type = ModContent.ProjectileType<AhmodStickyNade>();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = Main.masterMode ? 7 : Main.expertMode ? 8 : 10;
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                            NPC.Center + projVelocity.SafeNormalize(Vector2.Zero) * 10f,
                            projVelocity,
                            type,
                            damage,
                            0f,
                            Main.myPlayer);
                        NPC.netUpdate = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                    NPC.netUpdate = true;
                }

                TimerForShooting++;

                // reset every timer
                if (TimerForShooting >= 0)
                {
                    TimerForShooting = 0f;
                    AITimer = 0f;
                    NPC.netUpdate = true;
                }
            }
        }
        public void State_Suicide(Player player)
        {
            Lighting.AddLight(NPC.Center, Color.Yellow.ToVector3());
            NPC.damage = (int)(NPC.defDamage * 1.5f);
            NPC.TargetClosest(true);
            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.12f;
            NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.12f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -8f, 8f);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -8f, 8f);
            int npcTileX = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
            int npcTileY = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
            if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
                NPC.velocity.Y *= 0.95f;
            ExplosionTimer++;
            int size = 30;
            int dust3 = Dust.NewDust(NPC.Center, (int)(size / 2), (int)(size / 2), DustID.Smoke, 0f, 0f, 100, default, 1.7f);
            Main.dust[dust3].velocity *= 1.4f;
            for (int i = 0; i < 2; i++)
            {
                int dust2 = Dust.NewDust(NPC.Center, (int)(size / 2), (int)(size / 2), DustID.Torch, 0f, 0f, 100, default, 2.4f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(NPC.Center, (int)(size / 2), (int)(size / 2), DustID.Torch, 0f, 0f, 100, default, 1.6f);
                Main.dust[dust2].velocity *= 3f;
            }

            // explosive climax
            if (ExplosionTimer >= 180f)
            {
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
                Vector2 center = NPC.Center;
                NPC.width = 200;
                NPC.height = 200;
                NPC.Center = center;

                Rectangle myRect = NPC.getRect();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (Player target in Main.ActivePlayers)
                    {
                        if (target.getRect().Intersects(myRect))
                        {
                            int direction = NPC.Center.X - target.Center.X < 0 ? -1 : 1;
                            target.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, direction);
                        }
                    }
                }
                for (int i = 0; i < 45; i++)
                {
                    int dust = Dust.NewDust(NPC.Center, size, size, DustID.RainCloud, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(NPC.Center, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 27; i++)
                {
                    int dust = Dust.NewDust(NPC.Center, size, size, DustID.Torch, 0f, 0f, 100, default, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(NPC.Center, size, size, DustID.Torch, 0f, 0f, 100, default, 1.6f);
                    Main.dust[dust].velocity *= 3f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.StrikeInstantKill();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (CurrentState == AhmodAIState.Suicide)
                ExplosionTimer = 180f;
        }
        #endregion

        #region Item Drops & Misc Effects
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += isSuicide ? 0.3f : 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new OneFromRulesRule(1, new IItemDropRule[4]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.Bomb, 1, 2, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.Dynamite, 1, 1, 2),
                ItemDropRule.NotScalingWithLuck(ItemID.ScarabBomb, 1, 1, 3),
                ItemDropRule.NotScalingWithLuck(ItemID.ExplosivePowder, 1, 1, 3)
            }));
            npcLoot.Add(new OneFromRulesRule(2, new IItemDropRule[2]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.Cloud, 1, 7, 11),
                ItemDropRule.NotScalingWithLuck(ItemID.RainCloud, 1, 7, 11)
            }));
            npcLoot.Add(ItemDropRule.Common(ItemID.Ruby, 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AhmodInABottle>(), 20, 1, 1));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
                for (int k = 0; k < 25; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
        }
        #endregion
    }
}
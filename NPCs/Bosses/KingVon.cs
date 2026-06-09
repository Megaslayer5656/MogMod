using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Projectiles.EnemyProjectiles.Boss;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class KingVon : ModNPC
    {
        public override void SetDefaults() //You should be able to figure out what these do Will (if not check tmodloader documentation)
        {
            NPC.width = 200;
            NPC.height = 100;
            NPC.damage = 134;
            NPC.defense = 45;
            NPC.lifeMax = 92400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.npcSlots = 6f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme1"); //Music
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange([
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.KingVon")
            ]);
        }

        static Random random = new Random(); //You need to have this somewhere at the top of your file if you want to use random numbers later on
        public static int Phase2HeadSlot = -1;
        public int vonShotTimer = 0; // The very short timer between all of his shots (so a proj isn't spawned every frame)
        public static int vonShotTimerMax = 3;
        public int vonTpTimer = 0; //The timer for the cooldown on his teleport
        public static int vonTpTimerMax = 600;
        public int vonReloadTimer = 0; //The cooldown for the timer on his reload
        public static int vonReloadTimerMax = 175;
        public int vonSpecialTimer = 0; //The timer for the cooldown on his special attack
        public static int vonSpecialTimerMax = 420;
        public int vonRageTimer = 0; //The timer that determines how long he is in 'rage' mode (his dash)
        public static int vonRageTimerMax = 300;
        public int randRotate = random.Next(0, 11);
        public int syncTimer = 0;
        public int syncTimerMax = 180;
        public bool isDashing = false;


        public static float laserScale = 1.2f;
        public static float laserLength = 2000f;
        public static float laserLifetime = 100f;
        public int vonLaserEyes = 0;
        public int vonShooting = 0;
        public int vonCharge = 0;


        public static readonly SoundStyle VonShot = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2") //Shot sound effect
        {
            Volume = .2f,
            PitchVariance = .2f
        };

        public static readonly SoundStyle VonNade = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/VonNadeThrow") //Nade throw sound effect
        {
            Volume = 1.5f
        };

        public bool Phase2 //Does something for phase 2 not entirely sure what (stolen from examplemod)
        {
            get => NPC.ai[1] == 1f;
            set => NPC.ai[1] = value ? 1f : 0f;
        }

        public override void Load()
        {
            string texture = BossHeadTexture + "_Phase2"; //Loads second boss head for minimap
            Phase2HeadSlot = Mod.AddBossHeadTexture(texture, -1); //Sets variable for phase 2 head
        }

        public override void BossHeadSlot(ref int index)
        {
            int slot = Phase2HeadSlot;
            if (Phase2 && slot != -1)
            {
                index = slot;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                NPC.EncourageDespawn(10);
                return;
            }

            CheckPhase2(); //Checks if he is in phase 2

            if (Phase2)
            {
                DoPhase2(player);
            } else
            {
                DoPhase1(player);
            }

            if (syncTimer >= syncTimerMax)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    NetcodeHelper.NPCVelocitySync(NPC, NPC.velocity, NPC.position);
                }
                syncTimer = 0;
            }

            syncTimer++;
        }

        private void CheckPhase2()
        {
            if (Phase2)
            {
                return;
            }

            if (NPC.life < NPC.lifeMax * .5f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Phase2 = true;
                NPC.netUpdate = true;
            }
        }

        private void DoPhase1(Player player) //AI for phase 1
        {
            var entitySource = NPC.GetSource_FromAI(); //Source for projectiles
            Vector2 toPlayer = player.Center - NPC.Center; //Direction to player
            float speed = .015f; //Base speed
            float fastSpeed = .04f; //Speed during dash
            float nadeSpeed = .03f; //How fast his grenade flies
            float inertia = 40f;
            Vector2 moveTo = toPlayer * speed; //Direction * Speed (makes him go towards player if set as velocity)
            Vector2 moveToFast = toPlayer * fastSpeed; //Same as above but faster (for dash)
            Vector2 nadeToPlayer = toPlayer * nadeSpeed; //Direction * Nade Speed to make it go towards player
            
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia; //Sets his base move speed
            // TODO: Make him jump over obstacles
            if (vonReloadTimer <= vonReloadTimerMax * .55) //Checks if he's reloading
            {
                if (vonShotTimer >= vonShotTimerMax) //Timer between shots
                {
                    if (NPC.HasValidTarget)
                    { 
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(entitySource, NPC.Center, toPlayer * 15, ModContent.ProjectileType<VonGreenTracerProj>(), 60, .5f, 255);
                        }

                        vonShotTimer = 0; //Reset timer
                        if (Main.netMode != NetmodeID.Server)
                            SoundEngine.PlaySound(VonShot, NPC.Center);
                    }
                }
                else
                {
                    vonShotTimer += 1; //Adds to timer every tick
                    
                }
            }
            
            vonReloadTimer += 1; //Increases reload timer every tick
            
            if (vonReloadTimer >= vonReloadTimerMax)
            {
                vonReloadTimer = 0;
            }

            vonTpTimer += 1; //Increases tp timer every tick

            if (vonTpTimer >= vonTpTimerMax && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //TODO: Make him tp
                vonTpTimer = 0;
            }

            if (vonSpecialTimer >= vonSpecialTimerMax) //Checks if cooldown for special is up
            {
                if (NPC.HasValidTarget) //Makes sure the client doesn't try to run it to avoid desync (it should be ran by the server)
                {
                    int vonRandAttack = random.Next(0, 11); //creates random int to choose between the 2 special options

                    if (vonRandAttack > 5) //If the random int is greater than 5 throw a nade
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //TODO: Make a custom grenade with a bigger explosion
                            int vonNade = Projectile.NewProjectile(entitySource, NPC.Center, nadeToPlayer, ProjectileID.Grenade, 100, 2f, 255);
                            Main.projectile[vonNade].friendly = false;
                            Main.projectile[vonNade].hostile = true;
                            Main.projectile[vonNade].scale = 2f;
                            Main.projectile[vonNade].timeLeft = 60;
                        }

                        //if (vonSpecialTimer == vonSpecialTimerMax) //To ensure it only plays once
                        //{
                            if (Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(VonNade, NPC.Center);
                            }
                        //}

                        vonSpecialTimer = 0;
                    }
                    else //If the random int is 5 or less (uhh this doesn't work, I'll investigate some other time. I'm prob just gonna rewrite the entire phase 1 code.)
                    {
                        if (!isDashing)
                        {
                            isDashing = true;
                            vonRageTimer = 0;

                            if (Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                            }

                            if (isDashing)
                            {
                                if (vonRageTimer < vonRageTimerMax)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        NPC.velocity = (NPC.velocity * (inertia - 1) + moveToFast) / inertia; //Change velocity towards player
                                        NPC.velocity.Y = -30; //Change velocity upwards
                                        if (Main.netMode == NetmodeID.Server)
                                        {
                                            NetcodeHelper.NPCVelocitySync(NPC, NPC.velocity, NPC.Center);
                                        }
                                    }
                                    vonRageTimer += 1;
                                }
                                else
                                {
                                    isDashing = false;

                                    vonRageTimer = 0; //Reset dash timer
                                    vonSpecialTimer = 0; //Reset special timer
                                    NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia; //Reset move speed

                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetcodeHelper.NPCVelocitySync(NPC, NPC.velocity, NPC.Center);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            vonSpecialTimer += 1; //Adds 1 to special timer (cooldown) every tick
        }

        private void DoPhase2(Player player) //AI for phase 2
        {
            NPC.noGravity = true; //Remove gravity (because he flies in phase 2)
            NPC.noTileCollide = true;
            NPC.setNPCName("Von, Evil Incarnate", ModContent.NPCType<KingVon>()); //Change display name
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme2"); //Change music
            var entitySource = NPC.GetSource_FromAI(); //Source for projectiles
            Vector2 toPlayer = player.Center - NPC.Center; //All this is same as phase1 (until I change it)
            float speed = .015f;
            float fastSpeed = .04f;
            float nadeSpeed = .03f;
            float inertia = 40f;
            Vector2 moveTo = toPlayer * speed;
            Vector2 moveToFast = toPlayer * fastSpeed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

            vonLaserEyes++;
            vonShooting++;

            if (vonLaserEyes == 120)
                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0f, 0f), ModContent.ProjectileType<VonLaserSpawner>(), 120, 0, 255);
            if (vonLaserEyes == 240)
                vonLaserEyes = 0;

            if (vonShooting >= 90)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 kirk = new Vector2(8, 8).RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(entitySource, NPC.Center, kirk, ModContent.ProjectileType<VonGreenTracerProj>(), 60, .5f, 255);
                }
                if (vonShooting == 180)
                    vonShooting = 0;
            }
            if (vonLaserEyes == 0 && vonShooting == 0 || vonCharge > 0)
            {
                vonCharge++;
                if (vonCharge == 1)
                {
                    NPC.velocity *= .05f;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item149, NPC.Center);
                    }
                    Projectile.NewProjectile(entitySource, NPC.Center, toPlayer, ModContent.ProjectileType<VonTargetLaser>(), 0, 0, 255);
                }
                if (vonCharge >= 40)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    }
                    Vector2 charge = Vector2.Normalize(player.Center - NPC.Center) * 30f * 2f;
                    NPC.velocity = charge;
                    vonCharge = 0;
                    if (Main.netMode == NetmodeID.Server)
                        NetcodeHelper.NPCVelocitySync(NPC, NPC.velocity, NPC.position);
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SuperHealingPotion, 1, 8, 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VonBossBag>()));
        }
    }
}

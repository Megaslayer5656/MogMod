using Microsoft.Build.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = 50000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
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
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value ? 1f : 0f;
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
                    if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, toPlayer * 15, ModContent.ProjectileType<VonGreenTracerProj>(), 80, .5f, Main.myPlayer);
                        SoundEngine.PlaySound(VonShot, NPC.Center);
                        vonShotTimer = 0; //Reset timer
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
                int vonRandAttack = random.Next(0, 11); //creates random int to choose between the 2 special options
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) //Makes sure the client doesn't try to run it to avoid desync (it should be ran by the server)
                {
                    if (vonRandAttack > 5) //If the random int is greater than 5 throw a nade
                    {
                        //TODO: Make a custom grenade with a bigger explosion
                        int vonNade = Projectile.NewProjectile(entitySource, NPC.Center, nadeToPlayer, ProjectileID.Grenade, 100, 2f, Main.myPlayer);
                        Main.projectile[vonNade].friendly = false;
                        Main.projectile[vonNade].hostile = true;
                        Main.projectile[vonNade].scale = 2f;
                        Main.projectile[vonNade].timeLeft = 60;
                        SoundEngine.PlaySound(VonNade, NPC.Center); //TODO: Make this change in multiplayer
                        vonSpecialTimer = 0; //Reset special timer
                    }
                    else //If the random int is 5 or less
                    {
                        //TODO: Make a sound queue for when he jumps
                        while (vonRageTimer < vonRageTimerMax) //How long he is dashing for
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.velocity = (NPC.velocity * (inertia - 1) + moveToFast) / inertia; //Change velocity towards player
                                NPC.velocity.Y = -30; //Change velocity upwards
                                NPC.netUpdate = true;
                                vonRageTimer += 1; //Adds 1 to dash timer (how long he dashes for) every tick
                            }
                        }
                        vonRageTimer = 0; //Reset dash timer
                        vonSpecialTimer = 0; //Reset special timer
                        NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia; //Reset move speed
                    }
                }
            }
            vonSpecialTimer += 1; //Adds 1 to special timer (cooldown) every tick
        }

        private void DoPhase2(Player player) //AI for phase 2
        {
            NPC.noGravity = true; //Remove gravity (because he flies in phase 2)
            NPC.setNPCName("Von, Evil Incarnate", ModContent.NPCType<KingVon>()); //Change display name
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme2"); //Change music
            }
            var entitySource = NPC.GetSource_FromAI(); //Source for projectiles
            Vector2 toPlayer = player.Center - NPC.Center; //All this is same as phase1 (until I change it)
            float speed = .015f;
            float fastSpeed = .04f;
            float nadeSpeed = .03f;
            float inertia = 40f;
            Vector2 moveTo = toPlayer * speed;
            Vector2 moveToFast = toPlayer * fastSpeed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VonBossBag>()));
        }
    }
}

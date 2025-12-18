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
        public override void SetDefaults()
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
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme1");
            }
        }
        
        static Random random = new Random();
        public static int Phase2HeadSlot = -1;
        public int vonShotTimer = 0;
        public static int vonShotTimerMax = 3;
        public int vonTpTimer = 0;
        public static int vonTpTimerMax = 600;
        public int vonReloadTimer = 0;
        public static int vonReloadTimerMax = 175;
        public int vonSpecialTimer = 0;
        public static int vonSpecialTimerMax = 420;
        public int vonRageTimer = 0;
        public static int vonRageTimerMax = 300;
        public int randRotate = random.Next(0, 11);
        
        
        
        
        public static readonly SoundStyle VonShot = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2")
        {
            Volume = .2f,
            PitchVariance = .2f
        };

        public static readonly SoundStyle VonNade = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/VonNadeThrow")
        {
            Volume = 1.5f
        };

        public bool Phase2
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value ? 1f : 0f;
        }

        public override void Load()
        {
            string texture = BossHeadTexture + "_Phase2"; 
            Phase2HeadSlot = Mod.AddBossHeadTexture(texture, -1); 
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

            CheckPhase2();

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

        private void DoPhase1(Player player)
        {
            var entitySource = NPC.GetSource_FromAI();
            Vector2 toPlayer = player.Center - NPC.Center;
            float speed = .015f;
            float fastSpeed = .04f;
            float nadeSpeed = .03f;
            float inertia = 40f;
            Vector2 moveTo = toPlayer * speed;
            Vector2 moveToFast = toPlayer * fastSpeed;
            Vector2 nadeToPlayer = toPlayer * nadeSpeed;
            
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
            // TODO: Make him jump over obstacles
            if (vonReloadTimer <= vonReloadTimerMax * .55)
            {
                if (vonShotTimer >= vonShotTimerMax)
                {
                    if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, toPlayer * 15, ModContent.ProjectileType<VonGreenTracerProj>(), 10, .5f, Main.myPlayer);
                        SoundEngine.PlaySound(VonShot, NPC.Center);
                        vonShotTimer = 0;
                    }
                }
                else
                {
                    vonShotTimer += 1;
                    
                }
            }
            
            vonReloadTimer += 1;
            
            if (vonReloadTimer >= vonReloadTimerMax)
            {
                vonReloadTimer = 0;
            }

            vonTpTimer += 1;

            if (vonTpTimer >= vonTpTimerMax && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //TODO: Make him tp
                vonTpTimer = 0;
            }

            if (vonSpecialTimer >= vonSpecialTimerMax)
            {
                int vonRandAttack = random.Next(0, 11);
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (vonRandAttack > 5)
                    {
                        //TODO: Make a custom grenade with a bigger explosion
                        int vonNade = Projectile.NewProjectile(entitySource, NPC.Center, nadeToPlayer, ProjectileID.Grenade, 100, 2f, Main.myPlayer);
                        Main.projectile[vonNade].friendly = false;
                        Main.projectile[vonNade].hostile = true;
                        Main.projectile[vonNade].scale = 2f;
                        Main.projectile[vonNade].timeLeft = 60;
                        SoundEngine.PlaySound(VonNade, NPC.Center);
                        vonSpecialTimer = 0;
                    }
                    else
                    {
                        //TODO: Make a sound queue for when he jumps
                        while (vonRageTimer < vonRageTimerMax)
                        {
                            NPC.velocity = (NPC.velocity * (inertia - 1) + moveToFast) / inertia;
                            NPC.velocity.Y = -30;
                            vonRageTimer += 1;
                        }
                        vonRageTimer = 0;
                        vonSpecialTimer = 0;
                        NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
                    }
                }
            }
            vonSpecialTimer += 1;
        }

        private void DoPhase2(Player player)
        {
            NPC.noGravity = true;
            NPC.setNPCName("Von, Evil Incarnate", ModContent.NPCType<KingVon>());
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme2");
            }
            var entitySource = NPC.GetSource_FromAI();
            Vector2 toPlayer = player.Center - NPC.Center;
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

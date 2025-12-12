using Microsoft.Build.Framework;
using MogMod.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna;
using Terraria.DataStructures;

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
            NPC.knockBackResist = .5f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.npcSlots = 6f;
            NPC.aiStyle = -1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme1");
            }
        }

        public static int Phase2HeadSlot = -1;
        public static int shotTimer = 0;
        public int shotTimerMax = 3;
        public static readonly SoundStyle VonShot = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2")
        {
            Volume = .2f,
            PitchVariance = .2f
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
            Vector2 toPlayer = player.Center - NPC.Center;
            float speed = .025f;
            float inertia = 40f;
            Vector2 moveTo = toPlayer * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
            NPC.velocity.Y = 2f;
            // TODO: Make him jump over obstacles
            while (NPC.velocity.Y < 2f)
            {
                NPC.velocity.Y += .5f;
            }




            //TODO: Make him have real attacks instead of just shooting at you repeatedly
            if (shotTimer >= shotTimerMax)
            {
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var entitySource = NPC.GetSource_FromAI();
                    Projectile.NewProjectile(entitySource, NPC.Center, toPlayer * 15, ModContent.ProjectileType<VonGreenTracerProj>(), 10, .5f, Main.myPlayer);
                    SoundEngine.PlaySound(VonShot, NPC.Center);
                    shotTimer = 0;
                }
            } else
            {
                shotTimer += 1;
            }
        }

        private void DoPhase2(Player player)
        {
            //TODO: Give him ai for phase 2
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VonTheme2");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
                       
        }
    }
}
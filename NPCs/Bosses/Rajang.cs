using Microsoft.Build.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Projectiles;
using MogMod.Utilities;
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
    public class Rajang : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 200;
            NPC.height = 100;
            NPC.damage = 82;
            NPC.defense = 12;
            if (Phase2)
            {
                NPC.damage = 102;
                NPC.defense = 60;
            }
            NPC.lifeMax = 33500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.netAlways = true;
            NPC.npcSlots = 6f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            
            // plays the boss music for phase 1
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Rajang");
            }
        }

        static Random random = new Random();
        public static int Phase2HeadSlot = -1;
        public int rajangShotTimer = 0;
        public static int rajangShotTimerMax = 3;
        public int rajangTpTimer = 0;
        public static int rajangTpTimerMax = 600;
        public int rajangReloadTimer = 0;
        public static int rajangReloadTimerMax = 175;
        public int rajangSpecialTimer = 0;
        public static int rajangSpecialTimerMax = 420;
        public int rajangRageTimer = 0;
        public static int rajangRageTimerMax = 300;
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

        // used for different moves depending on what phase its in
        public bool Phase2
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value ? 1f : 0f;
        }

        // loads boss icons
        public override void Load()
        {
            string texture = BossHeadTexture + "_Phase2";
            Phase2HeadSlot = Mod.AddBossHeadTexture(texture, -1);
        }

        // the boss icon on the map
        public override void BossHeadSlot(ref int index)
        {
            int slot = Phase2HeadSlot;
            if (Phase2 && slot != -1)
            {
                index = slot;
            }
        }

        // prevents cheesing bosses
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        // boss targeting ai, despawns if dead
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
            }
            else
            {
                DoPhase1(player);
            }

        }

        // checks for phase 2
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
            //if (rajangReloadTimer <= rajangReloadTimerMax * .55)
            //{
            //    if (rajangShotTimer >= rajangShotTimerMax)
            //    {
            //        if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            //        {
            //            Projectile.NewProjectile(entitySource, NPC.Center, toPlayer * 15, ModContent.ProjectileType<VonGreenTracerProj>(), 10, .5f, Main.myPlayer);
            //            SoundEngine.PlaySound(VonShot, NPC.Center);
            //            rajangShotTimer = 0;
            //        }
            //    }
            //    else
            //    {
            //        rajangShotTimer += 1;

            //    }
            //}

            rajangReloadTimer += 1;

            if (rajangReloadTimer >= rajangReloadTimerMax)
            {
                rajangReloadTimer = 0;
            }

            rajangTpTimer += 1;

            if (rajangTpTimer >= rajangTpTimerMax && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //TODO: Make him tp
                rajangTpTimer = 0;
            }

            if (rajangSpecialTimer >= rajangSpecialTimerMax)
            {
                int rajangRandAttack = random.Next(0, 11);
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (rajangRandAttack > 9)
                    {
                        // nade out
                        //TODO: Make a custom grenade with a bigger explosion
                        int rajangNade = Projectile.NewProjectile(entitySource, NPC.Center, nadeToPlayer, ProjectileID.Grenade, 100, 2f, Main.myPlayer);
                        Main.projectile[rajangNade].friendly = false;
                        Main.projectile[rajangNade].hostile = true;
                        Main.projectile[rajangNade].scale = 2f;
                        Main.projectile[rajangNade].timeLeft = 60;
                        SoundEngine.PlaySound(VonNade, NPC.Center);
                        rajangSpecialTimer = 0;
                    }
                    else
                    {
                        //TODO: Make a sound queue for when he jumps
                        while (rajangRageTimer < rajangRageTimerMax)
                        {
                            NPC.velocity = (NPC.velocity * (inertia - 1) + moveToFast) / inertia;
                            NPC.velocity.Y = -30;
                            rajangRageTimer += 1;
                        }
                        rajangRageTimer = 0;
                        rajangSpecialTimer = 0;
                        NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
                    }
                }
            }
            rajangSpecialTimer += 1;
        }

        // phase 2 ()
        private void DoPhase2(Player player)
        {
            NPC.setNPCName("Furious Rajang", ModContent.NPCType<Rajang>());
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/FuriousRajang");

            }
            var entitySource = NPC.GetSource_FromAI();
            Vector2 toPlayer = player.Center - NPC.Center;
            float speed = .04f;
            float fastSpeed = .08f;
            float inertia = 50f;
            Vector2 moveTo = toPlayer * speed;
            Vector2 moveToFast = toPlayer * fastSpeed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
        }
        // makes it drop greater healing potion compared to lesser healing potion
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<RajangBossBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            //var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            //{
            //    // Materials
            //    normalOnly.Add(ModContent.ItemType<UltimateOrb>(), 1, 8, 10);
            //}

            npcLoot.Add(ItemDropRule.Common(ItemID.Banana, 1, 2, 7));

            // Trophy (always directly from boss, never in bag) (also not in the game right now)
            // npcLoot.Add(ModContent.ItemType<RajangTrophy>(), 10);

            // Relic (for master mode)
            // npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CryogenRelic>());
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Phase2)
                {
                    target.AddBuff(BuffID.Dazed, 360, true);
                }
                else
                {
                    target.AddBuff(BuffID.Dazed, 180, true);
                    // add to electric projectiles <-- i was gonna do that
                    //target.AddBuff(BuffID.Electrified, 360, true);
                    //Hey Will to do this I think you'd put this in the file for the custom projectile, not the boss
                }
            }
        }
    }
}

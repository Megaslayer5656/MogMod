using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Projectiles.EnemyProjectiles.Boss;
using MogMod.Tiles.Ores;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class DabDad : ModNPC
    {
        public static readonly SoundStyle VonShot = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Switch_Shot_2") //Shot sound effect
        {
            Volume = .2f,
            PitchVariance = .2f
        };
        public ref float Time => ref NPC.ai[0];
        public static readonly Color TextColor = new(44, 161, 39);
        public static readonly Color TextColorEvil = new(161, 39, 39);
        public override void SetDefaults()
        {
            NPC.width = 198;
            NPC.height = 225;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = Main.masterMode ? Main.bloodMoon ? 100000000 : 1000000 : 100000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 60, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Desperate");
            }
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
                
                NPC.velocity.Y -= 0.04f;
                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersTaunt with { Pitch = -1f }, NPC.Center);
                NPC.EncourageDespawn(10);
                return;
            }
            if (Main.zenithWorld)
            {
                Time++;
                if (Time == 80)
                    MiscUtils.BroadcastLocalizedText("Mods.MogMod.Status.Boss.DabDadEvilText1", TextColor);
                if (Time == 160)
                    MiscUtils.BroadcastLocalizedText("Mods.MogMod.Status.Boss.DabDadEvilText2", TextColorEvil);
                if (Time >= 200)
                {
                    var entitySource = NPC.GetSource_FromAI();
                    Vector2 toPlayer = player.Center - NPC.Center;
                    if (NPC.HasValidTarget)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(entitySource, NPC.Center, (toPlayer * 15).RotatedByRandom(MathHelper.ToRadians(35)), ModContent.ProjectileType<VonGreenTracerProj>(), 60, .5f, 255);
                        }

                        if (Main.netMode != NetmodeID.Server)
                            SoundEngine.PlaySound(VonShot, NPC.Center);
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LedX>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DabDadBossBag>()));
        }
        public override void OnKill()
        {
            if (!DownedBossSystem.downedDabDad)
            {
                ModContent.GetInstance<DabDadOreSystem>().BlessWorldWithDabDadOre();
            }
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDabDad, -1);
        }
    }
 }
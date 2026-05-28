using Microsoft.Xna.Framework;
using MogMod.Items.Placeable.Banners;
using MogMod.Projectiles.EnemyProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Enemies
{
    public class WarlockGolem : ModNPC
    {
        public float size = 0f;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.8f,
            };
            value.Position.X += 48f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }
        public override void SetDefaults()
        {
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.damage = 42;
            NPC.width = 64;
            NPC.height = 140;
            NPC.defense = 22;
            NPC.lifeMax = 570;
            NPC.knockBackResist = 0.05f;
            NPC.lavaImmune = true;
            AIType = -1;
            NPC.value = Item.buyPrice(gold: 4);
            NPC.HitSound = SoundID.NPCHit21;
            NPC.DeathSound = SoundID.NPCDeath24;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WarlockGolemBanner>();
            ItemID.Sets.KillsToBanner[BannerItem] = 25; // Custom kill count required for banner drop and bestiary unlock. Omit this line for the default 50 kill count.
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.WarlockGolem")
            ]);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt ||
                spawnInfo.Player.ZoneCrimson ||
                spawnInfo.Player.ZoneOldOneArmy ||
                spawnInfo.Player.ZoneSkyHeight ||
                spawnInfo.PlayerSafe ||
                spawnInfo.Player.ZoneDesert ||
                spawnInfo.Player.ZoneOverworldHeight ||
                !spawnInfo.Player.ZoneUnderworldHeight ||
                Main.eclipse ||
                Main.snowMoon ||
                Main.pumpkinMoon ||
                Main.invasionType != InvasionID.None)
                return 0f;
            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;
            return 0.05f;
        }
        public override void AI()
        {
            NPC.TargetClosest(true);
            if ((Main.player[NPC.target].position.Y > NPC.position.Y + (float)NPC.height && NPC.velocity.Y > 0f) || (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height && NPC.velocity.Y < 0f))
                NPC.noTileCollide = true;
            else
                NPC.noTileCollide = false;
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            float movementSpeed = 2f;
            bool stopMoving = false;
            if (NPC.ai[0] < 0f)
                NPC.ai[0] += 1f;
            if (Math.Abs(NPC.Center.X - player.Center.X) < 150f && NPC.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = 1f;
                    SoundEngine.PlaySound(SoundID.Zombie91, NPC.Center);
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                stopMoving = true;
                NPC.ai[1] += 1f;
                size = (NPC.ai[1] * 5f) + 100f;
                int dust3 = Dust.NewDust(NPC.Center, (int)(size / 2), (int)(size / 2), DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust3].velocity *= 1.4f;
                for (int i = 0; i < 50; i++)
                {
                    Vector2 randomOffset = Main.rand.NextVector2Circular(size / 2.1f, size / 2.1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center + randomOffset, DustID.Flare, NPC.DirectionFrom(NPC.Center + NPC.velocity + randomOffset) * Main.rand.NextFloat(3f, 5f));
                    d.fadeIn = .15f;
                    d.scale = .75f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 70; i++)
                {
                    Vector2 randPos = Main.rand.NextVector2CircularEdge(size / 2f, size / 2f);
                    Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, DustID.CopperCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                    telegraphDust.noGravity = true;
                }
                if (NPC.ai[1] >= 90f)
                {
                    int type = ModContent.ProjectileType<WarlockBoom>();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = Main.masterMode ? 16 : Main.expertMode ? 18 : 20;
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                            NPC.Center,
                            Vector2.Zero,
                            type,
                            damage,
                            6f,
                            Main.myPlayer);
                        NPC.netUpdate = true;
                    }
                    NPC.netUpdate = true;
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = -60f;
                }
            }
            if (stopMoving)
            {
                NPC.velocity.X *= 0.9f;
                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
            }
            else
            {
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * 20f + movementSpeed) / 21f;
                if (NPC.direction < 0)
                    NPC.velocity.X = (NPC.velocity.X * 20f - movementSpeed) / 21f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 2f);
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule postEvil = npcLoot.DefineConditionalDropSet(DropHelper.PostEvil());
            npcLoot.Add(ItemDropRule.Common(ItemID.Obsidian, 1, 12, 18));
            postEvil.Add(ItemID.Hellstone, 1, 12, 18);
        }
    }
}
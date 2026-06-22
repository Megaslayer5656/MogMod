using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace MogMod.NPCs.Enemies
{
    public class WanderingJellyfish : ModNPC
    {
        #region Setup
        public Player player => Main.player[NPC.target];
        public ref float AITimer => ref NPC.ai[1];
        public float explodeTimer = 1f;
        public override void SetStaticDefaults() => Main.npcFrameCount[NPC.type] = 4;
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 34;

            NPC.damage = 15;
            NPC.defense = 0;
            NPC.lifeMax = 20;
            NPC.knockBackResist = 1.2f;

            NPC.noGravity = true;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WanderingJellyfishBanner>();
        }
        #endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.WanderingJellyfish")
            ]);
        }
        public override bool PreKill()
        {
            if (AITimer >= (explodeTimer + 90f))
                return false;
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiritShard>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEssence>(), 8, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ItemID.Glowstick, 1, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.JellyfishNecklace, 100));
        }
        #endregion

        #region Spawning && AI
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneRain || spawnInfo.Player.ZoneDesert || !Condition.DownedEyeOfCthulhu.IsMet())
                return 0f;

            return SpawnCondition.OverworldDayRain.Chance * .15f;
        }
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.LightSkyBlue.ToVector3());
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
                NPC.TargetClosest(true);
            if (Main.rand.Next(0, 10) == 0)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 0, default, .75f);
            if (NPC.WithinRange(player.Center, player.Size.Length() * 2f) && !player.dead)
            {
                if (AITimer < explodeTimer)
                    AITimer = explodeTimer;
                NPC.netUpdate = true;
            }
            AIMovement(player);
            if (AITimer >= explodeTimer)
                State_Exploding(player);
        }
        public void AIMovement(Player player)
        {
            Vector2 epstein = new Vector2(NPC.Center.X + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
            bool canHitTarget = Collision.CanHit(new Vector2(epstein.X, epstein.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
            Vector2 einstein = Main.player[NPC.target].Center;
            Vector2 velocity = NPC.SafeDirectionTo(einstein) * 4f;

            // Movement calculations
            if (Vector2.Distance(epstein, einstein) > 40f || !canHitTarget)
                NPC.SimpleFlyMovement(velocity, .02f);

            NPC.netUpdate = true;
        }
        public void State_Exploding(Player player)
        {
            NPC.TargetClosest(true);
            AITimer++;
            int size = (int)((NPC.ai[1] * 1.5f) + 30f);
            Vector2 offset = new Vector2(size / 2f);
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(size / 2.1f, size / 2.1f);
                Dust d = Dust.NewDustPerfect(NPC.Center + randomOffset, DustID.SilverCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randomOffset) * Main.rand.NextFloat(1f, 3f));
                d.fadeIn = .15f;
                d.scale = .5f;
                d.noGravity = true;
            }
            for (int i = 0; i < 50; i++)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(size / 2f, size / 2f);
                Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, DustID.PlatinumCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
            }

            // explosive climax
            if (AITimer >= (explodeTimer + 90f))
            {
                NPC.value = 0f;
                NPC.extraValue = 0;
                SoundEngine.PlaySound(SoundID.Item94, NPC.Center);
                Vector2 center = NPC.Center;
                NPC.width = NPC.height = 150;
                NPC.Center = center;
                Rectangle myRect = NPC.getRect();

                for (int i = 0; i < 45; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.SilverCoin, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 27; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, 76, 0f, 0f, 100, default, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(NPC.Center - offset, size, size, 76, 0f, 0f, 100, default, 1.6f);
                    Main.dust[dust].velocity *= 3f;
                }
                NPC.StrikeInstantKill();
                foreach (Player target in Main.ActivePlayers)
                {
                    if (player.dead || !NPC.Hitbox.Intersects(player.Hitbox))
                        continue;
                    int direction = NPC.Center.X - target.Center.X < 0 ? -1 : 1;
                    target.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, -direction);
                }
                NPC.netUpdate = true;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        #endregion

        #region Framing && Hit Effects
        public override void FindFrame(int frameHeight)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;
                if (NPC.direction == 1)
                    NPC.spriteDirection = 1;
                if (NPC.direction == -1)
                    NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)(NPC.velocity.Y * (float)NPC.direction), (double)(NPC.velocity.X * (float)NPC.direction));
            }
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 76, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
                for (int k = 0; k < 25; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 76, hit.HitDirection, -1f, 0, default, 1f);
        }
        #endregion
    }
}
using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Enemies
{
    public class HellfireSpirit : ModNPC
    {
        #region Setup
        public ref float ExplosionTimer => ref NPC.ai[1];
        public const float ExplodeTime = 240f;
        public override void SetStaticDefaults() => Main.npcFrameCount[NPC.type] = 3;
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 30;

            NPC.damage = 50;
            NPC.defense = 10;
            NPC.lifeMax = 120;
            NPC.knockBackResist = .8f;

            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HellfireSpiritBanner>();
        }
        #endregion

        #region Bestiary
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.HellfireSpirit")
            ]);
        }
        #endregion
        
        #region AI
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.OrangeRed.ToVector3());
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            Vector2 between = player.Center - NPC.Center;
            NPC.rotation = between.ToRotation() - MathHelper.PiOver2;

            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.2f;
            NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.2f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10f, 10f);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -10f, 10f);

            ExplosionTimer++;

            int size = (int)((NPC.ai[1] * .1f) + 10f);
            Vector2 offset = new Vector2(size / 2f);
            Vector2 randomOffset = Main.rand.NextVector2Circular(size / 2.1f, size / 2.1f);
            Dust d = Dust.NewDustPerfect(NPC.Center + randomOffset, DustID.CopperCoin, NPC.DirectionFrom(NPC.Center + NPC.velocity + randomOffset) * Main.rand.NextFloat(3f, 5f));
            d.fadeIn = .15f;
            d.scale = .5f;
            d.noGravity = true;
            int dust3 = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.CopperCoin, 0f, 0f, 100, default, 1.7f);
            Main.dust[dust3].velocity *= 1.4f;

            // explosive climax
            if (ExplosionTimer >= ExplodeTime)
            {
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
                Vector2 center = NPC.Center;
                NPC.width = NPC.height = 150;
                NPC.Center = center;

                Rectangle myRect = NPC.getRect();

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
                    foreach (Player target in Main.ActivePlayers)
                    {
                        if (target.getRect().Intersects(myRect))
                        {
                            int direction = NPC.Center.X - target.Center.X < 0 ? -1 : 1;
                            target.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, direction);
                        }
                    }
                    NPC.StrikeInstantKill();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
        }
        public override void OnSpawn(IEntitySource source) => NPC.immune[Main.myPlayer] = 10;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            ExplosionTimer = ExplodeTime;
            target.AddBuff(BuffID.Burning, 180);
        }
        #endregion

        #region Item Drops & Misc Effects
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HellfireEssence>(), 1, 1, 2));
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Flare, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
                for (int k = 0; k < 25; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Flare, hit.HitDirection, -1f, 0, default, 1f);
        }
        #endregion
    }
}
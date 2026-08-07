using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.NPCs.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.ProjectileEnemies
{
    // 30x30
    public class HealingOrb : ModNPC
    {
        #region Setup
        public ref float AITimer => ref NPC.ai[1];
        public float explodeTime = 120f;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.ProjectileNPC[NPC.type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 30;

            NPC.damage = 0;
            NPC.lifeMax = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = false;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.DeathSound = SoundID.Dig;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }
        #endregion

        #region AI
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
            NPC.velocity = new(0f, -4f);
        }
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.LimeGreen.ToVector3());
            if (Main.rand.NextBool(10))
                Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(3) ? DustID.PoisonStaff : DustID.ChlorophyteWeapon, NPC.velocity.X * 0.25f, NPC.velocity.Y * 0.25f, 0, default, 1f);
            NPC.velocity.Y *= 0.95f;
            AITimer++;
            int size = (int)((AITimer * .75f) + 30f);
            Vector2 offset = new Vector2(size / 2f);
            for (int i = 0; i < 35; i++)
            {
                Vector2 randPos = Main.rand.NextVector2CircularEdge(size / 2f, size / 2f);
                Dust telegraphDust = Dust.NewDustPerfect(NPC.Center + randPos, Main.rand.NextBool(3) ? DustID.GreenTorch : DustID.JungleTorch, NPC.DirectionFrom(NPC.Center + NPC.velocity + randPos) * Main.rand.NextFloat(5f, 7f), 0, default, 1.5f);
                telegraphDust.noGravity = true;
            }
            NPC.dontTakeDamage = AITimer <= explodeTime - 90f;
            if (AITimer >= (explodeTime + 60f))
            {
                SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                Vector2 center = NPC.Center;
                NPC.width = NPC.height = 220;
                NPC.Center = center;
                Rectangle myRect = NPC.getRect();

                for (int i = 0; i < 45; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, Main.rand.NextBool(3) ? DustID.PoisonStaff : DustID.ChlorophyteWeapon, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.Smoke, 0f, 0f, 100, default, 1.7f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 27; i++)
                {
                    int dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.JungleTorch, 0f, 0f, 100, default, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(NPC.Center - offset, size, size, DustID.GreenTorch, 0f, 0f, 100, default, 1.6f);
                    Main.dust[dust].velocity *= 3f;
                }
                NPC.immortal = false;
                NPC.StrikeInstantKill();
                int heal = Main.hardMode ? MendingAspect.LifeHeal * 2 : MendingAspect.LifeHeal;
                if (Main.zenithWorld)
                {
                    foreach (Player target in Main.ActivePlayers)
                    {
                        if (target.dead || !NPC.Hitbox.Intersects(target.Hitbox))
                            continue;
                        target.immune = false;
                        target.immuneTime = 0;
                        target.statLife -= heal;
                        target.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.MendingGFB").ToNetworkText(target.name)), heal, 0);
                    }
                    foreach (NPC target in Main.ActiveNPCs)
                    {
                        if (!target.active || !NPC.Hitbox.Intersects(target.Hitbox))
                            continue;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (target.type != NPCID.TargetDummy)
                                target.life -= heal;
                            target.DamageEffect(heal, Color.OrangeRed);
                        }
                        if (target.life <= 0)
                        {
                            target.life = 0;
                            target.HitEffect();
                            target.checkDead();
                        }
                        target.netUpdate = true;
                    }
                }
                else
                {
                    foreach (Player target in Main.ActivePlayers)
                    {
                        if (target.dead || !NPC.Hitbox.Intersects(target.Hitbox))
                            continue;
                        target.HealLifeMult(heal);
                    }
                    foreach (NPC target in Main.ActiveNPCs)
                    {
                        if (!target.active || !NPC.Hitbox.Intersects(target.Hitbox) || target.type == NPC.type)
                            continue;
                        target.life += heal * 5;
                        target.HealEffect(heal * 5);
                        if (target.life > target.lifeMax)
                            target.life = target.lifeMax;
                        target.netUpdate = true;
                    }
                }
                NPC.active = true;
                NPC.netUpdate = true;
            }
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        #endregion

        #region Frames && Hit Effects
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += AITimer >= explodeTime ? 0.3f : 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(3) ? DustID.PoisonStaff : DustID.ChlorophyteWeapon, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
                for (int k = 0; k < 25; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool(3) ? DustID.PoisonStaff : DustID.ChlorophyteWeapon, hit.HitDirection, -1f, 0, default, 1f);
        }
        #endregion
    }
}

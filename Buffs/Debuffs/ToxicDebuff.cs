using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.NPCs.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.Debuffs
{
    public class ToxicDebuff : ModBuff
    {
        public const int DamageMin = 10;
        public const int DamageMax = 46;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.toxicDebuff = true;

            if (Main.zenithWorld)
                mogPlayer.toxicDamage = 54568;
            if (player.buffTime[buffIndex] < 1)
            {
                player.immune = false;
                player.immuneTime = 0;
                if (Main.zenithWorld)
                {
                    player.statLife -= mogPlayer.toxicDamage;
                    player.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.ToxicGFB").ToNetworkText(player.name)), mogPlayer.toxicDamage, 0);
                    return;
                }
                player.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Toxic" + Main.rand.Next(1, 2 + 1)).ToNetworkText(player.name)), mogPlayer.toxicDamage, 0);
                mogPlayer.toxicDamage = 0;
            }
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            MogModGlobalNPC mogNPC = npc.MogMod();
            mogNPC.toxicDebuff = true;
            if (npc.buffTime[buffIndex] < 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.type != NPCID.TargetDummy)
                        npc.life -= mogNPC.toxicDamage;
                    npc.DamageEffect(mogNPC.toxicDamage, Color.Purple);
                }
                if (npc.life <= 0)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.checkDead();
                }

                mogNPC.toxicDamage = 0;
                npc.netUpdate = true;
            }
        }
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            int hot = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.Poisoned, player.velocity.X * 1.1f, player.velocity.Y * 1.1f, 100, default, 0.8f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.15f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4, DustID.Venom, player.velocity.X * 1.1f, player.velocity.Y * 1.1f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.35f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= .95f;
                }
            }
        }
        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            int hot = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.Poisoned, npc.velocity.X * 1.1f, npc.velocity.Y * 1.1f, 100, default, 0.8f);
            Main.dust[hot].noGravity = false;
            Main.dust[hot].velocity *= 1.15f;
            Main.dust[hot].scale *= .95f;
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(npc.position - new Vector2(2f), npc.width + 4, npc.height + 4, DustID.Venom, npc.velocity.X * 1.1f, npc.velocity.Y * 1.1f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.35f;
                if (Main.rand.NextBool(4))
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.95f;
                }
            }
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;
            MogPlayer mogPlayer = player.MogMod();
            tip = base.Description.Format(mogPlayer.toxicDamage);
        }
    }
}
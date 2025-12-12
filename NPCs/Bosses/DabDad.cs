using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Audio;
using MogMod.Items.Weapons;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Common.Systems;

using MogMod.Tiles;
namespace MogMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class DabDad : ModNPC
    {
        
        public override void SetDefaults()
        {
            NPC.width = 256;
            NPC.height = 315;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
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
                
                NPC.EncourageDespawn(10);
                return;
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
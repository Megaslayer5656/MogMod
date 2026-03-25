using MogMod.Items.Accessories;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Items.Placeable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader.Utilities;

namespace MogMod.NPCs.Enemies
{
    public class FreakyTien : ModNPC
    {
        public override void SetStaticDefaults()
        {
            
        }
        
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 35;
            NPC.height = 62;
            NPC.aiStyle = 3;
            NPC.defense = 5;
            NPC.lifeMax = 69;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            //Main.npcFrameCount[NPC.type] = 9;
            //NPCID.Sets.ExtraFramesCount[NPC.type] = 0;
            //NPCID.Sets.AttackFrameCount[NPC.type] = 0;
            //NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = 1;
            AIType = NPCID.GoblinScout;
            //AnimationType = 48;
            NPC.scale = .05f;
        }

        //public override float SpawnChance(NPCSpawnInfo spawnInfo)
        //{
        //    return SpawnCondition.OverworldNightMonster.Chance * .1f;
        //}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FreakySauce>(), 10));
        }
    }
}

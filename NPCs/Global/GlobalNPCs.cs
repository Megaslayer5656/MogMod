using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.NPCs.Global
{
    public class GlobalNPCs : GlobalNPC
    {
        // do something for eye of skadi
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new CommonDrop(ModContent.ItemType<LedX>(), 10000, 1, 1, 1));
            globalLoot.Add(new CommonDrop(ModContent.ItemType<RedX>(), 100000, 1, 1, 1));
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DarkCaster)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlinkDagger>(), 10));
            }
            
            if (npc.type == NPCID.BlueJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
            if (npc.type == NPCID.GreenJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
            if (npc.type == NPCID.PinkJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
            if (npc.type == NPCID.BloodJelly)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
            if (npc.type == NPCID.Shark)
            {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 8));
            }
            if (npc.type == NPCID.Squid)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
            if (npc.type == NPCID.Crab)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HydrakanLatch>(), 15));
            }
        }
    }
}

using MogMod.Items.Accessories;
using MogMod.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class SolBadguy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 35;
            NPC.height = 52;
            NPC.aiStyle = 7;
            NPC.defense = 150;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 0;
            NPCID.Sets.AttackFrameCount[NPC.type] = 0;
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = -1;
            AnimationType = 48;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            for (var i = 0; i < 225; i++)
            {
                Player player = Main.player[i];
                foreach (Item item in player.inventory)
                {
                    if (item.type == ItemID.HellstoneBar)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                 "Sol Badguy",
                 "Frederick"
            };
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Mewing Streak";
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
           if (firstButton)
            {
                shop = "Shop";
            }
        }
        public override void AddShops()
        {
            new NPCShop(Type)
                .Add<Glue>()
                .Add<MewingGuide>()
                .Register();
        }
        public override string GetChat()
        {
            NPC.FindFirstNPC(ModContent.NPCType<SolBadguy>());
            switch (Main.rand.Next(5)) 
            {
                case 0:
                    return "What the sigma do you want?";
                case 1:
                    return "I'm the only sigma around here.";
                case 2:
                    return "You clearly need mewing lessons.";
                case 3:
                    return "Tricky.";
                default:
                    return "Only real sigmas jelq.";
            }
        }
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<MewingGuide>(), 1, false, 0, false, false);
        }
    }
}
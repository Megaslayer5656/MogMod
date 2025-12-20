using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Items.Accessories;
using MogMod.Items.Other;
using Terraria.ID;
using Terraria.Localization;
using MogMod.Items.Weapons.Ranged;
using MogMod.Items.Consumables;

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class Prapor : ModNPC
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 100;
            NPC.height = 85;
            NPC.aiStyle = 7;
            NPC.defense = 271;
            NPC.lifeMax = 67;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.scale = .5f;
            //Main.npcFrameCount[NPC.type] = 9;
            //NPCID.Sets.ExtraFramesCount[NPC.type] = 0;
            //NPCID.Sets.AttackFrameCount[NPC.type] = 0;
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = -1;
            //AnimationType = 48;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return NPC.downedBoss2;
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                 "Pavel Yegorovich Romanenko",
            };
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
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
            NPCShop shop = new(Type);
            shop.Add<Mosin>()
                .Add<MosinLPS>()
                .Add<Salewa>()
                .Add<IdeaRig>()
                .Add((ModContent.ItemType<Switch>()), Condition.DownedGolem)
                .Add((ModContent.ItemType<GreenTracerAmmo>()), Condition.DownedGolem)
                .Register();
        }
        public override string GetChat()
        {
            NPC.FindFirstNPC(ModContent.NPCType<Mendez>());
            switch (Main.rand.Next(5))
            {
                case 0:
                    return "Lost your gear, musketeer?";
                case 1:
                    return "My dogs are very obidient.";
                case 2:
                    return "Prap prap prap.";
                case 3:
                    return "Oi nerf gunner!";
                case 4:
                    return "Found my pocket watch yet?";
                case 5:
                    return "Don't listen to that skier guy, he's a bum.";
                default:
                    return "I sent my dogs to look for your stuff.";
            }
        }
    }
}

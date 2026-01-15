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

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class Mendez : ModNPC
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 35;
            NPC.height = 62;
            NPC.aiStyle = 7;
            NPC.defense = 80085;
            NPC.lifeMax = 67;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            //Main.npcFrameCount[NPC.type] = 9;
            //NPCID.Sets.ExtraFramesCount[NPC.type] = 0;
            //NPCID.Sets.AttackFrameCount[NPC.type] = 0;
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = -1;
            //AnimationType = 48;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            for (var i = 0; i < 225; i++)
            {
                Player player = Main.player[i];
                foreach (Item item in player.inventory)
                {
                    if (item.type == ModContent.ItemType<LedX>())
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
                 "J* Mendih",
                 "Justin Mendez",
                 "Mendez",
                 "tanky rizzler"
            };
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "o na";
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
            shop.Add<BizarreMusicBox>()
                .Add<DesperateMusicBox>()
                .Add<RajangMusicBox>()
                .Add<RideTheFireMusicBox>()
                .Add<KingVonMusicBox>()
                .Add<VonEvilIncarnateMusicBox>()
                .Add<LedX>()
                .Add((ItemID.ChlorophyteShotbow), Condition.DownedMechBossAny)
                .Add<Phasma>()
                .Add(ModContent.ItemType<EyeOfMendez>(), Condition.PlayerCarriesItem(ModContent.ItemType<RedX>()))
                .Register();
        }
        public override string GetChat()
        {
            NPC.FindFirstNPC(ModContent.NPCType<Mendez>());
            switch (Main.rand.Next(5))
            {
                case 0:
                    return "I found these in a chest.";
                case 1:
                    return "O na o na o na.";
                case 2:
                    return "tarkov.";
                case 3:
                    return "o na ragebait successful.";
                case 4:
                    return "liked by J* Mendih.";
                case 5:
                    return "su ban su ban suban o na";
                default:
                    return "ona.";
            }
        }
        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<LedX>(), 1, false, 0, false, false);
        }
    }
}
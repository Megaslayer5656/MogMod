using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Accessories;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Utilities;
using MogMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class SolBadguy : ModNPC
    {
        // attacking doesnt work properly since sol doesnt have a town npc spritesheet
        #region Setup
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            NPCID.Sets.DangerDetectRange[Type] = 120;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 18;
            NPCID.Sets.AttackAverageChance[Type] = 8;
            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate)
            ;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 35;
            NPC.height = 52;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.defense = 150;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = 48;
        }
        public override bool CanGoToStatue(bool toKingStatue) => true;
        #endregion

        #region Attacking
        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 134;
        }
        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            int itemType = ModContent.ItemType<Flamewall>();
            Main.GetItemDrawFrame(itemType, out item, out itemFrame);
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 60;
            knockback = 10f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 50;
            randExtraCooldown = 1;
        }
        #endregion

        #region Name && Spawning
        public override void AI()
        {
            if (!MogModWorld.spawnedSolBadguy)
                MogModWorld.spawnedSolBadguy = true;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (MogModWorld.spawnedSolBadguy)
                return true;
            foreach (Player player in Main.ActivePlayers)
            {
                bool strive = player.InventoryHas(ItemID.HellstoneBar) || player.PortableStorageHas(ItemID.HellstoneBar);
                if (strive)
                    return true;
            }
            return false;
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                 "Sol Badguy",
                 "Frederick",
                 "The Big SBG"
            };
        }
        #endregion

        #region Chatting
        public override bool CanChat() => true;
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
                .Add<GiantsMaul>(new Condition(MiscUtils.GetText("Condition.HasFoundGiantsMaul"), () => MogModWorld.HasFoundGiantsMaul))
                .Add<SoulFragment>(Condition.DownedMoonLord)
                .Add<AghanimShard>(Condition.DownedCultist)
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
        #endregion
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<MewingGuide>(), 1, false, 0, false, false);
        }
    }
}
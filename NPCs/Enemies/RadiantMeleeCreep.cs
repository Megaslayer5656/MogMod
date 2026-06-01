using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace MogMod.NPCs.Enemies
{
    public class RadiantMeleeCreep : ModNPC
    {
        #region Setup
        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 14;
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 40;

            NPC.damage = 14;
            NPC.defense = 4;
            NPC.lifeMax = 50;
            NPC.knockBackResist = .5f;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.value = Item.buyPrice(copper: 80);

            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.GoblinScout;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RadiantMeleeCreepBanner>();
        }
#endregion

        #region Bestiary && Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.RadiantMeleeCreep")
            ]);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CraftingRecipe>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepBlood>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.LeadBroadsword, 20, 1, 1));
        }
#endregion

        #region Spawning && Framing
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneOverworldHeight)
                return 0f;
            return SpawnCondition.OverworldDaySlime.Chance * 0.2f;
        }
        public override void FindFrame(int frameHeight)
        {
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 40f || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 7.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.frame.Y < frameHeight * 9)
                    NPC.frame.Y = frameHeight * 9;
                if (NPC.frame.Y > frameHeight * 13)
                    NPC.frame.Y = frameHeight * 9;
            }
            else if (NPC.velocity.Y != 0.0)
                NPC.frame.Y = frameHeight * 8;
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 7.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                    return;
                }
                if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 7)
                        NPC.frame.Y = 0;
                }
            }
        }
        #endregion
    }
}
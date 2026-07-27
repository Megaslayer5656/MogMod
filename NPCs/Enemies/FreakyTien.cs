using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Banners;
using MogMod.NPCs.Global;
using MogMod.Utilities;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace MogMod.NPCs.Enemies
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    #region Worm Head
    internal class FreakyTienHead : WormHead
    {
        #region Setup
        public const int TotalHealth = 180;
        public override int BodyType => ModContent.NPCType<FreakyTienBody>();
        public override int TailType => ModContent.NPCType<FreakyTienTail>();
        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "MogMod/NPCs/Enemies/FreakyTien_Bestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetDefaults()
        {
            // Head is 10 defense, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.aiStyle = -1;
            NPC.lifeMax = TotalHealth;
            NPC.damage = 34;

            Banner = Type;
            // These lines are only needed in the main body part.
            BannerItem = ModContent.ItemType<FreakyTienBanner>();
            //ItemID.Sets.KillsToBanner[BannerItem] = 25; // Custom kill count required for banner drop and bestiary unlock. Omit this line for the default 50 kill count.
        }
        #endregion

        #region Bestiary, Spawning, & Loot
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange([
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.MogMod.Bestiary.FreakyTien")
            ]);
        }
        // We would like this npc to spawn below the surface.
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.025f;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VitalityBooster>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<PointBooster>(), 2, 1, 1));
        }
        #endregion

        #region AI
        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 16;
            MaxSegmentLength = 20;

            CommonWormInit(this);
        }
        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        internal static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 10f;
            worm.Acceleration = 0.1f;
        }
        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (attackCounter > 0)
                {
                    attackCounter--; // tick down the attack counter.
                }

                Player target = Main.player[NPC.target];
                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 500 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 15, ProjectileID.WebSpit, 20, 0, Main.myPlayer);
                    Main.projectile[projectile].timeLeft = 300;
                    attackCounter = 500;
                    NPC.netUpdate = true;
                }
            }
        }
        #endregion
    }
    #endregion

    #region Worm Body
    internal class FreakyTienBody : WormBody
    {
        public override LocalizedText DisplayName => MiscUtils.GetText("NPCs.FreakyTienHead.DisplayName");
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.RespawnEnemyID[Type] = ModContent.NPCType<FreakyTienHead>();
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;
            NPC.lifeMax = FreakyTienHead.TotalHealth;
            NPC.damage = 38;
            NPC.dontCountMe = true;

            // Extra body parts should use the same Banner value as the main ModNPC.
            Banner = ModContent.NPCType<FreakyTienHead>();
        }
        public override void Init()
        {
            FreakyTienHead.CommonWormInit(this);
        }
    }
    #endregion

    #region Worm Tail
    internal class FreakyTienTail : WormTail
    {
        public override LocalizedText DisplayName => MiscUtils.GetText("NPCs.FreakyTienHead.DisplayName");
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.RespawnEnemyID[Type] = ModContent.NPCType<FreakyTienHead>();
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.aiStyle = -1;
            NPC.lifeMax = FreakyTienHead.TotalHealth;
            NPC.damage = 34;
            NPC.dontCountMe = true;

            // Extra body parts should use the same Banner value as the main ModNPC.
            Banner = ModContent.NPCType<FreakyTienHead>();
        }
        public override void Init()
        {
            FreakyTienHead.CommonWormInit(this);
        }
    }
    #endregion
}
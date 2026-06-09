using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Accessories;
using MogMod.Items.Ammo;
using MogMod.Items.Consumables;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class Prapor : ModNPC
    {
        #region Setup
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 20;
            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Love)
                .SetNPCAffection(ModContent.NPCType<Mendez>(), AffectionLevel.Like)
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
            ;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 100;
            NPC.height = 85;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.defense = 271;
            NPC.lifeMax = 67;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.scale = .5f;
            AnimationType = NPCID.ArmsDealer;
        }
        public override bool CanGoToStatue(bool toKingStatue) => true;
        #endregion

        #region Attacking
        public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
        {
            int itemType = ModContent.ItemType<Mosin>();
            Main.GetItemDrawFrame(itemType, out item, out itemFrame);
            scale = .5f;
            horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 15;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<MosinLPSProj>();
            attackDelay = 1;
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 35;
            knockback = 10f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 150;
            randExtraCooldown = 1;
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 20f;
        }
        #endregion

        #region Name && Spawning
        public override bool CanTownNPCSpawn(int numTownNPCs) => NPC.downedBoss2;
        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                 "Pavel Yegorovich Romanenko",
            };
        }
        #endregion

        #region Chatting
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
                //.Add((ModContent.ItemType<Switch>()), Condition.DownedGolem) // post von
                .Add(ModContent.ItemType<GreenTracerAmmo>(), Condition.DownedEowOrBoc)
                .Register();
        }
        public override string GetChat()
        {
            NPC.FindFirstNPC(ModContent.NPCType<Mendez>());
            switch (Main.rand.Next(8))
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
                case 6:
                    return "I heard tale of a fancy ore round the edge of the dungeon, why don't ya take a peep over?";
                case 7:
                    return "Be wary of any Scavs' near the outer ends of this land, they're armed n' dangerous.";
                default:
                    return "I sent my dogs to look for your stuff.";
            }
        }
        #endregion
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.IllegalGunParts, 1, false, 0, false, false);
        }
    }
}

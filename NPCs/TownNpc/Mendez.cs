using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Accessories;
using MogMod.Items.Armor.Other;
using MogMod.Items.Other;
using MogMod.Items.Placeable.MusicBoxes;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using MogMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.NPCs.TownNpc
{
    [AutoloadHead]
    public class Mendez : ModNPC
    {
        #region Setup
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.AttackType[Type] = 1; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
            NPCID.Sets.AttackTime[Type] = 60; // The amount of time it takes for the NPC's attack animation to be over once it starts. Measured in ticks.
            NPCID.Sets.AttackAverageChance[Type] = 2; // lower numbers are more aggresive
            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Love)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love)
                .SetNPCAffection(ModContent.NPCType<Prapor>(), AffectionLevel.Love)
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Dislike)
                //.SetNPCAffection(NPCID., AffectionLevel.Hate)
            ;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;

            NPC.width = 35;
            NPC.height = 62;

            NPC.lifeMax = 67;
            NPC.defense = 80085;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.ArmsDealer;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.HitSound = SoundID.NPCDeath1;
        }
        public override bool CanGoToStatue(bool toKingStatue) => true;
        #endregion

        #region Attacking
        public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
        {
            int itemType = ModContent.ItemType<Switch>();
            Main.GetItemDrawFrame(itemType, out item, out itemFrame);
            scale = .5f;
            horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 10;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<GreenTracerProj>();
            attackDelay = 1;

            // attack delay must be set to a higher number than the previous one so that the inBetweenShots bool works properly
            for (int i = 0; i < 18; i++)
                if (NPC.localAI[3] > attackDelay)
                    attackDelay += i;
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 2f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 90;
            randExtraCooldown = 1;
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 16f;
        }
        public override void TownNPCAttackShoot(ref bool inBetweenShots)
        {
            if (NPC.localAI[3] > 1)
                inBetweenShots = true;
        }
        #endregion

        #region Name && Spawning
        public override void AI()
        {
            if (!MogModWorld.spawnedMendez)
                MogModWorld.spawnedMendez = true;
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (MogModWorld.spawnedMendez)
                return true;
            foreach (Player player in Main.ActivePlayers)
            {
                bool tarkov = player.InventoryHas(ModContent.ItemType<LedX>()) || player.PortableStorageHas(ModContent.ItemType<LedX>());
                if (tarkov)
                    return true;
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
        #endregion

        #region Chatting
        public override bool CanChat() => true;
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
            else
            {
                Main.npcChatText = "may Tigz b with you";
                Player player = Main.LocalPlayer;
                player.AddBuff(ModContent.BuffType<GlueBuff>(), 60);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, player.Center);
            }
        }
        public override void AddShops()
        {
            NPCShop shop = new(Type);
            shop.Add<BizarreMusicBox>(Condition.Hardmode, Condition.InGraveyard)
                .Add<DesperateMusicBox>(Condition.BloodMoonOrHardmode)
                .Add<RajangMusicBox>(Condition.Hardmode, Condition.InJungle)
                .Add<RideTheFireMusicBox>(Condition.Hardmode, Condition.InUnderworld)
                .Add<KingVonMusicBox>(Condition.Hardmode)
                .Add<VonEvilIncarnateMusicBox>(Condition.Hardmode, Condition.NightOrEclipse)
                .Add<LedX>()
                .Add(ItemID.ChlorophyteShotbow, Condition.DownedMechBossAll)
                .Add<Phasma>()
                .AddWithCustomValue<PleaseStopMe>(Item.buyPrice(gold: 17, silver: 50), Condition.DownedEyeOfCthulhu, Condition.EclipseOrBloodMoon)
                .Add(ModContent.ItemType<EyeOfMendez>(), Condition.PlayerCarriesItem(ModContent.ItemType<RedX>()))
                .Register();
        }
        public override string GetChat()
        {
            NPC.FindFirstNPC(ModContent.NPCType<Mendez>());
            if (Main.bloodMoon)
                return "tigz fav streamer. we heart tigz. tigz is are lord and savyor";
            else if (!Main.dayTime)
            {
                switch (Main.rand.Next(7))
                {
                    case 0:
                        return "I.. found... these... in... a... chest~";
                    case 1:
                        return "O na-a-a~";
                    case 2:
                        return "tarkov.";
                    case 3:
                        return "o na sleepy rizzler time.";
                    case 4:
                        return "sleepy-o rizzler.";
                    case 5:
                        return "su ban su ban suban o na.";
                    default:
                        return "tis be' to the o na.";
                }
            }
            else
            {
                switch (Main.rand.Next(8))
                {
                    case 0:
                        return "I found these in a chest.";
                    case 1:
                        return "son.";
                    case 2:
                        return "tarkov.";
                    case 3:
                        return "o na ragebait successful.";
                    case 4:
                        return "liked by J* Mendih.";
                    case 5:
                        return "what the vud";
                    case 6:
                        return "tigz be to the suban";
                    default:
                        return "ona.";
                }
            }
        }
        #endregion
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ModContent.ItemType<LedX>(), 1, false, 0, false, false);
        }
    }
}
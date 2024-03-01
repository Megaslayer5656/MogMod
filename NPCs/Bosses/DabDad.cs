﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Audio;
using MogMod.Items.Weapons;
using MogMod.Items.Consumables;
namespace MogMod.NPCs.Bosses
{
    public class DabDad : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.aiStyle = NPCID.EyeofCthulhu;
            NPC.width = 130;
            NPC.height = 130;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.npcSlots = 10f;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Desperate");
            }
        }
        public override void AI()
        {
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {   
        }
    }
 }
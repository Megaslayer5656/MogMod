using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using MogMod.Buffs;
using MogMod.Common.Player;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace MogMod.Items.Weapons.Melee
{
    public class GoldenHydrakanLatch : ModItem
    {
        public override void SetDefaults()
        {
            //TODO: Nerf and possibly change projectile
            Item.width = 68;
            Item.height = 90;
            Item.scale = .65f;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useTurn = false;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileID.IchorArrow;
            Item.shootSpeed = 3.5f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<EssenceShift>(), 600);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.essenceShiftLevel += 1;
            }
        }
    }
}

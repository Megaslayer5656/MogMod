using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using MogMod.Buffs;
using MogMod.Projectiles;
using Terraria.Audio;
using MogMod.Utilities;
using Mono.Cecil;

namespace MogMod.Items.Weapons.Melee
{
    public class ThunderSeal : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 99;
            Item.height = 100;
            Item.damage = 58;
            Item.scale = .5f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 65, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = 0;
            Item.UseSound = SoundID.Item1;
            if (player.altFunctionUse == 2)
            {
                //Somehow make it not melee
                Item.useTime = 60;
                Item.useAnimation = 60;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shoot = ModContent.ProjectileType<StunEdge>();
                Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/StunEdge")
                {
                    Volume = .67f,
                    PitchVariance = .02f,
                };
            } else
            {
                Item.useTime = 16;
                Item.useAnimation = 16;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shoot = 0;
                Item.UseSound = SoundID.Item1;
            }
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<ShockState>())
            {
                //TODO: Eventually make it a lightning strike instead of a dust particle attack
                //TODO: Add different lightning sound on hit
                var source = target.GetSource_FromAI();
                for (int x = 0; x < 4; x++)
                {
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ThunderSealProj>(), Convert.ToInt32(Item.damage * 1.1), 0f, player.whoAmI, false, 0f);
                }
                target.DelBuff(target.FindBuffIndex(ModContent.BuffType<ShockState>()));
                }
            }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = 20;
        }
    }
}

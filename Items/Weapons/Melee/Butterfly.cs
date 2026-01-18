using MogMod.Buffs.Cooldowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Butterfly : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 200;
            Item.height = 210;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useTurn = false;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Cyan;
            Item.scale = .25f;
            Item.noMelee = false;
            Item.noUseGraphic = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ButterflyCooldown>())
            {
                player.AddBuff(BuffID.ShadowDodge, 600);
                player.AddBuff(ModContent.BuffType<ButterflyCooldown>(), 600);
                return true;
            }
            return false;
        }

        public override void PostUpdate()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.noUseGraphic = false;
        }
    }
}

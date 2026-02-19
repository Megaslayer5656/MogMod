using MogMod.Buffs.Debuffs;
using MogMod.NPCs.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Reduvia : ModItem
    {
        public int bloodDamage = 55;
        public override void SetDefaults() //TODO: Make this look better and add vfx
        {
            Item.width = 94;
            Item.height = 97;
            Item.damage = 13;
            Item.scale = .75f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Bleed1>(), 6000000);
            NPC modNPC = target.GetGlobalNPC<MogModGlobalNPC>();
        }

        //TODO: Add a recipe, and possibly a projectile
    }
}

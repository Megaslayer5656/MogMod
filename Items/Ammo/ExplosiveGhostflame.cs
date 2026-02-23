using MogMod.Projectiles.MagicProjectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class ExplosiveGhostflame : ModItem
    {
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = 36;

            Item.damage = 88;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<CannonOfHaimaBoom>();
            Item.shootSpeed = 7f;
            Item.ammo = ModContent.ItemType<GlintstonePebble>(); // so it can be used by the glintstone staff;
        }

        // replaces the "Ammo" description with "Sorcery";
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Ammo" && x.Mod == "Terraria");
            if (changedLine != null)
            {
                changedLine.Text = "Sorcery";
            }
        }
    }
}

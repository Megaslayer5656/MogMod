using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Accessories;
using MogMod.Items.Weapons.Magic;
using MogMod.NPCs.Global;
using MogMod.Projectiles.MagicProjectiles;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Reduvia : ModItem //Very important note: All of the blood stuff is set up in MogGlobalItem.cs, ModGlobalProjectile.cs, and MogModGlobalNPC.cs.
    {
        int shotCounter = 0;
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 97;
            Item.damage = 35;
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
            Item.shoot = ModContent.ProjectileType<BloodMagicProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool CanShoot(Player player)
        {
            if (shotCounter == 2)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override bool? UseItem(Player player)
        {
            shotCounter++;
            if (shotCounter > 2)
            {
                shotCounter = 0;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
              AddIngredient<BloodMagic>().
              AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 15).
              AddIngredient(ItemID.Bone, 10).
              AddTile(TileID.Anvils).
              Register();
        }
    }
}

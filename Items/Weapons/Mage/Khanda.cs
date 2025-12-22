using System.Collections.Generic;
using System.Linq;
using MogMod.Projectiles;
using MogMod.Items.Other;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Weapons.Melee;

namespace MogMod.Items.Weapons.Mage
{
    public class Khanda : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 46;
            Item.damage = 27;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<KhandaBeam>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Purple;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Crystalys>(1).
                AddIngredient<Phylactery>(1).
                AddRecipeGroup("CobaltBar", 12).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
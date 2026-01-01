using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Accessories;
using MogMod.Items.Other;
using MogMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class Phylactery : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 13;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 0f;
            Item.mana = 3;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhylacteryBeam>();
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Pink;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 6;

        public override bool CanUseItem(Player player)
        {
                Item.UseSound = SoundID.Item15;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTurn = false;
                Item.autoReuse = false;
                Item.noMelee = true;
                Item.channel = true;
            return base.CanUseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Diadem>(1).
                AddIngredient(ItemID.LargeAmethyst, 1).
                AddIngredient(ItemID.ShimmerBlock, 20).
                AddIngredient(ItemID.PinkGel, 18).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
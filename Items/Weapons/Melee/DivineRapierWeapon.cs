using MogMod.Items.Accessories;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class DivineRapierWeapon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 112;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Master;
            Item.shoot = ModContent.ProjectileType<DivineRapierProj>();
            Item.shootSpeed = 12f;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 96;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NorthPole, 1).
                AddIngredient<AghanimShard>(1).
                AddIngredient<DivineRapier>(1).
                AddIngredient(ItemID.LunarBar, 15).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
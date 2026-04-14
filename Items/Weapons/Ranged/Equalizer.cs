using Microsoft.Xna.Framework;
using MogMod.Items.Ammo;
using MogMod.Items.Global;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.RangedProjectiles;
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

namespace MogMod.Items.Weapons.Ranged
{
    public class Equalizer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 18;
            Item.damage = 103; //Might need to adjust this
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.UseSound = SoundID.Item91;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.ArmorPenetration = 15;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EnergyBulletProj>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, .5f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentVortex, 12).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}

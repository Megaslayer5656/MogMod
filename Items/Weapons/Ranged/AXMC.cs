using Microsoft.Xna.Framework;
using MogMod.Projectiles.RangedProjectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class AXMC : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 1150; //Damage of ap lapua in tarkov * 10
            Item.DamageType = DamageClass.Ranged;
            Item.width = 193;
            Item.height = 37;
            Item.scale = .5f;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10f;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/AXMCShot")
            {
                Volume = 2.25f,
                PitchVariance = .02f,
            };
            Item.value = Item.buyPrice(0, 75, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 200f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 71;
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25f, -.5f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<APLapua>();
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.scope = true;
                return true;
            }
            return base.AltFunctionUse(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Mosin>(1). // change to somehting else maybe
                AddIngredient(ItemID.SniperRifle, 1).
                AddIngredient(ItemID.VortexBeater, 1).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.LunarCraftingStation). // ancient manipulator
                Register();
        }
    }
}

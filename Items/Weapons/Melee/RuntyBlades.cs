using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RuntyBlades : ModItem, ILocalizedModType
    {
        // code taken from calamity mod sahara slicers
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public bool AltProjectile = true;
        public override void SetDefaults()
        {
            Item.width = Item.height = 36;

            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 12;
            Item.shoot = ModContent.ProjectileType<RuntyBladesProj>();
            Item.shootSpeed = 3.3f;
            Item.knockBack = 4f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            if (AltProjectile)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.7f }, player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RuntyBladesAltProj>(), damage, knockback, player.whoAmI);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                Projectile.NewProjectile(source, position, velocity * 0.75f, type, damage, knockback, player.whoAmI);
            }
            AltProjectile = !AltProjectile;
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RuntyBar>(16).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
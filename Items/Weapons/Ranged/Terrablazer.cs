using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    // elf melter + sinister spreader
    // holdout "laser" flamethrower
    // 62x24
    public class Terrablazer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static int AmmoSavedPercent = 33;
        public static Color MainColor1 = new(247, 255, 120);
        public static Color MainColor2 = new(89, 255, 71);
        public static Color MainColor3 = new(71, 255, 236);
        public static int ArmorPenetration = 25;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent, ArmorPenetration);
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 24;

            Item.damage = 55;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;

            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useAmmo = AmmoID.Gel;
            Item.shoot = ModContent.ProjectileType<TerrablazerHoldout>();
            Item.shootSpeed = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
        }
        public override bool RangedPrefix() => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] > 0 && Main.rand.Next(100) >= AmmoSavedPercent;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            holdout.velocity = (player.MogMod().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ElfMelter).
                AddIngredient<SinisterSpreader>().
                AddIngredient<SoulFragment>(3).
                AddIngredient<BrokenHeroGun>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
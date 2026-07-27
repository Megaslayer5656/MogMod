using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic //Very important note: All of the blood stuff is set up in MogGlobalItem.cs, MogModGlobalProjectileBleed.cs, and MogModGlobalNPC.cs.
{
    public class BloodMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int BloodDamage = 17;
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useTime = Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodMagicProjectile>();
            Item.shootSpeed = 10f;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.BloodMagic").ToNetworkText(player.name)), 3, -player.direction, false, false, -1, false, 9999, 0, 0);
            player.immune = false;
            player.immuneTime = 0;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
              AddIngredient(ItemID.Book, 1).
              AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 15).
              AddIngredient(ItemID.Deathweed, 5).
              AddTile(TileID.Bookcases).
              Register();
        }
    }
}

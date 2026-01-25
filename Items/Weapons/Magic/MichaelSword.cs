using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class MichaelSword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int SwingTime = 74;
        public const float TrailOffsetCompletionRatio = 0.2f;
        public const float ExplosionExpandFactor = 1.013f;
        public static readonly Color SwordColor1 = new(50, 100, 255); // light blue
        public static readonly Color SwordColor2 = new(50, 50, 210); // dark blue
        public static readonly SoundStyle SwingSound = SoundID.Item94;
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 92;
            Item.damage = 105;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.mana = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<MichaelSwordHoldout>();
            Item.shootSpeed = 60f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChargedBlasterCannon, 1).
                AddIngredient(ItemID.FireFeather, 1).
                AddIngredient(ItemID.IceFeather, 1).
                AddIngredient(ItemID.BrokenHeroSword, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
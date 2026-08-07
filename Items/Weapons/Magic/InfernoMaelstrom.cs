using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class InfernoMaelstrom : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.damage = 58;
            Item.mana = 8;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 20;
            Item.UseSound = SoundID.Item20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 5f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<InfernoMHoldout>();
            Item.shootSpeed = 5f;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void HoldItem(Player player) => player.MogMod().mouseWorldListener = true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<HellfireBar>(12).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
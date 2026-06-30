using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Other
{
    public class Phasma : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override string Texture => "Terraria/Images/Item_" + ItemID.Phantasm;
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<VonWarning>();
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;

            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2;
            Item.shootSpeed = 20f;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;

            Item.useTime = Item.useAnimation = 12;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Arrow;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item5;

            Item.noMelee = true;
            Item.autoReuse = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-2.25f, -0.05f);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
    }
}
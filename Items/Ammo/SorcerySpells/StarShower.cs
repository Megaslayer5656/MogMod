using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class StarShower : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 46;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
            Item.shoot = ModContent.ProjectileType<StarShowerProj>();
            Item.shootSpeed = 6f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GlintstoneStars>(1).
                AddRecipeGroup("CobaltBar", 14).
                AddIngredient(ItemID.SoulofSight, 7).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
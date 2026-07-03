using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class CannonOfHaima : SorcerySpell
    {
        public override int ManaCost => 32;
        public override int AttackSpeed => 60;
        public override SoundStyle UseSound => SoundID.Item84;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 12f;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
            Item.shoot = ModContent.ProjectileType<CannonOfHaimaProj>();
            Item.shootSpeed = 6f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofMight, 7).
                AddIngredient<UltimateOrb>(3).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}

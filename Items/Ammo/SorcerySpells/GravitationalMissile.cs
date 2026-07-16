using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class GravitationalMissile : SorcerySpell
    {
        public override int ManaCost => 26;
        public override int AttackSpeed => 44;
        public override SoundStyle UseSound => SoundID.Item92;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 82;
            Item.knockBack = 10f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.shoot = ModContent.ProjectileType<GravitationalMissileProj>();
            Item.shootSpeed = 1.5f;
            SorceryClass = SorceryID.Gravity;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShroomiteBar, 8).
                AddIngredient<FaeBar>(5).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
        public override bool? UseItem(Player player)
        {
            foreach (Projectile proj in Main.projectile)
                if (proj.active && proj.owner == Main.myPlayer && proj.type == Item.shoot)
                    proj.Kill();
            return true;
        }
    }
}
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class Starlight : SorcerySpell
    {
        public override int ManaCost => 4;
        public override int AttackSpeed => 20;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = -1;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
            Item.shoot = ModContent.ProjectileType<StarlightProj>();
            Item.shootSpeed = 0f;
            SorceryClass = SorceryID.Glintstone;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FallenStar).
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
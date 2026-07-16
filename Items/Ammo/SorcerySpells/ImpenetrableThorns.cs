using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class ImpenetrableThorns : SorcerySpell
    {
        public override int ManaCost => 12;
        public override int AttackSpeed => 44;
        public override int SpellSelfHurtDamage => 8;
        public override PlayerDeathReason SpellDeathReason => PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.BriarsOfPunishment").ToNetworkText(Main.player[Owner].name));
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 16;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.shoot = ModContent.ProjectileType<ImpenetrableThornsProj>();
            Item.shootSpeed = 20f;
            SorceryClass = SorceryID.Thorn;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpookyEssence>(8).
                AddIngredient<LizhardBloodVial>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float Spread = MathHelper.PiOver4 / 4f;
            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Spread * i) * 1.5f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
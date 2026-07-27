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

namespace MogMod.Items.Ammo.SorcerySpells.Thorn
{
    public class BriarsOfPunishment : SorcerySpell
    {
        public override int ManaCost => 8;
        public override int AttackSpeed => 40;
        public override int SpellSelfHurtDamage => 5;
        public override PlayerDeathReason SpellDeathReason => PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.BriarsOfPunishment").ToNetworkText(Main.player[Owner].name));
        public override SoundStyle UseSound => SoundID.Item8;
        public const int BloodDamage = 32;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 40;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<BriarsOfPunishmentProj>();
            Item.shootSpeed = 30f;
            SorceryClass = SorceryID.Thorn;

            MogGlobalItem mogItem = Item.MogMod();
            mogItem.visualBloodDamage = BloodDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodenSpike, 25).
                AddIngredient(ItemID.ChlorophyteBar, 8).
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float Spread = MathHelper.PiOver2 - MathHelper.PiOver4;
            for (int i = -4; i < 4; i++)
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Spread * i) * 2f, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class HellfirePickaxe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = Item.height = 38;

            Item.damage = 26;
            Item.knockBack = 5;
            Item.DamageType = DamageClass.Melee;

            // On the official wiki, https://terraria.wiki.gg/wiki/Pickaxes, the "Use time" column corresponds to Item.useAnimation and the "Mining speed" column corresponds to Item.useTime.
            Item.useTime = 6;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.pick = 200; // How strong the pickaxe is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; // melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? DustID.Lava : 174);
        }
        public override void AddRecipes() // hallowed tier
        {
            CreateRecipe().
               AddIngredient<GriefBar>(10).
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}

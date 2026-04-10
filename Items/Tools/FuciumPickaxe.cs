using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class FuciumPickaxe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = Item.height = 36;

            Item.damage = 16;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;

            // On the official wiki, https://terraria.wiki.gg/wiki/Pickaxes, the "Use time" column corresponds to Item.useAnimation and the "Mining speed" column corresponds to Item.useTime.
            Item.useTime = Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.pick = 100; // How strong the pickaxe is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; // melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.BubbleBurst_Pink);
        }
        public override void AddRecipes() // hellstone tier
        {
            CreateRecipe().
               AddIngredient<FuciumBar>(12).
               AddTile(TileID.Anvils).
               Register();
        }
    }
}

using MogMod.Items.Global;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class RustedHatchet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int NumBounces = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(NumBounces);
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;

            Item.damage = 18;
            Item.knockBack = 1.5f;
            Item.shootSpeed = 10f;
            Item.useTime = Item.useAnimation = 22;
            Item.DamageType = DamageClass.MeleeNoSpeed;

            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<RustedHatchetProj>();

            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = MogGlobalItem.RarityBlueBuyPrice;
        }
    }
}
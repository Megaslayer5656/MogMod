using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class ForceStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public static Color MainColor1 = new(120, 255, 149);
        public static Color MainColor2 = new(61, 255, 199);
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 80;

            Item.mana = 20;
            Item.useTime = Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<ForceStaffHoldout>();
            Item.shootSpeed = 2f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override bool CanReforge() => false;
        public override bool AllowPrefix(int pre) => false;
    }
}
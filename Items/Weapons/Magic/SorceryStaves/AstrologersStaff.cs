using MogMod.Items.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    public class AstrologersStaff : SorceryStaff
    {
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 15;
            Item.width = Item.height = 58;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.1f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.autoReuse = true;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 2;
        public override bool MagicPrefix() => true;
    }
}
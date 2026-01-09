using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using MogMod.Common.Classes;

namespace MogMod.Items.Consumables
{
    public class BoulderAmmo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = ModContent.GetInstance<BoulderClass>();
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.Boulder;
            Item.shootSpeed = 4.5f;
            Item.ammo = Item.type;
        }
    }
}

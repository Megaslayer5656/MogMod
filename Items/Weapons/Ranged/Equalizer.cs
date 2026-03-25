using MogMod.Items.Ammo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Items.Weapons.Ranged
{
    public class Equalizer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 18;
            Item.damage = 120;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item91;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.ArmorPenetration = 15;
            Item.useAmmo = ModContent.ItemType<EnergyBullet>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, .5f);
        }
    }
}

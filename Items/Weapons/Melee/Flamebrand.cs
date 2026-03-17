using MogMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace MogMod.Items.Weapons.Melee
{
    public class Flamebrand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Frostbrand;
        }

        public override void SetDefaults() 
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 23;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            //Item.value = 
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.BallofFire;
            Item.shootSpeed = 12f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Main.projectile[type].friendly = true;
            Main.projectile[type].hostile = false;
            Main.projectile[type].owner = player.whoAmI;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), 2f, 1f, 1f);

            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch);
            }
        }
    }
}

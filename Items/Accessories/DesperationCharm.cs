using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Accessories
{
    public class DesperationCharm : ModItem
    {
        public static readonly int AdditiveDamageBonus = 100;
        public static readonly int AttackSpeedBonus = 25;
        public override void SetDefaults() 
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = 8;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < player.statLifeMax2 * 0.25f)
                {
                player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
                player.GetAttackSpeed(DamageClass.Generic) += AttackSpeedBonus / 100f;
            }
        }
    }
}
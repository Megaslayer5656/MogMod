using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SangeAndYasha : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += .24f;
            player.statLifeMax2 += 50;
            player.lifeRegen += 6;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.accRunSpeed += player.accRunSpeed * .24f;
            player.lifeSteal *= 1.5f;

            // ankh shield immunity
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Stoned] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>(1).
                AddIngredient<Yasha>(1).
                AddIngredient(ItemID.AnkhShield, 1).
                AddIngredient<GriefBar>(7).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}

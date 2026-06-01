using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class EyeOfSkadi : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 25;
            player.GetDamage(DamageClass.Generic) += .10f;
            player.GetCritChance(DamageClass.Generic) += 10f;

            // not in ankh shield
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.Daybreak] = true;

            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingEyeOfSkadi = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EyeoftheGolem, 1).
                AddIngredient(ItemID.ShroomiteBar, 12).
                AddIngredient(ItemID.BlackLens, 1).
                AddIngredient<ScorchedCore>(1).
                AddIngredient<PointBooster>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}

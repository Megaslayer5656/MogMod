using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Items.Accessories
{
    public class TritonM43A : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public bool HidesNormalTooltip => true;
        int CurrentStats = 0;
        // makes localization way easier to modify
        // life
        public const int LifeAndManaBoost = 20;
        public const int LifeRegen = 2;
        public const int ManaRegen = 2;
        public const int ReducedDoTAmount = 6;
        // damage
        public const int CritBoost = 4;
        public const float SizeMult = 0.1f;
        public const float VelocityMult = 0.05f;
        // movement
        public const float MiningSpeed = 0.12f;
        public const float MovementSpeed = 0.12f;
        public const float JumpBoost = 0.6f; // 12%
        public override void SetDefaults()
        {
            Item.width = Item.height = 26;

            Item.accessory = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRigSlot = true;
            mogPlayer.wearingTriton = true;
            switch (CurrentStats)
            {
                case 0:
                    mogPlayer.wearingTritonLife = true;
                    break;
                case 1:
                    mogPlayer.wearingTritonDamage = true;
                    break;
                case 2:
                    mogPlayer.wearingTritonMovement = true;
                    break;
                default:
                    break;
            }
        }
        // shift right clicking changes CurrentStats
        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            CurrentStats++;
            if (CurrentStats > 2)
                CurrentStats = 0;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        // sync item when leaving world
        public override void SaveData(TagCompound tag) => tag.Add("stats", CurrentStats);
        public override void LoadData(TagCompound tag) => CurrentStats = tag.GetInt("stats");
        public override void NetSend(BinaryWriter writer) => writer.Write(CurrentStats);
        public override void NetReceive(BinaryReader reader) => CurrentStats = reader.ReadInt32();
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            var rigs = MogGlobalItem.EnableNeutralItemSlots;
            if (rigs.Contains(incomingItem.type) && rigs.Contains(equippedItem.type))
                return false;
            return true;
        }
        // draw the item in a different color depending on currentstats
        public override Color? GetAlpha(Color lightColor)
        {
            if (Main.zenithWorld)
                return Main.DiscoColor;
            switch (CurrentStats)
            {
                case 0:
                    return Color.OrangeRed;
                case 1:
                    return Color.Purple;
                case 2:
                    return Color.Teal;
                default:
                    break;
            }
            return lightColor;
        }
        // change the tooltip depending on CurrentStats (or in GFB worlds)
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[DESC]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
            TooltipLine gfb = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (Main.zenithWorld)
            {
                if (gfb != null)
                    gfb.OverrideColor = Main.DiscoColor;
                return;
            }
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string stats = string.Empty;
            if (index != -1 && Main.keyState.PressingShift())
                switch (CurrentStats)
                {
                    case 0:
                        index++;
                        TooltipLine life = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<TritonM43A>("TritonLife").Format(
                        LifeAndManaBoost, // {0}
                        LifeRegen.ToRegenPerSecond(), // {1}
                        ManaRegen.ToRegenPerSecond(), // {2}
                        ReducedDoTAmount.ToRegenPerSecond())); // {3}
                        life.OverrideColor = Color.OrangeRed;
                        tooltips.Insert(index, life);
                        break;
                    case 1:
                        index++;
                        TooltipLine damage = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<TritonM43A>("TritonDamage").Format(
                        CritBoost, // {0}
                        VelocityMult.ToPercent(), // {1}
                        SizeMult.ToPercent())); // {2}
                        damage.OverrideColor = Color.Purple;
                        tooltips.Insert(index, damage);
                        break;
                    case 2:
                        index++;
                        TooltipLine movement = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<TritonM43A>("TritonMovement").Format(
                        MovementSpeed.ToPercent(), // {0}
                        (JumpBoost / 5).ToPercent(), // {1}
                        MiningSpeed.ToPercent())); // {2}
                        movement.OverrideColor = Color.Teal;
                        tooltips.Insert(index, movement);
                        break;
                    default:
                        break;
                }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(8).
                AddIngredient<ManaEssence>(3).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Chest"}").
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
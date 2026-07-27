using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Items.Accessories.Boots
{
    public class PowerTreads : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public bool HidesNormalTooltip => true;
        public static int CurrentStats = 0;
        // makes localization way easier to modify
        public const float Acceleration = 8.5f;
        public const float MovementSpeed = 0.2f;
        // life
        public const int LifeBoost = 20;
        public const int LifeRegen = 4;
        public const int ReducedDoTAmount = 8;
        // damage
        public const int CritBoost = 4;
        public const float SizeMult = 0.1f;
        public const float VelocityMult = 0.07f;
        // building
        public const float MiningSpeed = 0.15f;
        public const float PlacementSpeed = 0.25f;
        public const double FlightTimeBoost = 0.2D;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;

            Item.accessory = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingPowerTreads = true;

            player.accRunSpeed = Acceleration;
            player.moveSpeed += MovementSpeed;
            player.rocketBoots = player.vanityRocketBoots = 2;
            if (!hideVisual)
                player.CancelAllBootRunVisualEffects();

            switch (CurrentStats)
            {
                case 0:
                    mogPlayer.wearingTreadsLife = true;
                    break;
                case 1:
                    mogPlayer.wearingTreadsDamage = true;
                    break;
                case 2:
                    mogPlayer.wearingTreadsBuilding = true;
                    break;
                default:
                    break;
            }
        }
        public override bool ConsumeItem(Player player) => false;
        // sync item when leaving world
        public override void SaveData(TagCompound tag) => tag.Add("stats", CurrentStats);
        public override void LoadData(TagCompound tag) => CurrentStats = tag.GetInt("stats");
        public override void NetSend(BinaryWriter writer) => writer.Write(CurrentStats);
        public override void NetReceive(BinaryReader reader) => CurrentStats = reader.ReadInt32();
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
            var Hotkey = KeybindSystem.BootsKeybind.TooltipHotkeyString();
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string stats = string.Empty;
            if (index != -1)
            {
                if (Main.keyState.PressingShift())
                {
                    switch (CurrentStats)
                    {
                        case 0:
                            index++;
                            TooltipLine life = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<PowerTreads>("TreadsLife").Format(
                            LifeBoost, // {0}
                            LifeRegen.ToRegenPerSecond(), // {1}
                            ReducedDoTAmount.ToRegenPerSecond())); // {2}
                            life.OverrideColor = Color.OrangeRed;
                            tooltips.Insert(index, life);
                            break;
                        case 1:
                            index++;
                            TooltipLine damage = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<PowerTreads>("TreadsDamage").Format(
                            CritBoost, // {0}
                            VelocityMult.ToPercent(), // {1}
                            SizeMult.ToPercent())); // {2}
                            damage.OverrideColor = Color.Purple;
                            tooltips.Insert(index, damage);
                            break;
                        case 2:
                            index++;
                            TooltipLine movement = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<PowerTreads>("TreadsBuilding").Format(
                            MiningSpeed.ToPercent(), // {0}
                            PlacementSpeed.ToPercent(), // {1}
                            FlightTimeBoost.ToPercent())); // {2}
                            movement.OverrideColor = Color.Teal;
                            tooltips.Insert(index, movement);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (Main.zenithWorld)
                    {
                        index++;
                        TooltipLine gfb = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<PowerTreads>("TooltipGFB").Format());
                        gfb.OverrideColor = Main.DiscoColor;
                        tooltips.Insert(index, gfb);
                    }
                    else
                    {
                        index++;
                        TooltipLine normal = new(Mod, "Tooltip0", MiscUtils.GetTextFromModItem<PowerTreads>("TooltipNormal").Format(
                        Hotkey)); // {0}
                        tooltips.Insert(index, normal);

                        //index++;
                        //TooltipLine holdShiftIndicator = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<PowerTreads>("TooltipNormal").Format(
                        //Hotkey)); // {0}
                        //holdShiftIndicator.OverrideColor = IHoldShiftTooltipItem.DefaultExtensionIndicatorColor;
                        //tooltips.Insert(index, holdShiftIndicator);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RocketBoots).
                AddIngredient(ItemID.Leather, 12).
                AddIngredient<FuciumBar>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
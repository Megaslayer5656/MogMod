using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.Classes;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Accessories;
using MogMod.Items.Accessories.Boots;
using MogMod.Items.Accessories.NeutralItems;
using MogMod.Items.Accessories.NeutralItems.Aspects;
using MogMod.Items.Accessories.Rigs;
using MogMod.Items.Ammo.SorcerySpells.Glintstone;
using MogMod.Items.Armor.Damascus;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Classless;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Rarities;
using MogMod.Utilities;
using Mono.Cecil;
using StructureHelper.Content.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Global
{
    public partial class MogGlobalItem : GlobalItem
    {
        public int cooldownTimer = 5;
        public int shotCounter = 0;
        public bool ultraCrit = false;
        public static List<int> ChestRigAccessories =
        [
            ModContent.ItemType<IdeaRig>(),
            ModContent.ItemType<TritonM43A>(),
            ModContent.ItemType<AzimutSSZhuk>(),
            ModContent.ItemType<OspreyMK4A>(),
        ];
        // makes melee weapons size bigger when wearing certain accessories
        public static List<int> MeleeSizeAlwaysAffects =
        [
            ItemID.TerraBlade,
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.PiercingStarlight,
            ItemID.TheHorsemansBlade,
            ItemID.LucyTheAxe,
            ModContent.ItemType<Gunlance>(), // will remove soon
            ModContent.ItemType<BlackBlade>(), // will remove soon
            ModContent.ItemType<OversizedAnchor>(), // will remove soon
            ItemID.TheAxe
        ];
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.SnowBlock] = ItemID.ShimmerBlock;
            ItemID.Sets.ShimmerTransformToItem[ItemID.WizardHat] = ModContent.ItemType<GlintstoneArc>();
            ItemID.Sets.ShimmerTransformToItem[ItemID.SparkleGuitar] = ModContent.ItemType<Polylute>();
            ItemID.Sets.ShimmerTransformToItem[ItemID.Frostbrand] = ModContent.ItemType<Flamebrand>();
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Flamebrand>()] = ItemID.Frostbrand; //So this one doesn't typically work because it can only be obtained by shimmering a frostbrand, and items can only be shimmered once ever according to terraria, so you can't shimmer this into a frostbrand unless you cheat it in and then shimmer it, but I'm leaving this in bc it's funny.
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<ATGMissile>()] = ModContent.ItemType<PlasmaShrimp>();
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<PlasmaShrimp>()] = ModContent.ItemType<ATGMissile>();
        }
        public override void SetDefaults(Item entity)
        {
            BloodDefaults(entity);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();

            // Feral Claws line melee speed adjustments and nonstacking
            // First removes all their melee speed so it can be given based on which you wear without stacking
            if (item.type == ItemID.FeralClaws)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Feral Claws 10%
                if (mogPlayer.gloveLevel < 1)
                    mogPlayer.gloveLevel = 1;
            }
            if (item.type == ItemID.PowerGlove || item.type == ItemID.BerserkerGlove)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Power/Berserker Glove 12%
                if (mogPlayer.gloveLevel < 2)
                    mogPlayer.gloveLevel = 2;
            }
            if (item.type == ItemID.MechanicalGlove)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Mechanical Glove 12%
                if (mogPlayer.gloveLevel < 3)
                    mogPlayer.gloveLevel = 3;
            }
            if (item.type == ItemID.FireGauntlet)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Fire Gauntlet 14%
                if (mogPlayer.gloveLevel < 4)
                    mogPlayer.gloveLevel = 4;
            }
            if (mogPlayer.wearingAghGauntlet && mogPlayer.gloveLevel < 5) // Agh Gauntlet 15%
                mogPlayer.gloveLevel = 5;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            MogPlayer mogPlayer = player.MogMod();
            if (mogPlayer.wearingChaosDice) ultraCrit = Main.rand.NextBool(ChaosDice.UltraCritChance);
            if (mogPlayer.wearingRigSlot && item.useAmmo == AmmoID.Bullet && !item.channel)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    shotCounter++;
                    //Main.NewText($"bullet counter is {shotCounter}, max shots is {mogPlayer.maxShots}, reuse delay is {item.reuseDelay}");
                    // the ammo remaining in the mag
                    if (shotCounter < mogPlayer.maxShots)
                    {
                        item.reuseDelay = 0;
                        if (MogClientConfig.Instance.AmmoEjection)
                        {
                            string goreType = "RigGunCasing";
                            Gore.NewGore(source, position, -velocity * 0.8f, Mod.Find<ModGore>(goreType).Type);
                        }
                    }
                    else
                    {
                        // the last bullet in the mag
                        if (shotCounter == mogPlayer.maxShots)
                        {
                            item.reuseDelay = mogPlayer.reloadTime;
                            if (MogClientConfig.Instance.AmmoEjection)
                            {
                                string goreType = "RigGunCasing";
                                Gore.NewGore(source, position, -velocity * 0.8f, Mod.Find<ModGore>(goreType).Type);
                            }
                        }
                        // reloading
                        else if (shotCounter > mogPlayer.maxShots)
                        {
                            shotCounter = 0;
                            item.reuseDelay = 0;
                            if (MogClientConfig.Instance.AmmoEjection)
                            {
                                string goreMag = "RigGunMag";
                                Gore.NewGore(source, position, velocity.RotatedBy(2f * -player.direction) * Main.rand.NextFloat(0.45f, 0.55f), Mod.Find<ModGore>(goreMag).Type);
                            }
                            SoundEngine.PlaySound(SoundID.Item149 with { Pitch = -0.1f }, player.Center);
                            SoundEngine.PlaySound(SoundID.Item108 with { Pitch = -0.2f }, player.Center);
                            return false; // its important to return here so that nothing interesting happens when we reload
                        }
                    }
                }
            }
            if ((mogPlayer.wearingNihilumRanged && item.DamageType == DamageClass.Ranged) && mogPlayer.nulledDebuff)
            {
                Projectile nullEssence = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<GreatswordOfSoulsProj>(), (int)(damage * 0.5), knockback, player.whoAmI);
                nullEssence.DamageType = DamageClass.Ranged;
            }
            if (mogPlayer.wearingEnchantedQuiver && item.useAmmo == AmmoID.Arrow && !item.channel)  
            {
                shotCounter++;
                if (shotCounter >= 3)
                {
                    Projectile.NewProjectileDirect(source, position, velocity * 0.5f, ModContent.ProjectileType<EnchantedArrowProj>(), damage * 2, knockback, player.whoAmI);
                    shotCounter = 0;
                }
            }
            return true;
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            MogPlayer mogPlayer = player.MogMod();
            if ((mogPlayer.wearingElvenQuiver || mogPlayer.wearingEnchantedQuiver) && item.useAmmo == AmmoID.Arrow)
                velocity *= mogPlayer.wearingEnchantedQuiver ? EnchantedQuiver.VelocityMult : ElvenQuiver.VelocityMult;
            if (mogPlayer.wearingTreadsDamage)
                velocity *= PowerTreads.VelocityMult + 1;
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // Check if the opened bag is the Eye of Cthulhu Treasure Bag
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<BrinyRind>(), 1, 9, 16));
            }
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) => Main.rand.NextFloat() <= player.MogMod().ammoCost;
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            MogPlayer mogPlayer = player.MogMod();
            if (target.type != NPCID.TargetDummy)
            {
                if (mogPlayer.wearingDamascus2 && hit.Crit && Main.zenithWorld)
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.Damascus").ToNetworkText(player.name)), 5, -player.direction, false, false, -1, false, 9999, 0, 0);
                    player.immune = false;
                    player.immuneTime = 0;
                }
                else if (mogPlayer.wearingDamascus2 && hit.Crit)
                {
                    int heal = 1;
                    player.HealLifestealMult(heal);
                }
                if (mogPlayer.wearingSatanic && player.HasBuff(ModContent.BuffType<SatanicBuff>()) && mogPlayer.satanicAccCooldown <= 0)
                //if (mogPlayer.wearingSatanic) // for testing
                {
                    mogPlayer.satanicAccCooldown = cooldownTimer * 2;
                    int heal = (int)(damageDone / 100) + 1;
                    player.HealLifestealMult(heal);
                }
            }
            if (mogPlayer.wearingChaosDice && ultraCrit && hit.Crit)
            {
                if (target.type != NPCID.TargetDummy)
                    player.HealLifestealMult(1);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.UltraCritTextSync);
                    packet.Write(target.lastInteraction);
                    packet.Write(target.whoAmI);
                    packet.Send();
                }
                else
                {
                    target.MogMod().UltraCritFX(target);
                }
            }
        }
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            MogPlayer mogPlayer = player.MogMod();
            if (mogPlayer.wearingDamascus1 && Main.zenithWorld)
                modifiers.CritDamage *= DamascusHelm.GFBCritMult;
            else if (mogPlayer.wearingDamascus1)
                modifiers.CritDamage *= DamascusHelm.CritMult + 1;
            if (mogPlayer.wearingChaosDice && ultraCrit)
                modifiers.CritDamage *= ChaosDice.CritMult;
        }
        public override bool InstancePerEntity => true;
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            MogPlayer modPlayer = player.MogMod();

            if (!item.IsAir && !item.noMelee || MeleeSizeAlwaysAffects.Contains(item.type))
            {
                if (modPlayer.wearingGiantsMaul)
                    scale *= GiantsMaul.SizeMult + (Main.zenithWorld ? -0.1f : 1);
                if (modPlayer.wearingTreadsDamage)
                    scale *= PowerTreads.SizeMult + 1;
            }
        }

        // taken from calamity mods rarity price system
        // these values should NOT be used for materials, potions, and ammo
        #region Rarity Price Table
        // Base numeric rarity pricing guide.
        private static readonly int Rarity0BuyPrice = Item.buyPrice(0, 0, 50, 0);
        private static readonly int Rarity1BuyPrice = Item.buyPrice(0, 1, 0, 0);
        private static readonly int Rarity2BuyPrice = Item.buyPrice(0, 2, 0, 0);
        private static readonly int Rarity3BuyPrice = Item.buyPrice(0, 5, 0, 0);
        private static readonly int Rarity4BuyPrice = Item.buyPrice(0, 10, 0, 0);
        private static readonly int Rarity5BuyPrice = Item.buyPrice(0, 20, 0, 0);
        private static readonly int Rarity6BuyPrice = Item.buyPrice(0, 35, 0, 0);
        private static readonly int Rarity7BuyPrice = Item.buyPrice(0, 45, 0, 0);
        private static readonly int Rarity8BuyPrice = Item.buyPrice(0, 60, 0, 0);
        private static readonly int Rarity9BuyPrice = Item.buyPrice(0, 80, 0, 0);
        private static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0); // Highest raw rarity used by vanilla items (ML drops)
        private static readonly int Rarity11BuyPrice = Item.buyPrice(1, 20, 0, 0); // End of vanilla rarities
        private static readonly int Rarity12BuyPrice = Item.buyPrice(1, 50, 0, 0); // Von rarity

        private static readonly int[] RarityBuyPriceArray = new int[] {
            Rarity0BuyPrice,
            Rarity1BuyPrice,
            Rarity2BuyPrice,
            Rarity3BuyPrice,
            Rarity4BuyPrice,
            Rarity5BuyPrice,
            Rarity6BuyPrice,
            Rarity7BuyPrice,
            Rarity8BuyPrice,
            Rarity9BuyPrice,
            Rarity10BuyPrice,
            Rarity11BuyPrice,
            Rarity12BuyPrice,
        };

        // Canonical names which are implemented as properties that reference the base numeric rarity prices.
        public static int RarityWhiteBuyPrice => Rarity0BuyPrice;
        public static int RarityBlueBuyPrice => Rarity1BuyPrice;
        public static int RarityGreenBuyPrice => Rarity2BuyPrice;
        public static int RarityOrangeBuyPrice => Rarity3BuyPrice;
        public static int RarityLightRedBuyPrice => Rarity4BuyPrice;
        public static int RarityPinkBuyPrice => Rarity5BuyPrice;
        public static int RarityLightPurpleBuyPrice => Rarity6BuyPrice;
        public static int RarityLimeBuyPrice => Rarity7BuyPrice;
        public static int RarityYellowBuyPrice => Rarity8BuyPrice;
        public static int RarityCyanBuyPrice => Rarity9BuyPrice;
        public static int RarityRedBuyPrice => Rarity10BuyPrice;
        public static int RarityPurpleBuyPrice => Rarity11BuyPrice;
        public static int RarityVonBuyPrice => Rarity12BuyPrice;
        #endregion

        #region Rarity / Price Helper Functions
        public static int GetBuyPrice(int rarity)
        {
            // Vanilla rarities go directly to the array.
            if (rarity >= ItemRarityID.White && rarity <= ItemRarityID.Purple)
                return RarityBuyPriceArray[rarity];

            // modded rarities aren't guaranteed to have the monotonic IDs, so they're handled directly.
            if (rarity == ModContent.RarityType<VonRarity>())
                return RarityVonBuyPrice;

            // Return 0 if it's not a progression based or other mod's rarity
            return 0;
        }

        public static int GetBuyPrice(Item item) => GetBuyPrice(item.rare);
        #endregion

        #region Custom Rarity Colors && Tooltips
        /// <summary>
        /// This array contains (almost) every single vanilla tooltip in reverse order starting at "Tooltip0".<br />
        /// Because "Tooltip0" is the first typical tooltip line, this is where MogMod tends to insert its tooltips.<br />
        /// When this line is not present, MogMod needs to insert tooltips in an <i>equivalent</i> position.<br />
        /// The best way to do this is to iterate backwards through all possible vanilla tooltip lines and pick the first one that is present.
        /// </summary>
        public static string[] MainTooltipBackupInsertionPositions =
        {
            "Material",
            "Consumable",
            "Ammo",
            "Placeable",
            "UseMana",
            "HealMana",
            "HealLife",
            "TileBoost",
            "HammerPower",
            "AxePower",
            "PickPower",
            "Defense",
            "Vanity",
            "Quest",
            "WandConsumes",
            "Equipable",
            "BaitPower",
            "NeedsBait",
            "FishingPower",
            "Knockback",
            "NoTransfer",
            "FavoriteDesc",
            "ItemName",
        };
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            #region Colors
            // Apply rarity coloration to the item's name.
            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
                ApplyRarityColor(item, nameLine);
            #endregion

            ApplyBleedTooltips(item, tooltips);

            #region Hold Shift Tooltips
            // Get the first index, last index and total count of standard vanilla tooltip lines.
            // The first index and count are used to delete all vanilla tooltips when holding SHIFT, if requested.
            // The last index is used to insert various extra tooltip lines in the right position.
            //
            // This code used to be in the HoldShiftTooltip utility, but is needed to correctly place other tooltips.
            int firstTooltipIndex = -1;
            int lastTooltipIndex = -1;
            int standardTooltipCount = 0;
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i]?.Name?.StartsWith("Tooltip") == true)
                {
                    if (firstTooltipIndex == -1)
                        firstTooltipIndex = i;
                    lastTooltipIndex = i;
                    standardTooltipCount++;
                }
            }

            // If there are no standard vanilla tooltip lines (e.g. Flintlock Pistol, which has no tooltip)
            // then a different position needs to be selected for typical insertion.
            bool noStandardTooltips = false;
            if (firstTooltipIndex == -1)
            {
                noStandardTooltips = true;
                foreach (string lineName in MainTooltipBackupInsertionPositions)
                {
                    int idx = tooltips.FindIndex((line) => line.Name == lineName);
                    if (idx != -1)
                    {
                        firstTooltipIndex = lastTooltipIndex = idx;
                        break;
                    }
                }
            }
            // Everything below this line can only apply to modded items. If the item is vanilla, stop here for efficiency.
            if (item.type < ItemID.Count)
                return;
            // Generic mechanical implementation of any and all Hold SHIFT tooltips.
            // For more information, see IHoldShiftTooltipItem.
            // Code taken from Calamity Mod, which was lifted from Iban's extended armor tooltips.
            if (item.ModItem is IHoldShiftTooltipItem holdShiftItem)
            {
                bool holdingShift = Main.keyState.PressingShift();

                // If holding SHIFT, actually display the extended tooltip.
                if (holdingShift && firstTooltipIndex != -1)
                {
                    string holdShiftText = holdShiftItem.TooltipExtensionText == LocalizedText.Empty ? item.ModItem.GetLocalizedValue(holdShiftItem.TooltipExtensionKey) : holdShiftItem.TooltipExtensionText.ToString();
                    TooltipLine holdShiftLine = new TooltipLine(Mod, IHoldShiftTooltipItem.ExtensionTooltipID, holdShiftText);
                    if (holdShiftItem.TooltipExtensionColor is not null)
                        holdShiftLine.OverrideColor = holdShiftItem.TooltipExtensionColor;

                    // If asked to, remove all standard tooltip lines. This moves the last tooltip index.
                    // This only occurs if the standard tooltip lines are ACTUALLY standard tooltips. Otherwise, don't remove anything!
                    if (holdShiftItem.HidesNormalTooltip && !noStandardTooltips)
                    {
                        tooltips.RemoveRange(firstTooltipIndex, standardTooltipCount);
                        lastTooltipIndex -= standardTooltipCount;
                    }

                    // Append the "Hold SHIFT" tooltip at the end of standard tooltips.
                    tooltips.Insert(++lastTooltipIndex, holdShiftLine);
                }

                // If not holding SHIFT, display the extension indicator if appropriate.
                if (!holdingShift && holdShiftItem.ShowExtensionIndicator)
                {
                    LocalizedText indicatorText = MiscUtils.GetText(holdShiftItem.ExtensionIndicatorKey);
                    TooltipLine indicator = new TooltipLine(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, indicatorText.Value);
                    if (holdShiftItem.ExtensionIndicatorColor is not null)
                        indicator.OverrideColor = holdShiftItem.ExtensionIndicatorColor;

                    // Append the extension indicator tooltip at the end of standard tooltips.
                    tooltips.Insert(++lastTooltipIndex, indicator);
                }

                // Generic support for flavor tooltips.
                // This is only necessary on items with Hold SHIFT tooltips.
                // The extended tooltip and tooltip extension indicator are placed above flavor tooltips for vanilla consistency.
                //
                // Flavor tooltips display unconditionally if defined. They are visible both when holding SHIFT and when not.
                if (holdShiftItem.HasFlavorTooltip && holdShiftItem.FlavorTooltipKey is not null)
                {
                    string flavorText = item.ModItem.GetLocalizedValue(holdShiftItem.FlavorTooltipKey);
                    TooltipLine flavorLine = new TooltipLine(Mod, IHoldShiftTooltipItem.FlavorTooltipID, flavorText);
                    if (holdShiftItem.FlavorTooltipColor is not null)
                        flavorLine.OverrideColor = holdShiftItem.FlavorTooltipColor;

                    // Append the flavor tooltip at the end of standard tooltips, after all Hold SHIFT tooltips and reminders.
                    tooltips.Insert(++lastTooltipIndex, flavorLine);
                }
            }
            #endregion
        }
        private void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            #region Endgame Weapons
            if (item.type == ModContent.ItemType<AghanimBlessing>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(34, 27, 194),
                    new Color(183, 27, 194),
                    new Color(194, 27, 83)
                });
            }
            if (item.type == ModContent.ItemType<DivineRapierWeapon>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(250, 231, 200),
                    new Color(200, 250, 224),
                    new Color(243, 200, 250)
                });
            }
            if (item.type == ModContent.ItemType<Megaslark>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(82, 156, 25),
                    new Color(25, 156, 106),
                    new Color(25, 106, 156)
                });
            }
            if (item.type == ModContent.ItemType<Flamewall>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(245, 44, 44),
                    new Color(237, 118, 31),
                    new Color(247, 194, 47),
                });
            }
            #endregion
            #region Special Weapons
            if (item.type == ModContent.ItemType<TheGravity>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(145, 38, 222),
                    new Color(222, 38, 41),
                    new Color(38, 69, 222)
                });
            }
            #endregion
            #region Elite Aspects
            if (item.type == ModContent.ItemType<OverloadingAspect>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(49, 230, 203),
                    new Color(49, 174, 230),
                    new Color(49, 94, 230)
                });
            }
            if (item.type == ModContent.ItemType<BlazingAspect>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(255, 24, 59),
                    new Color(255, 84, 24),
                    new Color(255, 151, 24)
                });
            }
            if (item.type == ModContent.ItemType<GildedAspect>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(255, 187, 29),
                    new Color(255, 234, 29),
                    new Color(229, 255, 29)
                });
            }
            if (item.type == ModContent.ItemType<MendingAspect>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(176, 230, 49),
                    new Color(114, 230, 49),
                    new Color(49, 230, 115)
                });
            }
            if (item.type == ModContent.ItemType<NoxiousAspect>())
            {
                nameLine.OverrideColor = MogModUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(145, 47, 237),
                    new Color(219, 47, 237),
                    new Color(237, 47, 145)
                });
            }
            #endregion
        }
        #endregion
    }
}

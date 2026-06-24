using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.MagicProjectiles.TheGravitySpells;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using MogMod.Common.Systems;
using Terraria.Localization;
using MogMod.Buffs.PotionBuffs.TheGravityBuffs;

namespace MogMod.Items.Weapons.Magic
{
    /*Spell List
     * * = proj sprited
     * # = dust effect
     * @ = copied texture
     * ! = ui sprited
     * 
     * OFFENSIVE SPELLS:
     * rotating water block (travels in a straight line) *!
     * lingering flame (circle of flame that comes to a stop) *!
     * 3 fast moving thunder swords (similar to sky fracture / kaya (use kaya proj texture)) @!
     * slow moving gravity magic orb (slowly travels in a straight line) *!
     * ice spikes (rises from the ground (refer to calamity mod hematemesis)) *!
     * void explosion (circle around you that deals damage after 3 seconds) #!
     * 
     * PLAYER SPELLS (spells that modify the player):
     * defense shield (+ 15 defense) #!
     * health regen (overtime / instant) #!
     * mana regen (overtime / instant) #!
     * movement boost (increased movement speed, jump height, && wing time) #!
     * teleport (teleports to cursor (similar to blink dagger)) #!
     * 
     * SLOT SPELLS:
     * full bookmark (bookmarks all empty spell slots, instant) {upwards light blue} #!
     * auto bookmark (auto bookmarks after firing a spell, duration) {upwards light yellow dust} #!
     * shuffle bookmark (randomizes current spells, instant) {upwards green dust} #!
     * replay spell (current spell fired does not get discarded, last until cast spell) {idle green dust} #!
     * 
     * STAFFS (staffs will be cast at the cursor):
     * speed rod (+extra updates to all spells) {green, dust fast upwards} *!
     * slow rod (-extra updates to all spells) {purple, dust slow downwards} @!
     * gravity rod (pulls in spells) {blue, swirling-in dust} @!
     * repulsion rod (pushes away spells) {red, spiraling-out dust} @!
     */
    public class TheGravity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        // for switching cards
        public static int SwitchCard = 0;
        ModKeybind keybindActive = null;
        // for storing cards
        public int CardNumb = 0;
        public static int Card1 = 0;
        public static int Card2 = 0;
        public static int Card3 = 0;
        public static int Card4 = 0;
        public static int CurrentCard = 0;
        private static int attackSpeed = 20;
        private static readonly List<int> cardList =
        [
            // attack cards (0 - 4) (might add more later)
            ModContent.ProjectileType<AghanimBlessingProj>(), // water cube
            ModContent.ProjectileType<AghanimProjectile>(), // fire circle
            ModContent.ProjectileType<BloodMagicProjectile>(), // ice spike
            ModContent.ProjectileType<BloodthornOrb>(), // gravity orb
            ModContent.ProjectileType<CannonOfHaimaProj>(), // void explosion
            // player cards (5 - 10)
            ModContent.ProjectileType<EmptySpellCard>(), // instant health
            ModContent.ProjectileType<EmptySpellCard>(), // instant mana
            ModContent.ProjectileType<EmptySpellCard>(), // overtime health
            ModContent.ProjectileType<EmptySpellCard>(), // overtime mana
            ModContent.ProjectileType<EmptySpellCard>(), // movement
            ModContent.ProjectileType<EmptySpellCard>(), // defense
            // slot cards (11 - 13)
            ModContent.ProjectileType<EmptySpellCard>(), // bookmark
            ModContent.ProjectileType<EmptySpellCard>(), // replay
            ModContent.ProjectileType<EmptySpellCard>(), // shuffle
            // staff cards (14 - 17)
            ModContent.ProjectileType<EmptySpellCard>(), // slow staff
            ModContent.ProjectileType<EmptySpellCard>(), // speed staff
            ModContent.ProjectileType<EmptySpellCard>(), // gravity staff
            ModContent.ProjectileType<EmptySpellCard>(), // repulsion staff
            // chaos cards (18 - 19)
            ModContent.ProjectileType<EmptySpellCard>(), // auto cast
            ModContent.ProjectileType<EmptySpellCard>(), // teleport
        ];
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;

            Item.mana = 10;
            Item.damage = 100;
            Item.knockBack = 8f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = attackSpeed;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<EmptySpellCard>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item43;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mogPlayerUI = player.GetModPlayer<MogPlayerUI>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (player.altFunctionUse == 2)
            {
                // update the ui texture && store the card
                switch (SwitchCard)
                {
                    case 0:
                        if (mogPlayerUI.theGravityCurrent1 == 1)
                            player.CheckMana(Item.mana, true, true);
                        // get a random card from the list of cards
                        CurrentCard = Main.rand.Next(cardList.Count);
                        // set the selected card to the random card
                        Card1 = CurrentCard;
                        // update the selected card ui
                        mogPlayerUI.theGravityCurrent1++;
                        return false;
                    case 1:
                        if (mogPlayerUI.theGravityCurrent2 == 1)
                            player.CheckMana(Item.mana, true, true);
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card2 = CurrentCard;
                        mogPlayerUI.theGravityCurrent2++;
                        return false;
                    case 2:
                        if (mogPlayerUI.theGravityCurrent3 == 1)
                            player.CheckMana(Item.mana, true, true);
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card3 = CurrentCard;
                        mogPlayerUI.theGravityCurrent3++;
                        return false;
                    case 3:
                        if (mogPlayerUI.theGravityCurrent4 == 1)
                            player.CheckMana(Item.mana, true, true);
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card4 = CurrentCard;
                        mogPlayerUI.theGravityCurrent4++;
                        return false;
                }
                return false;
            }
            else if (player.altFunctionUse != 2)
            {
                // if the card is an empty card cast nothing
                switch (SwitchCard)
                {
                    case 0:
                        if (mogPlayerUI.theGravityCurrent1 == 0)
                        {
                            CardNumb = ModContent.ProjectileType<EmptySpellCard>();
                            break;
                        }
                        CardNumb = cardList[Card1];
                        ApplyCards(player, Card1);
                        mogPlayerUI.theGravityCurrent1--;
                        break;
                    case 1:
                        if (mogPlayerUI.theGravityCurrent2 == 0)
                        {
                            CardNumb = ModContent.ProjectileType<EmptySpellCard>();
                            break;
                        }
                        CardNumb = cardList[Card2];
                        ApplyCards(player, Card2);
                        mogPlayerUI.theGravityCurrent2--;
                        break;
                    case 2:
                        if (mogPlayerUI.theGravityCurrent3 == 0)
                        {
                            CardNumb = ModContent.ProjectileType<EmptySpellCard>();
                            break;
                        }
                        CardNumb = cardList[Card3];
                        ApplyCards(player, Card3);
                        mogPlayerUI.theGravityCurrent3--;
                        break;
                    case 3:
                        if (mogPlayerUI.theGravityCurrent4 == 0)
                        {
                            CardNumb = ModContent.ProjectileType<EmptySpellCard>();
                            break;
                        }
                        CardNumb = cardList[Card4];
                        ApplyCards(player, Card4);
                        mogPlayerUI.theGravityCurrent4--;
                        break;
                }
                type = CardNumb;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
            else
                return true;
        }
        public void ApplyCards(Player player, int card)
        {
            var mogPlayerUI = player.GetModPlayer<MogPlayerUI>();
            int bufftime = 600;
            switch (card)
            {
                case 5: // instant health
                    player.statLife += 50;
                    player.HealEffect(50);
                    if (player.statLife > player.statLifeMax2)
                        player.statLife = player.statLifeMax2;
                    break;
                case 6: // instant mana
                    player.statMana += 150;
                    player.ManaEffect(150);
                    if (player.statMana > player.statManaMax2)
                        player.statMana = player.statManaMax2;
                    break;
                case 7: // overtime health
                    player.AddBuff(ModContent.BuffType<TheGravityHealthBuff>(), bufftime);
                    break;
                case 8: // overtime mana
                    player.AddBuff(ModContent.BuffType<TheGravityManaBuff>(), bufftime);
                    break;
                case 9: // movement
                    player.AddBuff(ModContent.BuffType<TheGravityMovementBuff>(), bufftime);
                    break;
                case 10: // defense
                    player.AddBuff(ModContent.BuffType<TheGravityDefenseBuff>(), bufftime);
                    break;
                case 11: // bookmark
                    if (mogPlayerUI.theGravityCurrent1 == 0)
                    {
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card1 = CurrentCard;
                        mogPlayerUI.theGravityCurrent1++;
                    }
                    if (mogPlayerUI.theGravityCurrent2 == 0)
                    {
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card2 = CurrentCard;
                        mogPlayerUI.theGravityCurrent2++;
                    }
                    if (mogPlayerUI.theGravityCurrent3 == 0)
                    {
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card3 = CurrentCard;
                        mogPlayerUI.theGravityCurrent3++;
                    }
                    if (mogPlayerUI.theGravityCurrent4 == 0)
                    {
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card4 = CurrentCard;
                        mogPlayerUI.theGravityCurrent4++;
                    }
                    break;
                case 12: // replay (add a ui effect for this instead of a buff)
                    player.AddBuff(ModContent.BuffType<TheGravityReplayBuff>(), bufftime);
                    break;
                case 13: // shuffle
                    for (int i = 0; i < 3; i++)
                    {
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card1 = CurrentCard;
                        mogPlayerUI.theGravityCurrent1++;
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card2 = CurrentCard;
                        mogPlayerUI.theGravityCurrent2++;
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card3 = CurrentCard;
                        mogPlayerUI.theGravityCurrent3++;
                        CurrentCard = Main.rand.Next(cardList.Count);
                        Card4 = CurrentCard;
                        mogPlayerUI.theGravityCurrent4++;
                    }
                    break;
                case 18: // auto bookmark
                    player.AddBuff(ModContent.BuffType<TheGravityAutoBuff>(), bufftime);
                    break;
                case 19:
                    SoundEngine.PlaySound(SoundID.Item8, player.Center);
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 dustCorner = player.position - 2f * Vector2.One;
                        Vector2 dustVel = player.velocity + new Vector2(0f, Main.rand.NextFloat(-5f, -1f));
                        int dust = Dust.NewDust(dustCorner, player.width + 4, player.height + 4, Main.rand.NextBool(3) ? 71 : 72, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.4f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.75f;
                        Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.85f;
                        Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dust].noGravity = false;
                            Main.dust[dust].scale *= 0.2f;
                        }
                    }
                    player.Teleport(Main.MouseWorld, TeleportationStyleID.DebugTeleport);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y, TeleportationStyleID.DebugTeleport); //Needed for multiplayer
                    break;
            }
        }
        public override void UpdateInventory(Player player)
        {
            if (KeybindSystem.TheGravityKeybind.JustPressed)
            {
                SwitchCard++;
                if (SwitchCard >= 4)
                    SwitchCard = 0;
            }
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = Item.useTime = 16;
                Item.UseSound = SoundID.Item45 with { Pitch = 0.25f};
            }
            else
            {
                Item.useAnimation = Item.useTime = attackSpeed;
                Item.UseSound = SoundID.Item43;
            }
            return true;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                reduce -= 10f;
                //mult *= 0f;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            list.IntegrateHotkey(KeybindSystem.TheGravityKeybind);
            List<Color> colorList = new List<Color>()
            {
                new Color(164, 97, 212),
                new Color(212, 97, 110),
                new Color(97, 107, 212),
            };

            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarFlareBook).
                AddIngredient<VortexOfConflagration>().
                AddIngredient<ShadowRealm>().
                AddIngredient<LagunaBlade>().
                AddIngredient(ItemID.LunarBar, 15).
                AddIngredient<SoulFragment>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
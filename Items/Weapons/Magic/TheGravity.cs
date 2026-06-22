using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.MagicProjectiles.TheGravitySpells;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    /*Weapon Details
    4 spell slots that each can contain a spell
    left click fires current spell and discards it, if empty, nothing happens
    right click bookmarks a spell if the spell slot is empty, switches current spell slot otherwise
    different spells interact differently with eachother (for more interesting damage potential)
    spell slots have ui elements && each spell has a unique ui element, refer to gunlance ui for help
    "Does meaning have a meaning"
    34x40
    post moon-lord
    */
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
        public static bool SwitchCard = false;
        public int CardNumb = 0;
        public static int CurrentCard = 0;
        private static readonly List<int> cardList =
        [
            ModContent.ProjectileType<AghanimBlessingProj>(),
            ModContent.ProjectileType<AghanimProjectile>(),
            ModContent.ProjectileType<BloodMagicProjectile>(),
            ModContent.ProjectileType<BloodthornOrb>(),
            ModContent.ProjectileType<CannonOfHaimaProj>(),
            ModContent.ProjectileType<DagonOneProj>(),
            ModContent.ProjectileType<DagonTwoProj>(),
            ModContent.ProjectileType<DagonThreeProj>(),
            ModContent.ProjectileType<DagonFourProj>(),
            ModContent.ProjectileType<DagonFiveProj>(),
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
            Item.useAnimation = Item.useTime = 24;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<EmptySpellCard>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item43;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
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
                Item.useAnimation = Item.useTime = 28;
                Item.UseSound = SoundID.Item43;
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*TODO:
             * make rightclicking a bookmarked card switch the selected card
             */
            var mogPlayerUI = player.GetModPlayer<MogPlayerUI>();
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (type == ModContent.ProjectileType<EmptySpellCard>())
                {
                    // get a random card from the list of cards
                    CurrentCard = Main.rand.Next(cardList.Count);
                    // set the proj fired to the random card
                    CardNumb = cardList[CurrentCard];
                    // update the ui texture
                    mogPlayerUI.theGravityCurrent++;
                }
                else
                {
                    SwitchCard = true;
                }
                return false;
            }
            else if (player.altFunctionUse != 2)
            {
                if (mogPlayerUI.theGravityCurrent == 0)
                    CardNumb = ModContent.ProjectileType<EmptySpellCard>();
                type = CardNumb;
                knockback *= 1.2f;
                velocity *= 1.2f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                mogPlayerUI.theGravityCurrent--;
                return false;
            }
            else
                return true;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0f;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
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
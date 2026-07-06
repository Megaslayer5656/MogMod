using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Weapons.Magic.DeckCards;
using MogMod.Projectiles.MagicProjectiles.TheGravitySpells;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class TheDeck : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        private static int attackSpeed = 20;
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;

        public static readonly SoundStyle ShuffleSfx = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ShuffleSfx")
        {
            Volume = 1f,
            PitchVariance = .2f,
            MaxInstances = 1,
        };

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 42;

            Item.mana = 20;
            Item.damage = 100;
            Item.knockBack = 8f;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = attackSpeed;

            Item.noMelee = true;
            Item.autoReuse = true;

            Item.shootSpeed = 20f;
            Item.scale = .5f;
            Item.shoot = ModContent.ProjectileType<EmptySpellCard>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item43;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }

        public static List<DeckCard> allDeckCards = //Might change this to just store the item ids if this causes issues
        [
            ModContent.GetInstance<HealingHandsCard>(),
            ModContent.GetInstance<DarkRitualCard>(),
        ];

        public static List<DeckCard> currentCards = new List<DeckCard>();

        public void shuffleDeck()
        {
            currentCards.Clear();

            for (int i = 0; i < allDeckCards.Count; i++)
            {
                if (allDeckCards[i].getEnabled() == true)
                {
                    int randIndex = Random.Shared.Next(0, currentCards.Count + 1);
                    currentCards.Insert(randIndex, allDeckCards[i]);
                }
            }
        }

        public void discard()
        {
            currentCards.RemoveAt(0);
        }

        public override bool Shoot(Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (currentCards.Count == 0) //This comes before the player.altFunctionUse == 2 thing so that you don't try to call currentCards.RemoveAt(0) when there isn't anything at 0.
            {
                shuffleDeck();
                SoundEngine.PlaySound(ShuffleSfx);
                return false;
            }

            if (player.altFunctionUse == 2)
            {
                discard();
                SoundEngine.PlaySound(SoundID.Item45 with { Pitch = .25f }, player.Center);
                return false;
            }

            currentCards[0].doEffect(player);
            currentCards.RemoveAt(0);
            return false;
        }

        public static string getCurrentCardName()
        {
            if (currentCards.Count > 0)
            {
                return currentCards[0].getCardName();
            }
            else
            {
                return "None";
            }
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.mana = 0;
            }
            else
            {
                Item.mana = 20;
            }
            return true;
        }
    }
}

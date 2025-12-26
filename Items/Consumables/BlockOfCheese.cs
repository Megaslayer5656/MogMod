using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class BlockOfCheese : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/CheeseConsume")
            {
                Volume = 1.1f,
                PitchVariance = 0.2f,
            };
            Item.consumable = true;
            Item.potion = true;
            Item.healMana = 400;
            Item.healLife = 250;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(0, 18, 40, 0);
            Item.buffType = ModContent.BuffType<Buffs.CheeseBuff>();
            Item.buffTime = 18000;
        }
        
        // gets rid of "Master" text in tooltip
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Master" && x.Mod == "Terraria");
            if (changedLine != null)
            {
                changedLine.Text = "";
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<GreaterHealingLotus>(3).
                AddIngredient<Cheese>(1).
                AddIngredient(ItemID.EndurancePotion, 1).
                AddIngredient(ItemID.Ectoplasm, 1).
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
        }
    }
}

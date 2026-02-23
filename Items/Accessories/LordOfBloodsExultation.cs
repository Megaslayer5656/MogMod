using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class LordOfBloodsExultation : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 57;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.exultationEquipped = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 10).
            AddRecipeGroup("IronBar", 5).
            AddTile(TileID.WorkBenches).
            Register();
        }
    }
}

using Microsoft.Xna.Framework;
using MogMod.UI.TheDeckUI.YourMod.UI;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using MogMod.Items.Weapons.Magic;

namespace MogMod.UI.TheDeckUI
{
    public class DeckUISystem : ModSystem
    {
        private UserInterface deckInterface;
        private TheDeckText deckUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                deckUI = new TheDeckText();
                deckUI.Activate();

                deckInterface = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;

            bool holdingDeck =
                player.HeldItem.type == ModContent.ItemType<TheDeck>();

            if (holdingDeck)
            {
                if (deckInterface.CurrentState != deckUI)
                    deckInterface.SetState(deckUI);
            }
            else
            {
                if (deckInterface.CurrentState != null)
                    deckInterface.SetState(null);
            }

            deckInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryLayer != -1)
            {
                layers.Insert(inventoryLayer, new LegacyGameInterfaceLayer(
                    "MogMod: Deck UI",
                    delegate
                    {
                        deckInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

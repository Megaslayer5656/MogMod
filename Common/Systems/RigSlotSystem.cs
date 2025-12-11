using MogMod.UI;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Common.Systems
{
    public class RigSlotSystem : ModSystem
    {
        public static UserInterface MogInterface;
        public static AccessorySlotsUI UI;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                MogInterface = new UserInterface();
                UI = new AccessorySlotsUI();

                UI.Activate();
                MogInterface.SetState(UI);
            }

        }
        public override void Unload()
        {
            UI = null;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (UI.IsVisible)
            {
                UI?.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryLayer != -1)
            {
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                        "Rig Slot: Custom Slot UI",
                        () => {
                            if (UI.IsVisible)
                            {
                                MogInterface.Draw(Main.spriteBatch, new GameTime());
                            }

                            return true;
                        },
                        InterfaceScaleType.UI));
            }
        }
    }
}

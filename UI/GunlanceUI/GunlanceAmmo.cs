using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace MogMod.UI.GunlanceUI
{
	// This custom UI will show whenever the player is holding the ExampleCustomResourceWeapon item and will display the player's custom resource amounts that are tracked in GunlanceAmmoPlayer
	internal class GunlanceAmmo : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
		private UIText text;
		private UIElement area;
		private UIImage barFrame;
        private UIImage barFrame2;
        private Color gradientA;
		private Color gradientB;

		public override void OnInitialize()
        {
            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding.
            area = new UIElement();
			area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
			area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
			area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
			area.Height.Set(60, 0f);

			barFrame = new UIImage(ModContent.Request<Texture2D>("MogMod/UI/GunlanceUI/GunlanceUnLoadedAmmo")); // Frame of our resource bar
            barFrame2 = new UIImage(ModContent.Request<Texture2D>("MogMod/UI/GunlanceUI/GunlanceLoadedAmmo")); // Frame of our resource bar
            barFrame.Left.Set(22, 0f);
			barFrame.Top.Set(0, 0f);
			barFrame.Width.Set(138, 0f);
			barFrame.Height.Set(34, 0f);

			text = new UIText("0/0", 0.8f); // text to show stat
			text.Width.Set(138, 0f);
			text.Height.Set(34, 0f);
			text.Top.Set(40, 0f);
			text.Left.Set(0, 0f);

			gradientA = new Color(123, 25, 138); // A dark purple
			gradientB = new Color(187, 91, 201); // A light purple

			area.Append(text);
			if (mogPlayerUI.gunlanceCurrent == mogPlayerUI.exampleResourceMax)
				area.Append(barFrame2);
            else
                area.Append(barFrame);
            Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			// This prevents drawing unless we are using an ExampleCustomResourceWeapon
			if (Main.LocalPlayer.HeldItem.ModItem is not Gunlance)
				return;

			base.Draw(spriteBatch);
		}

		// Here we draw our UI
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			// Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
			Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X += 12;
			hitbox.Width -= 24;
			hitbox.Y += 8;
			hitbox.Height -= 16;
		}

		public override void Update(GameTime gameTime) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Gunlance)
				return;

			base.Update(gameTime);
		}
	}


    // This class will only be autoloaded/registered if we're not loading on a server
    [Autoload(Side = ModSide.Client)]
    internal class GunlanceAmmoUISystem : ModSystem
    {
        private UserInterface GunlanceAmmoUserInterface;

        internal GunlanceAmmo GunlanceAmmo;

        public static LocalizedText ExampleResourceText { get; private set; }

        public override void Load()
        {
            GunlanceAmmo = new();
            GunlanceAmmoUserInterface = new();

			// TODO: fix this
            //GunlanceAmmoUserInterface.SetState(GunlanceAmmo);

			string category = "UI";
			ExampleResourceText ??= Mod.GetLocalization($"{category}.ExampleResource");
		}

        public override void UpdateUI(GameTime gameTime)
        {
            GunlanceAmmoUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "MogMod: Gunlance Ammo",
                    delegate {
                        GunlanceAmmoUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
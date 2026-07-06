using MogMod.Items.Weapons.Magic;
using System;
namespace MogMod.UI.TheDeckUI
{
    using Microsoft.Xna.Framework;
    using Terraria;
    using Terraria.GameContent.UI.Elements;
    using Terraria.UI;

    namespace YourMod.UI
    {
        public class TheDeckText : UIState
        {
            private UIText currentCardName;
            private UIText cardsLeft;

            public override void OnInitialize()
            {
                currentCardName = new UIText("", 1f);
                currentCardName.Left.Set(Main.screenWidth - 760f, 0f);
                currentCardName.Top.Set(30f, 0f);

                cardsLeft = new UIText("", 1f);
                cardsLeft.Left.Set(Main.screenWidth - 760f, 0f);
                cardsLeft.Top.Set(50f, 0f);

                Append(currentCardName);
                Append(cardsLeft);
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);

                currentCardName.SetText($"Current Card: {TheDeck.getCurrentCardName()}");
                cardsLeft.SetText($"Cards Left: {TheDeck.currentCards.Count}");
            }
        }
    }
}

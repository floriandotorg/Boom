using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Pages;

namespace Boom
{
    class RemoveAdsButton : Button
    {
        public override void LoadContent()
        {
            base.LoadContent();

            Text = "Remove Ads";
            Font = Load<SpriteFont>("InGameBoldFont");
            Color = Color.Gray;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Left;
            Tap += RemoveAdsButton_Tap;

            if (!Store.Available || Store.HasPurchased(GameSettings.RemoveAdsProductId))
            {
                Visible = false;
            }
        }

        void RemoveAdsButton_Tap(object sender)
        {
            Store.Purchase(GameSettings.RemoveAdsProductId,
                () =>
                {
                    Visible = false;
                    AdManager.UpdateAdStatus();
                });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            X = 10;
            Y = Superview.Height - Height - 10;
        }
    }
}

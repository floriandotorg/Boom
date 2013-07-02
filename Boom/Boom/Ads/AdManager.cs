using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using AdRotatorXNA;
using AdRotator.Model;

namespace Boom
{
    class AdManager
    {
        public static void Initialize(Game game)
        {
            if (!Store.HasPurchased(GameSettings.RemoveAdsProductId))
            {
                AdRotatorXNAComponent.Initialize(game);

#if DEBUG
                AdRotatorXNAComponent.Current.Log += log;
#endif

                AdRotatorXNAComponent.Current.SlidingAdDirection = SlideDirection.Top;
                AdRotatorXNAComponent.Current.SlidingAdDisplaySeconds = 50;
                AdRotatorXNAComponent.Current.SlidingAdHiddenSeconds = 20;
                AdRotatorXNAComponent.Current.AdPosition = Vector2.Zero;

                AdRotatorXNAComponent.Current.DefaultSettingsFileUri = @"Ads/defaultAdSettings.xml";

                AdRotatorXNAComponent.Current.SettingsUrl = "http://floyd-ug.de/23FBCE58-46CA-449A-BBC8-529602D6D368/boom/defaultAdSettings.xml";

                game.Components.Add(AdRotatorXNAComponent.Current);
            }
        }

        private static void log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void UpdateAdStatus()
        {
            if (Store.HasPurchased(GameSettings.RemoveAdsProductId) && AdRotatorXNAComponent.Current != null)
            {
                AdRotatorXNAComponent.Current.Enabled = false;
                AdRotatorXNAComponent.Current.Visible = false;
                AdRotatorXNAComponent.Current.Game.Components.Remove(AdRotatorXNAComponent.Current);
            }
        }
    }
}

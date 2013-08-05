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
using Microsoft.Phone.Tasks;

namespace Boom
{
    class RateReviewPopupView : View
    {
        private Label _likeLabel;
        private Button _reviewButton, _noButton;

        public RateReviewPopupView()
        {
            GameSettings.RateReviewShown = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            _likeLabel = new Label();
            AddSubview(_likeLabel);

            _reviewButton = new Button();
            AddSubview(_reviewButton);

            _noButton = new Button();
            AddSubview(_noButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _likeLabel.Text = "Do you like " + GameSettings.GameName + "?";
            _likeLabel.Font = Load<SpriteFont>("InGameLargeFont");

            _reviewButton.Text = "Click to Rate and Review";
            _reviewButton.Font = Load<SpriteFont>("InGameBoldFont");
            _reviewButton.AutoResize = false;
            _reviewButton.Height = 40;
            _reviewButton.Tap += _reviewButton_Tap;

            _noButton.Text = "no, thanks";
            _noButton.Font = Load<SpriteFont>("InGameFont");
            _noButton.AutoResize = false;
            _noButton.Height = 30;
            _noButton.Tap += _noButton_Tap;
        }

        void _noButton_Tap(object sender)
        {
            Dismiss(true);
        }

        void _reviewButton_Tap(object sender)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();

            Dismiss(true);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_likeLabel, 0);
            _likeLabel.Y = 15;

            CenterSubview(_reviewButton, Convert.ToInt32(Height * .118f));

            CenterSubview(_noButton, 0);
            _noButton.Y = Height - 13 - _noButton.Height;
        }
    }
}

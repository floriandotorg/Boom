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
//using Microsoft.Phone.Tasks;

namespace Boom
{
    class RateReviewPopupView : View
    {
        private Label _likeLabel, _textLabel;
        private Button _reviewButton;

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

            _textLabel = new Label();
            AddSubview(_textLabel);
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

            _textLabel.Text = "Please help us to improve this game";
            _textLabel.Font = Load<SpriteFont>("InGameFont");
            _textLabel.AutoResize = false;
			_textLabel.Height = 30;
        }

        void _noButton_Tap(object sender)
        {
            Dismiss(true);
        }

        void _reviewButton_Tap(object sender)
        {
//            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
//            marketplaceReviewTask.Show();

            Dismiss(true);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_likeLabel, 0);
			_likeLabel.Y = 15;

            CenterSubview(_textLabel, 0);

            CenterSubview(_reviewButton, 0);
			_reviewButton.Y = Height - 13 - _reviewButton.Height;
        }
    }
}

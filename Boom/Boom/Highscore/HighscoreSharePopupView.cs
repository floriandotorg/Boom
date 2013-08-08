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
//using Microsoft.Phone.Tasks;
using Pages;

namespace Boom
{
    class HighscoreShareView : View
    {
        private Label _headerLabel, _youAreLabel, _rankLabel;
        private Button _twitterButton, _facebookButton;
        private string _textLine1, _textLine2, _shareText;

        public HighscoreShareView(string textLine1, string textLine2, string shareText)
        {
            _textLine1 = textLine1;
            _textLine2 = textLine2;
            _shareText = shareText;
        }

        public override void Initialize()
        {
            base.Initialize();

            _headerLabel = new Label();
            AddSubview(_headerLabel);

            _youAreLabel = new Label();
            AddSubview(_youAreLabel);

            _rankLabel = new Label();
            AddSubview(_rankLabel);

            _twitterButton = new Button();
            AddSubview(_twitterButton);

            _facebookButton = new Button();
            AddSubview(_facebookButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _headerLabel.Text = "Congratulations!";
            _headerLabel.Font = Load<SpriteFont>("InGameLargeFont");

            _youAreLabel.Text = _textLine1;
            _youAreLabel.Font = Load<SpriteFont>("InGameFont");

            _rankLabel.Text = _textLine2;
            _rankLabel.Font = Load<SpriteFont>("InGameFont");

            _twitterButton.BackgroundTexture = Load<Texture2D>("TwitterButtonTexture");
            _twitterButton.BackgroundColor = Color.White;
            _twitterButton.AutoResize = false;
            _twitterButton.Height = _twitterButton.BackgroundTexture.Height;
            _twitterButton.Width = _twitterButton.BackgroundTexture.Width;
            _twitterButton.Tap += share;

            _facebookButton.BackgroundTexture = Load<Texture2D>("FacebookButtonTexture");
            _facebookButton.BackgroundColor = Color.White;
            _facebookButton.AutoResize = false;
            _facebookButton.Height = _facebookButton.BackgroundTexture.Height;
            _facebookButton.Width = _facebookButton.BackgroundTexture.Width;
            _facebookButton.Tap += share;
        }

        private void share(object sender)
        {
//            ShareLinkTask shareLinkTask = new ShareLinkTask();
//            shareLinkTask.LinkUri = new Uri("http://bit.ly/19Y4EoN", UriKind.Absolute);
//            shareLinkTask.Title = GameSettings.GameName;
//            shareLinkTask.Message = _shareText;
//            shareLinkTask.Show();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_headerLabel, 0);
            _headerLabel.Y = 15;

            CenterSubview(_youAreLabel, -15);

            CenterSubview(_rankLabel, +15);

            CenterSubview(_twitterButton, 0);
            _twitterButton.X -= 50;
            _twitterButton.Y = Height - 15 - _twitterButton.Height;

            CenterSubview(_facebookButton, 0);
            _facebookButton.X += 50;
            _facebookButton.Y = Height - 15 - _facebookButton.Height;
        }
    }
}


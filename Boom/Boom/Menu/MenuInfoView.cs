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
using Microsoft.Phone.Info;
using System.Reflection;

namespace Boom
{
    class MenuInfoView : View
    {
        private Label _titleLabel, _versionLabel, _byLabel, _musicByLabel;
        private Button _floydButton, _musicAuthorButton, _rateReviewLabel, _supportLabel, _policyLabel, _twitterButton, _facebookButton;

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel);

            _versionLabel = new Label();
            AddSubview(_versionLabel);

            _byLabel = new Label();
            AddSubview(_byLabel);

            _floydButton = new Button();
            AddSubview(_floydButton);

            _musicByLabel = new Label();
            AddSubview(_musicByLabel);

            _musicAuthorButton = new Button();
            AddSubview(_musicAuthorButton);

            _rateReviewLabel = new Button();
            AddSubview(_rateReviewLabel);

            _supportLabel = new Button();
            AddSubview(_supportLabel);

            _policyLabel = new Button();
            AddSubview(_policyLabel);

            _twitterButton = new Button();
            AddSubview(_twitterButton);

            _facebookButton = new Button();
            AddSubview(_facebookButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version_str = versionAttrib.Version.ToString().Substring(0, 3);

            BackgroundColor = Color.Transparent;

            _titleLabel.Text = GameSettings.GameName;
            _titleLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _titleLabel.Color = Color.White;

            _versionLabel.Text = "Version " + version_str;
            _versionLabel.Font = Load<SpriteFont>("InGameFont");
            _versionLabel.Color = Color.White;

            _byLabel.Text = "by";
            _byLabel.Font = Load<SpriteFont>("InGameFont");
            _byLabel.Color = Color.White;

            _floydButton.Text = "Floyd Games";
            _floydButton.Font = Load<SpriteFont>("InGameFont");
            _floydButton.Color = Color.White;
            _floydButton.Tap += _floydButton_Tap;

            _musicByLabel.Text = "Music by";
            _musicByLabel.Font = Load<SpriteFont>("InGameFont");
            _musicByLabel.Color = Color.White;

            _musicAuthorButton.Text = "Chris Zabriskie";
            _musicAuthorButton.Font = Load<SpriteFont>("InGameFont");
            _musicAuthorButton.Color = Color.White;
            _musicAuthorButton.Tap += _musicAuthorButton_Tap;

            _rateReviewLabel.Text = "Rate and Review";
            _rateReviewLabel.Font = Load<SpriteFont>("InGameFont");
            _rateReviewLabel.Color = Color.White;
            _rateReviewLabel.Tap += _rateReviewLabel_Tap;

            _supportLabel.Text = "Support";
            _supportLabel.Font = Load<SpriteFont>("InGameFont");
            _supportLabel.Color = Color.White;
            _supportLabel.Tap += _supportLabel_Tap;

            _policyLabel.Text = "Privacy Policy";
            _policyLabel.Font = Load<SpriteFont>("InGameFont");
            _policyLabel.Color = Color.White;
            _policyLabel.Tap += _policyLabel_Tap;

            _twitterButton.AutoResize = false;
            _twitterButton.Height = 70;
            _twitterButton.Width = 70;
            _twitterButton.BackgroundTexture = Load<Texture2D>("TwitterTexture");
            _twitterButton.BackgroundColor = Color.White;
            _twitterButton.Tap += _twitterButton_Tap;

            _facebookButton.AutoResize = false;
            _facebookButton.Height = 70;
            _facebookButton.Width = 70;
            _facebookButton.BackgroundTexture = Load<Texture2D>("FacebookTexture");
            _facebookButton.BackgroundColor = Color.White;
            _facebookButton.Tap += _facebookButton_Tap;
        }

        void _facebookButton_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://facebook.com/boomlygame");
            webBrowserTask.Show();
        }

        void _twitterButton_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://twitter.com/boomlygame");
            webBrowserTask.Show();
        }

        void _musicAuthorButton_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://chriszabriskie.com");
            webBrowserTask.Show();
        }

        void _floydButton_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://floydgames.com");
            webBrowserTask.Show();
        }

        void _policyLabel_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://floydgames.com/games/boomly/privacy-policy");
            webBrowserTask.Show();
        }

        void _supportLabel_Tap(object sender)
        {
            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version_str = versionAttrib.Version.ToString().Substring(0, 3);

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            string result = "unknown device";
            object deviceName;
            if (DeviceExtendedProperties.TryGetValue("DeviceName", out deviceName))
            {
                result = deviceName.ToString();
            }

            emailComposeTask.Subject = GameSettings.GameName + " Version " + version_str + " on " + result;
            emailComposeTask.To = "support.boomly@floydgames.com";

            emailComposeTask.Show();
        }

        void _rateReviewLabel_Tap(object sender)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_titleLabel, -250);
            CenterSubview(_versionLabel, -150);
            CenterSubview(_byLabel, -75);
            CenterSubview(_floydButton, -50);
            CenterSubview(_musicByLabel, 0);
            CenterSubview(_musicAuthorButton, 25);
            CenterSubview(_rateReviewLabel, 150);
            CenterSubview(_supportLabel, 200);
            CenterSubview(_policyLabel, 250);

            CenterSubview(_twitterButton, _twitterButton.Height / 2 + 2);
            _twitterButton.X = Width - _twitterButton.Width;

            CenterSubview(_facebookButton, -(_facebookButton.Height / 2) - 2);
            _facebookButton.X = Width - _facebookButton.Width;
        }
    }
}

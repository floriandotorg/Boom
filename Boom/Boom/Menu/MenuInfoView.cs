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
        private Label _titleLabel, _versionLabel, _byLabel, _floydLabel, _musicByLabel, _musicAuthorLabel;
        private Button _rateReviewLabel, _supportLabel, _policyLabel;

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel);

            _versionLabel = new Label();
            AddSubview(_versionLabel);

            _byLabel = new Label();
            AddSubview(_byLabel);

            _floydLabel = new Label();
            AddSubview(_floydLabel);

            _musicByLabel = new Label();
            AddSubview(_musicByLabel);

            _musicAuthorLabel = new Label();
            AddSubview(_musicAuthorLabel);

            _rateReviewLabel = new Button();
            AddSubview(_rateReviewLabel);

            _supportLabel = new Button();
            AddSubview(_supportLabel);

            _policyLabel = new Button();
            AddSubview(_policyLabel);
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

            _floydLabel.Text = "Floyd";
            _floydLabel.Font = Load<SpriteFont>("InGameFont");
            _floydLabel.Color = Color.White;

            _musicByLabel.Text = "Music by";
            _musicByLabel.Font = Load<SpriteFont>("InGameFont");
            _musicByLabel.Color = Color.White;

            _musicAuthorLabel.Text = "Chris Zabriskie";
            _musicAuthorLabel.Font = Load<SpriteFont>("InGameFont");
            _musicAuthorLabel.Color = Color.White;

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
        }

        void _policyLabel_Tap(object sender)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://www.floydug.com/apps/boomly/wp/privacy-policy-current.html");
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
            emailComposeTask.To = "support.boom@floydug.com";

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
            CenterSubview(_byLabel, -100);
            CenterSubview(_floydLabel, -75);
            CenterSubview(_musicByLabel, -30);
            CenterSubview(_musicAuthorLabel, -5);
            CenterSubview(_rateReviewLabel, 150);
            CenterSubview(_supportLabel, 200);
            CenterSubview(_policyLabel, 250);
        }
    }
}

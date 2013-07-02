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
    class MenuHighscoreView : View
    {
        private Label _titleLabel;
        private HighscoreTabView _highscoreTabView;

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel);

            _highscoreTabView = new HighscoreTabView(null);
            AddSubview(_highscoreTabView);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Transparent;

            _titleLabel.Text = "Highscore";
            _titleLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _titleLabel.Color = Color.White;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_titleLabel, -250);

            _highscoreTabView.Height = 420;
            _highscoreTabView.Width = 320;
            CenterSubview(_highscoreTabView, 80);
        }
    }
}

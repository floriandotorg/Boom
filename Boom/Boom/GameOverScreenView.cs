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
    class GameOverScreenView : View
    {
        private Label _titleLabel, _yourScoreLabel, _scoreLabel;
        private HighscoreTableView _highscoreTableView;
        private int _score;
        private SpeakerButton _speakerButton;

        public GameOverScreenView(int score)
        {
            _score = score;
        }

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel);

            _yourScoreLabel = new Label();
            AddSubview(_yourScoreLabel);

            _scoreLabel = new Label();
            AddSubview(_scoreLabel);

            _highscoreTableView = new HighscoreTableView(-1);
            AddSubview(_highscoreTableView);

            _speakerButton = new SpeakerButton();
            AddSubview(_speakerButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Transparent;

            _titleLabel.Text = "Highscore";
            _titleLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _titleLabel.Color = Color.White;

            _yourScoreLabel.Text = "Your Score:";
            _yourScoreLabel.Font = Load<SpriteFont>("InGameFont");
            _yourScoreLabel.Color = Color.White;

            _scoreLabel.Text = Convert.ToString(_score);
            _scoreLabel.Font = Load<SpriteFont>("InGameFont");
            _scoreLabel.Color = Color.White;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_titleLabel, -250);
            CenterSubview(_yourScoreLabel, -150);
            CenterSubview(_scoreLabel, -120);

            _highscoreTableView.Height = 300;
            _highscoreTableView.Width = 300;
            CenterSubview(_highscoreTableView, 75);
        }
    }
}

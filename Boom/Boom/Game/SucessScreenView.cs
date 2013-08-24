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
    class SucessScreenView : DismissOnTapView
    {
        private string _scoreText, _goalText;
        private Label _headerLabel, _goalLabel, _tapToResumeLabel, _currentScoreLabel, _scoreLabel;

        public SucessScreenView(string goalText, string scoreText)
        {
            _scoreText = scoreText;
            _goalText = goalText;
        }

        public override void Initialize()
        {
            base.Initialize();

            _headerLabel = new Label();
            AddSubview(_headerLabel);

            _goalLabel = new Label();
            AddSubview(_goalLabel);

            _tapToResumeLabel = new Label();
            AddSubview(_tapToResumeLabel);

            if (_scoreText != null)
            {
                _currentScoreLabel = new Label();
                AddSubview(_currentScoreLabel);

                _scoreLabel = new Label();
                AddSubview(_scoreLabel);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.White;

            _headerLabel.Text = "You won!";
            _headerLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _headerLabel.Color = Color.Black;

            _goalLabel.Text = _goalText;
            _goalLabel.Font = Load<SpriteFont>("InGameFont");
            _goalLabel.Color = Color.Black;

            _tapToResumeLabel.Text = "Tap to resume";
            _tapToResumeLabel.Font = Load<SpriteFont>("InGameFont");
            _tapToResumeLabel.Color = Color.Black;

            if (_scoreText != null)
            {
                _currentScoreLabel.Text = "Current Score";
                _currentScoreLabel.Font = Load<SpriteFont>("InGameFont");
                _currentScoreLabel.Color = Color.Black;

                _scoreLabel.Text = _scoreText;
                _scoreLabel.Font = Load<SpriteFont>("InGameFont");
                _scoreLabel.Color = Color.Black;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float h = (float)Viewport.Height / 2f;

            CenterSubview(_headerLabel, (int)(-h / 2f));
            CenterSubview(_goalLabel, (int)(-h / 2f) + 60);

            if (_scoreText != null)
            {
                CenterSubview(_tapToResumeLabel, (int)((-h / 2f) + h * .382f));
                CenterSubview(_currentScoreLabel, (int)((-h / 2f) + h - 50));
                CenterSubview(_scoreLabel, (int)((-h / 2f) + h - 50) + 32);
            }
            else
            {
                CenterSubview(_tapToResumeLabel, 30);
            }
        }
    }
}

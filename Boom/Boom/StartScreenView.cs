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
    class StartScreenView : View
    {
        private string _headerText, _scoreText, _goalText;
        private Label _headerLabel, _goalLabel, _tapToStartLabel, _currentScoreLabel, _scoreLabel;
        private SpeakerButton _speakerButton;

        public StartScreenView(string headerText, string goalText, string scoreText)
        {
            _headerText = headerText;
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

            _tapToStartLabel = new Label();
            AddSubview(_tapToStartLabel);

            _currentScoreLabel = new Label();
            AddSubview(_currentScoreLabel);
            
            _scoreLabel = new Label();
            AddSubview(_scoreLabel);

            _speakerButton = new SpeakerButton();
            AddSubview(_speakerButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Black * .6f;

            _headerLabel.Text = _headerText;
            _headerLabel.Font = Load<SpriteFont>("InGameLargeFont");

            _goalLabel.Text = _goalText;
            _goalLabel.Font = Load<SpriteFont>("InGameFont");

            _tapToStartLabel.Text = "Tap to start";
            _tapToStartLabel.Font = Load<SpriteFont>("InGameFont");

            _currentScoreLabel.Text = "Current Score";
            _currentScoreLabel.Font = Load<SpriteFont>("InGameFont");

            _scoreLabel.Text = _scoreText;
            _scoreLabel.Font = Load<SpriteFont>("InGameFont");
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float h = (float)Viewport.Height / 2f;

            CenterSubview(_headerLabel, (int)(-h / 2f));
            CenterSubview(_goalLabel, (int)(-h / 2f) + 60);
            CenterSubview(_tapToStartLabel, (int)((-h / 2f) + h * .382f));
            CenterSubview(_currentScoreLabel, (int)((-h / 2f) + h - 50));
            CenterSubview(_scoreLabel, (int)((-h / 2f) + h - 50) + 40);
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                Dismiss(true);
            }
            return true;
        }
    }
}

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
    class SucessScreenView : View
    {
        private string _scoreText, _goalText;
        private Label _headerLabel, _goalLabel, _tapToResumeLabel, _currentScoreLabel, _scoreLabel;
        private SpeakerButton _speakerButton;

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

            //_intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "You won!", Pos = l1, Color = Color.Black, Font = _ressources.gameOverfont },
            //                                                            new IntermediateScreen.TextLine() { Text = Score + "/" + goal + " of " + numBallsTotal, Pos = l2, Color = Color.Black, Font = _ressources.font },
            //                                                            new IntermediateScreen.TextLine() { Text = "Tap to resume", Pos = l3, Color = Color.Black, Font = _ressources.font },
            //                                                            new IntermediateScreen.TextLine() { Text = "Current Score", Pos = l4, Color = Color.Black, Font = _ressources.font },
            //                                                            new IntermediateScreen.TextLine() { Text = "" + (_score + Score), Pos = l5, Color = Color.Black, Font = _ressources.font } },
            //                                                            0f, 1f, 0f, Color.White, true, true);

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

            _currentScoreLabel.Text = "Current Score";
            _currentScoreLabel.Font = Load<SpriteFont>("InGameFont");
            _currentScoreLabel.Color = Color.Black;

            _scoreLabel.Text = _scoreText;
            _scoreLabel.Font = Load<SpriteFont>("InGameFont");
            _scoreLabel.Color = Color.Black;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float h = (float)Viewport.Height / 2f;

            CenterSubview(_headerLabel, (int)(-h / 2f));
            CenterSubview(_goalLabel, (int)(-h / 2f) + 60);
            CenterSubview(_tapToResumeLabel, (int)((-h / 2f) + h * .382f));
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

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
    public enum MenuPressedButton
    {
        Start,
        Resume,
        Highscore,
        Info
    }

    class MenuMainView : View
    {
        private Label _titleLabel;
        private Button _startButton, _resumeButton, _resumeSubButton, _highscoreButton, _helpButton, _infoButton;
        private Action _startOrResumePressed;
        private int _currentRound;

        public MenuPressedButton PressedButton;

        public MenuMainView(Action startOrResumePressed)
        {
            _startOrResumePressed = startOrResumePressed;
        }

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel);

            _startButton = new Button();
            AddSubview(_startButton);

            _resumeButton = new Button();
            AddSubview(_resumeButton);

            _resumeSubButton = new Button();
            AddSubview(_resumeSubButton);

            _highscoreButton = new Button();
            AddSubview(_highscoreButton);

            _helpButton = new Button();
            AddSubview(_helpButton);

            _infoButton = new Button();
            AddSubview(_infoButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Transparent;

            _titleLabel.Text = "Boom!";
            _titleLabel.Font = Load<SpriteFont>("TitleFont");
            _titleLabel.Color = Color.White;

            _startButton.Text = "Start";
            _startButton.Font = Load<SpriteFont>("MenuFont");
            _startButton.Color = Color.White;
            _startButton.Tap += _startButton_Tap;

            _resumeButton.Text = "Resume";
            _resumeButton.Font = Load<SpriteFont>("MenuFont");
            _resumeButton.Color = Color.White;
            _resumeButton.Tap += _resumeButton_Tap;

            _resumeSubButton.Font = Load<SpriteFont>("InGameSmallFont");
            _resumeSubButton.Color = Color.White;
            _resumeSubButton.Tap += _resumeButton_Tap;

            _highscoreButton.Text = "Highscore";
            _highscoreButton.Font = Load<SpriteFont>("MenuFont");
            _highscoreButton.Color = Color.White;
            _highscoreButton.Tap += _highscoreButton_Tap;

            _helpButton.Text = "Help";
            _helpButton.Font = Load<SpriteFont>("MenuFont");
            _helpButton.Color = Color.White;
            _helpButton.Tap += _helpButton_Tap;

            _infoButton.Text = "Info";
            _infoButton.Font = Load<SpriteFont>("MenuFont");
            _infoButton.Color = Color.White;
            _infoButton.Tap += _infoButton_Tap;
        }

        void _startButton_Tap(object sender)
        {
            PressedButton = MenuPressedButton.Start;
            _startOrResumePressed();
        }

        void _resumeButton_Tap(object sender)
        {
            PressedButton = MenuPressedButton.Resume;
            _startOrResumePressed();
        }

        void _highscoreButton_Tap(object sender)
        {
            PressedButton = MenuPressedButton.Highscore;
            Dismiss(true);
        }

        void _helpButton_Tap(object sender)
        {
            NavigationController.Navigate(new TutorialView(), true);
        }

        void _infoButton_Tap(object sender)
        {
            PressedButton = MenuPressedButton.Info;
            Dismiss(true);
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            if (_currentRound != GameSettings.CurrentRound)
            {
                NeedsRelayout = true;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            int menuItemGap = 100;
            int pos = -235;

            CenterSubview(_titleLabel, pos);
            pos += 170;

            _currentRound = GameSettings.CurrentRound;

            if (_currentRound < 2)
            {
                CenterSubview(_startButton, pos);
                pos += menuItemGap;

                _resumeButton.Visible = false;
                _resumeSubButton.Visible = false;
            }
            else
            {
                _resumeSubButton.Text = "Level " + _currentRound;

                pos -= 25;
                menuItemGap = 90;

                CenterSubview(_startButton, pos);
                pos += menuItemGap;

                CenterSubview(_resumeButton, pos);
                CenterSubview(_resumeSubButton, pos + 30);
                pos += menuItemGap;

                _resumeButton.Visible = true;
                _resumeSubButton.Visible = true;
            }

            CenterSubview(_highscoreButton, pos);
            pos += menuItemGap;

            CenterSubview(_helpButton, pos);
            pos += menuItemGap;

            CenterSubview(_infoButton, pos);
        }
    }
}

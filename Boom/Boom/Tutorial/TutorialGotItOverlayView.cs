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
    enum TutorialGotItResult
    {
        None,
        Start,
        Again
    }

    class TutorialGotItOverlayView : View
    {
        private Button _gotItButton, _letsStartButton, _againButton;

        public TutorialGotItResult Result;

        public override void Initialize()
        {
            base.Initialize();

            Result = TutorialGotItResult.None;

            _gotItButton = new Button();
            AddSubview(_gotItButton);

            _letsStartButton = new Button();
            AddSubview(_letsStartButton);

            _againButton = new Button();
            AddSubview(_againButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _gotItButton.Text = "I got it";
            _gotItButton.Font = Load<SpriteFont>("InGameBoldFont");
            _gotItButton.Color = Color.Black;
            _gotItButton.Tap += _gotItButton_Tap;

            _letsStartButton.Text = "Let's start!";
            _letsStartButton.Font = Load<SpriteFont>("InGameBoldFont");
            _letsStartButton.Color = Color.Black;
            _letsStartButton.Tap += _gotItButton_Tap;

            _againButton.Text = "again, please";
            _againButton.Font = Load<SpriteFont>("InGameFont");
            _againButton.Color = Color.Black;
            _againButton.AutoResize = false;
            _againButton.Height = 75;
            _againButton.Tap += _againButton_Tap;
        }

        void _againButton_Tap(object sender)
        {
            Result = TutorialGotItResult.Again;
            Dismiss(true);
        }

        void _gotItButton_Tap(object sender)
        {
            Result = TutorialGotItResult.Start;
            Dismiss(true);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Height = 200;
            Width = 200;
            Superview.CenterSubview(this, 0);

            CenterSubview(_gotItButton, -50);
            CenterSubview(_letsStartButton, -25);

            CenterSubview(_againButton, 70);
        }
    }
}

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
    class FailedScreenView : DismissOnTapView
    {
        private string _goalText;
        private Label _headerLabel, _goalLabel, _tapToRetryLabel;

        public FailedScreenView(string goalText)
        {
            _goalText = goalText;
        }

        public override void Initialize()
        {
            base.Initialize();

            _headerLabel = new Label();
            AddSubview(_headerLabel);

            _goalLabel = new Label();
            AddSubview(_goalLabel);

            _tapToRetryLabel = new Label();
            AddSubview(_tapToRetryLabel);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Black;

            _headerLabel.Text = "You failed!";
            _headerLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _headerLabel.Color = Color.Red;

            _goalLabel.Text = _goalText;
            _goalLabel.Font = Load<SpriteFont>("InGameFont");

            _tapToRetryLabel.Text = "Tap to retry";
            _tapToRetryLabel.Font = Load<SpriteFont>("InGameFont");
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float h = (float)Viewport.Height / 2f;

            CenterSubview(_headerLabel, (int)(-h / 2f));
            CenterSubview(_goalLabel, (int)(-h / 2f) + 60);
            CenterSubview(_tapToRetryLabel, 30);
        }
    }
}

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
    class StartScreenView : DismissOnTapView
    {
        private string _headerText, _goalText;
        private Label _headerLabel, _goalLabel, _tapToStartLabel;

        public StartScreenView(string headerText, string goalText) : base(true)
        {
            _headerText = headerText;
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
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float h = (float)Viewport.Height / 2f;

            CenterSubview(_headerLabel, (int)(-h / 2f));
            CenterSubview(_goalLabel, (int)(-h / 2f) + 60);
            CenterSubview(_tapToStartLabel, 30);
        }
    }
}

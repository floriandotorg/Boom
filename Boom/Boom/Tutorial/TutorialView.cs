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
    class TutorialView : Screen
    {
        private readonly bool _preGameTutorial;

        private Label _titleLabel;

        public TutorialView(bool preGameTutorial) : base(false)
        {
            _preGameTutorial = preGameTutorial;
        }

        public override void Initialize()
        {
            base.Initialize();

            _titleLabel = new Label();
            AddSubview(_titleLabel); 
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Transparent;

            _titleLabel.Text = "Tutorial";
            _titleLabel.Font = Load<SpriteFont>("InGameLargeFont");
            _titleLabel.Color = Color.White;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_titleLabel, -250);

            ShowOverlay(new TutorialRoundOverlayView(_preGameTutorial), true);
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            if (OverlayAnimationInfo != null && OverlayAnimationInfo.State == AnimationState.FadeOut)
            {
                SpriteBatch.Draw(Load<Texture2D>("Rectangle"), RectangleToSystem(Viewport.Bounds), Color.Black * (1f - OverlayAnimationInfo.Value));
            }
        }

        public override void OverlayWillDismiss(View overlay)
        {
            base.OverlayWillDismiss(overlay);

            if (_preGameTutorial && (overlay as TutorialRoundOverlayView).Result == TutorialGotItResult.Start)
            {
#if !DEBUG
                GameSettings.DidSeeTutorial = true;
#endif
                NavigationController.SwitchTo(new GameScreenView(1, 0), true);
            }
        }

        public override void OverlayDismissed(View overlay)
        {
            base.OverlayDismissed(overlay);

            ShowOverlay(new TutorialRoundOverlayView(_preGameTutorial), true);
        }

        public override Color ClearColor
        {
            get
            {
                if (Overlay != null)
                {
                    return Overlay.ClearColor;
                }
                else
                {
                    return base.ClearColor;
                }
            }
        }

        public override bool BackButtonPressed()
        {
            if (Overlay != null)
            {
                Overlay.Dismiss(true);
            }

            return base.BackButtonPressed();
        }
    }
}

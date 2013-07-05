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
    class TutorialRoundOverlayView : View, IRoundDelegate
    {
        private Label _tapHereLabel;
        private Round _round;
        private AnimationInfo _tapHereLabelAnimationInfo;

        private readonly bool _preGameTutorial;

        public TutorialGotItResult Result;

        public TutorialRoundOverlayView(bool preGameTutorial)
        {
            _preGameTutorial = preGameTutorial;
        }

        public override void Initialize()
        {
            base.Initialize();

            Result = TutorialGotItResult.None;

            _tapHereLabel = new Label();
            _tapHereLabel.Viewport = Viewport;
            _tapHereLabel.NavigationController = NavigationController;
            _tapHereLabel.Initialize();

            _tapHereLabelAnimationInfo = new AnimationInfo();
            _tapHereLabelAnimationInfo.FadeIn();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _tapHereLabel.LoadContent();

            _tapHereLabel.Text = "tap here";
            _tapHereLabel.Font = Load<SpriteFont>("InGameFont");

            _round = new Round(this);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _tapHereLabel.LayoutSubviews();

            CenterSubview(_tapHereLabel, 0);
            _tapHereLabel.Y = 265 + RoundSettings.NumBalls * (int)Ball.RadiusHugeSize - _tapHereLabel.Height / 2;
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            if (animationInfo.State == AnimationState.FadeOut && _tapHereLabelAnimationInfo.State != AnimationState.FadeOut)
            {
                _tapHereLabelAnimationInfo.FadeOut();
            }

            if (_tapHereLabelAnimationInfo.State != AnimationState.FadeOut && _round.Score != 0)
            {
                _tapHereLabelAnimationInfo.FadeOut();
            }

            if (_tapHereLabelAnimationInfo.State == AnimationState.FadeIn && _tapHereLabelAnimationInfo.Value.Inc())
            {
                _tapHereLabelAnimationInfo.State = AnimationState.Visible;
            }
            else if (_tapHereLabelAnimationInfo.State == AnimationState.FadeOut)
            {
                _tapHereLabelAnimationInfo.Value.Dec();
            }

            _tapHereLabel.Update(gameTime, _tapHereLabelAnimationInfo);

            if (_round.Update() && animationInfo.State == AnimationState.Visible && Overlay == null)
            {
                if (_preGameTutorial && _round.Score != 0)
                {
                    ShowOverlay(new TutorialGotItOverlayView(), true);
                }
                else
                {
                    Dismiss(true);
                }
            }
        }

        public override void OverlayWillDismiss(View overlay)
        {
            base.OverlayWillDismiss(overlay);

            Result = (overlay as TutorialGotItOverlayView).Result;

            Dismiss(true);
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            _tapHereLabel.Draw(gameTime, _tapHereLabelAnimationInfo);

            _round.Draw(SpriteBatch, animationInfo);
        }

        public override bool TouchInside(TouchLocation location)
        {
            Rectangle bounds = Viewport.Bounds;
            bounds.Height -= SpeakerButton.SpeakerIconSize + 30;
            return bounds.Contains(Utility.Vector2ToPoint(Vector2ToLocale(location.Position)));
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                _round.Touch(location);
            }

            return true;
        }

        public override Color ClearColor
        {
            get
            {
                return _round.BackgroundColor();
            }
        }

        #region Round Delegate

        public RoundSettings RoundSettings
        {
            get
            {
                return new RoundSettings(6, 6, 0, false);
            }
        }

        public int Score
        {
            get
            {
                return 0;
            }
        }

        public bool ShouldShowOverlays
        {
            get
            {
                return false;
            }
        }

        public bool IsScoreVisible
        {
            get
            {
                return false;
            }
        }

        public bool IsTutorial
        {
            get
            {
                return true;
            }
        }

        public Texture2D BallTexture
        {
            get
            {
                return Load<Texture2D>("BallTexture");
            }
        }

        public SoundEffect BlipSound
        {
            get
            {
                return Load<SoundEffect>("BlipSound");
            }
        }

        public SoundEffect VictorySound
        {
            get
            {
                return Load<SoundEffect>("VictorySound");
            }
        }

        public SpriteFont Font
        {
            get
            {
                return Load<SpriteFont>("InGameFont");
            }
        }

        public void ShowOverlay(View overlay)
        { }

        #endregion
    }
}

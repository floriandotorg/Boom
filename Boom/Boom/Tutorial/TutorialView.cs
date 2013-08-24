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
    class TutorialView : View, IRoundDelegate
    {
        private Round _round;

        private readonly bool _preGameTutorial;

        public TutorialView(bool preGameTutorial)
        {
            _preGameTutorial = preGameTutorial;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _round = new Round(this);
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            _round.Update();
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            _round.Draw(SpriteBatch, animationInfo);
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                _round.Touch(location);
            }

            return true;
        }

        public override void OverlayWillDismiss(View overlay)
        {
            base.OverlayWillDismiss(overlay);

            if (!_round.OverlayWillDismiss() && _preGameTutorial && _round.Score != 0)
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

            if (!_round.OverlayDismissed() && !_preGameTutorial)
            {
                _round = new Round(this);
            }
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
                return new RoundSettings(5, 5, 0, false);
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
                return true;
            }
        }

        public bool IsScoreVisible
        {
            get
            {
                return true;
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
                return Load<SpriteFont>("InGameScoreFont");
            }
        }

        public SpriteFont TapHereFont
        {
            get
            {
                return Load<SpriteFont>("InGameFont");
            }
        }

        public AnimationInfo RoundOverlayAnimationInfo
        {
            get
            {
                return (Overlay == null) ? null : OverlayAnimationInfo;
            }
        }

        public void ShowOverlay(View overlay)
        {
            ShowOverlay(overlay, true);
        }

        #endregion
    }
}

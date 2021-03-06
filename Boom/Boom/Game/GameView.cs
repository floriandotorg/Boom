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
    class GameView : View, IRoundDelegate
    {
#if DEBUG
        private readonly int NumRounds = 3;
#else
        private readonly int NumRounds = 12;
#endif

        private Round _round;
        private int _currentRoundNo;
        private int _score;

        public GameView(int startRound, int startScore)
        {
            _currentRoundNo = Math.Min(Math.Max(startRound, 1), NumRounds);
            _score = Math.Max(startScore, 0);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = Color.Transparent;

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

            AnimationInfo roundAnimationInfo = animationInfo;

            if (RoundOverlayAnimationInfo != null && RoundOverlayAnimationInfo.State == AnimationState.FadeIn)
            {
                roundAnimationInfo = RoundOverlayAnimationInfo;
            }

            if (animationInfo.State == AnimationState.FadeOut)
            {
                SpriteBatch.Draw(Load<Texture2D>("Rectangle"), RectangleToSystem(Viewport.Bounds), Color.Black * (1f - animationInfo.Value));
            }

            _round.Draw(SpriteBatch, roundAnimationInfo);
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

            _round.OverlayWillDismiss();
        }

        public override void OverlayDismissed(View overlay)
        {
            base.OverlayDismissed(overlay);

            if (!_round.OverlayDismissed())
            {
                nextRound();
            }
        }

        public override Color ClearColor
        {
            get
            {
                return _round.BackgroundColor();
            }
        }

        private void nextRound()
        {
            if (_round != null)
            {
                _score += _round.Score;
            }

            if (++_currentRoundNo > NumRounds)
            {
                GameSettings.CurrentRound = 1;
                GameSettings.CurrentScore = 0;

                ShowOverlay(new GameOverScreenView(_score), true);
            }
            else
            {
                GameSettings.CurrentRound = _currentRoundNo;
                GameSettings.CurrentScore = _score;
                
                _round = new Round(this);
            }
        }

        #region Round Delegate

        public RoundSettings RoundSettings
        {
#if DEBUG
            get
            {
                switch(_currentRoundNo)
                {
                    case 1: return new RoundSettings(10, 1, _currentRoundNo, false);
                    case 2: return new RoundSettings(10, 2, _currentRoundNo, false);
                    case 3: return new RoundSettings(10, 3, _currentRoundNo, true);
                    default: throw new InvalidOperationException();
                }
            }
#else
            get
            {
                switch(_currentRoundNo)
                {
                    case 1: return new RoundSettings(10, 1, _currentRoundNo, false);
                    case 2: return new RoundSettings(10, 2, _currentRoundNo, false);
                    case 3: return new RoundSettings(15, 3, _currentRoundNo, false);
                    case 4: return new RoundSettings(20, 5, _currentRoundNo, false);
                    case 5: return new RoundSettings(25, 10, _currentRoundNo, false);
                    case 6: return new RoundSettings(30, 15, _currentRoundNo, false);
                    case 7: return new RoundSettings(35, 20, _currentRoundNo, false);
                    case 8: return new RoundSettings(40, 27, _currentRoundNo, false);
                    case 9: return new RoundSettings(45, 33, _currentRoundNo, false);
                    case 10: return new RoundSettings(50, 40, _currentRoundNo, false);
                    case 11: return new RoundSettings(55, 48, _currentRoundNo, false);
                    case 12: default: return new RoundSettings(60, 55, _currentRoundNo, true);
                }
            }
#endif
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
                return false;
            }
        }

        public int Score
        {
            get
            {
                return _score;
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

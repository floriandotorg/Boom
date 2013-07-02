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
        private Round _round;

        public override void LoadContent()
        {
            base.LoadContent();

            _round = new Round(this);
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            if (_round.Update() && animationInfo.State == AnimationState.Visible)
            {
                Dismiss(true);
            }
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

        #region Round Delegate

        public RoundSettings RoundSettings
        {
            get
            {
                return new RoundSettings(8, 0, 0, false);
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

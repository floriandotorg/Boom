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
    class MenuView : View, IRoundDelegate
    {
        Round _round;
        SpeakerButton _speakerButton;

        public override void Initialize()
        {
            base.Initialize();

            _speakerButton = new SpeakerButton();
            AddSubview(_speakerButton);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _round = new Round(this);

            ShowOverlay(new MenuMainView(startOrResume), false);
        }

        private void startOrResume()
        {
            if ((Overlay as MenuMainView).PressedButton == MenuPressedButton.Start)
            {
                NavigationController.Navigate(new GameView(1, 0), true);
            }
            else
            {
                NavigationController.Navigate(new GameView(GameSettings.CurrentRound, GameSettings.CurrentScore), true);
            }
        }

        public override void OverlayDimissed(View overlay)
        {
            base.OverlayDimissed(overlay);

            if (overlay.GetType() == typeof(MenuMainView))
            {
                switch ((overlay as MenuMainView).PressedButton)
                {
                    case MenuPressedButton.Highscore:
                        ShowOverlay(new MenuHighscoreView(), true);
                        break;

                    case MenuPressedButton.Info:
                        ShowOverlay(new MenuInfoView(), true);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
            else if (overlay.GetType() == typeof(MenuInfoView) || overlay.GetType() == typeof(MenuHighscoreView))
            {
                ShowOverlay(new MenuMainView(startOrResume), true);
            }
        }

        public override bool BackButtonPressed()
        {
            if (!base.BackButtonPressed())
            {
                if (Overlay.GetType() == typeof(MenuInfoView) || Overlay.GetType() == typeof(MenuHighscoreView))
                {
                    Overlay.Dismiss(true);
                }
                else
                {
                    return false;
                }
            }

            return true;
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

            SpriteBatch.Draw(Load<Texture2D>("Rectangle"), RectangleToSystem(Viewport.Bounds), Color.Black * .7f);
        }

        #region Round Delegate

        public RoundSettings RoundSettings
        {
            get
            {
                return new RoundSettings(10, 10, 0, false);
            }
        }

        public int Score
        {
            get
            {
                return 0;
            }
        }

        public bool ShouldShowStartScreen
        {
            get
            {
                return false;
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

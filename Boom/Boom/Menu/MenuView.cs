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
    class MenuView : Screen, IRoundDelegate
    {
        Round _round;

        public MenuView() : base(false)
        { }

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
                if (GameSettings.DidSeeTutorial == false)
                {
                    NavigationController.Navigate(new TutorialView(true), true);
                }
                else
                {
                    NavigationController.Navigate(new GameView(1, 0), true);
                }
            }
            else
            {
                NavigationController.Navigate(new GameView(GameSettings.CurrentRound, GameSettings.CurrentScore), true);
            }
        }

        public override void OverlayDismissed(View overlay)
        {
            base.OverlayDismissed(overlay);

            if (overlay is MenuMainView)
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
            else if (overlay is MenuInfoView || overlay is MenuHighscoreView)
            {
                ShowOverlay(new MenuMainView(startOrResume), true);
            }
        }

        public override bool BackButtonPressed()
        {
            if (!base.BackButtonPressed())
            {
                if (Overlay is MenuInfoView || Overlay is MenuHighscoreView)
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

            SpriteBatch.Draw(Load<Texture2D>("Rectangle"), RectangleToSystem(Viewport.Bounds), Color.Black * .6f);
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

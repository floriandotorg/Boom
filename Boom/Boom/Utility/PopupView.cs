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
    class PopupView : View
    {
        private View _contentView;
        private readonly bool _userCanClose;
        private readonly float _heightPercentage;
        private bool _visible;

        public PopupView(View contentView, bool userCanClose, float heightPercentage)
        {
            _contentView = contentView;
            _userCanClose = userCanClose;
            _heightPercentage = heightPercentage;
            _visible = false;
        }

        public PopupView(View contentView) : this(contentView, true, 4f)
        { }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = new Color(70, 70, 70) * .8f;
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            _visible = animationInfo.State == AnimationState.Visible;

            if (animationInfo.State == AnimationState.Visible && Overlay == null)
            {
                ShowOverlay(_contentView, true);

                _contentView.Bounds = contentRectangle(animationInfo);
            }
        }

        private Rectangle contentRectangle(AnimationInfo animationInfo)
        {
            int height = Convert.ToInt32(((float)Height / _heightPercentage) * animationInfo.Value);
            return new Rectangle(0, Height / 2 - height / 2, Width, height);
        }

        private Rectangle crossRectangle(AnimationInfo animationInfo)
        {
            Rectangle rect = contentRectangle(animationInfo);
            Texture2D crossTexture = Load<Texture2D>("CrossTexture");
            return RectangleToSystem(new Rectangle(rect.X + Width - crossTexture.Width - 10, rect.Y + 10, crossTexture.Width, crossTexture.Height));
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            SpriteBatch.Draw(Load<Texture2D>("Rectangle"), contentRectangle(animationInfo), Color.Black);

            if (_userCanClose && Overlay != null)
            {
                SpriteBatch.Draw(Load<Texture2D>("CrossTexture"), crossRectangle(animationInfo), new Color(35, 35, 35) * OverlayAnimationInfo.Value);
            }
        }

        public override bool TouchDown(TouchLocation location)
        {
            try
            {
                if (!base.TouchDown(location))
                {
                    if (_visible)
                    {
                        Rectangle crossRect = crossRectangle(OverlayAnimationInfo);
                        crossRect.Inflate(20, 20);

                        if (_userCanClose && Overlay != null
                            && OverlayAnimationInfo.State == AnimationState.Visible
                            && (!contentRectangle(OverlayAnimationInfo).Contains(Utility.Vector2ToPoint(Vector2ToLocale(location.Position)))
                            || crossRect.Contains(Utility.Vector2ToPoint(location.Position))))
                        {
                            Overlay.Dismiss(true);
                        }
                    }
                }
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            return true;
        }

        public override void OverlayDismissed(View overlay)
        {
            base.OverlayDismissed(overlay);

            Dismiss(true);
        }

        public override bool BackButtonPressed()
        {
            if (!base.BackButtonPressed())
            {
                if (Overlay != null)
                {
                    Overlay.Dismiss(true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}

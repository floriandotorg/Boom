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

        public PopupView(View contentView)
        {
            _contentView = contentView;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            BackgroundColor = new Color(70, 70, 70) * .8f;
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            if (animationInfo.State == AnimationState.Visible && Overlay == null)
            {
                ShowOverlay(_contentView, true);

                _contentView.Bounds = contentRectangle(animationInfo);
            }
        }

        private Rectangle contentRectangle(AnimationInfo animationInfo)
        {
            int height = Convert.ToInt32(((float)Height / 4f) * animationInfo.Value);
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

            if (Overlay != null)
            {
                SpriteBatch.Draw(Load<Texture2D>("CrossTexture"), crossRectangle(animationInfo), new Color(35, 35, 35) * OverlayAnimationInfo.Value);
            }
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                if (Overlay != null && OverlayAnimationInfo.State == AnimationState.Visible
                    && (!contentRectangle(OverlayAnimationInfo).Contains(Utility.Vector2ToPoint(Vector2ToLocale(location.Position)))
                    || crossRectangle(OverlayAnimationInfo).Contains(Utility.Vector2ToPoint(location.Position))))
                {
                    Overlay.Dismiss(true);
                }
            }

            return true;
        }

        public override void OverlayDimissed(View overlay)
        {
            base.OverlayDimissed(overlay);

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

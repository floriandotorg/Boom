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
    class SpeakerButton : View
    {
        private const int SpeakerIconSize = 40;

        private Rectangle bounds()
        {
            return new Rectangle(Viewport.Width - SpeakerIconSize - 13, Viewport.Height - SpeakerIconSize - 10, SpeakerIconSize, SpeakerIconSize);
        }

        public override bool TouchInside(TouchLocation location)
        {
            return bounds().Contains(Utility.Vector2ToPoint(Superview.Vector2ToLocale(location.Position)));
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                if (VolumeManager.IsMute())
                {
                    VolumeManager.SetDefaultVolume();
                }
                else
                {
                    VolumeManager.Mute();
                }
            }

            return true;
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            Texture2D speaker;
            if (VolumeManager.IsMute())
            {
                speaker = Load<Texture2D>("SpeakerMuteTexture");
            }
            else
            {
                speaker = Load<Texture2D>("SpeakerTexture");
            }

            SpriteBatch.Draw(speaker, bounds(), new Color(128, 128, 128) * .5f * animationInfo.Value);

        }
    }
}

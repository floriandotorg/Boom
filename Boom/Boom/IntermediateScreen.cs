using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Boom
{
    class IntermediateScreen
    {
        public struct TextLine
        {
            public string Text;
            public int Pos;
            public Color Color;
            public SpriteFont Font;
        }

        private enum State
        {
            FadeIn,
            Visible,
            FadeOut,
            Finished
        }

        private const int NumFadingUpdates = 10;
        private const int SpeakerIconSize = 40;

        private Viewport _viewport;
        private BoomGame.RessourcesStruct _ressources;
        private State _state;
        private IEnumerable<TextLine> _lines;
        private float _from, _background, _to;
        private Color _backgroundColor;
        private Texture2D _rectTexture;
        private SineValue _fadeProcess = new SineValue(1, NumFadingUpdates);

        public IntermediateScreen(GraphicsDevice graphicsDevice, BoomGame.RessourcesStruct ressources)
        {
            _viewport = graphicsDevice.Viewport;
            _ressources = ressources;

            _rectTexture = new Texture2D(graphicsDevice, 1, 1);
            _rectTexture.SetData(new[] { Color.White });
        }

        public void Show(IEnumerable<TextLine> lines, float from, float background, float to, Color backgroundColor)
        {
            _state = State.FadeIn;
            _lines = lines;
            _from = from;
            _background = background;
            _to = to;
            _backgroundColor = backgroundColor;
        }

        public bool Update()
        {
            if (_state == State.FadeIn && _fadeProcess.Inc())
            {
                _state = State.Visible;
            }

            if (_state == State.FadeOut && _fadeProcess.Dec())
            {
                _state = State.Finished;
            }            

            if (_state == State.Finished)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Touch(TouchLocation touch)
        {
            if (touch.Position.X > _viewport.Width - SpeakerIconSize - 50 && touch.Position.Y > _viewport.Height - SpeakerIconSize - 50)
            {
                if (BoomGame.IsMute())
                {
                    BoomGame.SetDefaultVolume();
                }
                else
                {
                    BoomGame.Mute();
                }
            }
            else
            {
                if (_state == State.Visible)
                {
                    _state = State.FadeOut;
                    return true;
                }
            }
           
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = -1;

            if (_state == State.FadeIn)
            {
                alpha = _from - (float)_fadeProcess.Value * (_from - _background);
            }
            else if (_state == State.Visible)
            {
                alpha = _background;
            }
            else if (_state == State.FadeOut)
            {
                alpha = _to - (float)_fadeProcess.Value * (_to - _background);
            }
            else if (_state == State.Finished)
            {
                alpha = _to;
            }

            spriteBatch.Draw(_rectTexture, _viewport.Bounds, _backgroundColor * alpha);

            foreach (TextLine line in _lines)
            {
                Vector2 position = new Vector2((_viewport.Width - line.Font.MeasureString(line.Text).X) / 2, (_viewport.Height / 2) + line.Pos);
                spriteBatch.DrawString(line.Font, line.Text, position, line.Color * (float)_fadeProcess.Value);
            }

            Texture2D speaker;
            if (BoomGame.IsMute())
            {
                speaker = _ressources.speakerMuteTexture;
            }
            else
            {
                speaker = _ressources.speakerTexture;
            }

            spriteBatch.Draw(speaker, new Rectangle(_viewport.Width - SpeakerIconSize - 13, _viewport.Height - SpeakerIconSize - 10, SpeakerIconSize, SpeakerIconSize), new Color(128,128,128) * (.5f * (float)_fadeProcess.Value));
        }
    }
}

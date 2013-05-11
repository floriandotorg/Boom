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
        public interface IDrawable
        {
            void Draw(SpriteBatch spriteBatch, Viewport viewport, float fadeProcess);
            bool Touch(Viewport viewport, TouchLocation touch);
        }

        public struct TextLine : IDrawable
        {
            public string Text;
            public int Pos;
            public Color Color;
            public SpriteFont Font;
            public Action Tap;

            public void Draw(SpriteBatch spriteBatch, Viewport viewport, float fadeProcess)
            {
                Vector2 position = new Vector2((viewport.Width - Font.MeasureString(Text).X) / 2, (viewport.Height / 2) + Pos);
                spriteBatch.DrawString(Font, Text, position, Color * fadeProcess);
            }

            public bool Touch(Viewport viewport, TouchLocation touch)
            {
                var textSize = Font.MeasureString(Text);

                if (touch.Position.X > (viewport.Width / 2) - (textSize.X / 2) - 10 && touch.Position.X < (viewport.Width / 2) + (textSize.X / 2) + 10
                    && touch.Position.Y > (viewport.Height / 2) + Pos - 10 && touch.Position.Y < (viewport.Height / 2) + Pos + textSize.Y + 10)
                {
                    if (Tap != null)
                    {
                        Tap();
                        return true;
                    }
                }

                return false;
            }
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
        private bool _disappearOnTouch;
        private State _state;
        private IEnumerable<IDrawable> _drawables;
        private float _from, _background, _to;
        private Color _backgroundColor;
        private Texture2D _rectTexture;
        private SineValue _fadeProcess = new SineValue(1, NumFadingUpdates);

        public float To
        {
            get { return _to; }
            set { _to = value; }
        }

        public IntermediateScreen(GraphicsDevice graphicsDevice, BoomGame.RessourcesStruct ressources)
        {
            _viewport = graphicsDevice.Viewport;
            _ressources = ressources;

            _rectTexture = new Texture2D(graphicsDevice, 1, 1);
            _rectTexture.SetData(new[] { Color.White });
        }

        public void Show(IEnumerable<IDrawable> drawables, float from, float background, float to, Color backgroundColor, bool disappearOnTouch)
        {
            _state = State.FadeIn;
            _drawables = drawables;
            _from = from;
            _background = background;
            _to = to;
            _backgroundColor = backgroundColor;
            _disappearOnTouch = disappearOnTouch;
        }

        public void Hide()
        {
            _fadeProcess.Value = 1;
            _state = State.FadeOut;
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
            bool drawableHandledTouch = false;

            foreach (var drawable in _drawables)
            {
                if (drawable.Touch(_viewport, touch))
                {
                    drawableHandledTouch = true;
                    break;
                }
            }

            if (!drawableHandledTouch)
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
                    if (_disappearOnTouch && _state == State.Visible)
                    {
                        _state = State.FadeOut;
                        return true;
                    }
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

            foreach (var drawable in _drawables)
            {
                drawable.Draw(spriteBatch, _viewport, (float)_fadeProcess.Value);
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

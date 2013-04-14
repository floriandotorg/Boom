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

        private Viewport _viewport;
        private State _state;
        private int _updateCounter;
        private IEnumerable<TextLine> _lines;
        private float _from, _background, _to;
        private Color _backgroundColor;
        private Texture2D _rectTexture;

        public IntermediateScreen(GraphicsDevice graphicsDevice)
        {
            _viewport = graphicsDevice.Viewport;

            _rectTexture = new Texture2D(graphicsDevice, 1, 1);
            _rectTexture.SetData(new[] { Color.White });
        }

        public void Show(IEnumerable<TextLine> lines, float from, float background, float to, Color backgroundColor)
        {
            _state = State.FadeIn;
            _updateCounter = 0;
            _lines = lines;
            _from = from;
            _background = background;
            _to = to;
            _backgroundColor = backgroundColor;
        }

        public bool Update()
        {
            if (_state == State.FadeIn && ++_updateCounter >= NumFadingUpdates)
            {
                _state = State.Visible;
            }

            if (_state == State.FadeOut && --_updateCounter <= 0)
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
            if (_state == State.Visible)
            {
                _state = State.FadeOut;
                _updateCounter = NumFadingUpdates;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float fadeProcess = (float)_updateCounter / (float)NumFadingUpdates;

            float alpha = -1;

            if (_state == State.FadeIn)
            {
                alpha = _from - fadeProcess * (_from - _background);
            }
            else if (_state == State.Visible)
            {
                alpha = _background;
            }
            else if (_state == State.FadeOut)
            {
                alpha = _to - fadeProcess * (_to - _background);
            }
            else if (_state == State.Finished)
            {
                alpha = _to;
            }

            spriteBatch.Draw(_rectTexture, _viewport.Bounds, _backgroundColor * alpha);

            foreach (TextLine line in _lines)
            {
                Vector2 position = new Vector2((_viewport.Width - line.Font.MeasureString(line.Text).X) / 2, (_viewport.Height / 2) + line.Pos);
                spriteBatch.DrawString(line.Font, line.Text, position, line.Color * fadeProcess);
            }
        }
    }
}

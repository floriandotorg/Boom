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

        private Viewport _viewport;
        private State _state;
        private IEnumerable<TextLine> _lines;

        public IntermediateScreen(Viewport viewport)
        {
            _viewport = viewport;
        }

        public void Show(IEnumerable<TextLine> lines)
        {
            _state = State.Visible;
            _lines = lines;
        }

        public bool Update()
        {
            if (_state == State.Finished)
            {
                return true;
            }

            return false;
        }

        public void Touch(TouchLocation touch)
        {
            _state = State.Finished;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (TextLine line in _lines)
            {
                Vector2 position = new Vector2((_viewport.Width - line.Font.MeasureString(line.Text).X) / 2, (_viewport.Height / 2) + line.Pos);
                spriteBatch.DrawString(line.Font, line.Text, position, line.Color);
            }
        }
    }
}

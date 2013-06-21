using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    class HighscoreTable : IntermediateScreen.IDrawable
    {
        private const int ListLenght = 10;

        private class Entry
        {
            public string Name;
            public int Score;
            public bool User;
        }

        private SpriteFont _font, _userFont;
        private Highscore _highscore = new Highscore();
        private List<Entry> _entries = new List<Entry>();
        private int _pos, _height, _width, _userScore;
        private string _text, _userName;
        private Entry _userEntry;

        public HighscoreTable(int pos, int height, int width, int userScore, SpriteFont font, SpriteFont userFont)
        {
            _pos = pos;
            _height = height;
            _width = width;
            _userScore = userScore;
            _font = font;
            _userFont = userFont;

            LoadScores();
        }

        private void LoadScores()
        {
            _text = "loading ..";

            _highscore.GetScores(ListLenght,
                scores =>
                {
                    foreach (var score in scores)
                    {
                        if (_userEntry == null && _userScore > score.Value)
                        {
                            _userEntry = new Entry() { Name = "Tap to enter name", Score = _userScore, User = true };
                            _entries.Add(_userEntry);
                        }
                        if (_entries.Count >= ListLenght)
                        {
                            break;
                        }
                        _entries.Add(new Entry() { Name = score.Key, Score = score.Value, User = false });
                        if (_entries.Count >= ListLenght)
                        {
                            break;
                        }
                    }

                    if (_userEntry == null && _entries.Count < ListLenght && _userScore > 0)
                    {
                        _userEntry = new Entry() { Name = "Tap to enter name", Score = _userScore, User = true };
                        _entries.Add(_userEntry);
                    }

                    if (_entries.Count == 0)
                    {
                        _text = "no scores";
                    }
                },
                error =>
                {
                    _text = "unable to load highscores";
                });
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport, float fadeProcess)
        {
            if (_entries.Count == 0)
            {
                Vector2 position = new Vector2((viewport.Width - _font.MeasureString(_text).X) / 2, (viewport.Height / 2) + _pos + (_height / 2));
                spriteBatch.DrawString(_font, _text, position, Color.White * fadeProcess);

                if (_text == "unable to load highscores")
                {
                    string text = "tap to retry";
                    position = new Vector2((viewport.Width - _font.MeasureString(text).X) / 2, (viewport.Height / 2) + _pos + (_height / 2) + 60);
                    spriteBatch.DrawString(_font, text, position, Color.White * fadeProcess);
                }
            }
            else
            {
                int gap = _height / ListLenght;
                int n = 0;

                foreach (var entry in _entries)
                {
                    Color color = Color.White;
                    SpriteFont font = _font;

                    if (entry.User)
                    {
                        color = Color.Red;
                        font = _userFont;
                    }

                    Vector2 position = new Vector2((viewport.Width / 2) - (_width / 2), (viewport.Height / 2) + _pos + ++n * gap);
                    spriteBatch.DrawString(font, entry.Name, position, color * fadeProcess);

                    string val = Convert.ToString(entry.Score);
                    position = new Vector2((viewport.Width / 2) + (_width / 2) - font.MeasureString(val).X, (viewport.Height / 2) + _pos + n * gap);
                    spriteBatch.DrawString(font, val, position, color * fadeProcess);
                }
            }
        }

        public bool Touch(Viewport viewport, TouchLocation touch)
        {
            if (touch.Position.X > (viewport.Width  / 2) - (_width / 2) && touch.Position.X < (viewport.Width  / 2) + (_width / 2)
                && touch.Position.Y > (viewport.Height / 2) + _pos && touch.Position.Y < (viewport.Height / 2) + _pos + _height)
            {
                if (_entries.Count > 0 && _userEntry != null)
                {
                    ShowKeyboardInput("");
                    return true;
                }
                else if (_entries.Count == 0)
                {
                    LoadScores();
                    return true;
                }
                
            }
            return false;
        }

        void ShowKeyboardInput(string text)
        {
            Guide.BeginShowKeyboardInput(PlayerIndex.One, "Highscore", "Enter your name:", text, new AsyncCallback(OnEndShowKeyboardInput), null);
        }

        void OnEndShowMessageBox(IAsyncResult result)
        {
            Guide.EndShowMessageBox(result);
            while (Guide.IsVisible)
            {
                Thread.Sleep(1);
            }

            ShowKeyboardInput(_userName);
        }

        void OnEndShowKeyboardInput(IAsyncResult result)
        {
            string name = Guide.EndShowKeyboardInput(result).Trim();
            while (Guide.IsVisible)
            {
                Thread.Sleep(1);
            }

            _userName = name;

            if (name != null)
            {
                Regex regex = new Regex("^[A-Za-z0-9]+(([A-Za-z0-9]|-|_| )?[A-Za-z0-9]+)*$");

                if (name.Length < 2 || name.Length > 14 || !regex.IsMatch(name))
                {
                    Guide.BeginShowMessageBox("Error", "Your name must be at least 2 characters long and may contain letters (a-z), numbers (0-9), spaces, underscores (_) and dashes (-).\nIt can be up to 14 characters.", new string[] { "Retry" }, 0, MessageBoxIcon.None, new AsyncCallback(OnEndShowMessageBox), null);
                }
                else
                {
                    _userEntry.Name = name;
                    _userEntry.User = false;
                    _userEntry = null;

                    _highscore.Submit(error => System.Diagnostics.Debug.WriteLine(error), name, _userScore);
                }
            }
        }
    }
}

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
using Pages;

namespace Boom
{
    class HighscoreTableView : View
    {
        private const int ListLenght = 10;

        private class Entry
        {
            public string Name;
            public int Score;
            public bool User;
        }

        private Highscore _highscore;
        private List<Entry> _entries;
        private int  _userScore;
        private string _text, _userName;
        private Entry _userEntry;

        public HighscoreTableView(int userScore)
        {
            _userScore = userScore;
        }

        public override void Initialize()
        {
            base.Initialize();

            _highscore = new Highscore();
            _entries = new List<Entry>();
        }

        public override void LoadContent()
        {
            base.LoadContent();

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

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            if (_entries.Count == 0)
            {
                Vector2 position = new Vector2((Width - Load<SpriteFont>("InGameFont").MeasureString(_text).X) / 2, (Height / 2));
                SpriteBatch.DrawString(Load<SpriteFont>("InGameFont"), _text, Vector2ToSystem(position), Color.White * animationInfo.Value);

                if (_text == "unable to load highscores")
                {
                    string text = "tap to retry";
                    position = new Vector2((Width - Load<SpriteFont>("InGameFont").MeasureString(text).X) / 2, (Height / 2) + 60);
                    SpriteBatch.DrawString(Load<SpriteFont>("InGameFont"), text, Vector2ToSystem(position), Color.White * animationInfo.Value);
                }
            }
            else
            {
                int gap = Height / ListLenght;
                int n = 0;

                foreach (var entry in _entries)
                {
                    Color color = Color.White;
                    SpriteFont font = Load<SpriteFont>("InGameFont");

                    if (entry.User)
                    {
                        color = Color.Red;
                        font = Load<SpriteFont>("InGameBoldFont");
                    }

                    Vector2 position = new Vector2(0, ++n * gap);
                    SpriteBatch.DrawString(font, entry.Name, Vector2ToSystem(position), color * animationInfo.Value);

                    string val = Convert.ToString(entry.Score);
                    position = new Vector2(Width - font.MeasureString(val).X, + n * gap);
                    SpriteBatch.DrawString(font, val, Vector2ToSystem(position), color * animationInfo.Value);
                }
            }
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
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

            return true;
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

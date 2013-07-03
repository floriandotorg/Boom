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
using Scoreoid.Kit;

namespace Boom
{
    public enum HighscoreTableViewState
    {
        Loading,
        Loaded,
        UnableToLoad
    }

    class HighscoreTableView : View
    {
        private readonly int ListLenght = 10;

        private class Entry
        {
            public string Name;
            public int Score;
            public bool NewScore;
            public bool UserScore;
        }

        public HighscoreTableViewState State;

        public bool HasUserScore
        {
            get
            {
                return _userEntry != null;
            }
        }

        public int UserRank
        {
            get
            {
                return _entries.IndexOf(_userEntry) + 1;
            }
        }

        private List<Entry> _entries;
        private Entry _userEntry;
        private HighscoreTabView _highscoreTabView;
        private Action<Action<IEnumerable<Score>>, Action<SKError>> _loadHighscoreAction;

        public HighscoreTableView(HighscoreTabView highscoreTabView, Action<Action<IEnumerable<Score>>, Action<SKError>> loadHighscoreAction)
        {
            _highscoreTabView = highscoreTabView;
            _loadHighscoreAction = loadHighscoreAction;
            State = HighscoreTableViewState.Loading;
        }

        public override void Initialize()
        {
            base.Initialize();

            _entries = new List<Entry>();
        }

        public void LoadScores()
        {
            _loadHighscoreAction(
                scores =>
                {
                    _userEntry = null;
                    _entries.Clear();

                    foreach (var score in scores)
                    {
                        if (_userEntry == null && _highscoreTabView.UserScore != null && _highscoreTabView.UserScore > score.Value)
                        {
                            _userEntry = new Entry() { Name = "Tap to enter name", Score = _highscoreTabView.UserScore.Value, NewScore = true, UserScore = true };
                            _entries.Add(_userEntry);
                        }

                        if (_entries.Count >= ListLenght)
                        {
                            break;
                        }

                        _entries.Add(new Entry() { Name = score.Name, Score = score.Value, NewScore = false, UserScore = score.IsLocalePlayer });
                        
                        if (_entries.Count >= ListLenght)
                        {
                            break;
                        }
                    }

                    if (_userEntry == null && _entries.Count < ListLenght && _highscoreTabView.UserScore != null)
                    {
                        _userEntry = new Entry() { Name = "Tap to enter name", Score = _highscoreTabView.UserScore.Value, NewScore = true, UserScore = true };
                        _entries.Add(_userEntry);
                    }

                    State = HighscoreTableViewState.Loaded;
                    _highscoreTabView.StateChanged();
                },
                error =>
                {
                    State = HighscoreTableViewState.UnableToLoad;
                    _highscoreTabView.StateChanged();
                });
        }

        public override void Draw(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Draw(gameTime, animationInfo);

            if (_entries.Count == 0)
            {
                string text = "no scores";
                Vector2 position = new Vector2((Width - Load<SpriteFont>("InGameFont").MeasureString(text).X) / 2, (Height / 2));
                SpriteBatch.DrawString(Load<SpriteFont>("InGameFont"), text, Vector2ToSystem(position), Color.White * animationInfo.Value);
            }
            else
            {
                int gap = Height / ListLenght;
                int n = 0;

                foreach (var entry in _entries)
                {
                    Color color = Color.White;
                    SpriteFont font = Load<SpriteFont>("InGameFont");

                    if (entry.UserScore)
                    {
                        font = Load<SpriteFont>("InGameBoldFont");
                    }

                    if (entry.NewScore)
                    {
                        color = Color.Red;
                    }

                    Vector2 position = new Vector2(0, n * gap);
                    SpriteBatch.DrawString(font, entry.Name, Vector2ToSystem(position), color * animationInfo.Value);

                    string val = Convert.ToString(entry.Score);
                    position = new Vector2(Width - font.MeasureString(val).X, + n * gap);
                    SpriteBatch.DrawString(font, val, Vector2ToSystem(position), color * animationInfo.Value);

                    ++n;
                }
            }
        }
    }
}

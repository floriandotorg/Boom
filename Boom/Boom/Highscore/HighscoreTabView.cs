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
    class HighscoreTabView : View
    {
        private const int MaxInitHighscoreAttemps = 3;

        private HighscoreTableView _dailyTable, _weeklyTable, _allTimeTable;
        private Button _dailyButton, _weeklyButton, _allTimeButton;
        private Label _loadingLabel, _unableToLoadLabel, _tapToRetryLabel;

        private int _initHighscoreAttemps;
        public int UserScore;
        private string _userName;

        public HighscoreTabView(int userScore)
        {
            UserScore = userScore;
        }

        private void unableToLoad()
        {
            _dailyTable.Visible = false;
            _weeklyTable.Visible = false;
            _allTimeTable.Visible = false;
            _dailyButton.Visible = false;
            _weeklyButton.Visible = false;
            _allTimeButton.Visible = false;
            _loadingLabel.Visible = false;

            _unableToLoadLabel.Visible = true;
            _tapToRetryLabel.Visible = true;
        }

        public void InitHighscore()
        {
            if (Highscore.State == HighscoreState.NotInitialized)
            {
                if (_initHighscoreAttemps < MaxInitHighscoreAttemps)
                {
                    ++_initHighscoreAttemps;
                    Highscore.Initialize(error => { if (error != null) System.Diagnostics.Debug.WriteLine(error.LocalizedDescription); else System.Diagnostics.Debug.WriteLine("Highscore sccuess"); });
                }
                else
                {
                    unableToLoad();
                }
            }

            if (Highscore.State == HighscoreState.Initializing)
            {
                PerformActionAfterDelay(InitHighscore, TimeSpan.FromMilliseconds(500));
            }
            else
            {
                LoadScores();
            }
        }

        public void LoadScores()
        {
            if (Highscore.State != HighscoreState.Initialized)
            {
                InitHighscore();
            }
            else
            {
                _dailyTable.LoadScores();
                _weeklyTable.LoadScores();
                _allTimeTable.LoadScores();
            }
        }

        public void StateChanged()
        {
            if (_dailyTable.State == HighscoreTableViewState.UnableToLoad ||
                _weeklyTable.State == HighscoreTableViewState.UnableToLoad ||
                _allTimeTable.State == HighscoreTableViewState.UnableToLoad)
            {
                unableToLoad();
            }
            else if (_dailyTable.State == HighscoreTableViewState.Loaded &&
                     _weeklyTable.State == HighscoreTableViewState.Loaded &&
                     _allTimeTable.State == HighscoreTableViewState.Loaded)
            {
                _loadingLabel.Visible = false;
                _unableToLoadLabel.Visible = false;
                _tapToRetryLabel.Visible = false;

                _dailyButton.Visible = true;
                _weeklyButton.Visible = true;
                _allTimeButton.Visible = true;

                if (!_allTimeTable.Visible && !_weeklyTable.Visible && !_dailyTable.Visible)
                {
                    if (_allTimeTable.HasUserScore)
                    {
                        switchToAllTime(null);
                    }
                    else if (_weeklyTable.HasUserScore)
                    {
                        switchToWeekly(null);
                    }
                    else
                    {
                        switchToDaily(null);
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            _dailyTable = new HighscoreTableView(this, Highscore.LoadLast24HoursScores);
            AddSubview(_dailyTable);

            _weeklyTable = new HighscoreTableView(this, Highscore.LoadLast7DaysScores);
            AddSubview(_weeklyTable);

            _allTimeTable = new HighscoreTableView(this, Highscore.LoadAllTimeScores);
            AddSubview(_allTimeTable);

            _dailyButton = new Button();
            AddSubview(_dailyButton);

            _weeklyButton = new Button();
            AddSubview(_weeklyButton);

            _allTimeButton = new Button();
            AddSubview(_allTimeButton);

            _loadingLabel = new Label();
            AddSubview(_loadingLabel);

            _unableToLoadLabel = new Label();
            AddSubview(_unableToLoadLabel);

            _tapToRetryLabel = new Label();
            AddSubview(_tapToRetryLabel);

            LoadScores();
        }

        private void switchToDaily(object sender)
        {
            _dailyTable.Visible = true;
            _weeklyTable.Visible = false;
            _allTimeTable.Visible = false;

            _dailyButton.Color = Color.White;
            _weeklyButton.Color = Color.Gray;
            _allTimeButton.Color = Color.Gray;

            _dailyButton.Font = Load<SpriteFont>("InGameBoldFont");
            _weeklyButton.Font = Load<SpriteFont>("InGameFont");
            _allTimeButton.Font = Load<SpriteFont>("InGameFont");

            NeedsRelayout = true;
        }

        private void switchToWeekly(object sender)
        {
            _dailyTable.Visible = false;
            _weeklyTable.Visible = true;
            _allTimeTable.Visible = false;

            _dailyButton.Color = Color.Gray;
            _weeklyButton.Color = Color.White;
            _allTimeButton.Color = Color.Gray;

            _dailyButton.Font = Load<SpriteFont>("InGameFont");
            _weeklyButton.Font = Load<SpriteFont>("InGameBoldFont");
            _allTimeButton.Font = Load<SpriteFont>("InGameFont");

            NeedsRelayout = true;
        }

        private void switchToAllTime(object sender)
        {
            _dailyTable.Visible = false;
            _weeklyTable.Visible = false;
            _allTimeTable.Visible = true;

            _dailyButton.Color = Color.Gray;
            _weeklyButton.Color = Color.Gray;
            _allTimeButton.Color = Color.White;

            _dailyButton.Font = Load<SpriteFont>("InGameFont");
            _weeklyButton.Font = Load<SpriteFont>("InGameFont");
            _allTimeButton.Font = Load<SpriteFont>("InGameBoldFont");

            NeedsRelayout = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _dailyTable.Visible = false;
            _weeklyTable.Visible = false;
            _allTimeTable.Visible = false;

            _dailyButton.AutoResize = false;
            _dailyButton.HorizontalAlignment = HorizontalAlignment.Center;
            _dailyButton.VerticalAlignment = VerticalAlignment.Center;
            _dailyButton.Text = "Today";
            _dailyButton.Tap += switchToDaily;
            _dailyButton.Font = Load<SpriteFont>("InGameFont");
            _dailyButton.Visible = false;

            _weeklyButton.AutoResize = false;
            _weeklyButton.HorizontalAlignment = HorizontalAlignment.Center;
            _weeklyButton.VerticalAlignment = VerticalAlignment.Center;
            _weeklyButton.Text = "This Week";
            _weeklyButton.Tap += switchToWeekly;
            _weeklyButton.Font = Load<SpriteFont>("InGameFont");
            _weeklyButton.Visible = false;

            _allTimeButton.AutoResize = false;
            _allTimeButton.HorizontalAlignment = HorizontalAlignment.Center;
            _allTimeButton.VerticalAlignment = VerticalAlignment.Center;
            _allTimeButton.Text = "All Time";
            _allTimeButton.Tap += switchToAllTime;
            _allTimeButton.Font = Load<SpriteFont>("InGameFont");
            _allTimeButton.Visible = false;

            _loadingLabel.Text = "loading ..";
            _loadingLabel.Font = Load<SpriteFont>("InGameFont");

            _unableToLoadLabel.Text = "unable to load scores";
            _unableToLoadLabel.Font = Load<SpriteFont>("InGameFont");
            _unableToLoadLabel.Visible = false;

            _tapToRetryLabel.Text = "tap to retry";
            _tapToRetryLabel.Font = Load<SpriteFont>("InGameFont");
            _tapToRetryLabel.Visible = false;
        }

        private void layoutTable(View table)
        {
            table.X = 10;
            table.Y = 0;
            table.Width = Width - table.X * 2;
            table.Height = Height - 70;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            layoutTable(_dailyTable);
            layoutTable(_weeklyTable);
            layoutTable(_allTimeTable);

            int buttonWidth = Convert.ToInt32((float)Width / 3f);

            _dailyButton.X = 0;
            _dailyButton.Y = Height - 75;
            _dailyButton.Height = 75;
            _dailyButton.Width = buttonWidth;

            _weeklyButton.X = buttonWidth;
            _weeklyButton.Y = Height - 75;
            _weeklyButton.Height = 75;
            _weeklyButton.Width = buttonWidth;

            _allTimeButton.X = buttonWidth * 2;
            _allTimeButton.Y = Height - 75;
            _allTimeButton.Height = 75;
            _allTimeButton.Width = buttonWidth;

            CenterSubview(_loadingLabel, 0);
            CenterSubview(_unableToLoadLabel, 0);
            CenterSubview(_tapToRetryLabel, 60);
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                if (_tapToRetryLabel.Visible)
                {
                    LoadScores();
                }
                else if ((_dailyTable.Visible && _dailyTable.HasUserScore) ||
                    (_weeklyTable.Visible && _weeklyTable.HasUserScore) ||
                    (_allTimeTable.Visible && _allTimeTable.HasUserScore))
                {
                    ShowKeyboardInput(GameSettings.HighscoreLastUsername);
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
            string name = Guide.EndShowKeyboardInput(result);
            while (Guide.IsVisible)
            {
                Thread.Sleep(1);
            }

            _userName = "";

            if (name != null)
            {
                name = name.Trim();
                _userName = name;

                Regex regex = new Regex("^[A-Za-z0-9]+(([A-Za-z0-9]|-|_| )?[A-Za-z0-9]+)*$");

                if (name.Length < 2 || name.Length > 14 || !regex.IsMatch(name))
                {
                    Guide.BeginShowMessageBox("Error", "Your name must be at least 2 characters long and may contain letters (a-z), numbers (0-9), spaces, underscores (_) and dashes (-).\nIt can be up to 14 characters.", new string[] { "Edit" }, 0, MessageBoxIcon.None, new AsyncCallback(OnEndShowMessageBox), null);
                }
                else
                {
                    Highscore.SubmitScore(name, UserScore,
                        error =>
                        {
                            if (error != null)
                            {
                                System.Diagnostics.Debug.WriteLine(error);
                            }
                            else
                            {
                                GameSettings.HighscoreLastUsername = name;
                                LoadScores();
                            }
                        });
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreoid.Kit;

namespace Boom
{
    struct Score
    {
        public string Name;
        public int Value;
        public bool IsLocalePlayer;

        public Score(string name, int value, bool isLocalePlayer)
        {
            Name = name;
            Value = value;
            IsLocalePlayer = isLocalePlayer;
        }
    }

    public enum HighscoreState
    {
        NotInitialized,
        Initializing,
        Initialized
    }

    class Highscore
    {
        public const string LeaderboardID = "1";

        public const string Apikey = "a59a292915406d8b865c894c9ed5eef94a7c12e3";
        public const string GameID = "f106029924";
        public const string Platform = "WP7";

        public const string SecurityKey = "3d5fa25b";

        private static HighscoreState _state = HighscoreState.NotInitialized;
        public static HighscoreState State
        {
            get
            {
                return _state;
            }
        }

        public static string PlayerName
        {
            get
            {
                _ensureInitialized();
                return _player.PlayerData.FirstName;
            }
        }

        private static SKLocalPlayer _player;

        public static void Initialize(Action<SKError> callback)
        {
            if (_state != HighscoreState.NotInitialized)
            {
                throw new InvalidOperationException("Highscore already initialized or initializing");
            }

            _state = HighscoreState.Initializing;

            SKSettings.Apikey = Apikey;
            SKSettings.GameID = GameID;
            SKSettings.SecurityKey = SecurityKey;
            SKSettings.Platform = Platform;
            SKSettings.PlayerUsername = GameSettings.HighscorePlayerID;

            if (String.IsNullOrEmpty(GameSettings.HighscorePlayerID))
            {
                GameSettings.HighscorePlayerID = Guid.NewGuid().ToString();
                SKSettings.PlayerUsername = GameSettings.HighscorePlayerID;
            }

            _player = SKLocalPlayer.CreatePlayer();

            _player.Authenticate(
                error =>
                {
                    if (error != null)
                    {
                        if (error.ErrorCode == SKErrorCode.PlayerNotFound)
                        {
                            _player.Create(
                                create_error =>
                                {
                                    if (create_error != null)
                                    {
                                        _state = HighscoreState.NotInitialized;
                                        callback(create_error);
                                    }
                                    else
                                    {
                                        _player.Authenticate(authenticate_error =>
                                            {
                                                if (authenticate_error != null)
                                                {
                                                    _state = HighscoreState.NotInitialized;
                                                    callback(authenticate_error);
                                                }
                                                else
                                                {
                                                    _state = HighscoreState.Initialized;
                                                    callback(null);
                                                }
                                            });
                                    }
                                });
                        }
                        else
                        {
                            _state = HighscoreState.NotInitialized;
                            callback(error);
                        }
                    }
                    else
                    {
                        _state = HighscoreState.Initialized;
                        callback(null);
                    }
                });
        }

        private static void _ensureInitialized()
        {
            if (_state != HighscoreState.Initialized)
            {
                throw new InvalidOperationException("call Initialize first");
            }
        }

        public static void SubmitScore(string name, int score, Action<SKError> callback)
        {
            _ensureInitialized();

            SKScore skScore = new SKScore(LeaderboardID);
            skScore.Value = score;
            skScore.Data = name;

            skScore.Submit(
                error =>
                {
                    if (error != null)
                    {
                        callback(error);
                    }
                    else
                    {
                        callback(null);
                    }
                });
        }

        private static void loadScores(SKLeaderboard leaderboard, Action<IEnumerable<Score>> success, Action<SKError> failed)
        {
            leaderboard.LoadScores(
                (scores, error) =>
                {
                    if (error != null)
                    {
                        failed(error);
                    }
                    else
                    {
                        var scoreList = new List<Score>();

                        foreach (var score in scores)
                        {
                            scoreList.Add(new Score(score.Data, score.Value, score.Username == _player.Username));
                        }

                        success(scoreList);
                    }
                });
        }

        public static void LoadAllTimeScores(Action<IEnumerable<Score>> success, Action<SKError> failed)
        {
            _ensureInitialized();

            SKLeaderboard leaderboard = new SKLeaderboard(LeaderboardID);

            leaderboard.OrderBy = "score";
            leaderboard.Direction = "desc";
            leaderboard.PageSize = 10;

            loadScores(leaderboard, success, failed);
        }

        public static void LoadLast7DaysScores(Action<IEnumerable<Score>> success, Action<SKError> failed)
        {
            _ensureInitialized();

            SKLeaderboard leaderboard = new SKLeaderboard(LeaderboardID);

            leaderboard.OrderBy = "score";
            leaderboard.Direction = "desc";
            leaderboard.PageSize = 10;

            leaderboard.FromDate = DateTime.Now.AddDays(-7).ToUniversalTime();
            leaderboard.ToDate = DateTime.Now.ToUniversalTime();

            loadScores(leaderboard, success, failed);
        }

        public static void LoadLast24HoursScores(Action<IEnumerable<Score>> success, Action<SKError> failed)
        {
            _ensureInitialized();

            SKLeaderboard leaderboard = new SKLeaderboard(LeaderboardID);

            leaderboard.OrderBy = "score";
            leaderboard.Direction = "desc";
            leaderboard.PageSize = 10;

            leaderboard.FromDate = DateTime.Now.AddHours(-24).ToUniversalTime();
            leaderboard.ToDate = DateTime.Now.ToUniversalTime();

            loadScores(leaderboard, success, failed);
        }
    }
}

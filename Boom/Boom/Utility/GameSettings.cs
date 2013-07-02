using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pages;

namespace Boom
{
    class GameSettings : GameSettings<GameSettings>
    {
        public const string RemoveAdsProductId = "RemoveAds";

        private const string SpeakerSettingsKey = "Speaker";
        private const string CurrentRoundSettingsKey = "CurrentRound";
        private const string CurrentScoreSettingsKey = "CurrentScore";
        private const string HighscorePlayerIDSettingsKey = "HighscorePlayerIDSettingsKey";
        private const string HighscoreLastUsernameSettingsKey = "HighscoreLastUsername";

        protected override void Initialize()
        {
            base.Initialize();

            AddSetting(SpeakerSettingsKey, true);
            AddSetting(CurrentRoundSettingsKey, 1);
            AddSetting(CurrentScoreSettingsKey, 0);
            AddSetting(HighscorePlayerIDSettingsKey, "");
            AddSetting(HighscoreLastUsernameSettingsKey, "");
        }

        public static bool Speaker
        {
            get
            {
                return (bool)Get(SpeakerSettingsKey);
            }
            set
            {
                Set(SpeakerSettingsKey, value);
            }
        }

        public static int CurrentRound
        {
            get
            {
                return (int)Get(CurrentRoundSettingsKey);
            }
            set
            {
                Set(CurrentRoundSettingsKey, value);
            }
        }

        public static int CurrentScore
        {
            get
            {
                return (int)Get(CurrentScoreSettingsKey);
            }
            set
            {
                Set(CurrentScoreSettingsKey, value);
            }
        }

        public static string HighscorePlayerID
        {
            get
            {
                return (string)Get(HighscorePlayerIDSettingsKey);
            }
            set
            {
                Set(HighscorePlayerIDSettingsKey, value);
            }
        }

        public static string HighscoreLastUsername
        {
            get
            {
                return (string)Get(HighscoreLastUsernameSettingsKey);
            }
            set
            {
                Set(HighscoreLastUsernameSettingsKey, value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pages;

namespace Boom
{
    class GameSettings : GameSettings<GameSettings>
    {
        public static readonly string RemoveAdsProductId = "RemoveAds";

        private static readonly string SpeakerSettingsKey = "Speaker";
        private static readonly string CurrentRoundSettingsKey = "CurrentRound";
        private static readonly string CurrentScoreSettingsKey = "CurrentScore";
        private static readonly string HighscorePlayerIDSettingsKey = "HighscorePlayerIDSettingsKey";
        private static readonly string HighscoreLastUsernameSettingsKey = "HighscoreLastUsername";
        private static readonly string DidSeeTutorialSettingsKey = "DidSeeTutorial";

        protected override void Initialize()
        {
            base.Initialize();

            AddSetting(SpeakerSettingsKey, true);
            AddSetting(CurrentRoundSettingsKey, 1);
            AddSetting(CurrentScoreSettingsKey, 0);
            AddSetting(HighscorePlayerIDSettingsKey, "");
            AddSetting(HighscoreLastUsernameSettingsKey, "");
            AddSetting(DidSeeTutorialSettingsKey, false);
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

        public static bool DidSeeTutorial
        {
            get
            {
                return (bool)Get(DidSeeTutorialSettingsKey);
            }
            set
            {
                Set(DidSeeTutorialSettingsKey, value);
            }
        }
    }
}
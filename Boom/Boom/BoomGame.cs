using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Reflection;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;
using AdRotatorXNA;
using AdRotator.Model;
using StoreLauncher;

namespace Boom
{
    /// <summary>
    /// Dies ist der Haupttyp für Ihr Spiel
    /// </summary>
    public class BoomGame : Microsoft.Xna.Framework.Game
    {
        private const string RemoveAdsProductId = "RemoveAds";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private StoreBase _store;

        public struct RessourcesStruct
        {
            public SpriteFont font;
            public SpriteFont gameOverfont;
            public SpriteFont boldFont;
            public SpriteFont smallFont;
            public SpriteFont titelFont;
            public SpriteFont menuFont;
            public Texture2D ballTexture;
            public Texture2D speakerTexture;
            public Texture2D speakerMuteTexture;
            public Song backgroundSong;
            public SoundEffect blipSound;
            public SoundEffect victorySound;
        }

        private enum State
        {
            InGameFadingMainMenu,
            FadingMainMenu,
            MainMenu,
            FadingHighscore,
            Highscore,
            FadingInfo,
            Info,
            FadingInGame,
            InGame,
            GameCompleted
        }

        private const string SpeakerSettingsKey = "Speaker";
        private const string CurrentRoundSettingsKey = "CurrentRound";
        private const string CurrentScoreSettingsKey = "CurrentScore";

        private RessourcesStruct _ressources = new RessourcesStruct();
        private bool touching = false;
        private List<Round> _rounds = new List<Round>();
        private List<Round>.Enumerator _currentRound;
        private int _score = 0;
        private int _currentRoundNo = 1;
        private IsolatedStorageSettings _applicationSettings = IsolatedStorageSettings.ApplicationSettings;
        private Highscore _highscore = new Highscore();
        private IntermediateScreen _intermediateScreen;
        private State _state;
        private Round _backroundRound;
        private bool _hasAds = true;

        public BoomGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            // Frame-Rate ist standardmäßig 30 fps für das Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Batterielebensdauer bei Sperre verlängern.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        public static bool IsMute()
        {
            return SoundEffect.MasterVolume == 0f;
        }

        public static void Mute()
        {
            IsolatedStorageSettings.ApplicationSettings[SpeakerSettingsKey] = false;
            SoundEffect.MasterVolume = 0f;
            MediaPlayer.Volume = 0f;
        }

        public static void SetDefaultVolume()
        {
            IsolatedStorageSettings.ApplicationSettings[SpeakerSettingsKey] = true;
            SoundEffect.MasterVolume = .4f;
            MediaPlayer.Volume = .7f;
        }

        public static void LogOut(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void initAdRotator()
        {
            AdRotatorXNAComponent.Initialize(this);

            AdRotatorXNAComponent.Current.Enabled = false;
            AdRotatorXNAComponent.Current.Visible = false;

#if DEBUG 
            AdRotatorXNAComponent.Current.Log += LogOut;
            AdRotatorXNAComponent.Current.DefaultHouseAdImage = Content.Load<Texture2D>("DefaultAdImage");
#endif

            AdRotatorXNAComponent.Current.SlidingAdDirection = SlideDirection.None;
            AdRotatorXNAComponent.Current.AdPosition = Vector2.Zero;

            AdRotatorXNAComponent.Current.DefaultSettingsFileUri = "defaultAdSettings.xml";

            AdRotatorXNAComponent.Current.SettingsUrl = "http://floyd-ug.de/23FBCE58-46CA-449A-BBC8-529602D6D368/boom/defaultAdSettings.xml";

            Components.Add(AdRotatorXNAComponent.Current);
        }

        private void updateAdStatus()
        {
            if (_hasAds)
            {
                AdRotatorXNAComponent.Current.Visible = true;
                AdRotatorXNAComponent.Current.Enabled = true;
            }
            else
            {
                AdRotatorXNAComponent.Current.Visible = false;
                AdRotatorXNAComponent.Current.Enabled = false;
            }
        }

        private void purchaseAdRemover()
        {
            _store.RequestProductPurchaseAsync(RemoveAdsProductId, false).Completed = (IAsyncOperationBase<string> operation, StoreAsyncStatus status) =>
                {
                    if (status == StoreAsyncStatus.Completed)
                    {
                        _hasAds = false;
                        IntermediateScreen.hasAds = null;
                        updateAdStatus();
                    }
                };
        }

        private void initializeStore()
        {
            if (Environment.OSVersion.Version.Major >= 8)
            {
                _store = StoreLauncher.StoreLauncher.GetStoreInterface("StoreWrapper.Store, StoreWrapper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }

            if (_store != null)
            {
                if (_store.LicenseInformation.ProductLicenses.Keys.Contains(RemoveAdsProductId) && _store.LicenseInformation.ProductLicenses[RemoveAdsProductId].IsActive)
                {
                    _hasAds = false;
                }
                else
                {
                    IntermediateScreen.hasAds = new Action(() => purchaseAdRemover());
                }
            }

            updateAdStatus();
        }

        /// <summary>
        /// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
        /// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
        /// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            initAdRotator();

            base.Initialize();

            initializeStore();

            try
            {
                if ((bool)_applicationSettings[SpeakerSettingsKey])
                {
                    SetDefaultVolume();
                }
                else
                {
                    Mute();
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                SetDefaultVolume();
            }

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_ressources.backgroundSong);
            }

            _backroundRound = new Round(20, 1, graphics.GraphicsDevice, _ressources, true);

            InitNewGame();
            MainMenu();
        }

        /// <summary>
        /// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
        /// Ihr gesamter Content geladen wird.
        /// </summary>
        protected override void LoadContent()
        {
            _ressources.ballTexture = Content.Load<Texture2D>("Ball");
            _ressources.speakerMuteTexture = Content.Load<Texture2D>("SpeakerMute");
            _ressources.speakerTexture = Content.Load<Texture2D>("Speaker");
            _ressources.font = Content.Load<SpriteFont>("InGameFont");
            _ressources.titelFont = Content.Load<SpriteFont>("TitleFont");
            _ressources.gameOverfont = Content.Load<SpriteFont>("GameOverFont");
            _ressources.boldFont = Content.Load<SpriteFont>("InGameBoldFont");
            _ressources.smallFont = Content.Load<SpriteFont>("InGameSmallFont");
            _ressources.menuFont = Content.Load<SpriteFont>("MenuFont");
            _ressources.backgroundSong = Content.Load<Song>("Background");
            _ressources.blipSound = Content.Load<SoundEffect>("Blip");
            _ressources.victorySound = Content.Load<SoundEffect>("Victory");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            _intermediateScreen = new IntermediateScreen(graphics.GraphicsDevice, _ressources);
        }

        /// <summary>
        /// UnloadContent wird einmal pro Spiel aufgerufen und ist der Ort, wo
        /// Ihr gesamter Content entladen wird.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        private void GameOver()
        {
            _state = State.GameCompleted;
            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "Game Completed!", Font = _ressources.gameOverfont, Color = Color.White, Pos = -250 },
                                                                          new IntermediateScreen.TextLine() { Text = "Your Score:", Font = _ressources.font, Color = Color.White, Pos = -150 },
                                                                          new IntermediateScreen.TextLine() { Text = Convert.ToString(_score), Font = _ressources.font, Color = Color.White, Pos = -120 },
                                                                          new HighscoreTable(-50, 300, 300, _score, _ressources.font, _ressources.boldFont) },
                                                                          1, 1, 1, Color.Black, false, true);
        }

        private void InitNewGame()
        {
            _rounds.Clear();

            _rounds.Add(new Round(10, 1, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(10, 2, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(15, 3, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(20, 5, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(25, 10, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(30, 15, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(35, 20, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(40, 27, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(45, 33, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(50, 40, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(55, 48, graphics.GraphicsDevice, _ressources, false));
            _rounds.Add(new Round(60, 55, graphics.GraphicsDevice, _ressources, false));

            _currentRound = _rounds.GetEnumerator();
            _currentRoundNo = 0;

            try
            {
                _score = (int)_applicationSettings[CurrentScoreSettingsKey];

                for (int i = 0; i < (int)_applicationSettings[CurrentRoundSettingsKey]; ++i)
                {
                    ++_currentRoundNo;
                    _currentRound.MoveNext();
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                _applicationSettings[CurrentScoreSettingsKey] = 0;
                _applicationSettings[CurrentRoundSettingsKey] = 1;
                _currentRound.MoveNext();
            }

            _currentRound.Current.StartRound(_score, _currentRoundNo);
        }

        private void MenuStartGame()
        {
            _state = State.FadingInGame;
            _applicationSettings[CurrentScoreSettingsKey] = 0;
            _applicationSettings[CurrentRoundSettingsKey] = 1;
            InitNewGame();
            _intermediateScreen.To = 1f;
            _intermediateScreen.Hide();
        }

        private void MenuResumeGame()
        {
            _state = State.FadingInGame;
            InitNewGame();
            _intermediateScreen.To = 1f;
            _intermediateScreen.Hide();
        }

        private void MenuHighscore()
        {
            _state = State.FadingHighscore;
            _intermediateScreen.Hide();
        }

        private void Highscore()
        {
            _state = State.Highscore;
            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "Highscore", Font = _ressources.gameOverfont, Color = Color.White, Pos = -250 },
                                                                          new HighscoreTable(-150, 350, 300, -1, _ressources.font, _ressources.boldFont) },
                                                                          .6f, .6f, .6f, Color.Black, true, true);
        }

        private void InfoReview()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void InfoSupport()
        {
            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version_str = versionAttrib.Version.ToString().Substring(0, 3);

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            string result = "unknown device";
            object deviceName;
            if (DeviceExtendedProperties.TryGetValue("DeviceName", out deviceName))
            {
                result = deviceName.ToString();
            }

            emailComposeTask.Subject = "Boom Version " + version_str + " on " + result;
            emailComposeTask.To = "support.boom@floyd-ug.de";

            emailComposeTask.Show();
        }

        private void InfoPrivacyPolicy()
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://www.floyd-ug.de/apps/boom/privacy-policy-current.html");
            webBrowserTask.Show();
        }

        private void Info()
        {
            _state = State.Info;

            // Get Version
            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version_str = versionAttrib.Version.ToString().Substring(0, 3);

            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "Boom!", Font = _ressources.gameOverfont, Color = Color.White, Pos = -250 },
                                                                          new IntermediateScreen.TextLine() { Text = "Version " + version_str, Font = _ressources.font, Color = Color.White, Pos = -150 },
                                                                          new IntermediateScreen.TextLine() { Text = "by", Font = _ressources.font, Color = Color.White, Pos = -100 },
                                                                          new IntermediateScreen.TextLine() { Text = "Floyd", Font = _ressources.font, Color = Color.White, Pos = -75 },
                                                                          new IntermediateScreen.TextLine() { Text = "Music by", Font = _ressources.font, Color = Color.White, Pos = -30 },
                                                                          new IntermediateScreen.TextLine() { Text = "Chris Zabriskie", Font = _ressources.font, Color = Color.White, Pos = -5 },
                                                                          new IntermediateScreen.TextLine() { Text = "Rate and Review", Font = _ressources.font, Color = Color.LightGray, Pos = 150, Tap=InfoReview },
                                                                          new IntermediateScreen.TextLine() { Text = "Support", Font = _ressources.font, Color = Color.LightGray, Pos = 200, Tap=InfoSupport },
                                                                          new IntermediateScreen.TextLine() { Text = "Privacy Policy", Font = _ressources.font, Color = Color.LightGray, Pos = 250, Tap=InfoPrivacyPolicy }},
                                                                          .6f, .6f, .6f, Color.Black, true, true);
        }

        private void MenuInfo()
        {
            _state = State.FadingInfo;
            _intermediateScreen.Hide();
        }

        private void MainMenu()
        {
            _state = State.MainMenu;

            var content = new List<IntermediateScreen.IDrawable>();

            int menuItemGap = 125;
            int pos = -265;

            content.Add(new IntermediateScreen.TextLine() { Text = "Boom!", Font = _ressources.titelFont, Color = Color.White, Pos = pos });
            pos += 200;

            if ((int)_applicationSettings[CurrentRoundSettingsKey] == 1)
            {
                content.Add(new IntermediateScreen.TextLine() { Text = "Start", Font = _ressources.menuFont, Color = Color.White, Pos = pos, Tap = MenuStartGame });
                pos += menuItemGap;
            }
            else
            {
                menuItemGap = 100;

                content.Add(new IntermediateScreen.TextLine() { Text = "Start", Font = _ressources.menuFont, Color = Color.White, Pos = pos, Tap = MenuStartGame });
                pos += menuItemGap;

                content.Add(new IntermediateScreen.TextLine() { Text = "Resume", Font = _ressources.menuFont, Color = Color.White, Pos = pos, Tap = MenuResumeGame });
                content.Add(new IntermediateScreen.TextLine() { Text = "Level " + (int)_applicationSettings[CurrentRoundSettingsKey], Font = _ressources.smallFont, Color = Color.White, Pos = pos+30, Tap = MenuResumeGame });
                pos += menuItemGap;
            }

            content.Add(new IntermediateScreen.TextLine() { Text = "Highscore", Font = _ressources.menuFont, Color = Color.White, Pos = pos, Tap = MenuHighscore });
            pos += menuItemGap;

            content.Add(new IntermediateScreen.TextLine() { Text = "Info", Font = _ressources.menuFont, Color = Color.White, Pos = pos, Tap = MenuInfo });
            pos += menuItemGap;

            _intermediateScreen.Show(content, .6f, .6f, .6f, Color.Black, false, true);
        }

        private void NextRound()
        {
            _score += _currentRound.Current.Score;

            if (!_currentRound.MoveNext())
            {
                GameOver();
                _applicationSettings[CurrentScoreSettingsKey] = 0;
                _applicationSettings[CurrentRoundSettingsKey] = 1;
            }
            else
            {
                _applicationSettings[CurrentScoreSettingsKey] = _score;
                _applicationSettings[CurrentRoundSettingsKey] = (int)_applicationSettings[CurrentRoundSettingsKey] + 1;
                if(++_currentRoundNo == _rounds.Count)
                {
                    _currentRound.Current.StartRound(_score, -1);
                }
                else
                {
                    _currentRound.Current.StartRound(_score, _currentRoundNo);
                }
            }
        }

        /// <summary>
        /// Ermöglicht dem Spiel die Ausführung der Logik, wie zum Beispiel Aktualisierung der Welt,
        /// Überprüfung auf Kollisionen, Erfassung von Eingaben und Abspielen von Ton.
        /// </summary>
        /// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
        protected override void Update(GameTime gameTime)
        {
            // Ermöglicht ein Beenden des Spiels
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                if (_state == State.Highscore || _state == State.GameCompleted || _state == State.Info)
                {
                    _state = State.FadingMainMenu;
                    _intermediateScreen.Hide();
                }
                else if (_state == State.InGame)
                {
                    _state = State.InGameFadingMainMenu;
                    _currentRound.Current.Hide();
                }
                else
                {
                    this.Exit();
                }
            }

            HandleTouches();

            if (_state == State.InGame || _state == State.InGameFadingMainMenu)
            {
                if (_currentRound.Current.Update())
                {
                    if (_state == State.InGameFadingMainMenu)
                    {
                        MainMenu();
                    }
                    else
                    {
                        NextRound();
                    }
                }
            }
            else
            {
                if (_state != State.GameCompleted)
                {
                    _backroundRound.Update();
                }

                if (_intermediateScreen.Update())
                {
                    switch (_state)
                    {
                        case State.FadingInGame:
                            _state = State.InGame;
                            break;

                        case State.Info:
                        case State.Highscore:
                        case State.FadingMainMenu:
                            MainMenu();
                            break;

                        case State.FadingHighscore:
                            Highscore();
                            break;

                        case State.FadingInfo:
                            Info();
                            break;

                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }

            base.Update(gameTime);
        }

        private void HandleTouches()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (!touching && touches.Count > 0)
            {
                touching = true;
                if (_state == State.InGame)
                {
                    _currentRound.Current.Touch(touches.First());
                }
                else
                {
                    _intermediateScreen.Touch(touches.First());
                }
            }
            else if (touches.Count == 0)
            {
                touching = false;
            }
        }

        /// <summary>
        /// Dies wird aufgerufen, wenn das Spiel selbst zeichnen soll.
        /// </summary>
        /// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_state == State.InGame)
            {
                GraphicsDevice.Clear(_currentRound.Current.BackgroundColor());
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            spriteBatch.Begin();

            if (_state == State.InGame || _state == State.InGameFadingMainMenu)
            {
                _currentRound.Current.Draw(spriteBatch);
            }
            else
            {
                if (_state != State.GameCompleted)
                {
                    _backroundRound.Draw(spriteBatch);
                }
                _intermediateScreen.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

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

namespace Boom
{
    /// <summary>
    /// Dies ist der Haupttyp für Ihr Spiel
    /// </summary>
    public class BoomGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public struct RessourcesStruct
        {
            public SpriteFont font;
            public SpriteFont gameOverfont;
            public SpriteFont boldFont;
            public Texture2D ballTexture;
            public Texture2D speakerTexture;
            public Texture2D speakerMuteTexture;
            public Song backgroundSong;
            public SoundEffect blipSound;
            public SoundEffect victorySound;
        }

        private const string CurrentRoundSettingsKey = "CurrentRound";

        private RessourcesStruct _ressources = new RessourcesStruct();
        private bool touching = false;
        private List<Round> _rounds = new List<Round>();
        private List<Round>.Enumerator _currentRound;
        private bool _gameOver = false;
        private int _score = 0;
        private int _currentRoundNo = 1;
        private IsolatedStorageSettings _applicationSettings = IsolatedStorageSettings.ApplicationSettings;
        private Highscore _highscore = new Highscore();
        private IntermediateScreen _intermediateScreen;

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
            SoundEffect.MasterVolume = 0f;
            MediaPlayer.Volume = 0f;
        }

        public static void SetDefaultVolume()
        {
            SoundEffect.MasterVolume = .8f;
            MediaPlayer.Volume = .1f;
        }

        /// <summary>
        /// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
        /// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
        /// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            //_highscore.Submit(msg => System.Diagnostics.Debug.WriteLine(msg), "Hans", 98);

            SetDefaultVolume();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_ressources.backgroundSong);

            _rounds.Add(new Round(10, 1, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(10, 2, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(15, 3, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(20, 5, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(25, 7, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(30, 10, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(35, 15, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(40, 21, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(45, 27, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(50, 33, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(55, 44, graphics.GraphicsDevice, _ressources));
            //_rounds.Add(new Round(60, 55, graphics.GraphicsDevice, _ressources));

            _currentRound = _rounds.GetEnumerator();

#if DEBUG
            _currentRound.MoveNext();
            GameOver();
            return;
#endif

            try
            {
                for (int i = 0; i < (int)_applicationSettings[CurrentRoundSettingsKey]; ++i)
                {
                    _currentRound.MoveNext();
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                _applicationSettings[CurrentRoundSettingsKey] = 1;
                _currentRound.MoveNext();
            }
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
            _ressources.gameOverfont = Content.Load<SpriteFont>("GameOverFont");
            _ressources.boldFont = Content.Load<SpriteFont>("InGameBoldFont");
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
            _gameOver = true;
            _score = new Random(DateTime.Now.Millisecond).Next(1,400);
            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "Game Completed!", Font = _ressources.gameOverfont, Color = Color.White, Pos = -250 },
                                                                          new IntermediateScreen.TextLine() { Text = "Your Score:", Font = _ressources.font, Color = Color.White, Pos = -150 },
                                                                          new IntermediateScreen.TextLine() { Text = Convert.ToString(_score), Font = _ressources.font, Color = Color.White, Pos = -120 },
                                                                          new HighscoreTable(-50, 300, 300, _score, _ressources.font, _ressources.boldFont) },
                                                                          1, 1, 1, Color.Black);
        }

        private void NextRound()
        {
            _score += _currentRound.Current.Score;

            if (!_currentRound.MoveNext())
            {
                GameOver();
                _applicationSettings[CurrentRoundSettingsKey] = 1;
            }
            else
            {
                _applicationSettings[CurrentRoundSettingsKey] = (int)_applicationSettings[CurrentRoundSettingsKey] + 1;
                _currentRound.Current.StartRound(_score, ++_currentRoundNo);
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
                this.Exit();

            HandleTouches();

            if (!_gameOver)
            {
                if (_currentRound.Current.Update())
                {
                    NextRound();
                }
            }
            else
            {
                _intermediateScreen.Update();
            }

            base.Update(gameTime);
        }

        private void HandleTouches()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (!touching && touches.Count > 0)
            {
                touching = true;
                if (!_gameOver)
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
            if (!_gameOver)
            {
                GraphicsDevice.Clear(_currentRound.Current.BackgroundColor());
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            spriteBatch.Begin();

            if (!_gameOver)
            {
                _currentRound.Current.Draw(spriteBatch);
            }
            else
            {
                _intermediateScreen.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

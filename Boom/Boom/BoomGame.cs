using System;
using System.Collections.Generic;
using System.Linq;
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
            public Texture2D ballTexture;
            public Song backgroundSong;
            public SoundEffect blipSound;
            public SoundEffect victorySound;
        }

        private RessourcesStruct _ressources = new RessourcesStruct();
        private bool touching = false;
        private List<Round> _rounds = new List<Round>();
        private List<Round>.Enumerator _currentRound;
        private bool _gameOver = false;

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

        /// <summary>
        /// Ermöglicht dem Spiel, alle Initialisierungen durchzuführen, die es benötigt, bevor die Ausführung gestartet wird.
        /// Hier können erforderliche Dienste abgefragt und alle nicht mit Grafiken
        /// verbundenen Inhalte geladen werden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            SoundEffect.MasterVolume = .8f;

            MediaPlayer.Volume = .1f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_ressources.backgroundSong);

            _rounds.Add(new Round(10, 1, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(10, 2, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(15, 3, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(20, 5, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(25, 7, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(30, 10, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(35, 15, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(40, 21, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(45, 27, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(50, 33, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(55, 44, graphics.GraphicsDevice, _ressources));
            _rounds.Add(new Round(60, 55, graphics.GraphicsDevice, _ressources));

            _currentRound = _rounds.GetEnumerator();
            _currentRound.MoveNext();
        }
        
        /// <summary>
        /// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
        /// Ihr gesamter Content geladen wird.
        /// </summary>
        protected override void LoadContent()
        {
            _ressources.ballTexture = Content.Load<Texture2D>("Ball");
            _ressources.font = Content.Load<SpriteFont>("InGameFont");
            _ressources.gameOverfont = Content.Load<SpriteFont>("GameOverFont");
            _ressources.backgroundSong = Content.Load<Song>("Background");
            _ressources.blipSound = Content.Load<SoundEffect>("Blip");
            _ressources.victorySound = Content.Load<SoundEffect>("Victory");

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent wird einmal pro Spiel aufgerufen und ist der Ort, wo
        /// Ihr gesamter Content entladen wird.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Entladen Sie jeglichen Nicht-ContentManager-Inhalt hier
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

            if (!_gameOver)
            {
                HandleTouches();

                if (_currentRound.Current.Update())
                {
                    if (!_currentRound.MoveNext())
                    {
                        _gameOver = true;
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
                _currentRound.Current.Touch(touches.First());
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
                {
                    string text = "Game Completed!";
                    Vector2 position = new Vector2((graphics.GraphicsDevice.Viewport.Width - _ressources.gameOverfont.MeasureString(text).X) / 2, (graphics.GraphicsDevice.Viewport.Height / 2) - 100);
                    spriteBatch.DrawString(_ressources.gameOverfont, text, position, Color.White);
                }

                {
                    string text = "Your Score:";
                    Vector2 position = new Vector2((graphics.GraphicsDevice.Viewport.Width - _ressources.font.MeasureString(text).X) / 2, (graphics.GraphicsDevice.Viewport.Height / 2) - 40);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }

                {
                    int score = 0;
                    int possible = 0;

                    foreach (Round round in _rounds)
                    {
                        score += round.Score;
                        possible += round.Possible;
                    }

                    string text = score + " of " + possible;
                    Vector2 position = new Vector2((graphics.GraphicsDevice.Viewport.Width - _ressources.font.MeasureString(text).X) / 2, (graphics.GraphicsDevice.Viewport.Height / 2) - 20);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

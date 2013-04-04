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
            public Texture2D ballTexture;
            public Song backgroundSong;
            public SoundEffect blipSound;
            public SoundEffect victorySound;
        }

        private RessourcesStruct _ressources = new RessourcesStruct();
        private bool touching = false;
        private Round _currentRound;

        public BoomGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
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

            _currentRound = new Round(graphics.GraphicsDevice.Viewport, _ressources);
        }
        
        /// <summary>
        /// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
        /// Ihr gesamter Content geladen wird.
        /// </summary>
        protected override void LoadContent()
        {
            _ressources.ballTexture = Content.Load<Texture2D>("Ball");
            _ressources.font = Content.Load<SpriteFont>("InGameFont");
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

            HandleTouches();

            _currentRound.Update();

            base.Update(gameTime);
        }

        private void HandleTouches()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (!touching && touches.Count > 0)
            {
                touching = true;
                _currentRound.Touch(touches.First());
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
            GraphicsDevice.Clear(_currentRound.BackgroundColor());

            spriteBatch.Begin();

            _currentRound.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

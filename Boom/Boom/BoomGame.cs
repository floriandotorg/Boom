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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D ballTexture;  
        IList<Ball> balls = new List<Ball>();
        bool touching = false;
        Random random = new Random(DateTime.Now.Millisecond);
        bool catcher = false;
        SpriteFont font;
        int numBallsTotal = 10;
        int caught;
        int goal = 2;

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
            ballTexture = Content.Load<Texture2D>("Ball");
            font = Content.Load<SpriteFont>("InGameFont");

            balls.Add(new Ball());

            base.Initialize();
        }

        void CreateBalls(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Color ballColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                Vector2 center = new Vector2((float)random.Next(graphics.GraphicsDevice.Viewport.Width-20)+10, (float)random.Next(graphics.GraphicsDevice.Viewport.Height-20)+10);
                Vector2 velocity = new Vector2((random.NextDouble() > .5 ? -1 : 1) * 2, (random.NextDouble() > .5 ? -1 : 1) * 2);
                balls.Add(new Ball(this, ballColor * 0.5f, ballTexture, center, velocity));
            }
        }

        /// <summary>
        /// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
        /// Ihr gesamter Content geladen wird.
        /// </summary>
        protected override void LoadContent()
        {
            // Erstellen Sie einen neuen SpriteBatch, der zum Zeichnen von Texturen verwendet werden kann.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CreateBalls(numBallsTotal);
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

            foreach (Ball ball in balls)
            {
                ball.Update();
            }

            foreach (Ball collided in balls.Where( x => x.Collided ))
            {
                foreach (Ball free in balls.Where( x => !x.Collided ))
                {
                    collided.CheckAndHandleCollision(free);
                }
            }

            caught = balls.Where(x => x.Caught).Count() - 1;

            base.Update(gameTime);
        }

        private void HandleTouches()
        {
            TouchCollection touches = TouchPanel.GetState();
            if (!touching && touches.Count > 0)
            {
                touching = true;
                //if (!catcher)
                {
                    balls[0] = new Ball(this, Color.White, ballTexture, touches.First().Position, new Vector2(0));
                    balls[0].Collision();
                    catcher = true;
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
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (Ball ball in balls)
            {
                ball.Draw(spriteBatch);
            }

            string text = "Points: " + caught + "/" + "1" + " from " + numBallsTotal;
            Vector2 position = new Vector2(10, graphics.GraphicsDevice.Viewport.Height - font.MeasureString(text).Y - 10);
            spriteBatch.DrawString(font, text, position, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

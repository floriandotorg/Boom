using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class Round
    {
        private const float _ballVelocity = 1f;

        private Viewport _viewport;
        private BoomGame.RessourcesStruct _ressources;

        private IList<Ball> balls = new List<Ball>();
        private Random random = new Random(DateTime.Now.Millisecond);
        private bool catcher;
        private int numBallsTotal;
        private int goal;
        private SineValue backgroundColor = new SineValue(220.0, 30);

        private enum State
        {
            StartScreen,
            InGame,
            FailedScreen,
            SucessScreen,
            RoundEnded
        }

        private State _state;

        public int Score
        {
            get
            {
                return balls.Where(x => x.Collided).Count() - 1;
            }
        }

        public int Possible
        {
            get
            {
                return numBallsTotal;
            }
        }

        public Round(int numBalls, int goal, Viewport viewport, BoomGame.RessourcesStruct ressouces)
        {
            numBallsTotal = numBalls;
            this.goal = goal;
            _viewport = viewport;
            _ressources = ressouces;

            _state = State.StartScreen;
        }

        private void init()
        {
            _state = State.InGame;
            catcher = false;
            backgroundColor.Value = 0;

            balls.Clear();

            balls.Add(new Ball());
            createBalls(numBallsTotal);
        }

        private void createBalls(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Color ballColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                Vector2 center = new Vector2((float)random.Next(_viewport.Width - 20) + 10, (float)random.Next(_viewport.Height - 20) + 10);
                Vector2 velocity = new Vector2((random.NextDouble() > .5 ? -1 : 1) * _ballVelocity, (random.NextDouble() > .5 ? -1 : 1) * _ballVelocity);
                balls.Add(new Ball(_viewport, ballColor * 0.5f, _ressources.ballTexture, center, velocity));
            }
        }

        public bool Update()
        {
            if (_state == State.InGame)
            {
                foreach (Ball ball in balls)
                {
                    ball.Update();
                }

                foreach (Ball collided in balls.Where(x => x.Collided && !x.Destroyed))
                {
                    foreach (Ball free in balls.Where(x => !x.Collided && !x.Destroyed))
                    {
                        if (collided.CheckAndHandleCollision(free))
                        {
                            _ressources.blipSound.Play(.3f, 0f, 0f);
                        }
                    }
                }

                if (Score >= goal)
                {
                    if (!backgroundColor.IsMax)
                    {
                        if (backgroundColor.IsMin)
                        {
                            _ressources.victorySound.Play(.9f, 0f, 0f);
                        }

                        backgroundColor.Inc();
                    }
                }

                if (catcher && balls.Where(x => x.Collided && !x.Destroyed).Count() == 0)
                {
                    if (Score >= goal)
                    {
                        _state = State.SucessScreen;
                    }
                    else
                    {
                        _state = State.FailedScreen;
                    }
                }
            }
            else if (_state == State.RoundEnded)
            {
                return true;
            }

            return false;
        }

        public Color BackgroundColor()
        {
            return new Color((int)backgroundColor.Value, (int)backgroundColor.Value, (int)backgroundColor.Value);
        }

        public void Touch(TouchLocation touch)
        {
            if (_state == State.InGame && !catcher)
            {
                balls[0] = new Ball(_viewport, Color.White, _ressources.ballTexture, touch.Position, new Vector2(0));
                balls[0].Collision();
                catcher = true;
            }
            else if (_state == State.StartScreen || _state == State.FailedScreen)
            {
                init();
            }
            else if (_state == State.SucessScreen)
            {
                _state = State.RoundEnded;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state == State.InGame)
            {
                foreach (Ball ball in balls)
                {
                    ball.Draw(spriteBatch);
                }

                string text = "Points: " + Score + "/" + goal + " from " + numBallsTotal;
                Vector2 position = new Vector2(10, _viewport.Height - _ressources.font.MeasureString(text).Y - 10);
                spriteBatch.DrawString(_ressources.font, text, position, Color.White);
            }
            else if (_state == State.StartScreen)
            {
                {
                    string text = "Objective";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.gameOverfont.MeasureString(text).X) / 2, (_viewport.Height / 2) - 100);
                    spriteBatch.DrawString(_ressources.gameOverfont, text, position, Color.White);
                }

                {
                    string text =  goal + " of " + numBallsTotal;
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) - 40);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }

                {
                    string text = "Tap to start";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) + 100);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }
            }
            else if (_state == State.FailedScreen)
            {
                {
                    string text = "You failed!";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.gameOverfont.MeasureString(text).X) / 2, (_viewport.Height / 2) - 100);
                    spriteBatch.DrawString(_ressources.gameOverfont, text, position, Color.Red);
                }

                {
                    string text = Score + "/" + goal + " of " + numBallsTotal;
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) - 40);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }

                {
                    string text = "Tap to retry";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) + 100);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.White);
                }
            }
            else if (_state == State.SucessScreen)
            {
                {
                    string text = "You won!";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.gameOverfont.MeasureString(text).X) / 2, (_viewport.Height / 2) - 100);
                    spriteBatch.DrawString(_ressources.gameOverfont, text, position, Color.LimeGreen);
                }

                {
                    string text = Score + "/" + goal + " of " + numBallsTotal;
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) - 40);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.Black);
                }

                {
                    string text = "Tap to resume";
                    Vector2 position = new Vector2((_viewport.Width - _ressources.font.MeasureString(text).X) / 2, (_viewport.Height / 2) + 100);
                    spriteBatch.DrawString(_ressources.font, text, position, Color.Black);
                }
            }
        }
    }
}

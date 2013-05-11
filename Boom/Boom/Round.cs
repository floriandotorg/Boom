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
        private const float _ballVelocity = 1.5f;

        private Viewport _viewport;
        private BoomGame.RessourcesStruct _ressources;

        private IList<Ball> balls = new List<Ball>();
        private Random random = new Random(DateTime.Now.Millisecond);
        private bool catcher;
        private int numBallsTotal;
        private int goal;
        private int _score, _roundNo = 0;
        private SineValue backgroundColor = new SineValue(220.0, 30);
        private IntermediateScreen _intermediateScreen;

        private enum State
        {
            StartScreen,
            InGame,
            InGameSimulation,
            FailedScreen,
            SucessScreen,
            HideOut,
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

        public Round(int numBalls, int goal, GraphicsDevice graphicsDevice, BoomGame.RessourcesStruct ressouces, bool noStartScreen)
        {
            numBallsTotal = numBalls;
            this.goal = goal;
            _viewport = graphicsDevice.Viewport;
            _ressources = ressouces;
            _intermediateScreen = new IntermediateScreen(graphicsDevice, ressouces);

            if (!noStartScreen)
            {
                StartScreen();
            }
            else
            {
                init();
                _state = State.InGameSimulation;
            }
        }

        public void Hide()
        {
            if (_state == State.StartScreen || _state == State.FailedScreen)
            {
                _intermediateScreen.To = 1f;
            }
            else if(_state != State.SucessScreen)
            {
                _intermediateScreen.Show(new List<IntermediateScreen.IDrawable>(), 0f, 0f, 1f, Color.Black, false);
            }

            _intermediateScreen.Hide();

            _state = State.HideOut;
        }

        public void StartRound(int score, int roundNo)
        {
            _score = score;
            _roundNo = roundNo;

            StartScreen();
        }

        private void StartScreen()
        {
            init();
            _state = State.StartScreen;

            float h = (float)_viewport.Height / 2f;
            int l1 = (int)(-h / 2f);
            int l2 = (int)(-h / 2f) + 60;
            int l3 = (int)((-h / 2f) + h * .382f);
            int l4 = (int)((-h / 2f) + h - 50);
            int l5 = (int)((-h / 2f) + h - 50) + 40;


            string roundName = "Level " + _roundNo;
            if(_roundNo < 0)
            {
                roundName = "Final Level";
            }

            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = roundName, Pos = l1, Color = Color.White, Font = _ressources.gameOverfont },
                                                                        new IntermediateScreen.TextLine() { Text = goal + " of " + numBallsTotal, Pos = l2, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Tap to start", Pos = l3, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Current Score", Pos = l4, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "" + (_score + Score), Pos = l5, Color = Color.White, Font = _ressources.font } },
                                                                        1f, .6f, 0f, Color.Black, true);
        }

        private void FailedScreen()
        {
            _state = State.FailedScreen;

            float h = (float)_viewport.Height / 2f;
            int l1 = (int)(-h / 2f);
            int l2 = (int)(-h / 2f) + 60;
            int l3 = (int)((-h / 2f) + h * .382f);
            int l4 = (int)((-h / 2f) + h - 50);
            int l5 = (int)((-h / 2f) + h - 50) + 40;

            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "You failed!", Pos = l1, Color = Color.Red, Font = _ressources.gameOverfont },
                                                                        new IntermediateScreen.TextLine() { Text = Score + "/" + goal + " of " + numBallsTotal, Pos = l2, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Tap to retry", Pos = l3, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Current Score", Pos = l4, Color = Color.White, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "" + (_score + Score), Pos = l5, Color = Color.White, Font = _ressources.font } },
                                                                        0f, 1f, 0f, Color.Black, true);
        }

        private void SucessScreen()
        {
            _state = State.SucessScreen;

            float h = (float)_viewport.Height / 2f;
            int l1 = (int)(-h / 2f);
            int l2 = (int)(-h / 2f) + 60;
            int l3 = (int)((-h / 2f) + h * .382f);
            int l4 = (int)((-h / 2f) + h - 50);
            int l5 = (int)((-h / 2f) + h - 50) + 40;

            _intermediateScreen.Show(new IntermediateScreen.IDrawable[] { new IntermediateScreen.TextLine() { Text = "You won!", Pos = l1, Color = Color.Black, Font = _ressources.gameOverfont },
                                                                        new IntermediateScreen.TextLine() { Text = Score + "/" + goal + " of " + numBallsTotal, Pos = l2, Color = Color.Black, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Tap to resume", Pos = l3, Color = Color.Black, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "Current Score", Pos = l4, Color = Color.Black, Font = _ressources.font },
                                                                        new IntermediateScreen.TextLine() { Text = "" + (_score + Score), Pos = l5, Color = Color.Black, Font = _ressources.font } },
                                                                        0f, 1f, 0f, Color.White, true);
        }

        private void init()
        {
            catcher = false;
            backgroundColor.Value = 0;

            balls.Clear();

            balls.Add(new Ball());
            createBalls(numBallsTotal);
        }

        float minDistance(Vector2 prostect, IEnumerable<Ball> balls)
        {
            float result = float.MaxValue;
            foreach (var ball in balls)
            {
                result = Math.Min(result, Vector2.Distance(ball.Center, prostect));
            }
            return result;
        }

        private void createBalls(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Color ballColor = new Color(0,0,0);
                while (ballColor.R + ballColor.G + ballColor.B < 80)
                {
                    ballColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                }

                Vector2 center;
                do
                {
                    center = new Vector2((float)random.Next(_viewport.Width - 20) + 10, (float)random.Next(_viewport.Height - 20) + 10);
                }
                while (minDistance(center, balls) < 30);

                Vector2 velocity = new Vector2((random.NextDouble() > .5 ? -1 : 1) * _ballVelocity, (random.NextDouble() > .5 ? -1 : 1) * _ballVelocity);
                
                balls.Add(new Ball(_viewport, ballColor * 0.5f, _ressources.ballTexture, center, velocity));
            }
        }

        public bool Update()
        {
            if (_state == State.RoundEnded)
            {
                return true;
            }
            
            foreach (Ball ball in balls)
            {
                ball.Update();
            }

            if (_state == State.InGame)
            {
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

                if (catcher && balls.Where(x => x.Expanding).Count() == 0)
                {
                    foreach (Ball ball in balls.Where(x => !x.Collided && !x.Destroyed && !x.Dead))
                    {
                        ball.Die();
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

                if (balls.Where(x => x.Size >= 5).Count() == 0)
                {
                    if (Score >= goal)
                    {
                        SucessScreen();
                    }
                    else
                    {
                        FailedScreen();
                    }
                }
            }
            else if (_state == State.StartScreen || _state == State.FailedScreen || _state == State.SucessScreen || _state == State.HideOut)
            {
                if (_intermediateScreen.Update())
                {
                    if (_state == State.SucessScreen || _state == State.HideOut)
                    {
                        _state = State.RoundEnded;
                        return true;
                    }
                    else if (_state == State.StartScreen || _state == State.FailedScreen)
                    {
                        _state = State.InGame;
                    }
                }
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
            else if (_state == State.StartScreen || _state == State.FailedScreen || _state == State.SucessScreen)
            {
                if (_intermediateScreen.Touch(touch))
                {
                    if (_state == State.FailedScreen)
                    {
                        init();
                    }

                    if (_state == State.SucessScreen)
                    {
                        backgroundColor.Value = 0;
                    }
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ball ball in balls)
            {
                ball.Draw(spriteBatch);
            }

            if (_state == State.InGame || _state == State.StartScreen || _state == State.FailedScreen)
            {
                string text = "Points: " + Score + "/" + goal + " of " + numBallsTotal;
                Vector2 position = new Vector2(10, _viewport.Height - _ressources.font.MeasureString(text).Y - 10);
                spriteBatch.DrawString(_ressources.font, text, position, Color.White);
            }

            if (_state == State.StartScreen || _state == State.FailedScreen || _state == State.SucessScreen || _state == State.HideOut)
            {
                _intermediateScreen.Draw(spriteBatch);
            }
        }
    }
}

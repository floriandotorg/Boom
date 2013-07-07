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
using Pages;

namespace Boom
{
    struct RoundSettings
    {
        public RoundSettings(int numBalls, int goal, int roundNo, bool finalRound)
        {
            NumBalls = numBalls;
            Goal = goal;
            RoundNo = roundNo;
            IsFinalRound = finalRound;
        }

        public int RoundNo;
        public bool IsFinalRound;
        public int NumBalls;
        public int Goal;
    }

    class Round
    {
        private readonly float _ballVelocity = 1.5f;

        private IRoundDelegate _roundDelegate;

        private RoundSettings _roundSettings;
        private Viewport _viewport;
        private Texture2D _ballTexture;
        private SoundEffect _blipSound;
        private SoundEffect _victorySound;
        private SpriteFont _font;

        private IList<Ball> balls = new List<Ball>();
        private Random random = new Random(DateTime.Now.Millisecond);
        private bool catcher;
        private int _score;
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

        public Round(IRoundDelegate roundDelegate)
        {
            _roundDelegate = roundDelegate;
            _roundSettings = _roundDelegate.RoundSettings;
            _viewport = roundDelegate.Viewport;
            _ballTexture = roundDelegate.BallTexture;
            _blipSound = roundDelegate.BlipSound;
            _victorySound = roundDelegate.VictorySound;
            _font = roundDelegate.Font;
            _score = roundDelegate.Score;

            StartScreen();
        }

        private void StartScreen()
        {
            init();

            if (_roundDelegate.ShouldShowOverlays)
            {
                _state = State.StartScreen;

                string roundName = "Level " + _roundSettings.RoundNo;
                if (_roundSettings.IsFinalRound)
                {
                    roundName = "Final Level";
                }

                _roundDelegate.ShowOverlay(new StartScreenView(roundName, "" + _roundSettings.Goal + " of " + _roundSettings.NumBalls, "" + (_score + Score)));
            }
            else
            {
                _state = State.InGame;
            }
        }

        private void FailedScreen()
        {
            if (_roundDelegate.ShouldShowOverlays)
            {
                _state = State.FailedScreen;

                _roundDelegate.ShowOverlay(new FailedScreenView(Score + "/" + _roundSettings.Goal + " of " + _roundSettings.NumBalls, "" + _score));
            }
            else
            {
                init();
                _state = State.InGame;
            }
        }

        private void SucessScreen()
        {
            if (_roundDelegate.ShouldShowOverlays)
            {
                _state = State.SucessScreen;

                _roundDelegate.ShowOverlay(new SucessScreenView(Score + "/" + _roundSettings.Goal + " of " + _roundSettings.NumBalls, "" + (_score + Score)));
            }
            else
            {
                _state = State.RoundEnded;
            }
        }

        private void init()
        {
            catcher = false;
            backgroundColor.Value = 0;

            balls.Clear();

            balls.Add(new Ball());
            createBalls(_roundSettings.NumBalls);
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
                while (ballColor.R + ballColor.G + ballColor.B < 100)
                {
                    ballColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                }

                Vector2 center;
                Vector2 velocity;

                if (_roundDelegate.IsTutorial)
                {
                    center = new Vector2(_viewport.Width / 2 - Ball.RadiusNormalSize / 2, 265 + i * Ball.RadiusHugeSize);
                    velocity = Vector2.Zero;
                }
                else
                {
                    do
                    {
                        center = new Vector2((float)random.Next(_viewport.Width - 20) + 10, (float)random.Next(_viewport.Height - 20) + 10);
                    }
                    while (minDistance(center, balls) < 30);

                    velocity = new Vector2((random.NextDouble() > .5 ? -1 : 1) * _ballVelocity, (random.NextDouble() > .5 ? -1 : 1) * _ballVelocity);
                }
                
                balls.Add(new Ball(_viewport, ballColor * 0.5f, _ballTexture, center, velocity));
            }
        }

        public bool Update()
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
                        _blipSound.Play(.3f, 0f, 0f);
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

            if (_state == State.InGame && Score >= _roundSettings.Goal)
            {
                if (!backgroundColor.IsMax)
                {
                    if (backgroundColor.IsMin)
                    {
                        _victorySound.Play(.9f, 0f, 0f);
                    }

                    backgroundColor.Inc();
                }
            }

            if (_roundDelegate.IsTutorial && _state != State.RoundEnded)
            {
                if (!balls[0].Destroyed && !balls[0].Expanding && balls[0].Size <= 55 && balls.Where(x => x.Collided).Count() == 1)
                {
                    _state = State.RoundEnded;
                }
            }

            if (_state == State.InGame && balls.Where(x => x.Size >= 5).Count() == 0)
            {
                if (Score >= _roundSettings.Goal)
                {
                    SucessScreen();
                }
                else
                {
                    FailedScreen();
                }
            }

            if (_state == State.RoundEnded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OverlayWillDismiss()
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

        public bool OverlayDismissed()
        {
            if (_state == State.SucessScreen)
            {
                _state = State.RoundEnded;
                return false;
            }
            else if (_state == State.StartScreen || _state == State.FailedScreen)
            {
                _state = State.InGame;
            }

            return true;
        }

        public Color BackgroundColor()
        {
            return new Color((int)backgroundColor.Value, (int)backgroundColor.Value, (int)backgroundColor.Value);
        }

        public void Touch(TouchLocation touch)
        {
            if (_state == State.InGame && !catcher)
            {
                balls[0] = new Ball(_viewport, Color.White, _ballTexture, touch.Position, Vector2.Zero);
                balls[0].Collision();
                catcher = true;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, AnimationInfo animationInfo)
        {
            foreach (Ball ball in balls)
            {
                ball.Draw(spriteBatch, animationInfo);
            }

            if (_roundDelegate.IsScoreVisible && (_state == State.InGame || _state == State.StartScreen || _state == State.FailedScreen))
            {
                float opacity = Convert.ToSingle(Math.Min(1, balls.Skip(1).Max(x => x.Size) / Ball.RadiusNormalSize));
                string text = "Points: " + Score + "/" + _roundSettings.Goal + " of " + _roundSettings.NumBalls;
                Vector2 position = new Vector2(10, _viewport.Height - _font.MeasureString(text).Y - 10);
                spriteBatch.DrawString(_font, text, position, Color.White * animationInfo.Value * opacity);
            }
        }
    }
}

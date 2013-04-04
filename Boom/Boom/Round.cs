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
        private Viewport _viewport;
        private BoomGame.RessourcesStruct _ressources;

        private IList<Ball> balls = new List<Ball>();
        private Random random = new Random(DateTime.Now.Millisecond);
        private bool catcher = false;
        private int numBallsTotal = 10;
        private int caught;
        private int goal = 1;
        private SineValue backgroundColor = new SineValue(220.0, 30) { Value = 0 };

        public Round(Viewport viewport, BoomGame.RessourcesStruct ressouces)
        {
            _viewport = viewport;
            _ressources = ressouces;

            balls.Add(new Ball());
            CreateBalls(numBallsTotal);
        }

        void CreateBalls(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Color ballColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                Vector2 center = new Vector2((float)random.Next(_viewport.Width - 20) + 10, (float)random.Next(_viewport.Height - 20) + 10);
                Vector2 velocity = new Vector2((random.NextDouble() > .5 ? -1 : 1) * 2, (random.NextDouble() > .5 ? -1 : 1) * 2);
                balls.Add(new Ball(_viewport, ballColor * 0.5f, _ressources.ballTexture, center, velocity));
            }
        }

        public bool Update()
        {
            foreach (Ball ball in balls)
            {
                ball.Update();
            }

            foreach (Ball collided in balls.Where(x => x.Collided))
            {
                foreach (Ball free in balls.Where(x => !x.Collided))
                {
                    if (collided.CheckAndHandleCollision(free))
                    {
                        _ressources.blipSound.Play(.5f, 0f, 0f);
                    }
                }
            }

            caught = balls.Where(x => x.Caught).Count() - 1;

            if (caught >= goal && !backgroundColor.IsMax)
            {
                if (backgroundColor.IsMin)
                {
                    _ressources.victorySound.Play(1f, 0f, 0f);
                }

                backgroundColor.Inc();
            }

            return false;
        }

        public Color BackgroundColor()
        {
            return new Color((int)backgroundColor.Value, (int)backgroundColor.Value, (int)backgroundColor.Value);
        }

        public void Touch(TouchLocation touch)
        {
            //if (!catcher)
            {
                balls[0] = new Ball(_viewport, Color.White, _ressources.ballTexture, touch.Position, new Vector2(0));
                balls[0].Collision();
                catcher = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ball ball in balls)
            {
                ball.Draw(spriteBatch);
            }

            string text = "Points: " + caught + "/" + goal + " from " + numBallsTotal;
            Vector2 position = new Vector2(10, _viewport.Height - _ressources.font.MeasureString(text).Y - 10);
            spriteBatch.DrawString(_ressources.font, text, position, Color.White);
        }
    }
}

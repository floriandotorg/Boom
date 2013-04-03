using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Boom
{
    class Ball
    {
        private Game game;

        private const float radiusNormalSize = 10.0f;
        private const float radiusHugeSize = 75.0f;
        private const double maxRadiusExpandingFactor = Math.PI / 2.0;
        private const double radiusExpandingFactorInc = maxRadiusExpandingFactor/30.0;

        private Vector2 velocity;
        private Color color;
        private Texture2D texture;
        private Vector2 center;

        private float radius;
        private double radiusExpandingFactor = Math.Asin(radiusNormalSize / radiusHugeSize);

        private const int numHugeUpdatesToShrink = 20;
        private int numHugeUpdates;

        private Vector2 topLeft
        {
            get
            {
                return new Vector2(center.X - radius, center.Y - radius);
            }
        }

        private float scale
        {
            get
            {
                return (this.radius * 2) / (float)texture.Bounds.Width;
            }
        }

        private enum State
        {
            Normal,
            Expanding,
            Huge,
            Shrinking,
            Destroyed
        }

        private State state = State.Normal;

        public bool Collided
        {
            get
            {
                return state != State.Normal && state != State.Destroyed;
            }
        }

        public bool Caught
        {
            get
            {
                return state != State.Normal;
            }
        }

        public void CheckAndHandleCollision(Ball other)
        {
            if (Vector2.Distance(this.center, other.center) <= this.radius + other.radius)
            {
                other.Collision();
            }
        }

        public Ball()
        {
            state = State.Destroyed;
        }

        public Ball(BoomGame game, Color color, Texture2D texture, Vector2 center, Vector2 velocity)
        {
            this.game = game;
            this.color = color;
            this.texture = texture;
            this.center = center;
            this.velocity = velocity;
        }

        public void Draw(SpriteBatch batch)
        {
            if (state != State.Destroyed)
            {
                batch.Draw(texture, topLeft, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void Update()
        {
            if (state != State.Destroyed)
            {
                radius = radiusHugeSize * (float)Math.Sin(radiusExpandingFactor);

                BounceBall();

                center += velocity;
            }

            if (state == State.Expanding)
            {
                radiusExpandingFactor += radiusExpandingFactorInc;

                if (radiusExpandingFactor >= maxRadiusExpandingFactor)
                {
                    state = State.Huge;
                }
            }
            else if (state == State.Huge)
            {
                if (++numHugeUpdates >= numHugeUpdatesToShrink)
                {
                    state = State.Shrinking;
                }
            }
            else if (state == State.Shrinking)
            {
                radiusExpandingFactor -= radiusExpandingFactorInc;

                if (radiusExpandingFactor <= .0f)
                {
                    state = State.Destroyed;
                }
            }
        }

        public void Collision()
        {
            if (state == State.Normal)
            {
                state = State.Expanding;
                velocity = new Vector2(0);
            }
        }

        private void BounceBall()
        {
            Vector2 newTopLeft = topLeft + velocity;
            float left, right, top, bottom;
            left = newTopLeft.X;
            right = newTopLeft.X + (radius * 2);
            top = newTopLeft.Y;
            bottom = newTopLeft.Y + (radius * 2);

            if (top < 0 || bottom > game.GraphicsDevice.Viewport.Height)
            {
                velocity.Y *= -1;
            }

            if (left < 0 || right > game.GraphicsDevice.Viewport.Width)
            {
                velocity.X *= -1;
            }
        }
    }
}

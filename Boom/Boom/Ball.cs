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
        private Viewport viewport;

        private const float radiusNormalSize = 10.0f;
        private const float radiusHugeSize = 60.0f;
        private const int radiusSizeingSpeed = 30;

        private SineValue radius = new SineValue(radiusHugeSize, radiusSizeingSpeed) { Value = radiusNormalSize };

        private Vector2 velocity;
        private Color color;
        private Texture2D texture;
        private Vector2 center;

        private const int numHugeUpdatesToShrink = 20;
        private const int numHugeUpdatesToDie = 15;
        private int updateCounter;

        private Vector2 topLeft
        {
            get
            {
                return new Vector2(center.X - (float)radius.Value, center.Y - (float)radius.Value);
            }
        }

        private float scale
        {
            get
            {
                return ((float)this.radius.Value * 2f) / (float)texture.Bounds.Width;
            }
        }

        private enum State
        {
            Normal,
            Expanding,
            Huge,
            Shrinking,
            Destroyed,
            DyingWait,
            Dying,
            Dead
        }

        private State state = State.Normal;

        public bool Collided
        {
            get
            {
                return state != State.Normal && !Dead;
            }
        }

        public bool Destroyed
        {
            get
            {
                return state == State.Destroyed;
            }
        }

        public bool Expanding
        {
            get
            {
                return state == State.Expanding || state == State.Huge;
            }
        }

        public bool Dead
        {
            get
            {
                return state == State.DyingWait || state == State.Dying || state == State.Dead;
            }
        }

        public double Size
        {
            get
            {
                return radius.Value;
            }
        }

        public bool CheckAndHandleCollision(Ball other)
        {
            if (Vector2.Distance(this.center, other.center) <= this.radius.Value + other.radius.Value)
            {
                other.Collision();
                return true;
            }

            return false;
        }

        public Ball()
        {
            state = State.Destroyed;
        }

        public Ball(Viewport viewport, Color color, Texture2D texture, Vector2 center, Vector2 velocity)
        {
            this.viewport = viewport;
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
                BounceBall();

                center += velocity;
            }

            if (state == State.Expanding)
            {
                radius.Inc();

                if (radius.IsMax)
                {
                    state = State.Huge;
                }
            }
            else if (state == State.Huge)
            {
                if (++updateCounter >= numHugeUpdatesToShrink)
                {
                    state = State.Shrinking;
                    radius.Reverse();
                }
            }
            else if (state == State.Shrinking)
            {
                radius.Dec();

                if (radius.IsMin)
                {
                    state = State.Destroyed;
                }
            }
            else if (state == State.DyingWait)
            {
                if (++updateCounter >= numHugeUpdatesToDie)
                {
                    state = State.Dying;
                    radius.Reverse();
                }
            }
            else if (state == State.Dying)
            {
                radius.Dec();

                if (radius.IsMin)
                {
                    state = State.Dead;
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

        public void Die()
        {
            state = State.DyingWait;
            updateCounter = 0;
        }

        private void BounceBall()
        {
            Vector2 newTopLeft = topLeft + velocity;
            float left, right, top, bottom;
            left = newTopLeft.X;
            right = newTopLeft.X + ((float)radius.Value * 2f);
            top = newTopLeft.Y;
            bottom = newTopLeft.Y + ((float)radius.Value * 2f);

            if (top < 0 || bottom > viewport.Height)
            {
                velocity.Y *= -1;
            }

            if (left < 0 || right > viewport.Width)
            {
                velocity.X *= -1;
            }
        }
    }
}

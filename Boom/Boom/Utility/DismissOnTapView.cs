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
using Pages;

namespace Boom
{
    class DismissOnTapView : View
    {
        private readonly bool _dismissOnTap;

        private bool _visible;

        public DismissOnTapView() : this(true)
        { }

        public DismissOnTapView(bool dismissOnTap)
        {
            _dismissOnTap = dismissOnTap;
        }

        public override void Initialize()
        {
            base.Initialize();

            _visible = false;
        }

        public override void Update(GameTime gameTime, AnimationInfo animationInfo)
        {
            base.Update(gameTime, animationInfo);

            _visible = animationInfo.State == AnimationState.Visible;
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                if (_dismissOnTap && _visible)
                {
                    Dismiss(true);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}

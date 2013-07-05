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
    class Screen : View
    {
        private readonly bool _dismissOnTap, _hasRemoveAdsButton;

        private SpeakerButton _speakerButton;
        private RemoveAdsButton _removeAdsButton;

        public Screen() : this(true)
        { }

        public Screen(bool dismissOnTap) : this(dismissOnTap, true)
        { }

        public Screen(bool dismissOnTap, bool hasRemoveAdsButton)
        {
            _dismissOnTap = dismissOnTap;
            _hasRemoveAdsButton = hasRemoveAdsButton;
        }

        public override void Initialize()
        {
            base.Initialize();

            _speakerButton = new SpeakerButton();
            AddSubview(_speakerButton);

            if (_hasRemoveAdsButton)
            {
                _removeAdsButton = new RemoveAdsButton();
                AddSubview(_removeAdsButton);
            }
        }

        public override bool TouchDown(TouchLocation location)
        {
            if (!base.TouchDown(location))
            {
                if (_dismissOnTap)
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

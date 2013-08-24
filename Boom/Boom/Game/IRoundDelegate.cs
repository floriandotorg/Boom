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
    interface IRoundDelegate
    {
        RoundSettings RoundSettings { get; }
        int Score { get; }
        bool ShouldShowOverlays { get; }
        bool IsScoreVisible { get; }
        bool IsTutorial { get; }

        Viewport Viewport { get; }
        Texture2D BallTexture { get; }
        SoundEffect BlipSound { get; }
        SoundEffect VictorySound { get; }
        SpriteFont Font { get; }
        SpriteFont TapHereFont { get; }
        AnimationInfo RoundOverlayAnimationInfo { get; }

        void ShowOverlay(View overlay);
    }
}

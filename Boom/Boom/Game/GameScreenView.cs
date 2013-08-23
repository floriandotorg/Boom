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
    class GameScreenView : View
    {
        GameView _gameView;

        public GameScreenView(int startRound, int startScore)
        {
            _gameView = new GameView(startRound, startScore);
        }

        public override void Initialize()
        {
            base.Initialize();

            AddSubview(_gameView);

            ShowOverlay(new Screen(false), false);
        }

        public override Color ClearColor
        {
            get
            {
                return _gameView.ClearColor;
            }
        }
    }
}

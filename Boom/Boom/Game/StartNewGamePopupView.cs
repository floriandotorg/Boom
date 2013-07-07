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
    class StartNewGamePopupView : View
    {
        private Button _backToMainMenu, _startNewGameButton;

        public override void Initialize()
        {
            base.Initialize();

            _startNewGameButton = new Button();
            AddSubview(_startNewGameButton);

            _backToMainMenu = new Button();
            AddSubview(_backToMainMenu);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _startNewGameButton.Text = "Start New Game";
            _startNewGameButton.Font = Load<SpriteFont>("InGameBoldFont");
            _startNewGameButton.AutoResize = false;
            _startNewGameButton.Height = 40;
            _startNewGameButton.Tap += _startNewGameButton_Tap;

            _backToMainMenu.Text = "Back to Menu";
            _backToMainMenu.Font = Load<SpriteFont>("InGameFont");
            _backToMainMenu.AutoResize = false;
            _backToMainMenu.Height = 40;
            _backToMainMenu.Tap += _backToMainMenu_Tap;
        }

        void _backToMainMenu_Tap(object sender)
        {
            NavigationController.Back(true);
        }

        void _startNewGameButton_Tap(object sender)
        {
            NavigationController.SwitchTo(new GameView(1, 0), true);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CenterSubview(_startNewGameButton, -25);
            CenterSubview(_backToMainMenu, 25);
        }
    }
}

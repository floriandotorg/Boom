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
    class NavigationController : Pages.NavigationController
    {
        public NavigationController(GraphicsDeviceManager graphics)
            : base(graphics)
        {
            ++GameSettings.GameStarts;

            try
            {
                Highscore.Initialize(error => { if (error != null) System.Diagnostics.Debug.WriteLine(error.LocalizedDescription); else System.Diagnostics.Debug.WriteLine("Highscore sccuess"); });
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
        }

        public void Initialize()
        {
            base.Initialize(new MenuView());
        }

        public override void LoadContent(SpriteBatch spriteBatch, ContentManager content)
        {
            base.LoadContent(spriteBatch, content);

            Load<Texture2D>("BallTexture");
            Load<Texture2D>("SpeakerMuteTexture");
            Load<Texture2D>("SpeakerTexture");
            Load<Texture2D>("TwitterButtonTexture");
            Load<Texture2D>("FacebookButtonTexture");
            Load<Texture2D>("CrossTexture");
            Load<Texture2D>("TwitterTexture");
            Load<Texture2D>("FacebookTexture");
            Load<SpriteFont>("InGameFont");
            Load<SpriteFont>("TitleFont");
            Load<SpriteFont>("InGameLargeFont");
            Load<SpriteFont>("InGameBoldFont");
            Load<SpriteFont>("InGameScoreFont");
            Load<SpriteFont>("InGameSmallFont");
            Load<SpriteFont>("MenuFont");
            Load<SoundEffect>("BlipSound");
            Load<SoundEffect>("VictorySound");
            Load<Song>("BackgroundSong");

            if (GameSettings.Speaker)
            {
                VolumeManager.SetDefaultVolume();
            }
            else
            {
                VolumeManager.Mute();
            }

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Load<Song>("BackgroundSong"));
            }
        }
    }
}

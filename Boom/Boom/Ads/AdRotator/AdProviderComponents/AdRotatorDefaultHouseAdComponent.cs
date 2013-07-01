using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#if(!XBOX360)
using AdRotatorXNA.Helpers;
#endif
using System.IO.IsolatedStorage;
using System.IO;


namespace AdRotatorXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    internal class AdRotatorDefaultHouseAd : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private static AdRotatorDefaultHouseAd instance;

        private static Texture2D DefaultTexture;

        private static Texture2D DefaultHouseAdRemoteTexture;

        private static string DefaultHouseAdRemoteURL;

        private bool ImageLoadFail = false;

        public delegate void OnAdFailed(object sender, EventArgs e);
        public delegate void OnAdLoaded(object sender, EventArgs e);
        public delegate void OnAdClicked(object sender, EventArgs e);

        public event OnAdFailed AdLoadingFailed;
        public event OnAdLoaded AdLoaded;
        public event OnAdClicked AdClicked;
        
        private Vector2 adPosition = Vector2.Zero;

        private Rectangle BannerRect = Rectangle.Empty;

        private bool Initialised = false;

        private AdRotatorDefaultHouseAd(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }


        public static AdRotatorDefaultHouseAd Current
        {
            get
            {
                if (AdRotatorXNAComponent.game == null)
                {
                    return null;
                }
                if (instance == null)
                {
                    instance = new AdRotatorDefaultHouseAd(AdRotatorXNAComponent.game);
                }

                return instance;
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Initialised = true;
            base.Initialize();
        }

        public static void Initialize(Game _game, Texture2D defaultTexture, string defaultHouseAdRemoteURL)
        {
            if (defaultTexture == null)
            {
                defaultTexture = new Texture2D(_game.GraphicsDevice, AdRotatorXNAComponent.Current.AdWidth, AdRotatorXNAComponent.Current.AdHeight);
                defaultTexture.SetData<Color>(new Color[1] { Color.Gray });
            }
            DefaultTexture = defaultTexture;
            DefaultHouseAdRemoteURL = defaultHouseAdRemoteURL;
        }

        protected override void LoadContent()
        {
            if (DefaultHouseAdRemoteURL != null && !string.IsNullOrEmpty(DefaultHouseAdRemoteURL))
            {
                GetRemoteImageFromURL();
            }
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (AdRotatorXNAFunctions.TestAdClicked(BannerRect))
            {
                if (AdClicked != null)
                {
                    AdClicked(null, new EventArgs());
                }
            }

            base.Update(gameTime);
        }

        public void UpdateAdPosition(Vector2 newposition)
        {
            adPosition = newposition;
            if (Initialised) UpdateBanner();
        }

        private void UpdateBanner()
        {
            BannerRect = new Rectangle((int)adPosition.X, (int)adPosition.Y, AdRotatorXNAComponent.Current.AdWidth, AdRotatorXNAComponent.Current.AdHeight);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = AdRotatorXNAComponent.Current.spriteBatch;
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (DefaultHouseAdRemoteTexture != null)
            {
                spriteBatch.Draw(DefaultHouseAdRemoteTexture, BannerRect, Color.White);
            }
            else
            {
                if (DefaultHouseAdRemoteURL == null || string.IsNullOrEmpty(DefaultHouseAdRemoteURL) || (DefaultHouseAdRemoteURL != null && ImageLoadFail))
                {
                    spriteBatch.Draw(DefaultTexture, BannerRect, Color.White);
                }
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }

        void GetRemoteImageFromURL()
        {
#if(!XBOX360)
            if (DefaultHouseAdRemoteURL != null && !string.IsNullOrEmpty(DefaultHouseAdRemoteURL))
            {
                ImageDownload.GetImageFromURL(GraphicsDevice, DefaultHouseAdRemoteURL, (s, e) =>
                {
                    if (e != null)
                    {
                        LoadRemoteImageFromISO();
                        Console.WriteLine(e.Message);
                        if (AdLoadingFailed != null)
                        {
                            AdLoadingFailed(e, new EventArgs());
                            ImageLoadFail = true;
                        }
                    }
                    else
                    {
                        DefaultHouseAdRemoteTexture = s;
                        SaveRemoteImagetoISO();
                        UpdateBanner();

                        if (AdLoaded != null)
                        {
                            AdLoaded(DefaultHouseAdRemoteURL, new EventArgs());
                        }
                    }
                });
            }
            else
            {
#endif

                LoadRemoteImageFromISO();
#if(!XBOX360)

            }
#endif
        }

        void SaveRemoteImagetoISO()
        {
            if (DefaultHouseAdRemoteTexture != null && DefaultHouseAdRemoteTexture.Height > 1 && DefaultHouseAdRemoteTexture.Width > 1)
            {
                try
                {
                    using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("DefaultHouseAdRemoteTexture", FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication()))
                    {
                        DefaultHouseAdRemoteTexture.SaveAsPng(isfStream, DefaultHouseAdRemoteTexture.Width, DefaultHouseAdRemoteTexture.Height);
                    }
                }
                catch
                {
                }
            }
        }

        void LoadRemoteImageFromISO()
        {
            try
            {
                using (IsolatedStorageFileStream isfStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("DefaultHouseAdRemoteTexture", FileMode.Open))
                {
                    DefaultHouseAdRemoteTexture = Texture2D.FromStream(this.GraphicsDevice, isfStream);
                }
            }
            catch
            {
                ImageLoadFail = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using AdRotator.Networking;
using AdRotatorXNA.Helpers;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AdRotatorXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    internal class AdInneractiveComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private static AdInneractiveComponent instance;
        private static string AdInneractiveAppID;

        public delegate void OnAdFailed(object sender, EventArgs e);
        public delegate void OnAdLoaded(object sender, EventArgs e);
        public delegate void OnAdClicked(object sender, EventArgs e);

        public event OnAdFailed AdLoadingFailed;
        public event OnAdLoaded AdLoaded;
        public event OnAdClicked AdClicked;


        private SpriteBatch spriteBatch;

        private bool Initialised = false;

        private Texture2D AdImage;
        private Uri AdUrl;

        private Rectangle BannerRect = Rectangle.Empty;

        private Vector2 adPosition = Vector2.Zero;

        private AdInneractiveComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public static AdInneractiveComponent Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdInneractiveComponent(AdRotatorXNAComponent.game);
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

        public static void Initialize(Game _game, string _ID)
        {
            if (string.IsNullOrEmpty(_ID))
            {
                _ID = "0";
            }
            AdInneractiveAppID = _ID;
        }
                
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = AdRotatorXNAComponent.Current.spriteBatch;
            GetAdWebResponse(null);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
                try
                {
                    WebBrowserTask wb = new WebBrowserTask();
                    wb.Uri = AdUrl;
                    wb.Show();
                }
                catch { }
                finally
                {
                    if (AdClicked != null)
                    {
                        AdClicked(AdUrl, new EventArgs());
                    }
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
            // TODO: Add your drawing code here
            if (AdImage != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(AdImage, BannerRect, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        protected internal void GetAdWebResponse(Action OnComplete)
        {
            Uri uri = new Uri(string.Format("http://m2m1.inner-active.com/simpleM2M/clientRequestAd?aid={0}&po=659&v=Sm2m-1.5.3&w=300&h=75", AdInneractiveAppID));
            AdWebRequest.ReadInnerActiveAdResponse(uri,(results, ex) =>
            {
                if (ex != null)
                {
                    Console.WriteLine(ex.Message);
                    if (AdLoadingFailed != null)
                    {
                        AdLoadingFailed(ex, new EventArgs());
                    }
                    return;
                }

                List<AdWebResponse> AdWebResponseItems = results;

                ImageDownload.GetImageFromURL(GraphicsDevice, AdWebResponseItems[0].ImageURL,(s,e) =>
                {
                    if (e != null)
                    {
                        Console.WriteLine(e.Message);
                        if (AdLoadingFailed != null)
                        {
                            AdLoadingFailed(e, new EventArgs());
                        }
                    }
                    else
                    {
                        AdImage = s;
                        AdUrl = new Uri(AdWebResponseItems[0].URL);

                        UpdateBanner();

                        if (AdLoaded != null)
                        {
                            AdLoaded(results, new EventArgs());
                        }
                    }
                });

                //preload About section
                OnComplete.IfNotNullInvoke();
            });
        }
    }
}

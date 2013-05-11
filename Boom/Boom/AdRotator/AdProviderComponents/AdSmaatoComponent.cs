using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;


namespace AdRotatorXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    internal class AdSmaatoComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private static AdSmaatoComponent instance;
        private SpriteBatch spriteBatch;

        private bool Initialised = false;

        private static string SmaatoPublisherId;
        private static string SmaatoAppId;

        public delegate void OnAdFailed(object sender, string ErrorCode, string ErrorDescription);
        public delegate void OnAdLoaded(object sender, System.EventArgs e);
        public delegate void OnAdClicked(object sender, System.EventArgs e);

        public event OnAdFailed AdLoadingFailed;
        public event OnAdLoaded AdLoaded;
        public event OnAdClicked AdClicked;

        private SOMAWP7.SomaAd somaAd;

        private Texture2D AdImage;
        private Uri AdUrl;

        private Rectangle BannerRect = Rectangle.Empty;

        private Vector2 adPosition = new Vector2(0,0);

        string currentAdImageFileName = "";

        public bool IsTest = false;

        private AdSmaatoComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public static AdSmaatoComponent Current
        {
            get
            {
                if (AdRotatorXNAComponent.game == null)
                {
                    return null;
                }
                if (instance == null)
                {
                    instance = new AdSmaatoComponent(AdRotatorXNAComponent.game);
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

        public static void Initialize(Game _game, string _ID, string _Publisher)
        {
            if (string.IsNullOrEmpty(_ID))
            {
                _ID = "0";
            }
            SmaatoAppId = _ID;
            SmaatoPublisherId = _Publisher;
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = AdRotatorXNAComponent.Current.spriteBatch;
            
            // TODO: use this.Content to load your game content here
            try
            {
                somaAd = new SOMAWP7.SomaAd();
                somaAd.Adspace = int.Parse(SmaatoAppId); // 0;   // Developer Ads 
                somaAd.Pub = int.Parse(SmaatoPublisherId); // 0;       // Developer Ads
                somaAd.AdSpaceHeight = 80;
                somaAd.AdSpaceWidth = 480;

                somaAd.GetAdError += new SOMAWP7.SomaAd.OnGetAdError(somaAd_GetAdError);
                somaAd.NewAdAvailable += new SOMAWP7.SomaAd.OnNewAdAvailable(somaAd_NewAdAvailable);

                somaAd.GetAd();
            }
            catch 
            {
                if (AdLoadingFailed != null)
                {
                    AdLoadingFailed(null,"","Failed to Load Content");
                }
            }

        }

        void somaAd_NewAdAvailable(object sender, System.EventArgs e)
        {

            // if there is a new ad, get it from Isolated Storage and  show it
            if (somaAd.Status == "success" && somaAd.AdImageFileName != null && somaAd.ImageOK)
            {
                AdUrl = new Uri(somaAd.Uri);
                try
                {
                    if (currentAdImageFileName != somaAd.AdImageFileName)
                    {
                        currentAdImageFileName = somaAd.AdImageFileName;
                        using (IsolatedStorageFile myIsoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream myAd = new IsolatedStorageFileStream(somaAd.AdImageFileName, FileMode.Open, myIsoStore))
                            {
                                AdImage = Texture2D.FromStream(this.GraphicsDevice, myAd);
                            }
                        }
                    }
                }
                catch (IsolatedStorageException ise)
                {
                    string message = ise.Message;
                }
            }
            if (AdLoaded != null)
            {
                AdLoaded(sender, e);
            }
            Debug.WriteLine("AdDuplexAdLoaded");
        }

        void somaAd_GetAdError(object sender, string ErrorCode, string ErrorDescription)
        {
            if (AdLoadingFailed != null)
            {
                AdLoadingFailed(sender, ErrorCode, ErrorDescription);
            }
            Debug.WriteLine("AdDuplexAdLoadError");
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            somaAd.NewAdAvailable -= somaAd_NewAdAvailable;
            somaAd.GetAdError -= somaAd_GetAdError;
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
                    if (AdUrl != null)
                    {
                        WebBrowserTask wb = new WebBrowserTask();
                        wb.Uri = AdUrl;
                        wb.Show();
                    }
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
    }
}

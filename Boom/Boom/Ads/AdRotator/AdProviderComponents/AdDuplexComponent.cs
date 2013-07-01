using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AdRotatorXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    internal class AdDuplexComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private static AdDuplexComponent instance;
        private SpriteBatch spriteBatch;

        private static string AdDuplexAppID;

        public delegate void OnAdFailed(object sender, AdDuplex.AdLoadingErrorEventArgs e);
        public delegate void OnAdLoaded(object sender, AdDuplex.AdLoadedEventArgs e);
        public delegate void OnAdClicked(object sender, AdDuplex.AdClickEventArgs e);

        public event OnAdFailed AdLoadingFailed;
        public event OnAdLoaded AdLoaded;
        public event OnAdClicked AdClicked;

        private AdDuplex.Xna.AdManager adDuplex;
        private Vector2 adPosition = new Vector2(0,0);

        public bool IsTest = false;

        private AdDuplexComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public static AdDuplexComponent Current
        {
            get
            {
                if (AdRotatorXNAComponent.game == null)
                {
                    return null;
                }
                if (instance == null)
                {
                    instance = new AdDuplexComponent(AdRotatorXNAComponent.game);
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
            base.Initialize();
        }

        public static void Initialize(Game _game, string _ID)
        {
            if (string.IsNullOrEmpty(_ID))
            {
                _ID = "0";
            }
            AdDuplexAppID = _ID;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = AdRotatorXNAComponent.Current.spriteBatch;
            
            // TODO: use this.Content to load your game content here
            adDuplex = new AdDuplex.Xna.AdManager(this.Game, AdDuplexAppID);
            adDuplex.LoadContent();
            adDuplex.AdClick += adDuplex_AdClick;
            adDuplex.AdLoaded += adDuplex_AdLoaded;
            adDuplex.AdLoadingError += adDuplex_AdLoadingError;
        }

        void adDuplex_AdLoadingError(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            if (AdLoadingFailed != null)
            {
                AdLoadingFailed(sender, e); 
            }
            Debug.WriteLine("AdDuplexAdLoadError");
        }

        void adDuplex_AdLoaded(object sender, AdDuplex.AdLoadedEventArgs e)
        {
            if (AdLoaded != null)
            {
                AdLoaded(sender, e);
            }
            Debug.WriteLine("AdDuplexAdLoaded");
        }

        void adDuplex_AdClick(object sender, AdDuplex.AdClickEventArgs e)
        {
            if (AdClicked != null)
            {
                AdClicked(sender, e);
            }
            Debug.WriteLine("AdDuplexAdClicked");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            adDuplex.AdClick -= adDuplex_AdClick;
            adDuplex.AdLoaded -= adDuplex_AdLoaded;
            adDuplex.AdLoadingError -= adDuplex_AdLoadingError;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            adDuplex.IsTest = IsTest;
            adDuplex.Update(gameTime);
            base.Update(gameTime);
        }
        
        public void UpdateAdPosition(Vector2 newposition)
        {
            adPosition = newposition;
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            adDuplex.Draw(spriteBatch, adPosition);

            base.Draw(gameTime);
        }
    }
}

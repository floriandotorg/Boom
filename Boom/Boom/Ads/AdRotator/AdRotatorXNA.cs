#if(WINDOWS_PHONE)
#if(!NONLOCATION)
using System.Device.Location;
#endif
using Microsoft.Phone.Net.NetworkInformation;
#endif
#if(!XBOX360)
using System.Diagnostics;
using Microsoft.Advertising.Mobile.Xna;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using AdRotator.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AdRotatorXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AdRotatorXNAComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private static AdRotatorXNAComponent instance;
        internal SpriteBatch spriteBatch;
        internal static Game game;

        public string DeviceIP = "192.168.0.1";
        
        private bool _loaded = false;
        private bool _initialised = false;
        private bool _started = false;

        public delegate void LogHandler(string message);

        public event LogHandler Log;

        private AdType CurrentAdType;

        private AdCultureDescriptor CurrentCulture = null;

        private const string SETTINGS_FILE_NAME = "AdRotatorSettings";

        /// <summary>
        /// The displayed ad control instance
        /// </summary>
        private GameComponent _currentAdControl;

        /// <summary>
        /// Random generato
        /// </summary>
        private static Random _rnd = new Random();

        /// <summary>
        /// List of the ad types that have failed to load
        /// </summary>
        private static List<AdType> _failedAdTypes = new List<AdType>();

        /// <summary>
        /// The ad settings based on which the ad descriptor for the current UI culture can be selected
        /// </summary>
        private AdSettings _settings;

        /// <summary>
        /// Indicates whether there has been an attemt to fetch the remote settings file
        /// </summary>
        private static bool _remoteAdSettingsFetched = false;

        /// <summary>
        /// Indicates whether a network has been detected and available, is turned off if none found
        /// </summary>
        private bool _isNetworkAvailable = true;

        public static bool LocationEnabled = true;


        #region LoggingEventCode
        protected void OnLog(string message)
        {
            if (Log != null)
            {
                Log(message);
            }
        }
        #endregion

        #region SettingsUrl

        /// <summary>
        /// Gets or sets the URL of the remote ad descriptor file
        /// </summary>
        public string SettingsUrl
        {
            get { return (string)SettingsUrlProperty; }
            set { SettingsUrlProperty = value; }
        }

        private string SettingsUrlProperty = "";

        #endregion

        #region DefaultSettingsFileUri

        public string DefaultSettingsFileUri
        {
            get { return (string)DefaultSettingsFileUriProperty; }
            set { DefaultSettingsFileUriProperty = value; }
        }

        private string DefaultSettingsFileUriProperty = "defaultAdSettings.xml";

        #endregion

        #region DefaultAdType

        public AdType DefaultAdType
        {
            get { return (AdType)DefaultAdTypeProperty; }
            set { DefaultAdTypeProperty = value; }
        }

        private AdType DefaultAdTypeProperty = AdType.None;

        #endregion

        #region IsEnabled

        /// <summary>
        /// When set to false the control does not display
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)IsEnabledProperty; }
            set 
            {
                IsEnabledProperty = value;
                _currentAdControl.Enabled = value;
            }
        }

        private bool IsEnabledProperty = true;

        #endregion

        #region IsTest

        /// <summary>
        /// When set to false the control does not display
        /// </summary>
        public bool IsTest
        {
            get { return (bool)IsTestProperty;}
            set { IsTestProperty = value;}
        }

        private bool IsTestProperty = false;

        #endregion

        #region SlidingAd Properties

        #region SlidingAdDirection

        /// <summary>
        /// Timer to track how long the control has been hidden or displayed
        /// Internal value
        /// </summary>
        private float SlidingAdTimer = 0;

        /// <summary>
        /// Direction the popup will hide / appear from
        /// If not set the AdControl will remain on screen
        /// </summary>
        public SlideDirection SlidingAdDirection
        {
            get { return (SlideDirection)SlidingAdDirectionProperty; }
            set { SlidingAdDirectionProperty = value; }
        }

        private SlideDirection SlidingAdDirectionProperty = SlideDirection.None;

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 0;


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        private AdTransitionState adTransitionStateProperty = AdTransitionState.Active;

        public AdTransitionState AdTransitionState
        {
            get { return adTransitionStateProperty; }
            set { adTransitionStateProperty = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.FromSeconds(8);


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.FromSeconds(8);
        #endregion

        #region SlidingAdDisplaySeconds

        /// <summary>
        /// Amount of time in seconds the ad is displayed on Screen if <see cref="SlidingAdType"/> is set to something else than None
        /// </summary>
        public int SlidingAdDisplaySeconds
        {
            get { return (int)SlidingAdDisplaySecondsProperty.Seconds; }
            set { SlidingAdDisplaySecondsProperty = TimeSpan.FromSeconds(value); }
        }

        private TimeSpan SlidingAdDisplaySecondsProperty = TimeSpan.FromSeconds(10);

        #endregion

        #region SlidingAdHiddenSeconds

        /// <summary>
        ///  Amount of time in seconds to wait before displaying the ad again 
        ///  (if <see cref="SlidingAdType"/> is set to something else than None).
        ///  Basically the lower this number the more the user is "nagged" by the ad coming back now and again
        /// </summary>
        public int SlidingAdHiddenSeconds
        {
            get { return (int)SlidingAdHiddenSecondsProperty.Seconds; }
            set { SlidingAdHiddenSecondsProperty = TimeSpan.FromSeconds(value); }
        }

        private TimeSpan SlidingAdHiddenSecondsProperty = TimeSpan.FromSeconds(20);

        #endregion

        #endregion

        #region ADProviderProperties


        #region DefaultHouseAd

        /// <summary>
        /// Default Ad Image to display if no network present to get Ads
        /// </summary>
        public Texture2D DefaultHouseAdImage { get; set; }

        public string DefaultHouseAdRemoteURL
        {
            get
            {
                if (string.IsNullOrEmpty(DefaultHouseAdRemoteURLProperty))
                {
                    DefaultHouseAdRemoteURLProperty = GetAppID(AdType.DefaultHouseAd);
                }
                return (string)DefaultHouseAdRemoteURLProperty;
            }
            set { DefaultHouseAdRemoteURLProperty = value; }
        }

        private string DefaultHouseAdRemoteURLProperty = "";

        public delegate void DefaultHouseAdClickEventHandler();

        public event DefaultHouseAdClickEventHandler DefaultHouseAdClick;
        #endregion
#if(!XBOX360)

        #region Pubcenter
        #region PubCenterAppId

        public string PubCenterAppId
        {
            get 
            {
                if (string.IsNullOrEmpty(PubCenterAppIdProperty))
                {
                    PubCenterAppIdProperty = GetAppID(AdType.PubCenter);
                }
                return (string)PubCenterAppIdProperty;
            }
            set { PubCenterAppIdProperty = value; }
        }

        private string PubCenterAppIdProperty = "";

        #endregion

        #region PubCenterAdUnitId

        public string PubCenterAdUnitId
        {
            get
            {
                if (string.IsNullOrEmpty(PubCenterAdUnitIdProperty))
                {
                    PubCenterAdUnitIdProperty = GetSecondaryID(AdType.PubCenter);
                }
                return (string)PubCenterAdUnitIdProperty;
            }
            set { PubCenterAdUnitIdProperty = value; }
        }

        private string PubCenterAdUnitIdProperty = ""; //other test values: Image480_80, Image300_50, TextAd 

        #endregion

        DrawableAd bannerAd;

#if(!NONLOCATION)
        // We will use this to find the device location for better ad targeting. 
        private GeoCoordinateWatcher gcw = null;
#endif
        #endregion

        #region AdDuplexAppId

        public string AdDuplexAppId
        {
            get 
            {
                if (string.IsNullOrEmpty(AdDuplexAppIdProperty))
                {
                    AdDuplexAppIdProperty = GetAppID(AdType.AdDuplex);
                }
                return (string)AdDuplexAppIdProperty;
            }            
            set { AdDuplexAppIdProperty = value; }
        }

        private string AdDuplexAppIdProperty = "";

        #endregion

        #region InneractiveAppId

        public string InneractiveAppId
        {
            get 
            {
                if (string.IsNullOrEmpty(InneractiveAppIdProperty))
                {
                    InneractiveAppIdProperty = GetAppID(AdType.InnerActive);
                }
                return (string)InneractiveAppIdProperty;
            }   
            set { InneractiveAppIdProperty = value; }
        }

        private string InneractiveAppIdProperty = string.Empty;

        #endregion

        #region MobFox

        public string MobFoxAppId
        {
            get 
            {
                if (string.IsNullOrEmpty(MobFoxAppIdProperty))
                {
                    MobFoxAppIdProperty = GetAppID(AdType.MobFox);
                }
                return (string)MobFoxAppIdProperty;
            }  
            set { MobFoxAppIdProperty = value; }
        }

        private string MobFoxAppIdProperty = string.Empty;


        public bool MobFoxIsTest
        {
            get { return (bool)MobFoxIsTestProperty; }
            set { MobFoxIsTestProperty = value; }
        }

        private bool MobFoxIsTestProperty = false;
                
        #endregion

        #if(!NONLOCATION)
        #region Smaato

        #region SmaatoPublisherId

        public string SmaatoPublisherId
        {
            get
            {
                if (string.IsNullOrEmpty(SmaatoPublisherIdProperty))
                {
                    SmaatoPublisherIdProperty = GetSecondaryID(AdType.Smaato);
                }
                return (string)SmaatoPublisherIdProperty;
            }
            set { SmaatoPublisherIdProperty = value; }
        }

        private string SmaatoPublisherIdProperty = string.Empty;
        #endregion

        #region SmaatoAppId
        public string SmaatoAppId
        {
            get
            {
                if (string.IsNullOrEmpty(SmaatoAppIdProperty))
                {
                    SmaatoAppIdProperty = GetAppID(AdType.Smaato);
                }
                return (string)SmaatoAppIdProperty;
            }
            set { SmaatoAppIdProperty = value; }
        }

        private string SmaatoAppIdProperty = string.Empty;

        #endregion

        #endregion
#endif
#endif

        #region AdValidSettings

        private bool IsDefaultHouseAdValid
        {
            get
            {
                return DefaultHouseAdImage != null;
            }
        }
 #if(!XBOX360)       
        private bool IsPubCenterValid
        {
            get
            {
                return !String.IsNullOrEmpty(PubCenterAppId) && !String.IsNullOrEmpty(PubCenterAdUnitId);
            }
        }

        private bool IsAdDuplexValid
        {
            get
            {
                return !String.IsNullOrEmpty(AdDuplexAppId);
            }
        }

        private bool IsInneractiveValid
        {
            get
            {
                return !String.IsNullOrEmpty(InneractiveAppId);
            }
        }

        private bool IsMobFoxValid
        {
            get
            {
                return !String.IsNullOrEmpty(MobFoxAppId);
            }
        }
#if(!NONLOCATION)
        private bool IsSmaatoValid
        {
            get
            {
                int SmaatoIDTest;
                return (!String.IsNullOrEmpty(SmaatoAppId) && int.TryParse(SmaatoAppId, out SmaatoIDTest) && !String.IsNullOrEmpty(SmaatoPublisherId) && int.TryParse(SmaatoPublisherId, out SmaatoIDTest));
            }
        }
#endif
#endif

        #endregion

        #endregion

        #region Ad Size Settings
        private int AdWidthProperty = 480;

        public int AdWidth
        {
            get { return AdWidthProperty; }
            set { AdWidthProperty = value; }
        }


        private int AdHeightProperty = 100;

        public int AdHeight
        {
            get { return AdHeightProperty; }
            set { AdHeightProperty = value; }
        }
        

        #endregion

        #region Ad Placement Settings
        /// <summary>
        /// Top Left point where the Ad will be placed on screen
        /// Be sure to update for the current orientation your game is runing at
        /// </summary>
        private Vector2 AdPositionPropery = Vector2.Zero;

        public Vector2 AdPosition
        {
            get { return AdPositionPropery; }
            set { AdPositionPropery = value; UpdateAdPostition(); }
        }
        
        #endregion


        private AdRotatorXNAComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public static AdRotatorXNAComponent Current
        {
            get
            {
                if (game == null)
                {
                    return null;
                }
                if (instance == null)
                {
                    instance = new AdRotatorXNAComponent(game);
                }

                return instance;
            }
        }

        #region XNA Specific
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LoadAdSettings();

            base.Initialize();
        }

        public static void Initialize(Game _game)
        {
            game = _game;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: Add your initialization code here

            _loaded = true;
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            if (_currentAdControl != null)
                ((DrawableGameComponent)_currentAdControl).UpdateOrder = this.UpdateOrder;
            base.OnUpdateOrderChanged(sender, args);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (_currentAdControl != null)
            {
                ((DrawableGameComponent)_currentAdControl).Enabled = this.Enabled;
//#if(WINDOWS_PHONE)
//                if (this.Enabled && !_isNetworkAvailable && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None && NetworkInterface.GetIsNetworkAvailable())
//                    _isNetworkAvailable = true;
//#endif

            }
            base.OnEnabledChanged(sender, args);
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            if (_currentAdControl != null)
                ((DrawableGameComponent)_currentAdControl).Visible = this.Visible;
            base.OnVisibleChanged(sender, args);
        }

        protected override void OnDrawOrderChanged(object sender, EventArgs args)
        {
            if (_currentAdControl != null)
                ((DrawableGameComponent)_currentAdControl).DrawOrder = this.DrawOrder;
            base.OnDrawOrderChanged(sender, args);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (!_started && _initialised)
            {
                _started = true;
                Init();
            }

            //Check Current Ad State and transition as required
            if (SlidingAdDirection != SlideDirection.None && _loaded)
            {
                switch (AdTransitionState)
                {
                    case AdTransitionState.TransitionOn:
                        // Otherwise the screen should transition on and become active.
                        if (UpdateTransition(gameTime, transitionOnTime, -1))
                        {
                            // Still busy transitioning.
                            AdTransitionState = AdTransitionState.TransitionOn;
                        }
                        else
                        {
                            // Transition finished!
                            AdTransitionState = AdTransitionState.Active;
                            SlidingAdTimer = 0;
                        }
                        break;
                    case AdTransitionState.Active:
                        if (SlidingAdTimer > SlidingAdDisplaySecondsProperty.TotalMilliseconds)
                        {
                            // Finished displaying, hide Ad
                            AdTransitionState = AdTransitionState.TransitionOff;
                            SlidingAdTimer = 0;
                        }
                        else
                        {
                            SlidingAdTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        }
                        break;
                    case AdTransitionState.TransitionOff:
                        // If the screen is covered by another, it should transition off.
                        if (UpdateTransition(gameTime, transitionOffTime, 1))
                        {
                            // Still busy transitioning.
                            AdTransitionState = AdTransitionState.TransitionOff;
                        }
                        else
                        {
                            // Transition finished!
                            Invalidate(false);
                            AdTransitionState = AdTransitionState.Hidden;
                            SlidingAdTimer = 0;
                        }
                        break;
                    case AdTransitionState.Hidden:
                        if (SlidingAdTimer > SlidingAdHiddenSecondsProperty.TotalMilliseconds)
                        {
                            // Ad ready to display
                            AdTransitionState = AdTransitionState.TransitionOn;
                            SlidingAdTimer = 0;

                        }
                        else
                        {
                            SlidingAdTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        }
                        break;
                    default:
                        break;
                }


                // Make the menu slide into place during transitions, using a
                // power curve to make things look more interesting (this makes
                // the movement slow down as it nears the end).
                float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

                // start at Y = 175; each X value is generated per entry
                Vector2 position = AdPosition;

                if (AdTransitionState == AdTransitionState.TransitionOn)
                {
                    switch (SlidingAdDirection)
                    {
                        case SlideDirection.Top:
                            position.Y += transitionOffset * 5;
                            break;
                        case SlideDirection.Bottom:
                            position.Y -= transitionOffset * 5;
                            break;
                        case SlideDirection.Left:
                            position.X += transitionOffset * 10;
                            break;
                        case SlideDirection.Right:
                            position.X -= transitionOffset * 10;
                            break;
                        default:
                            break;
                    }
                }
                else if (AdTransitionState == AdTransitionState.TransitionOff)
                {
                    switch (SlidingAdDirection)
                    {
                        case SlideDirection.Top:
                            position.Y -= transitionOffset * 5;
                            break;
                        case SlideDirection.Bottom:
                            position.Y += transitionOffset * 5;
                            break;
                        case SlideDirection.Left:
                            position.X -= transitionOffset * 10;
                            break;
                        case SlideDirection.Right:
                            position.X += transitionOffset * 10;
                            break;
                        default:
                            break;
                    }
                }

                // set the entry's position
                AdPosition = position;
                UpdateAdPostition();
            }
            else if (_currentAdControl != null)
            {
                UpdateAdPostition();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        #endregion



        /// <summary>
        /// Displays a new ad
        /// </summary>
        /// <param name="selectNextAdType">If set to true, selects the next ad type in order, otherwise chooses 
        ///     a random one that hasn't had issues loading previously</param>
        /// <returns></returns>
        public void Invalidate()
        {
            Invalidate(false);
        }

        public string Invalidate(bool selectNextAdType)
        {

            string adTypeName = "";

            if (!_loaded || !_initialised)
            {
                OnLog("Control tested before loaded");
                return "";
            }
            if (!IsEnabled)
            {
                OnLog("Ad control disabled");
                return adTypeName;
            }
            else
            {
                OnLog("Ads are enabled for display");
            }

            RemoveEventHandlersFromAdControl();
            this.Game.Components.Remove(_currentAdControl);
            //AdType adType = AdType.None;
            if (_isNetworkAvailable)
            {
                if (selectNextAdType)
                {
                    CurrentAdType = CurrentAdType++;
                    if (CurrentAdType >= AdType.None)
                    {
                        CurrentAdType = AdType.None;
                    }
                }
                else
                {
                    CurrentAdType = GetNextAdType();
                }
            }
            else
            {
                CurrentCulture = CurrentCulture == null ? GetAdDescriptorBasedOnUICulture() : CurrentCulture;
                if (CurrentCulture == null)
                {
                    //Failed to get any add configuration, control disabled
                    CurrentAdType = AdType.None;
                    Enabled = false;
                }
                else
                {
                    var DefaultHouseAd = CurrentCulture.AdProbabilities.FirstOrDefault(x => x.AdType == AdType.DefaultHouseAd);
                    if (DefaultHouseAd != null)
                    {
                        CurrentAdType = AdType.DefaultHouseAd;
                    }
                    else
                    {
                        CurrentAdType = AdType.None;
                    }
                }
            }
            OnLog(string.Format("Ads being requested for: {0}", CurrentAdType.ToString()));
            switch (CurrentAdType)
            {
#if(!XBOX360)

                case AdType.PubCenter:
                    _currentAdControl = CreatePubCentertAdControl();
                    break;
                case AdType.AdDuplex:
                    _currentAdControl = CreateAdDuplexControl();
                    break;
                case AdType.InnerActive:
                    _currentAdControl = CreateInneractiveControl();
                    break;
                case AdType.MobFox:
                    _currentAdControl = CreateMobFoxControl();
                    break;
#if(!NONLOCATION)
                case AdType.Smaato:
                    _currentAdControl = CreateSmaatoControl();
                    break;
#endif
#endif
                case AdType.DefaultHouseAd:
                    _currentAdControl = CreateDefaultHouseAdControl();
                    break;
                default:
                    _currentAdControl = null;
                    return adTypeName;
            }
            if (_currentAdControl == null)
            {
                OnAdLoadFailed(CurrentAdType);
                OnLog(string.Format("Ads failed for: {0}", CurrentAdType.ToString()));
                adTypeName = "failed - " + CurrentAdType.ToString();
                return adTypeName;
            }

            ApplyXNAProperties(_currentAdControl);

            AddEventHandlersToAdControl();
            this.Game.Components.Add(_currentAdControl);

            if (CurrentAdType != AdType.None)
            {
                adTypeName = CurrentAdType.ToString();
                OnLog(string.Format("Ads being served for: {0}", CurrentAdType.ToString()));
                return adTypeName;
			}
            else
            {
                IsEnabled = false;
                OnLog("No ads available, nothing to show");
            }
            return adTypeName;
        }

        private void ApplyXNAProperties(GameComponent CurrentAdControl)
        {
            ((DrawableGameComponent)CurrentAdControl).Enabled = this.Enabled;
            ((DrawableGameComponent)CurrentAdControl).Visible = this.Visible;
            ((DrawableGameComponent)CurrentAdControl).DrawOrder = this.DrawOrder;
            ((DrawableGameComponent)CurrentAdControl).UpdateOrder = this.UpdateOrder;
        }

        /// <summary>
        /// Generates what the next ad type to display should be
        /// </summary>
        /// <returns></returns>
        private AdType GetNextAdType()
        {
            if (CurrentCulture == null)
            {
                CurrentCulture = GetAdDescriptorBasedOnUICulture();
            }

            if (CurrentCulture == null)
            {
                return DefaultAdType;
            }

            var defaultHouseAd = CurrentCulture.AdProbabilities.FirstOrDefault(x => x.AdType == AdType.DefaultHouseAd && !_failedAdTypes.Contains(x.AdType));

            var validDescriptors = CurrentCulture.AdProbabilities
                .Where(x => x.ProbabilityValue > 0 && !_failedAdTypes.Contains(x.AdType)
                            && x.ProbabilityValue > 0
                            && IsAdTypeValid(x.AdType))
                .ToList();
            if (validDescriptors.Count == 0 && defaultHouseAd == null)
            {
                return DefaultAdType;
            }
            var totalValueBetweenValidAds = validDescriptors.Sum(x => x.ProbabilityValue);
            var randomValue = _rnd.NextDouble() * totalValueBetweenValidAds;
            double totalCounter = 0;
            foreach (var probabilityDescriptor in validDescriptors)
            {
                totalCounter += probabilityDescriptor.ProbabilityValue;
                if (randomValue < totalCounter)
                {
                    return probabilityDescriptor.AdType;
                }
            }
            if (defaultHouseAd != null)
            {
                return AdType.DefaultHouseAd;
            }
            return DefaultAdType;
        }

        /// <summary>
        /// Called when the settings have been loaded. Clears all failed ad types and invalidates the control
        /// </summary>
        private void Init()
        {
            _failedAdTypes.Clear();
            Invalidate(false);
        }

        private bool IsAdTypeValid(AdType adType)
        {
            switch (adType)
            {
#if(!XBOX360)

                case AdType.PubCenter:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsPubCenterValid.ToString()));
                    return IsPubCenterValid;
                case AdType.AdDuplex:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsAdDuplexValid.ToString()));
                    return IsAdDuplexValid;
                case AdType.InnerActive:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsInneractiveValid.ToString()));
                    return IsInneractiveValid;
                case AdType.MobFox:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsMobFoxValid.ToString()));
                    return IsMobFoxValid;
#if(!NONLOCATION)
                case AdType.Smaato:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsSmaatoValid.ToString()));
                    return IsSmaatoValid;
#endif
#endif
                case AdType.DefaultHouseAd:
                    OnLog(string.Format("Testing \"{0}\" - Result {1}", adType.ToString(), IsDefaultHouseAdValid.ToString()));
                    return IsDefaultHouseAdValid;
            }
            //Davide Cleopadre www.cleosolutions.com
            //if for any reason the AdType cannot be found is not valid
            //if we add new ads type the control will continue to work
            //also not updated
            return false;
        }

        private void AddEventHandlersToAdControl()
        {
            //Event Handler registration handled by individual components
        }

        private void RemoveEventHandlersFromAdControl()
        {
            //SJ Review for XNA
#if(!XBOX360)

            var pubCenterAd = _currentAdControl as AdGameComponent;
            if (pubCenterAd != null)
            {
                bannerAd.ErrorOccurred -= new EventHandler<Microsoft.Advertising.AdErrorEventArgs>(pubCenterAd_ErrorOccurred);
                bannerAd.AdRefreshed -= new EventHandler(pubCenterAd_AdRefreshed);
                AdGameComponent.Current.Enabled = false;
#if(!NONLOCATION)
                this.gcw.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gcw_PositionChanged);
                this.gcw.StatusChanged -= new EventHandler<GeoPositionStatusChangedEventArgs>(gcw_StatusChanged);
                this.gcw.Stop();
#endif
            }
            var adDuplexAd = _currentAdControl as AdDuplexComponent;
            if (adDuplexAd != null)
            {
                AdDuplexComponent.Current.AdLoaded -= Current_AdDuplex_AdLoaded;
                AdDuplexComponent.Current.AdLoadingFailed -= Current_AdDuplex_AdLoadingFailed;
                AdDuplexComponent.Current.AdClicked -= Current_AdDuplex_AdClicked;
            }
            var innerativeAd = _currentAdControl as AdInneractiveComponent;
            if (innerativeAd != null)
            {
                AdInneractiveComponent.Current.AdLoaded -= Current_Inneractive_AdLoaded;
                AdInneractiveComponent.Current.AdLoadingFailed -= Current_Inneractive_AdLoadingFailed;
                AdInneractiveComponent.Current.AdClicked -= Current_Inneractive_AdClicked;
            }
            var MobFoxAd = _currentAdControl as AdMobFoxComponent;
            if (MobFoxAd != null)
            {
                AdMobFoxComponent.Current.AdLoaded -= Current_MobFox_AdLoaded;
                AdMobFoxComponent.Current.AdLoadingFailed -= Current_MobFox_AdLoadingFailed;
                AdMobFoxComponent.Current.AdClicked -= Current_MobFox_AdClicked;
            }
            var DefaultHouseAd = _currentAdControl as AdRotatorDefaultHouseAd;
            if (DefaultHouseAd != null)
            {
                AdRotatorDefaultHouseAd.Current.AdClicked -= Current_DefaultHouseAd_AdClicked;
            }
#if(!NONLOCATION)
            var SmaatoAd = _currentAdControl as AdSmaatoComponent;
            if (SmaatoAd != null)
            {
                AdSmaatoComponent.Current.AdLoaded -= Current_Smaato_AdLoaded;
                AdSmaatoComponent.Current.AdLoadingFailed -= Current_Smaato_AdLoadingFailed;
                AdSmaatoComponent.Current.AdClicked -= Current_Smaato_AdClicked;
            }
#endif
#endif
        }

        private AdCultureDescriptor GetAdDescriptorBasedOnUICulture()
        {
            if (_settings == null || _settings.CultureDescriptors == null)
            {
                return null;
            }
            var cultureLongName = Thread.CurrentThread.CurrentUICulture.Name;
            if (String.IsNullOrEmpty(cultureLongName))
            {
                cultureLongName = AdSettings.DEFAULT_CULTURE;
            }
            var cultureShortName = cultureLongName.Substring(0, 2);
            var descriptor = _settings.CultureDescriptors.Where(x => x.CultureName == cultureLongName).FirstOrDefault();
            if (descriptor != null)
            {
                return descriptor;
            }
            var sameLanguageDescriptor = _settings.CultureDescriptors.Where(x => x.CultureName.StartsWith(cultureShortName)).FirstOrDefault();
            if (sameLanguageDescriptor != null)
            {
                return sameLanguageDescriptor;
            }
            var defaultDescriptor = _settings.CultureDescriptors.Where(x => x.CultureName == AdSettings.DEFAULT_CULTURE).FirstOrDefault();
            if (defaultDescriptor != null)
            {
                return defaultDescriptor;
            }
            return null;
        }

        private void RemoveAdFromFailedAds(AdType adType)
        {
            if (_failedAdTypes.Contains(adType))
            {
                _failedAdTypes.Remove(adType);
            }
        }

        /// <summary>
        /// Called when <paramref name="adType"/> has failed to load
        /// </summary>
        /// <param name="adType"></param>
        private void OnAdLoadFailed(AdType adType)
        {
            OnLog(string.Format("Ads failed request for: {0}", adType.ToString()));
            if (!_failedAdTypes.Contains(adType))
            {
                _failedAdTypes.Add(adType);
            }

            Invalidate(false);
        }

        /// <summary>
        /// Called when <paramref name="adType"/> has succeeded to load
        /// </summary>
        /// <param name="adType"></param>
        private void OnAdLoadSucceeded(AdType adType)
        {
            OnLog(string.Format("Ads being successfully served for: {0}", adType.ToString()));
            if (_failedAdTypes.Contains(adType))
            {
                _failedAdTypes.Remove(adType);
            }
        }

        public void UpdateAdPostition()
        {
            var adDefaultAd = _currentAdControl as AdRotatorDefaultHouseAd;
            if (adDefaultAd != null)
            {
                adDefaultAd.UpdateAdPosition(AdPosition);
                return;
            } 
#if(!XBOX360)

            var pubCenterAd = _currentAdControl as AdGameComponent;
            if (pubCenterAd != null && bannerAd != null)
            {
                //var posX = this.Game.Window.CurrentOrientation == DisplayOrientation.Portrait ? (int)AdPosition.X : (int)AdPosition.X - 240;
                bannerAd.DisplayRectangle = new Rectangle((int)AdPosition.X, (int)AdPosition.Y, AdWidth, AdHeight);
                return;
            }
            //Experimental
            //var pubCenterAd = _currentAdControl as PubCenterComponent;
            //if (pubCenterAd != null)
            //{
            //    //var posX = this.Game.Window.CurrentOrientation == DisplayOrientation.Portrait ? (int)AdPosition.X : (int)AdPosition.X - 240;
            //    pubCenterAd.UpdateAdPosition(AdPosition);
            //    return;
            //}
            var adDuplexAd = _currentAdControl as AdDuplexComponent;
            if (adDuplexAd != null)
            {
                //var posX = this.Game.Window.CurrentOrientation == DisplayOrientation.Portrait ? (int)AdPosition.X : (int)AdPosition.X - 240;
                adDuplexAd.UpdateAdPosition(AdPosition);
                return;
            }
            var innerativeAd = _currentAdControl as AdInneractiveComponent;
            if (innerativeAd != null)
            {
                innerativeAd.UpdateAdPosition(AdPosition);
                return;
            }
            var MobFoxAd = _currentAdControl as AdMobFoxComponent;
            if (MobFoxAd != null)
            {
                MobFoxAd.UpdateAdPosition(AdPosition);
                return;
            }
#if(!NONLOCATION)
            var SmaatoAd = _currentAdControl as AdSmaatoComponent;
            if (SmaatoAd != null)
            {
                SmaatoAd.UpdateAdPosition(AdPosition);
                return;
            }
#endif
#endif
        }

        private string GetAppID(AdType adType)
        {
            return CurrentCulture.AdProbabilities
                    .Where(x => x.AdType == adType)
                    .First().AppID;
        }

        private string GetSecondaryID(AdType adType)
        {
            return CurrentCulture.AdProbabilities
                    .Where(x => x.AdType == adType)
                    .First().SecondaryID;
        }

        /// <summary>
        /// Loads the ad settings object either from isolated storage or from the resource path defined in DefaultSettingsFileUri.
        /// </summary>
        /// <returns></returns>
        private void LoadAdSettings()
        {
            //If not checked remote && network available - get remote
            if (!_remoteAdSettingsFetched && !String.IsNullOrEmpty(SettingsUrl) && _isNetworkAvailable)
            {
                FetchAdSettingsThreaded();
            }
            else
            {
                try
                {
                    var isfData = IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream isfStream = null;
                    if (isfData.FileExists(SETTINGS_FILE_NAME))
                    {
                        using (isfStream = new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, isfData))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(AdSettings));
                            try
                            {
                                var settings = (AdSettings)xs.Deserialize(isfStream);
                                //return settings;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                }
            }

            if (_settings == null)
            {
                _settings = GetDefaultSettings();
                OnLog("Failed Setings retrieved from local trying defaults");
            }
            if (_settings == null)
            {
                OnLog("Ad control disabled no settings available");
                IsEnabled = false;
            }
            else
            {
                //Everything OK, continue loading
                _initialised = true;
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Invalidate);
            }
            //return _settings;
        }

        private AdSettings GetDefaultSettings()
        {
            if (DefaultSettingsFileUri != null)
            {
                var xs = new XmlSerializer(typeof(AdSettings));
                try
                {
                    var defaultSettingsFileInfo = TitleContainer.OpenStream(DefaultSettingsFileUri);
                    var settings = (AdSettings)xs.Deserialize(defaultSettingsFileInfo);
                    return settings;
                }
                catch (Exception e)
                {
                    var error = e;
                    OnLog("Loading Defaults failed, control disabled");
                }
            }
            return new AdSettings();

        }

        /// <summary>
        /// Fetches the ad settings file from the address specified at <see cref=""/>
        /// </summary>
        public void FetchAdSettingsFile(object state)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(SettingsUrl));
            request.BeginGetResponse(r =>
            {
                try
                {
                    var httpRequest = (HttpWebRequest)r.AsyncState;
                    var httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                    var settingsStream = httpResponse.GetResponseStream();

                    var s = new System.Xml.Serialization.XmlSerializer(typeof(AdSettings));
                    _settings = (AdSettings)s.Deserialize(settingsStream);
                    // Only persist the settings if they've been retreived from the remote file
                    SaveAdSettings(_settings);
                    _remoteAdSettingsFetched = true;
                    _initialised = true; OnLog("Setings retrieved from remote");
                }
                catch
                {
                    _remoteAdSettingsFetched = true;
                    _initialised = true;
                    LoadAdSettings(); // GetDefaultSettings();
                }
            }, request);
        }

        /// <summary>
        /// Fetches the ad settings file from the address specified at using a seperate thread <see cref=""/>
        /// </summary>
        private void FetchAdSettingsThreaded()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(FetchAdSettingsFile));
            //Thread AdSettingsLoad = new Thread(FetchAdSettingsFile);
            //AdSettingsLoad.Start();
        }

        /// <summary>
        /// Saves the passed settings file to isolated storage
        /// </summary>
        /// <param name="settings"></param>
        private static void SaveAdSettings(AdSettings settings)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(AdSettings));
                using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication()))
                {
                    xs.Serialize(isfStream, settings);
                }
            }
            catch
            {
            }
        }

        #region Specific ad controls

        #region DefaultHouseAd
        private GameComponent CreateDefaultHouseAdControl()
        {
            try
            {
                // Initialize the AdGameComponent with your ApplicationId and add it to the game. 
                AdRotatorDefaultHouseAd.Initialize(this.Game, DefaultHouseAdImage, DefaultHouseAdRemoteURL);
                AdRotatorDefaultHouseAd.Current.AdClicked += new AdRotatorDefaultHouseAd.OnAdClicked(Current_DefaultHouseAd_AdClicked);
                AdRotatorDefaultHouseAd.Current.AdLoaded += new AdRotatorDefaultHouseAd.OnAdLoaded(Current_AdLoaded);
                AdRotatorDefaultHouseAd.Current.AdLoadingFailed += new AdRotatorDefaultHouseAd.OnAdFailed(Current_AdLoadingFailed);
                return AdRotatorDefaultHouseAd.Current;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Current_AdLoadingFailed(object sender, EventArgs e)
        {
            //Ad Failed to Download from remote URL
            
        }

        void Current_AdLoaded(object sender, EventArgs e)
        {
            //Ad Successfully loaded from remote URL
        }

        void Current_DefaultHouseAd_AdClicked(object sender, EventArgs e)
        {
            if (DefaultHouseAdClick != null)
            {
                DefaultHouseAdClick();
            }
        }
        #endregion
#if(!XBOX360)

        #region PubCenter

        private GameComponent CreatePubCentertAdControl()
        {
            try
            {
                // Initialize the AdGameComponent with your ApplicationId and add it to the game. 
                if (!AdGameComponent.Initialized) AdGameComponent.Initialize(this.Game, PubCenterAppId);

                CreateAd();
                return AdGameComponent.Current;
            }
            catch (Exception e)
            {
                if (e != null)
                {
                    OnLog(string.Format("PubCenter Failed: {0}", e.Message.ToString()));
                }
                OnAdLoadFailed(AdType.PubCenter);
                return null;
            }
        }

        /// <summary> 
        /// Create a DrawableAd with desired properties. 
        /// </summary> 
        private void CreateAd()
        {
            bannerAd = AdGameComponent.Current.CreateAd(PubCenterAdUnitId, new Rectangle((int)AdPosition.X, (int)AdPosition.Y, AdWidth, AdHeight), true);

            // Add handlers for events (optional). 
            bannerAd.ErrorOccurred += new EventHandler<Microsoft.Advertising.AdErrorEventArgs>(pubCenterAd_ErrorOccurred);
            bannerAd.AdRefreshed += new EventHandler(pubCenterAd_AdRefreshed);

            // Set some visual properties (optional). 
            //bannerAd.BorderEnabled = true; // default is true 
            //bannerAd.BorderColor = Color.White; // default is White 
            //bannerAd.DropShadowEnabled = true; // default is true 

            // Provide the location to the ad for better targeting (optional). 
            // This is done by starting a GeoCoordinateWatcher and waiting for the location to be available. 
            // The callback will set the location into the ad.  
            // Note: The location may not be available in time for the first ad request. 
#if(!NONLOCATION)
                AdGameComponent.Current.Enabled = false;
                this.gcw = new GeoCoordinateWatcher();
                this.gcw.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gcw_PositionChanged);
                this.gcw.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(gcw_StatusChanged);
                this.gcw.Start();               
#else
                AdGameComponent.Current.Enabled = true;
#endif

        }

        /// <summary> 
        /// This is called whenever a new ad is received by the ad client. 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void pubCenterAd_AdRefreshed(object sender, EventArgs e)
        {
            OnLog(string.Format("pubCenter Success: {0}", e.ToString()));
            OnAdLoadSucceeded(AdType.PubCenter);
        }

        /// <summary> 
        /// This is called when an error occurs during the retrieval of an ad. 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e">Contains the Error that occurred.</param> 
        private void pubCenterAd_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            if (e != null)
            {
                OnLog(string.Format("PubCenter Failed: {0}", e.Error.Message.ToString()));
            }
            OnAdLoadFailed(AdType.PubCenter);
        }
#if(!NONLOCATION)
        private void gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Stop the GeoCoordinateWatcher now that we have the device location. 
            this.gcw.Stop();

            bannerAd.LocationLatitude = e.Position.Location.Latitude;
            bannerAd.LocationLongitude = e.Position.Location.Longitude;

            AdGameComponent.Current.Enabled = true;

            Debug.WriteLine("Device lat/long: " + e.Position.Location.Latitude + ", " + e.Position.Location.Longitude);
        }

        private void gcw_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Disabled || e.Status == GeoPositionStatus.NoData)
            {
                // in the case that location services are not enabled or there is no data 
                // enable ads anyway 
                AdGameComponent.Current.Enabled = true;
                Debug.WriteLine("GeoCoordinateWatcher Status :" + e.Status);
            }
        }
#endif
        #endregion

        #region AdDuplex
        private GameComponent CreateAdDuplexControl()
        {
            try
            {
                AdDuplexComponent.Initialize(this.Game, AdDuplexAppId);
                AdDuplexComponent.Current.AdLoaded += Current_AdDuplex_AdLoaded;
                AdDuplexComponent.Current.AdLoadingFailed += Current_AdDuplex_AdLoadingFailed;
                AdDuplexComponent.Current.AdClicked += Current_AdDuplex_AdClicked;

                AdDuplexComponent.Current.IsTest = IsTest;

                return AdDuplexComponent.Current;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Current_AdDuplex_AdClicked(object sender, AdDuplex.AdClickEventArgs e)
        {
            //Need to Investigate CLick Link operation
            //MarketplaceDetailTask MD = new MarketplaceDetailTask();
            //MD.ContentIdentifier = e.Ad.MarketplaceAppId;
            //MD.Show();
            OnLog(string.Format("AdDuplex Ad Clicked: {0}", e.Ad.MarketplaceAppId));
        }

        void Current_AdDuplex_AdLoadingFailed(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            OnLog(string.Format("AdDuplex Failed: {0}", e.Error.Message.ToString()));
            OnAdLoadFailed(AdType.AdDuplex);
        }

        void Current_AdDuplex_AdLoaded(object sender, AdDuplex.AdLoadedEventArgs e)
        {
            OnLog(string.Format("AdDuplex Success: {0}", e.ToString()));
            OnAdLoadSucceeded(AdType.AdDuplex);
        }

        #endregion

        #region InnerActive
        private GameComponent CreateInneractiveControl()
        {
            try
            {
                AdInneractiveComponent.Initialize(this.Game, InneractiveAppId);
                AdInneractiveComponent.Current.AdLoaded += Current_Inneractive_AdLoaded;
                AdInneractiveComponent.Current.AdLoadingFailed += Current_Inneractive_AdLoadingFailed;
                AdInneractiveComponent.Current.AdClicked += Current_Inneractive_AdClicked;

                return AdInneractiveComponent.Current;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Current_Inneractive_AdClicked(object sender, EventArgs e)
        {
            OnLog(string.Format("InnerActive Ad Clicked: {0}", e.ToString()));
        }

        void Current_Inneractive_AdLoadingFailed(object sender, EventArgs e)
        {
            OnLog(string.Format("InnerActive Failed: {0}", e.ToString()));
            OnAdLoadFailed(AdType.InnerActive);
        }

        void Current_Inneractive_AdLoaded(object sender, EventArgs e)
        {
            OnLog(string.Format("InnerActive Success: {0}", e.ToString()));
            OnAdLoadSucceeded(AdType.InnerActive);
        }

        #endregion

        #region MobFox
        private GameComponent CreateMobFoxControl()
        {
            try
            {
                AdMobFoxComponent.Initialize(this.Game, MobFoxAppId);
                AdMobFoxComponent.Current.isTest = MobFoxIsTest;
                AdMobFoxComponent.Current.AdLoaded += Current_MobFox_AdLoaded;
                AdMobFoxComponent.Current.AdLoadingFailed += Current_MobFox_AdLoadingFailed;
                AdMobFoxComponent.Current.AdClicked += Current_MobFox_AdClicked;

                return AdMobFoxComponent.Current;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Current_MobFox_AdClicked(object sender, EventArgs e)
        {
            OnLog(string.Format("MobFox Ad Clicked: {0}", e.ToString()));
        }

        void Current_MobFox_AdLoadingFailed(object sender, EventArgs e)
        {
            OnLog(string.Format("MobFox Failed: {0}", e.ToString()));
            OnAdLoadFailed(AdType.MobFox);
        }

        void Current_MobFox_AdLoaded(object sender, EventArgs e)
        {
            OnLog(string.Format("MobFox Success: {0}", e.ToString()));
            OnAdLoadSucceeded(AdType.MobFox);
        }

        #endregion
#if(!NONLOCATION)
        #region Smaato
        private GameComponent CreateSmaatoControl()
        {
            try
            {
                AdSmaatoComponent.Initialize(this.Game, SmaatoAppId,SmaatoPublisherId);
                AdSmaatoComponent.Current.AdLoaded += Current_Smaato_AdLoaded;
                AdSmaatoComponent.Current.AdLoadingFailed += Current_Smaato_AdLoadingFailed;
                AdSmaatoComponent.Current.AdClicked += Current_Smaato_AdClicked;

                return AdSmaatoComponent.Current;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void Current_Smaato_AdClicked(object sender, EventArgs e)
        {
            OnLog(string.Format("Smaato Ad Clicked: {0}", e.ToString()));
        }

        void Current_Smaato_AdLoadingFailed(object sender, string ErrorCode, string ErrorDescription)
        {
            OnLog(string.Format("Smaato Failed: {0} - {1}", ErrorCode.ToString(), ErrorDescription.ToString()));
            OnAdLoadFailed(AdType.Smaato);
        }

        void Current_Smaato_AdLoaded(object sender, EventArgs e)
        {
            OnLog(string.Format("Smaato Success: {0}", e.ToString()));
            OnAdLoadSucceeded(AdType.Smaato);
        }

        #endregion
#endif
#endif
        #endregion

        /// <summary> 
        /// Clean up the GeoCoordinateWatcher 
        /// </summary> 
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
#if(!XBOX360)
#if(!NONLOCATION)

                if (this.gcw != null)
                {
                    this.gcw.Dispose();
                    this.gcw = null;
                }
#endif
#endif
            }
        }

    }
}

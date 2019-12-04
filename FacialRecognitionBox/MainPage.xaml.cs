using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FacialRecognitionBox.FacialRecognition;
using FacialRecognitionBox.Helpers;
using Microsoft.ProjectOxford.Face;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FacialRecognitionBox
{
    public sealed partial class MainPage : Page
    {
        // Webcam Related Variables:
        private WebcamHelper webcam;

        // Oxford Related Variables:
        private bool initializedOxford = false;

        // Speech Related Variables:
        private SpeechHelper speech;

        // GPIO Related Variables:
        private GpioHelper gpioHelper;
        private bool gpioAvailable;
        private bool doorBellJustPressed = false;

        /// <summary>
        /// Called when the page is first navigated to.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Causes this page to save its state when navigating to other pages
            NavigationCacheMode = NavigationCacheMode.Enabled;

            if (initializedOxford == false)
            {
                // If Oxford facial recognition has not been initialized, attempt to initialize it
                InitializeOxford();
            }

            if(gpioAvailable == false)
            {
                // If GPIO is not available, attempt to initialize it
                InitializeGpioAsync();
            }

            // If user has set the DisableLiveCameraFeed within Constants.cs to true, disable the feed:
            if(GeneralConstants.DisableLiveCameraFeed)
            {
                LiveFeedPanel.Visibility = Visibility.Collapsed;
                DisabledFeedGrid.Visibility = Visibility.Visible;
            }
            else
            {
                LiveFeedPanel.Visibility = Visibility.Visible;
                DisabledFeedGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called once, when the app is first opened. Initializes Oxford facial recognition.
        /// </summary>
        public async void InitializeOxford()
        {
            // initializedOxford bool will be set to true when Oxford has finished initialization successfully
            initializedOxford = await OxfordFaceAPIHelper.InitializeOxford();
        }

        /// <summary>
        /// Called once, when the app is first opened. Initializes device GPIO.
        /// </summary>
        public async Task InitializeGpioAsync()
        {
            try
            {
                // Attempts to initialize application GPIO. 
                gpioHelper = new GpioHelper();
                gpioAvailable = gpioHelper.Initialize();
            }
            catch
            {
                // This can fail if application is run on a device, such as a laptop, that does not have a GPIO controller
                gpioAvailable = false;
            }

            // If initialization was successfull, attach doorbell pressed event handler
            if (gpioAvailable)
            {
                gpioHelper.GetDoorBellPin().ValueChanged += DoorBellPressed;
            }
        }

        /// <summary>
        /// Triggered when webcam feed loads both for the first time and every time page is navigated to.
        /// If no WebcamHelper has been created, it creates one. Otherwise, simply restarts webcam preview feed on page.
        /// </summary>
        private async void WebcamFeed_Loaded(object sender, RoutedEventArgs e)
        {
            if (webcam == null || !webcam.IsInitialized())
            {
                // Initialize Webcam Helper
                webcam = new WebcamHelper();
                await webcam.InitializeCameraAsync();

                // Set source of WebcamFeed on MainPage.xaml
                WebcamFeed.Source = webcam.mediaCapture;
            }
            else if(webcam.IsInitialized())
            {
                WebcamFeed.Source = webcam.mediaCapture;
            }
        }

        /// <summary>
        /// Triggered when media element used to play synthesized speech messages is loaded.
        /// Initializes SpeechHelper and greets user.
        /// </summary>
        private async void SpeechMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (speech == null)
            {
                speech = new SpeechHelper(speechMediaElement);

                await speech.Read(SpeechContants.InitialGreetingMessage, 3.0);
            }
            else
            {
                // Prevents media element from re-greeting visitor
                speechMediaElement.AutoPlay = false;
            }

            await Task.Delay(TimeSpan.FromSeconds(3.0));
            await speech.Read(SpeechContants.ProjectReadyMessage, 2.0);
        }

        /// <summary>
        /// Triggered when user presses physical door bell button
        /// </summary>
        private async void DoorBellPressed(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (!doorBellJustPressed)
            {
                // Checks to see if even was triggered from a press or release of button
                if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    //Doorbell was just pressed
                    doorBellJustPressed = true;

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await DoorbellPressed();
                    });

                }
            }
        }

        /// <summary>
        /// Triggered when user presses virtual doorbell app bar button
        /// </summary>
        private async void DoorbellButton_Click(object sender, RoutedEventArgs e)
        {
            if (!doorBellJustPressed)
            {
                doorBellJustPressed = true;
                await DoorbellPressed();
            }
        }

        /// <summary>
        /// Called when user hits physical or vitual doorbell buttons. Captures photo of current webcam view and sends it to Oxford for facial recognition processing.
        /// </summary>
        private async Task DoorbellPressed()
        {
            // Display analysing visitors grid to inform user that doorbell press was registered
            AnalysingVisitorGrid.Visibility = Visibility.Visible;

            // List to store visitors recognized by Oxford Face API
            // Count will be greater than 0 if there is an authorized visitor at the door
            List<string> recognizedVisitors = new List<string>();

            // Confirms that webcam has been properly initialized and oxford is ready to go
            if (webcam.IsInitialized() && initializedOxford)
            {
                // Stores current frame from webcam feed in a temporary folder
                StorageFile image = await webcam.CapturePhoto();

                try
                {
                    // Oxford determines whether or not the visitor is on the Whitelist and returns true if so
                    recognizedVisitors = await OxfordFaceAPIHelper.IsFaceInWhitelist(image);
                }
                catch (FaceRecognitionException fe)
                {
                    switch (fe.ExceptionType)
                    {
                        // Fails and catches as a FaceRecognitionException if no face is detected in the image
                        case FaceRecognitionExceptionType.NoFaceDetected:
                            break;
                    }
                }
                catch
                {
                }
                
                if(recognizedVisitors.Count > 0)
                {
                    // If everything went well and a visitor was recognized, unlock the door:
                    UnlockDoor(recognizedVisitors[0]);
                }
                else
                {
                    // Otherwise, inform user that they were not recognized by the system
                    await speech.Read(SpeechContants.VisitorNotRecognizedMessage, 2.0);
                }
            }
            else
            {
                if (!webcam.IsInitialized())
                {
                    // The webcam has not been fully initialized for whatever reason:
                    await speech.Read(SpeechContants.NoCameraMessage, 3.0);
                }

                if(!initializedOxford)
                {
                }
            }

            doorBellJustPressed = false;
            AnalysingVisitorGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Unlocks door and greets visitor
        /// </summary>
        private async void UnlockDoor(string visitorName)
        {
            // Greet visitor
            await speech.Read(SpeechContants.VisitorWelcomeMessage(visitorName), 1.5);

            if (gpioAvailable)
            {
                // Unlock door for specified ammount of time
                gpioHelper.UnlockDoor();
            }
        }

        /// <summary>
        /// Triggered when the user selects the Shutdown button in the app bar. Closes app.
        /// </summary>
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            // Exit app
            Application.Current.Exit();
        }
    }
}

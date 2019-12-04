namespace FacialRecognitionDoor
{
    /// <summary>
    /// General constant variables
    /// </summary>
    public static class GeneralConstants
    {
        // This variable should be set to false for devices that have GPU support. Set it to true for Raspberry Pi.
        public const bool DisableLiveCameraFeed = false;

        // Azure Face API Primary Key should be entered here
        public const string FaceResourceKey = "xxxxxxxxxxxxxxx";

        // Enter the API endpoint address.
        public const string FaceResourceName = "xxxxxxxxxxxx"; 
        public const string FaceResourceEndpoint = "https://" + FaceResourceName + ".cognitiveservices.azure.com/face/v1.0";

        // Name of the folder in which all Whitelist data is stored
        // TODO: Folder name without blank spaces for Raspberry Pi and Windows IoT
        public const string WhiteListFolderName = "FacialRecognitionDoorWhitelist";

        // Fixed ID for the shared WhiteList
        public const string FixedWhiteListID = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
    }

    /// <summary>
    /// Constant variables that hold messages to be read via the SpeechHelper class
    /// </summary>
    public static class SpeechContants
    {
        public const string InitialGreetingMessage = "Welcome to the Idiwork Facial Recognition project!";

        public const string ProjectReadyMessage = "The Idiwork Facial Recognition door is ready now!";

        public const string VisitorNotRecognizedMessage = "Sorry! I don't recognize you, so I cannot open the door.";

        public const string NoCameraMessage = "Sorry! It seems like your camera has not been initialized.";

        public static string AddVisitorsMessage = "Adding whitelisted visitors.";

        public static string GeneralGreetingMessage(string visitorName)
        {
            return "Welcome " + visitorName + "! I will open the door for you.";
        }
    }

    /// <summary>
    /// Constant variables that hold values used to interact with device GPIO (Rasberry Pi 2 model B).
    /// </summary>
    public static class GpioConstants
    {
        // The GPIO pin that the doorbell button is attached to.
        public const int ButtonPinID = 5;

        // The GPIO pin that the door LED light is attached to.
        public const int DoorLockPinID = 4;

        // The GPIO pin that the door servo motor is attached to.
        public const int ServoMotorPinID = 18;

        // The amount of time in seconds that the door will remain unlocked.
        public const int DoorLockOpenDurationSeconds = 10;
    }
}

namespace FacialRecognitionBox
{
    public static class GeneralConstants
    {
        public const bool DisableLiveCameraFeed = true;

        public const string FaceResourceKey = "xxxxxxxxxxxxxxx";

        public const string FaceResourceName = "xxxxxxxxxxxx";

        public const string FaceResourceEndpoint = "https://" + FaceResourceName + ".cognitiveservices.azure.com/face/v1.0";

        public const string WhiteListFolderName = "FacialRecognitionDoorWhitelist";

        public const string FixedPersonGroupID = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
    }

    public static class SpeechContants
    {
        public const string InitialGreetingMessage = "Idiwork Facial Recognition Experiment.";

        public const string ProjectReadyMessage = "The smart box is ready.";

        public const string VisitorNotRecognizedMessage = "Sorry. I don't recognize you.";

        public const string NoCameraMessage = "Your camera has not been initialized.";

        public static string VisitorWelcomeMessage(string personaName)
        {
            return $"Welcome {personaName}. I will open the door.";
        }
    }

    public static class GpioConstants
    {
        public const int ButtonPinID = 5;

        public const int DoorLockPinID = 4;

        public const int ServoMotorPinID = 18;

        public const int DoorLockOpenDurationSeconds = 10;
    }
}

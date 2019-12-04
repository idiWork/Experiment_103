using FacialRecognitionBox.FacialRecognition;
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace FacialRecognitionBox.Helpers
{
    /// <summary>
    /// Allows easy access to oxford functions such as adding a visitor to whitelist and checing to see if a visitor is on the whitelist
    /// </summary>
    static class OxfordFaceAPIHelper
    {
        /// <summary>
        /// Initializes Oxford API. Builds existing whitelist or creates one if one does not exist.
        /// </summary>
        public async static Task<bool> InitializeOxford()
        {
            // Attempts to open whitelist folder, or creates one
            StorageFolder whitelistFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync(GeneralConstants.WhiteListFolderName, CreationCollisionOption.OpenIfExists);

            // Creates a new instance of the Oxford API Controller
            FaceApiRecognizer sdkController = FaceApiRecognizer.Instance;

            // Attempts to open whitelist ID file, or creates one
            StorageFile WhiteListIdFile = await whitelistFolder.CreateFileAsync("WhiteListId.txt", CreationCollisionOption.OpenIfExists);

            // Reads whitelist file and stores value
            string savedWhitelistId = await FileIO.ReadTextAsync(WhiteListIdFile);

            // If the ID has not been created, creates a whitelist ID
            if (savedWhitelistId == "")
            {
                //string id = Guid.NewGuid().ToString(); // TODO: Try to match this ID with the centralized WhiteListId
                string id = GeneralConstants.FixedPersonGroupID;
                await FileIO.WriteTextAsync(WhiteListIdFile, id);
                savedWhitelistId = id;
            }

            // Return true to indicate that Oxford was initialized successfully
            return true;
        }

        /// <summary>
        /// Checks to see if a whitelisted visitor is in passed through image. Returns list of whitelisted visitors. If no authorized users are detected, returns an empty list.
        /// </summary>
        public async static Task<List<string>> IsFaceInWhitelist(StorageFile image)
        {
            FaceApiRecognizer sdkController = FaceApiRecognizer.Instance;
            List<string> matchedImages = new List<string>();
            matchedImages = await sdkController.FaceRecognizeAsync(image);

            return matchedImages;
        }
    }
}

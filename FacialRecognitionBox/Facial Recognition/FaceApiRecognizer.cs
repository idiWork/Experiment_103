using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using System.Diagnostics;
using Windows.Storage;
using Microsoft.ProjectOxford.Face.Contract;

namespace FacialRecognitionBox.FacialRecognition
{
    class FaceApiRecognizer
    {
        #region Private members

        private static readonly Lazy<FaceApiRecognizer> _recognizer = new Lazy<FaceApiRecognizer>( () => new FaceApiRecognizer());

        private IFaceServiceClient _faceApiClient = null;

        #endregion

        #region Properties

        /// <summary>
        /// Face API Recognizer instance
        /// </summary>
        public static FaceApiRecognizer Instance
        {
            get
            {
                return _recognizer.Value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// Initial Face Api client
        /// </summary>
        private FaceApiRecognizer() {
            _faceApiClient = new FaceServiceClient(GeneralConstants.FaceResourceKey, GeneralConstants.FaceResourceEndpoint);
        }

        #endregion

        #region Face

        /// <summary>
        /// Detect face and return the face id of a image file
        /// </summary>
        /// <param name="imageFile">
        /// image file to detect face
        /// </param>
        /// <returns>face id</returns>
        private async Task<Guid[]> DetectFacesFromImage(StorageFile imageFile)
        {
            var stream = await imageFile.OpenStreamForReadAsync();
            var faces = await _faceApiClient.DetectAsync(stream);
            if (faces == null || faces.Length < 1)
            {
                throw new FaceRecognitionException(FaceRecognitionExceptionType.NoFaceDetected);
            }

            return FaceApiUtils.FacesToFaceIds(faces) ;
        }

        #endregion

        #region Face recognition

        public async Task<List<string>> FaceRecognizeAsync(StorageFile imageFile)
        {
            var recogResult = new List<string>();

            if(!FaceApiUtils.ValidateImageFile(imageFile))
            {
                throw new FaceRecognitionException(FaceRecognitionExceptionType.InvalidImage);
            }

            // detect all faces in the image
            var faceIds = await DetectFacesFromImage(imageFile);

            // try to identify all faces to person
            var identificationResults = await _faceApiClient.IdentifyAsync(GeneralConstants.FixedPersonGroupID, faceIds);

            // add identified person name to result list
            foreach(var result in identificationResults)
            {
                if(result.Candidates.Length > 0)
                {
                    var person = await _faceApiClient.GetPersonAsync(GeneralConstants.FixedPersonGroupID, result.Candidates[0].PersonId);
                    var personName = person.Name;
                    recogResult.Add(personName);
                }
            }

            return recogResult;
        }

        #endregion
    }
}

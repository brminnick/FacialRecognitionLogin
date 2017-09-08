using System;
using Microsoft.ProjectOxford.Face;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
namespace FacialRecognitionLogin
{
    public static class FacialRecognitionHelpers
    {
        #region Constant Fields
        const string _personGroupId = "personGroupId";
        const string _personGroupName = "Person Group Name";
        static readonly Lazy<FaceServiceClient> _faceServiceClientHolder = new Lazy<FaceServiceClient>(() => new FaceServiceClient(AzureConstants.FacialRecognitionAPIKey));
        #endregion

        #region Properties
        static FaceServiceClient FaceServiceClient => _faceServiceClientHolder.Value;
        #endregion

        #region Methods
        public static async Task AddNewFace(string username, Stream photo)
        {
            await FaceServiceClient.CreatePersonGroupAsync(_personGroupId, _personGroupName);

            var personResult = await FaceServiceClient.CreatePersonAsync(_personGroupId, username);

            var temp = await FaceServiceClient.AddPersonFaceAsync(_personGroupId, personResult.PersonId, photo);
        }

        public static async Task<bool> IsFaceIdentified(string username, Stream photo)
        {
			var personListTask = FaceServiceClient.GetPersonsAsync(_personGroupId);

            var faces = await FaceServiceClient.DetectAsync(photo);

            var results = await FaceServiceClient.IdentifyAsync(_personGroupId, faces.Select(x => x.FaceId).ToArray());

            var candidateList = results.SelectMany(x => x.Candidates).ToList();

            await personListTask;

            var submittedPerson = personListTask.Result.Where(x => x.Name.Equals(username)).FirstOrDefault();

            return candidateList.Any(x => x.PersonId.Equals(submittedPerson?.PersonId));
        }
        #endregion
    }
}

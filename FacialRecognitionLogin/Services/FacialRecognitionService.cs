using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public static class FacialRecognitionService
    {
        const string _personGroupId = "persongroupid";
        const string _personGroupName = "Facial Recognition Login Group";
        readonly static Lazy<FaceClient> _faceApiClientHolder = new Lazy<FaceClient>(() =>
             new FaceClient(new ApiKeyServiceClientCredentials(AzureConstants.FacialRecognitionAPIKey), new HttpClient(), false) { Endpoint = AzureConstants.FaceApiBaseUrl });

        static int _networkIndicatorCount;

        static FaceClient FaceApiClient => _faceApiClientHolder.Value;

        public static async Task RemoveExistingFace(Guid userId)
        {
            await UpdateActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                await FaceApiClient.PersonGroupPerson.DeleteAsync(_personGroupId, userId).ConfigureAwait(false);
            }
            catch (APIErrorException e) when (e.Response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                Debug.WriteLine("Person Does Not Exist");
                DebugService.PrintException(e);
            }
            finally
            {
                await UpdateActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<Guid> AddNewFace(string username, Stream photo)
        {
            await UpdateActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                await CreatePersonGroup().ConfigureAwait(false);

                var createPersonResult = await FaceApiClient.PersonGroupPerson.CreateAsync(_personGroupId, username).ConfigureAwait(false);

                var faceResult = await FaceApiClient.PersonGroupPerson.AddFaceFromStreamAsync(_personGroupId, createPersonResult.PersonId, photo).ConfigureAwait(false);

                var trainingStatus = await TrainPersonGroup(_personGroupId).ConfigureAwait(false);
                if (trainingStatus.Status is TrainingStatusType.Failed)
                    throw new Exception(trainingStatus.Message);

                return faceResult.PersistedFaceId;
            }
            finally
            {
                await UpdateActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<bool> IsFaceIdentified(string username, Stream photo)
        {
            await UpdateActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                var personGroupListTask = FaceApiClient.PersonGroupPerson.ListAsync(_personGroupId);

                var facesDetected = await FaceApiClient.Face.DetectWithStreamAsync(photo).ConfigureAwait(false);
                var faceDetectedIds = facesDetected.Select(x => x.FaceId ?? new Guid()).ToArray();

                var facesIdentified = await FaceApiClient.Face.IdentifyAsync(faceDetectedIds, _personGroupId).ConfigureAwait(false);

                var candidateList = facesIdentified.SelectMany(x => x.Candidates).ToList();

                var personGroupList = await personGroupListTask.ConfigureAwait(false);

                var matchingUsernamePersonList = personGroupList.Where(x => x.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase));

                return candidateList.Select(x => x.PersonId).Intersect(matchingUsernamePersonList.Select(y => y.PersonId)).Any();
            }
            catch
            {
                return false;
            }
            finally
            {
                await UpdateActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        static async Task UpdateActivityIndicatorStatus(bool isActivityIndicatorDisplayed)
        {
            if (isActivityIndicatorDisplayed)
            {
                await MainThread.InvokeOnMainThreadAsync(() => GetBaseViewModel().IsInternetConnectionActive = Application.Current.MainPage.IsBusy = true);
                _networkIndicatorCount++;
            }
            else if (--_networkIndicatorCount <= 0)
            {
                await MainThread.InvokeOnMainThreadAsync(() => GetBaseViewModel().IsInternetConnectionActive = Application.Current.MainPage.IsBusy = false);
                _networkIndicatorCount = 0;
            }
        }

        static async Task CreatePersonGroup()
        {
            try
            {
                await FaceApiClient.PersonGroup.CreateAsync(_personGroupId, _personGroupName).ConfigureAwait(false);
            }
            catch (APIErrorException e) when (e.Response.StatusCode is HttpStatusCode.Conflict)
            {
                Debug.WriteLine("Person Group Already Exists");
                DebugService.PrintException(e);
            }
        }

        static async Task<TrainingStatus> TrainPersonGroup(string personGroupId)
        {
            TrainingStatus trainingStatus;

            await FaceApiClient.PersonGroup.TrainAsync(personGroupId).ConfigureAwait(false);

            do
            {
                trainingStatus = await FaceApiClient.PersonGroup.GetTrainingStatusAsync(_personGroupId).ConfigureAwait(false);
            }
            while (!hasTrainingStatusCompleted(trainingStatus));

            return trainingStatus;

            static bool hasTrainingStatusCompleted(in TrainingStatus trainingStatus) =>
                trainingStatus.Status != TrainingStatusType.Failed && trainingStatus.Status != TrainingStatusType.Succeeded;
        }

        static BaseViewModel GetBaseViewModel()
        {
            var currentPage = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault()
                                ?? Application.Current.MainPage.Navigation.NavigationStack.Last();

            return (BaseViewModel)currentPage.BindingContext;
        }
    }
}

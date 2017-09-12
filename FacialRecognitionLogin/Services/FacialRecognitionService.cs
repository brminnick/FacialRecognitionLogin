using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Face;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public static class FacialRecognitionService
    {
        #region Constant Fields
        const string _personGroupId = "persongroupid";
        const string _personGroupName = "Facial Recognition Login Group";
        static readonly Lazy<FaceServiceClient> _faceServiceClientHolder = new Lazy<FaceServiceClient>(() => new FaceServiceClient(AzureConstants.FacialRecognitionAPIKey));
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Properties
        static FaceServiceClient FaceServiceClient => _faceServiceClientHolder.Value;
        #endregion

        #region Methods
        public static async Task RemoveFace(Guid userId)
        {
            UpdateActivityIndicatorStatus(true);
            try
            {
                await FaceServiceClient.DeletePersonAsync(_personGroupId, userId);
            }
            catch (FaceAPIException e)
            {
                if (!e.HttpStatus.Equals(HttpStatusCode.NotFound))
                    throw e;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        public static async Task<Guid> AddNewFace(string username, Stream photo)
        {
            UpdateActivityIndicatorStatus(true);

            try
            {
                await CreatePersonGroup();

                var personResult = await FaceServiceClient.CreatePersonAsync(_personGroupId, username);

                var faceResult = await FaceServiceClient.AddPersonFaceAsync(_personGroupId, personResult.PersonId, photo);

                await FaceServiceClient.TrainPersonGroupAsync(_personGroupId);

                return faceResult.PersistedFaceId;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        public static async Task<bool> IsFaceIdentified(string username, Stream photo)
        {
            UpdateActivityIndicatorStatus(true);
            try
            {
                var personListTask = FaceServiceClient.GetPersonsAsync(_personGroupId);
                var faces = await FaceServiceClient.DetectAsync(photo);
                var results = await FaceServiceClient.IdentifyAsync(_personGroupId, faces.Select(x => x.FaceId).ToArray());

                var candidateList = results.SelectMany(x => x.Candidates).ToList();

                await personListTask;

                var matchingUsernamePersonList = personListTask.Result.Where(x => x.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase));

                //foreach (var candiate in candidateList)
                //{
                //    var isMatchFound = matchingUsernamePersonList.Select(x => x.PersonId).Contains(candiate.PersonId);

                //    if (isMatchFound)
                //        return true;
                //}

                return candidateList.Select(x => x.PersonId).Intersect(matchingUsernamePersonList.Select(y => y.PersonId)).Any();

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        static void UpdateActivityIndicatorStatus(bool isActivityIndicatorDisplayed)
        {
            var baseNavigationPage = Application.Current.MainPage as NavigationPage;
            var currentPage = baseNavigationPage.CurrentPage as ContentPage;
            var currentViewModel = currentPage.BindingContext as BaseViewModel;

            if (isActivityIndicatorDisplayed)
            {
                currentViewModel.IsInternetConnectionActive = true;
                _networkIndicatorCount++;
            }
            else
            {
                if (--_networkIndicatorCount == 0)
                    currentViewModel.IsInternetConnectionActive = false;
            }
        }

        static async Task CreatePersonGroup()
        {
            try
            {
                await FaceServiceClient.CreatePersonGroupAsync(_personGroupId, _personGroupName);
            }
            catch (FaceAPIException e)
            {
                if (!e.HttpStatus.Equals(System.Net.HttpStatusCode.Conflict))
                    throw e;
            }
        }
        #endregion
    }
}

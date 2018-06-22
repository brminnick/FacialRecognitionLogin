using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public static class FacialRecognitionService
	{
		#region Constant Fields
		const string _personGroupId = "persongroupid";
		const string _personGroupName = "Facial Recognition Login Group";
		static readonly Lazy<FaceAPI> _faceApiClientHolder = new Lazy<FaceAPI>(() =>
			new FaceAPI(new ApiKeyServiceClientCredentials(AzureConstants.FacialRecognitionAPIKey)) { AzureRegion = AzureRegions.Westus });
		#endregion

		#region Fields
		static int _networkIndicatorCount = 0;
		#endregion

		#region Properties
		static FaceAPI FaceApiClient => _faceApiClientHolder.Value;
		#endregion

		#region Methods
		public static async Task RemoveExistingFace(Guid userId)
		{
			UpdateActivityIndicatorStatus(true);
			try
			{
				await FaceApiClient.PersonGroupPerson.DeleteAsync(_personGroupId, userId).ConfigureAwait(false);
			}
			catch (APIErrorException e) when (e.Response.StatusCode.Equals(HttpStatusCode.NotFound))
			{
				Debug.WriteLine("Person Does Not Exist");
                DebugHelpers.PrintException(e);
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
				await CreatePersonGroup().ConfigureAwait(false);

				var createPersonResult = await FaceApiClient.PersonGroupPerson.CreateAsync(_personGroupId, username).ConfigureAwait(false);

				var faceResult = await FaceApiClient.PersonGroupPerson.AddPersonFaceFromStreamAsync(_personGroupId, createPersonResult.PersonId, photo).ConfigureAwait(false);

				var trainingStatus = await TrainPersonGroup(_personGroupId).ConfigureAwait(false);
				if (trainingStatus.Status.Equals(TrainingStatusType.Failed))
					throw new Exception(trainingStatus.Message);

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
				var personGroupListTask = FaceApiClient.PersonGroupPerson.ListAsync(_personGroupId);

				var facesDetected = await FaceApiClient.Face.DetectWithStreamAsync(photo).ConfigureAwait(false);
				var faceDetectedIds = facesDetected.Select(x => x.FaceId ?? new Guid()).ToArray();

				var facesIdentified = await FaceApiClient.Face.IdentifyAsync(_personGroupId, faceDetectedIds).ConfigureAwait(false);

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
				UpdateActivityIndicatorStatus(false);
			}
		}

		static void UpdateActivityIndicatorStatus(bool isActivityIndicatorDisplayed)
		{
			var viewModel = GetCurrentViewModel();

			if (isActivityIndicatorDisplayed)
			{
				viewModel.IsInternetConnectionActive = Application.Current.MainPage.IsBusy = true;
				_networkIndicatorCount++;
			}
			else if (--_networkIndicatorCount <= 0)
			{
				viewModel.IsInternetConnectionActive = Application.Current.MainPage.IsBusy = false;
				_networkIndicatorCount = 0;
			}
		}

		static async Task CreatePersonGroup()
		{
			try
			{
				await FaceApiClient.PersonGroup.CreateAsync(_personGroupId, _personGroupName).ConfigureAwait(false);
			}
			catch (APIErrorException e) when (e.Response.StatusCode.Equals(HttpStatusCode.Conflict))
			{
				Debug.WriteLine("Person Group Already Exists");
				DebugHelpers.PrintException(e);
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
			while (!(trainingStatus.Status == TrainingStatusType.Failed || trainingStatus.Status == TrainingStatusType.Succeeded));

			return trainingStatus;
		}

		static BaseViewModel GetCurrentViewModel()
		{
			Page currentPage;
			if (Application.Current.MainPage.Navigation.ModalStack.Any())
				currentPage = Application.Current.MainPage.Navigation.ModalStack.LastOrDefault();
			else
				currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();

			return currentPage.BindingContext as BaseViewModel;
		}
		#endregion
	}
}

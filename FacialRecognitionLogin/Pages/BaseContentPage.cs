using Xamarin.Forms;
using System;

namespace FacialRecognitionLogin
{
	public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		#region Constructors
		protected BaseContentPage() => BindingContext = ViewModel;
		#endregion

		#region Properties
		protected T ViewModel { get; } = new T();
		#endregion

		#region Methods
		protected abstract void SubscribeEventHandlers();

		protected abstract void UnsubscribeEventHandlers();

		protected override void OnAppearing()
		{
			base.OnAppearing();

			SubscribeEventHandlers();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			UnsubscribeEventHandlers();
		}
		#endregion
	}
}

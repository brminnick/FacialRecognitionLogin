using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AsyncAwaitBestPractices;

namespace FacialRecognitionLogin
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        readonly WeakEventManager _notifyPropertyChangedEventManager = new WeakEventManager();

        bool _isInternetConnectionActive;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _notifyPropertyChangedEventManager.AddEventHandler(value);
            remove => _notifyPropertyChangedEventManager.RemoveEventHandler(value);
        }

        public bool IsInternetConnectionInactive => !IsInternetConnectionActive;

        public bool IsInternetConnectionActive
        {
            get => _isInternetConnectionActive;
            set => SetProperty(ref _isInternetConnectionActive, value, () => OnPropertyChanged(nameof(IsInternetConnectionInactive)));
        }

        protected void SetProperty<T>(ref T backingStore, in T value, in Action? onChanged = null, [CallerMemberName] in string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        void OnPropertyChanged([CallerMemberName] in string propertyName = "") =>
            _notifyPropertyChangedEventManager?.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));
    }
}

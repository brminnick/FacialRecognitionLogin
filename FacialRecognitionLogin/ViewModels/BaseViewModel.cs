using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FacialRecognitionLogin
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Fields
        bool _isInternetConnectionActive;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public bool IsInternetConnectionActive
        {
            get => _isInternetConnectionActive;
            set => SetProperty(ref _isInternetConnectionActive, value);
        }
        #endregion

        #region Methods
        protected void SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

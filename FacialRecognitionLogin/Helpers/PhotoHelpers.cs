using System;
using System.IO;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

namespace FacialRecognitionLogin
{
    public static class PhotoHelpers
    {
        #region Events
        public static event EventHandler NoCameraDetected;
        #endregion

        #region Methods
        public static async Task<Stream> GetPhotoStreamFromCamera()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnNoCameraDetected();
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Small,
                DefaultCamera = CameraDevice.Front,
            });

            return GetPhotoStream(file, false);
        }

        static Stream GetPhotoStream(MediaFile mediaFile, bool disposeMediaFile)
        {
            var stream = mediaFile.GetStream();

            if (disposeMediaFile)
                mediaFile.Dispose();

            return stream;
        }

        static void OnNoCameraDetected() =>
            NoCameraDetected?.Invoke(null, EventArgs.Empty);
        #endregion
    }
}

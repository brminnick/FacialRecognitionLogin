using System;
using System.IO;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using Xamarin.Forms;

namespace FacialRecognitionLogin
{
    public static class PhotoService
    {
        readonly static WeakEventManager _noCameraDetectedEventManager = new WeakEventManager();
        readonly static WeakEventManager _permissionsDeniedEventManager = new WeakEventManager();

        public static event EventHandler NoCameraDetected
        {
            add => _noCameraDetectedEventManager.AddEventHandler(value);
            remove => _noCameraDetectedEventManager.RemoveEventHandler(value);
        }
        public static event EventHandler PermissionsDenied
        {
            add => _permissionsDeniedEventManager.AddEventHandler(value);
            remove => _permissionsDeniedEventManager.RemoveEventHandler(value);
        }

        public static async Task<MediaFile?> GetMediaFileFromCamera()
        {
            await CrossMedia.Current.Initialize().ConfigureAwait(false);

            var arePermissionsGranted = await ArePermissionsGranted().ConfigureAwait(false);
            if (!arePermissionsGranted)
            {
                OnPermissionsDenied();
                return null;
            }

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnNoCameraDetected();
                return null;
            }

            return await Device.InvokeOnMainThreadAsync(() =>
            {
                return CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    DefaultCamera = CameraDevice.Front,
                });
            }).ConfigureAwait(false);
        }

        static async Task<bool> ArePermissionsGranted()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            return cameraStatus is PermissionStatus.Granted
                    && storageStatus is PermissionStatus.Granted;
        }

        static void OnNoCameraDetected() => _noCameraDetectedEventManager.HandleEvent(null, EventArgs.Empty, nameof(NoCameraDetected));
        static void OnPermissionsDenied() => _permissionsDeniedEventManager.HandleEvent(null, EventArgs.Empty, nameof(PermissionsDenied));
    }
}

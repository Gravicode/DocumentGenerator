using DocumentGenerator.Web.Helpers;

namespace DocumentGenerator.Web.Data
{
    public class AppState
    {

        public event Action<string> OnProfileChange;
        public event Action<string> OnDriveChange;
        public event Action<string> OnStorageChange;
        public event Action<string> OnFolderChange;

        public void RefreshProfile(string username)
        {
            ProfileStateChanged(username);
        }

        public void RefreshDrive(string username)
        {
            DriveStateChanged(username);
        }
        public void RefreshStorage(string username)
        {
            StorageStateChanged(username);
        }
        public void RefreshFolder(string UID)
        {
            FolderStateChanged(UID);
        }

        private void FolderStateChanged(string UID) => OnFolderChange?.Invoke(UID);
        private void StorageStateChanged(string username) => OnStorageChange?.Invoke(username);
        private void DriveStateChanged(string username) => OnDriveChange?.Invoke(username);
        private void ProfileStateChanged(string username) => OnProfileChange?.Invoke(username);


    }
}

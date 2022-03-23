using System;

namespace LeanplumSDK
{
    public class FileTransferManager
    {
                public delegate void NoPendingDownloadsHandler();
        public FileTransferManager()
        {
        }

        public static int PendingDownloads { get; private set; }

        private static event NoPendingDownloadsHandler noPendingDownloads;

        public static event NoPendingDownloadsHandler NoPendingDownloads
        {
            add
            {
                noPendingDownloads += value;
                if (PendingDownloads == 0)
                {
                    value();
                }
            }
            remove { noPendingDownloads -= value; }
        }


        public void DownloadFile()
        {

        }

        internal virtual void DownloadAssetNow()
        {
        }

        public static void ClearNoPendingDownloads()
        {
            noPendingDownloads = null;
        }
    }
}

﻿using System;
using System.Linq;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Images;
using JoyReactor.Core.Model.Web;
using JoyReactor.Core.ViewModels.Common;
using Microsoft.Practices.ServiceLocation;
using PCLStorage;

namespace JoyReactor.Core.ViewModels
{
    public class GalleryViewModel : ScopedViewModel
    {
        public string ImagePath { get { return Get<string>(); } set { Set(value); } }

        public int Progress { get { return Get<int>(); } set { Set(value); } }

        public bool IsVideo { get { return CheckIsVideo(); } }

        bool isActivated;

        public async override void OnActivated()
        {
            base.OnActivated();

            if (!isActivated)
            {
                isActivated = true;
                var downloader = new Downloader
                {
                    ImageUrl = GetImageUri(),
                    ProgressCallback = s => Progress = s,
                };
                ImagePath = (await downloader.DownloadAsync()).Path;
                Progress = 100;
            }
        }

        Uri GetImageUri()
        {
            var original = new Uri(GetOriginalImageUrl());
            return CheckIsVideo()
                ? original
                : new BaseImageRequest.ThumbnailUri(original).ToUri();
        }

        bool CheckIsVideo()
        {
            return GetOriginalImageUrl().EndsWith(".mp4");
        }

        string GetOriginalImageUrl()
        {
            return BaseNavigationService.Instance.GetArgument<string>();
        }

        public static bool IsCanShow(string imageUrl)
        {
            return imageUrl != null && (new[] { ".jpeg", ".jpg", ".png", ".mp4", ".gif" }.Any(imageUrl.EndsWith));
        }

        class Downloader
        {
            internal Uri ImageUrl { get; set; }

            internal Action<int> ProgressCallback;

            internal async Task<IFile> DownloadAsync()
            {
                var targetDir = await FileSystem.Current.LocalStorage.CreateFolderAsync("full-images", CreationCollisionOption.OpenIfExists);
                var targetName = GetTargetName();
                if (await targetDir.CheckExistsAsync(targetName) != ExistenceCheckResult.FileExists)
                {
                    var temp = await targetDir.CreateFileAsync(Guid.NewGuid() + "tmp", CreationCollisionOption.ReplaceExisting);
                    using (var response = await CreateImageRequest())
                    {
                        using (var targetStream = await temp.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            int lastUpdateProgress = 0;
                            var buf = new byte[4 * 1024];
                            int count, totalCopied = 0;
                            while ((count = await response.Stream.ReadAsync(buf, 0, buf.Length)) != 0)
                            {
                                await targetStream.WriteAsync(buf, 0, count);
                                totalCopied += count;
                                if (Environment.TickCount - lastUpdateProgress > 1000 / 60)
                                {
                                    ProgressCallback(Math.Min(99, (int)(100f * totalCopied / response.ContentLength)));
                                    lastUpdateProgress = Environment.TickCount;
                                }
                            }
                        }
                    }
                    await temp.RenameAsync(GetTargetName());
                }
                return await targetDir.GetFileAsync(GetTargetName());
            }

            Task<WebResponse> CreateImageRequest()
            {
                var client = ServiceLocator.Current.GetInstance<WebDownloader>();
                return client.ExecuteAsync(ImageUrl, new RequestParams { Referer = new Uri("http://joyreactor.cc/") });
            }

            string GetTargetName()
            {
                return ImageUrl.GetHashCode() + ".jpeg";
            }
        }
    }
}
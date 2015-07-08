﻿using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.ViewModels.Common;
using System;
using System.Windows.Input;

namespace JoyReactor.Core.ViewModels
{
    public class PostItemViewModel
    {
        #region Post properites

        internal Post Post { get { return post; } }

        public string Image { get { return post.Image; } }

        public DateTime Created { get { return post.Created.DateTimeFromUnixTimestampMs(); } }

        public string UserName { get { return post.UserName; } }

        public string UserImage { get { return post.UserImage; } }

        #endregion

        public float ImageAspect { get { return (float)post.ImageWidth / post.ImageHeight; } }

        public bool IsVideo { get { return post.Video != null; } }

        public ICommand OpenImageCommand { get; set; }

        Post post;

        public PostItemViewModel(Post post = null)
        {
            this.post = post;
            OpenImageCommand = new Command(() =>
            {
                OpenImageInFullscreen(post.Video ?? post.Image);
            });
        }

        void OpenImageInFullscreen(string mediaUri)
        {
            if (GalleryViewModel.IsCanShow(mediaUri))
                BaseNavigationService.Instance.ImageFullscreen(mediaUri);
        }

        public class Divider : PostItemViewModel
        {
        }
    }
}
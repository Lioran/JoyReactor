﻿using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using JoyReactor.Core.Model;

namespace JoyReactor.Android.Widget
{
    public class WebImageView : ImageView
    {
        string imageSource;

        public float ImageSize { get ; set; }

        public float ImageSizeDip
        { 
            get { return ImageSize / Resources.DisplayMetrics.Density; }
            set { ImageSize = value * Resources.DisplayMetrics.Density; }
        }

        public string ImageSource
        {
            get { return imageSource; }
            set { UpdateImageSource(value); }
        }

        public WebImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        void UpdateImageSource(string imageSource)
        {
            if (this.imageSource != imageSource)
            {
                this.imageSource = imageSource;

//                var u = imageSource == null ? null : new Uri(imageSource); // u == null отменяет закачки
//                iModel.Load(this, u, 0, s =>
//                    {
//                        if (s == null)
//                            SetImageDrawable(null);
//                        else
//                            SetImageBitmap((Bitmap)s);
//                    }); 
                new ImageRequest()
                    .SetToken(this)
                    .SetUrl(imageSource)
                    .CropIn((int)ImageSize)
                    .Into<Bitmap>(SetImageBitmap);
            }
        }
    }
}
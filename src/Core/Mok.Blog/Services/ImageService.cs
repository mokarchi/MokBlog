using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Mok.Blog.Enums;
using Mok.Blog.Services.Interfaces;
using Mok.Medias;
using Mok.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    public class ImageService : IImageService
    {
        private readonly AppSettings _appSettings;
        private readonly IStorageProvider _storageProvider;
        public ImageService(IStorageProvider storageProvider,
                            IOptionsSnapshot<AppSettings> appSettings)
        {
            _storageProvider = storageProvider;
            _appSettings = appSettings.Value;
        }
        /// <summary>
        /// "Blog"
        /// </summary>
        public const string BLOG_APP_NAME = "Blog";

        /// <summary>
        /// Small image (sm) size 600 pixel.
        /// </summary>
        /// <remarks>
        /// Good for media gallery, small low res phones.
        /// </remarks>
        public const int SMALL_IMG_SIZE = 600;

        /// <summary>
        /// Medium image (md) size 1200 pixel (1x default).
        /// </summary>
        public const int MEDIUM_IMG_SIZE = 1200;

        /// <summary>
        /// Medium large image (ml) size 1800 pixel (1.5x).
        /// </summary>
        public const int MEDIUM_LARGE_IMG_SIZE = 1800;

        /// <summary>
        /// Given a blog post's body html, it replaces all img tags with one that is updated for Repsonsive Images.
        /// </summary>
        /// <param name="body">A blog post's body html.</param>
        /// <returns></returns>
        public async Task<string> ProcessResponsiveImageAsync(string body)
        {
            if (body.IsNullOrEmpty()) return body;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(body);

                var imgNodes = doc.DocumentNode.SelectNodes("//img");
                if (imgNodes == null || imgNodes.Count <= 0) return body;

                bool changed = false;
                foreach (var imgNode in imgNodes)
                {
                    var imgNodeNew = await GetResponsiveImgNodeAsync(imgNode);
                    if (imgNodeNew != null)
                    {
                        imgNode.ParentNode.ReplaceChild(imgNodeNew, imgNode);
                        changed = true;
                    }
                }

                return changed ? doc.DocumentNode.OuterHtml : body;
            }
            catch (Exception)
            {
                return body;
            }
        }

        /// <summary>
        /// Returns a img node that is enhanced for Responsive Images.
        /// </summary>
        /// <param name="imgNode"></param>
        /// <returns></returns>
        /// <remarks>
        /// To understand the logic here <seealso cref="https://github.com/FanrayMedia/responsive-images"/>.
        /// </remarks>
        private async Task<HtmlNode> GetResponsiveImgNodeAsync(HtmlNode imgNode)
        {
            var src = imgNode.Attributes["src"]?.Value;
            if (src.IsNullOrEmpty()) return null;

            // e.g. src="https://localhost:44381/media/blog/2019/04/md/pic.png"
            var strAppType = $"{EAppType.Blog.ToString().ToLower()}/";
            var idxLastSlash = src.LastIndexOf('/');
            var fileName = src.Substring(idxLastSlash + 1, src.Length - idxLastSlash - 1);
            if (fileName.IsNullOrEmpty()) return null;

            var idxAppType = src.IndexOf(strAppType) + strAppType.Length;
            var strTimeSize = src.Substring(idxAppType, idxLastSlash - idxAppType); //2019/04/md
            var year = Convert.ToInt32(strTimeSize.Substring(0, 4));
            var month = Convert.ToInt32(strTimeSize.Substring(5, 2));

            var media = await _mediaSvc.GetMediaAsync(fileName, year, month);
            var resizeCount = media.ResizeCount;
            if (resizeCount <= 0) return null;

            var srcset = "";
            if (resizeCount == 1)
            {
                srcset = $"{GetAbsoluteUrl(media, EImageSize.Small)} {SMALL_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Original)} {media.Width}w";
            }
            else if (resizeCount == 2)
            {
                srcset = $"{GetAbsoluteUrl(media, EImageSize.Small)} {SMALL_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Medium)} {MEDIUM_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Original)} {media.Width}w";
            }
            else if (resizeCount == 3)
            {
                srcset = $"{GetAbsoluteUrl(media, EImageSize.Small)} {SMALL_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Medium)} {MEDIUM_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.MediumLarge)} 2x, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Original)} 3x";
            }
            else if (resizeCount == 4)
            {
                srcset = $"{GetAbsoluteUrl(media, EImageSize.Small)} {SMALL_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Medium)} {MEDIUM_IMG_SIZE}w, " +
                         $"{GetAbsoluteUrl(media, EImageSize.MediumLarge)} 2x, " +
                         $"{GetAbsoluteUrl(media, EImageSize.Large)} 3x"; // cap it at lg, so no orig here
            }

            // use media width to calc maxWidth and defaultWidth, height is not involved
            var maxWidth = media.Width < MEDIUM_LARGE_IMG_SIZE ? media.Width : MEDIUM_LARGE_IMG_SIZE;
            var defaultWidth = media.Width < MEDIUM_LARGE_IMG_SIZE ? media.Width : MEDIUM_LARGE_IMG_SIZE;
            var sizes = $"(max-width: {maxWidth}px) 100vw, {defaultWidth}px";

            imgNode.Attributes.Add("srcset", srcset);
            imgNode.Attributes.Add("sizes", sizes);

            return imgNode;
        }

        /// <summary>
        /// Returns absolute url to an image.
        /// </summary>
        /// <remarks>
        /// Based on the resize count, the url returned could be original or one of the resized image.
        /// </remarks>
        /// <param name="media">The media record representing the image.</param>
        /// <param name="size">The image size.</param>
        /// <returns></returns>
        public string GetAbsoluteUrl(Media media, EImageSize size)
        {
            var endpoint = _storageProvider.StorageEndpoint;
            var container = endpoint.EndsWith('/') ? _appSettings.MediaContainerName : $"/{_appSettings.MediaContainerName}";

            if ((size == EImageSize.Original || media.ResizeCount <= 0) ||
                (media.ResizeCount == 1 && size != EImageSize.Small) || // small
                (media.ResizeCount == 2 && size == EImageSize.MediumLarge) || // small, medium
                (media.ResizeCount == 2 && size == EImageSize.Large) || // small, medium
                (media.ResizeCount == 3 && size == EImageSize.Large)) // small, medium, medium large
            {
                size = EImageSize.Original;
            }

            var imagePath = GetImagePath(media.UploadedOn, size);
            var fileName = media.FileName;

            return $"{endpoint}{container}/{imagePath}/{fileName}";
        }

        /// <summary>
        /// Returns the stored image path, "{app}/{year}/{month}" or "{app}/{year}/{month}/{sizePath}".
        /// </summary>
        /// <param name="uploadedOn"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetImagePath(DateTimeOffset uploadedOn, EImageSize size)
        {
            var app = BLOG_APP_NAME.ToLowerInvariant();
            var year = uploadedOn.Year.ToString();
            var month = uploadedOn.Month.ToString("d2");
            var sizePath = "";

            switch (size)
            {
                case EImageSize.Large:
                    sizePath = "lg";
                    break;
                case EImageSize.MediumLarge:
                    sizePath = "ml";
                    break;
                case EImageSize.Medium:
                    sizePath = "md";
                    break;
                case EImageSize.Small:
                    sizePath = "sm";
                    break;
                default:
                    sizePath = null;
                    break;
            }

            return size == EImageSize.Original ? $"{app}/{year}/{month}" : $"{app}/{year}/{month}/{sizePath}";
        }

    }
}

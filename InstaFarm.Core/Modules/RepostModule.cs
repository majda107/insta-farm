using System;
using System.Linq;
using System.Threading.Tasks;
using InstaFarm.Core.Helpers;
using InstaFarm.Core.Models;
using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;

namespace InstaFarm.Core.Modules
{
    public class RepostModule : ModuleBase
    {
        public string RepostFrom { get; set; }
        public string Caption { get; set; }

        public RepostModule(InstaFarmUser user, string repostFrom, string caption) : base(user)
        {
            this.RepostFrom = repostFrom;
            this.Caption = caption;
        }

        public override async Task Execute()
        {
            this.Logger.Log("Executing repost module...");

            var posts = await this.User.Api.UserProcessor.GetUserMediaAsync(this.RepostFrom,
                PaginationParameters.MaxPagesToLoad(1));
            if (!posts.Succeeded || posts.Value.Count <= 0) throw new Exception("Couldn't fetch user media!");

            this.Logger.Log("Fetched user posts");

            var media = await this.User.Api.MediaProcessor.GetMediaByIdAsync(posts.Value[0].InstaIdentifier);
            if (!media.Succeeded) throw new Exception("Couldn't fetch media...");

            this.Logger.Log("Fetched user media");

            if (media.Value.Videos.Count > 0)
            {
                this.Logger.Log("Fetching video...");

                var video = media.Value.Videos.First();
                var thumb = media.Value.Images.First();

                var date = DateTime.Now.Ticks;
                var filename = $"video-{date.ToString()}.mp4";
                var filenameThumb = $"thumb-{date.ToString()}.jpg";

                await Task.WhenAll(new Task[]
                {
                    Downloader.DownloadFile(video.Uri, filename),
                    Downloader.DownloadFile(thumb.Uri, filenameThumb)
                });

                this.Logger.Log("Video downloaded...");

                await this.User.Api.MediaProcessor.UploadVideoAsync((p) => this.Logger.Log(p.UploadState.ToString()),
                    new InstaVideoUpload(new InstaVideo(filename, 0, 0), new InstaImage(filenameThumb, 0, 0)),
                    this.Caption);

                this.Logger.Log("Uploading video done...");
                // this.Logger.Log("Re-posting videos not supported yet!");
            }
            else if (media.Value.Images.Count > 0)
            {
                this.Logger.Log("Fetching image...");

                var filename = $"image-{DateTime.Now.Ticks.ToString()}.jpg";
                await Downloader.DownloadFile(media.Value.Images.First().Uri, filename);

                this.Logger.Log("Image downloaded!");

                await this.User.Api.MediaProcessor.UploadPhotoAsync((p) => this.Logger.Log(p.UploadState.ToString()),
                    new InstaImageUpload(filename), this.Caption);

                this.Logger.Log("Uploading image done...");
            }
            else
            {
                throw new Exception("No photo / videos in this media...");
            }
        }
    }
}
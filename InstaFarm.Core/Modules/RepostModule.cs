using System;
using System.Linq;
using System.Threading.Tasks;
using InstaFarm.Core.Helpers;
using InstaFarm.Core.Models;
using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;

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

            if (media.Value.Images.Count > 0)
            {
                this.Logger.Log("Fetching image...");

                var filename = $"image-{DateTime.Now.Ticks.ToString()}.jpg";
                await Downloader.DownloadFile(media.Value.Images.First().Uri, filename);

                this.Logger.Log("Image downloaded!");

                await this.User.Api.MediaProcessor.UploadPhotoAsync((p) => this.Logger.Log(p.Caption),
                    new InstaImageUpload(filename, 1000, 1000), this.Caption);

                this.Logger.Log("Uploading image done...");
            }
            else if (media.Value.Videos.Count > 0)
            {
                // var filename = $"video-{DateTime.Now.ToString()}.mp4";
                // await Downloader.DownloadFile(media.Value.Videos.First().Uri, filename);
                //
                // await this.User.Api.MediaProcessor.UploadVideoAsync((p) => this.Logger.Log(p.Caption),
                //     new InstaVideoUpload(new InstaVideo(filename, 1000, 1000), new InstaImage(filename, 1000, 1000)), this.Caption);
                //
                // this.Logger.Log("Uploading image done...");
                this.Logger.Log("Re-posting videos not supported yet!");
            }
            else
            {
                throw new Exception("No photo / videos in this media...");
            }
        }
    }
}
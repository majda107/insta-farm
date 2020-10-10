using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaFarm.Core.Extension;
using InstaFarm.Core.Models;
using InstagramApiSharp;

namespace InstaFarm.Core.Modules
{
    public class FollowLikersModule : ModuleBase
    {
        public int Count { get; set; }
        public List<string> Sources { get; set; }

        public FollowLikersModule(InstaFarmUser user, int count) : base(user)
        {
            this.Count = count;
            this.Sources = new List<string>();
        }

        public override async Task Execute()
        {
            this.Logger.Log("Executing repost module...");

            if (this.Sources.Count <= 0)
                throw new Exception("Not enough captions / sources");

            var source = this.Sources.ChooseRandom();

            this.Logger.Log($"Getting media from {source}...");

            var posts = await this.User.Api.UserProcessor.GetUserMediaAsync(source,
                PaginationParameters.MaxPagesToLoad(1));
            if (!posts.Succeeded || posts.Value.Count <= 0) throw new Exception("Couldn't fetch user media!");

            this.Logger.Log("Fetched user posts");

            // var media = await this.User.Api.MediaProcessor.GetMediaByIdAsync(posts.Value[0].InstaIdentifier);
            // if (!media.Succeeded) throw new Exception("Couldn't fetch media...");
            //
            // this.Logger.Log("Fetched user media");

            var likers = await this.User.Api.MediaProcessor.GetMediaLikersAsync(posts.Value.First().InstaIdentifier);
            if (!likers.Succeeded) throw new Exception("Couldn't fetch likers...");

            this.Logger.Log("Fetched media likers");


            int cap = Math.Min(this.Count, likers.Value.Count);
            var r = new Random();

            this.Logger.Log($"Found {cap} profiles to follow...!");

            for (int i = 0; i < cap; i++)
            {
                var follow = likers.Value[i];

                this.Logger.Log($"[{i + 1}/{cap}] Following {follow.UserName}");
                await this.User.Api.UserProcessor.FollowUserAsync(follow.Pk);
                this.Logger.Log($"[{i + 1}/{cap}] Followed {follow.UserName}");

                await Task.Delay(r.Next(200, 1500));
            }

            this.Logger.Log("Follow-liking done!");
        }
    }
}
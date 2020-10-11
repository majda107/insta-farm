using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaFarm.Core.Extension;
using InstaFarm.Core.Models;
using InstagramApiSharp;

namespace InstaFarm.Core.Modules
{
    public class CommentModule : ModuleBase
    {
        public List<String> Targets { get; set; }
        public List<String> Comments { get; set; }

        public CommentModule(InstaFarmUser u) : base(u)
        {
            this.Targets = new List<string>();
            this.Comments = new List<string>();
        }

        public override async Task Execute()
        {
            this.Logger.Log("Executing repost module...");

            if (this.Targets.Count <= 0 || this.Comments.Count <= 0)
                throw new Exception("Not enough targets / comments");

            var comment = this.Comments.ChooseRandom();
            var target = this.Targets.ChooseRandom();

            this.Logger.Log($"Commenting {target} - {comment}...");

            var posts = await this.User.Api.UserProcessor.GetUserMediaAsync(target,
                PaginationParameters.MaxPagesToLoad(1));
            if (!posts.Succeeded || posts.Value.Count <= 0) throw new Exception("Couldn't fetch user posts!");

            this.Logger.Log($"Commenting...");
            var res = await this.User.Api.CommentProcessor.CommentMediaAsync(posts.Value.First().InstaIdentifier,
                comment);
            this.Logger.Log($"Done! (succeed? {res.Succeeded})");
        }
    }
}
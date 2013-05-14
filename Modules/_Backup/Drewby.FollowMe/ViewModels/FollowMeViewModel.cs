using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Drewby.FollowMe.Models;

namespace Drewby.FollowMe.ViewModels
{
    public class FollowMeViewModel
    {
        FollowMePart _part;

        public FollowMeViewModel(FollowMePart part)
        {
            _part = part;

            AddService("twitter", part.TwitterUrl);
            AddService("facebook", part.FacebookUrl);
            AddService("rss", part.RssUrl);
            AddService("email", part.EmailUrl);
            AddService("flickr", part.FlickrUrl);
            AddService("youtube", part.YouTubeUrl);
        }

        public List<SocialService> _services = new List<SocialService>();

        public List<SocialService> Services
        {
            get { return _services; }
        }

        private void AddService(string name, string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                _services.Add(new SocialService { Name = name, Image = name, Url = url });
            }
        }

    }
}
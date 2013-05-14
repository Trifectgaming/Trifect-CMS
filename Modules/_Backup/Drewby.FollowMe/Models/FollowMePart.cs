using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;


namespace Drewby.FollowMe.Models
{
    public class FollowMeRecord : ContentPartRecord
    {
        public virtual string TwitterUrl { get; set; }
        public virtual string FacebookUrl { get; set; }
        public virtual string EmailUrl { get; set; }
        public virtual string RssUrl { get; set; }
        public virtual string FlickrUrl { get; set; }
        public virtual string YouTubeUrl { get; set; }
    }

    public class FollowMePart : ContentPart<FollowMeRecord>
    {
        public string TwitterUrl
        {
            get { return Record.TwitterUrl; }
            set { Record.TwitterUrl = value; }
        }

        public string FacebookUrl
        {
            get { return Record.FacebookUrl; }
            set { Record.FacebookUrl = value; }
        }

        public string EmailUrl
        {
            get { return Record.EmailUrl; }
            set { Record.EmailUrl = value; }
        }

        public string RssUrl
        {
            get { return Record.RssUrl; }
            set { Record.RssUrl = value; }
        }

        public string FlickrUrl
        {
            get { return Record.FlickrUrl; }
            set { Record.FlickrUrl = value; }
        }

        public string YouTubeUrl
        {
            get { return Record.YouTubeUrl; }
            set { Record.YouTubeUrl = value; }
        }

    }

}
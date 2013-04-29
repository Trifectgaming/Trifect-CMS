using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace LatestTwitter.Models
{
    public class TwitterWidgetRecord : ContentPartRecord
    {
		public TwitterWidgetRecord()
		{
			ShowReplies = true;
		}

        public virtual string Username { get; set; }
        public virtual int Count { get; set; }
        public virtual int CacheMinutes { get; set; }
        public virtual bool ShowAvatars { get; set; }
        public virtual bool ShowUsername { get; set; }
        public virtual bool ShowTimestamps { get; set; }
        public virtual bool ShowMentionsAsLinks { get; set; }
        public virtual bool ShowHashtagsAsLinks { get; set; }
        public virtual bool ShowTimestampsAsLinks { get; set; }
        public virtual string HashTagsFilter { get; set; }
		public virtual bool ShowReplies { get; set; }
    }

    public class TwitterWidgetPart : ContentPart<TwitterWidgetRecord>
    {
        [Required]
        public string Username
        {
            get { return Record.Username; }
            set { Record.Username = value; }
        }

        [Required]
        [DefaultValue("5")]
        [DisplayName("Number of Tweets to display")]
        public int Count
        {
            get { return Record.Count; }
            set { Record.Count = value; }
        }

        [Required]
        [DefaultValue("5")]
        [DisplayName("Time to cache Tweets (in minutes)")]
        public int CacheMinutes
        {
            get { return Record.CacheMinutes; }
            set { Record.CacheMinutes = value; }
        }

        public bool ShowAvatars
        {
            get { return Record.ShowAvatars; }
            set { Record.ShowAvatars = value; }
        }

        public bool ShowUsername {
            get { return Record.ShowUsername; }
            set { Record.ShowUsername = value; }
        }

        public bool ShowTimestamps
        {
            get { return Record.ShowTimestamps; }
            set { Record.ShowTimestamps = value; }
        }

        public bool ShowMentionsAsLinks
        {
            get { return Record.ShowMentionsAsLinks; }
            set { Record.ShowMentionsAsLinks = value; }
        }

        public bool ShowHashtagsAsLinks
        {
            get { return Record.ShowHashtagsAsLinks; }
            set { Record.ShowHashtagsAsLinks = value; }
        }

        public bool ShowTimestampsAsLinks
        {
            get { return Record.ShowTimestampsAsLinks; }
            set { Record.ShowTimestampsAsLinks = value; }
        }
        
        public string HashTagsFilter
        {
            get { return Record.HashTagsFilter; }
            set { Record.HashTagsFilter = value; }
        }

		public bool ShowReplies
		{
			get { return Record.ShowReplies; }
			set { Record.ShowReplies = value; }
		}
    }
}
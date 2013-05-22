using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Orchard.PopForumsAdapter.Models
{
    public class ForumPartRecord : ContentPartRecord
    {
        public virtual int ForumID { get; set; }
        public virtual int? CategoryID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual int TopicCount { get; set; }
        public virtual int PostCount { get; set; }
        public virtual DateTime LastPostTime { get; set; }
        public virtual string LastPostName { get; set; }
        public virtual string UrlName { get; set; }
        public virtual string ForumAdapterName { get; set; }
    }

    public class ForumPart : ContentPart<ForumPartRecord>
    {
        public int? CategoryID
        {
            get { return Record.CategoryID; }
            set { Record.CategoryID = value; }
        }

        public string Title
        {
            get { return Record.Title; }
            set { Record.Title = value; }
        }

        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public bool IsVisible
        {
            get { return Record.IsVisible; }
            set { Record.IsVisible = value; }
        }
    }
}
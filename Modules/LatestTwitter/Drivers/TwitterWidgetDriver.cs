using System;
using System.Collections.Generic;using System.Linq;using System.Web;using System.Xml.Linq;using LatestTwitter.Contracts.Services;
using LatestTwitter.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace LatestTwitter.Drivers
{
    public class TwitterWidgetDriver 
        : ContentPartDriver<TwitterWidgetPart>
    {
        protected ITweetRetrievalService TweetRetrievalService { get; private set; }

        public TwitterWidgetDriver(ITweetRetrievalService tweetRetrievalService)
        {
            this.TweetRetrievalService = tweetRetrievalService;
        }

        // GET
        protected override DriverResult Display(
            TwitterWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_TwitterWidget",
                () => shapeHelper.Parts_TwitterWidget(
                    Username: part.Username ?? "",
                    Tweets: TweetRetrievalService.GetTweetsFor(part),
                    ShowAvatars: part.ShowAvatars,
                    ShowUsername: part.ShowUsername,
                    ShowTimestamps: part.ShowTimestamps,
                    ShowMentionsAsLinks: part.ShowMentionsAsLinks,
                    ShowHashtagsAsLinks: part.ShowHashtagsAsLinks,
                    ShowTimestampsAsLinks: part.ShowTimestampsAsLinks,
                    HashTagsFilter: part.HashTagsFilter
                    ));
        }

        // GET
        protected override DriverResult Editor(TwitterWidgetPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_TwitterWidget_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/TwitterWidget",
                    Model: part,
                    Prefix: Prefix));
        }

        // POST
        protected override DriverResult Editor(
            TwitterWidgetPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
		
		protected override void Importing(TwitterWidgetPart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {
            var partName = part.PartDefinition.Name;

            part.Username = context.Attribute(partName, "Username");
            part.Count = int.Parse(context.Attribute(partName, "Count"));
            part.CacheMinutes = int.Parse(context.Attribute(partName, "CacheMinutes"));
            part.ShowAvatars = Boolean.Parse(context.Attribute(partName, "ShowAvatars"));
            part.ShowUsername = Boolean.Parse(context.Attribute(partName, "ShowUsername"));
            part.ShowHashtagsAsLinks = Boolean.Parse(context.Attribute(partName, "ShowHashtagsAsLinks"));
            part.ShowMentionsAsLinks = Boolean.Parse(context.Attribute(partName, "ShowMentionsAsLinks"));
			part.ShowReplies = Boolean.Parse(context.Attribute(partName, "ShowReplies"));
            part.ShowTimestamps = Boolean.Parse(context.Attribute(partName, "ShowTimestamps"));
		    part.ShowTimestampsAsLinks = Boolean.Parse(context.Attribute(partName, "ShowTimestampsAsLinks"));
            part.HashTagsFilter = context.Attribute(partName, "HashTagsFilter");
		}

        protected override void Exporting(TwitterWidgetPart part, Orchard.ContentManagement.Handlers.ExportContentContext context) {
            var partName = part.PartDefinition.Name;

            context.Element(partName).SetAttributeValue("Username", part.Username);
            context.Element(partName).SetAttributeValue("Count", part.Count);
            context.Element(partName).SetAttributeValue("CacheMinutes", part.CacheMinutes);
            context.Element(partName).SetAttributeValue("ShowAvatars", part.ShowAvatars);
            context.Element(partName).SetAttributeValue("ShowUsername", part.ShowUsername);
            context.Element(partName).SetAttributeValue("ShowHashtagsAsLinks", part.ShowHashtagsAsLinks);
            context.Element(partName).SetAttributeValue("ShowMentionsAsLinks", part.ShowMentionsAsLinks);
			context.Element(partName).SetAttributeValue("ShowReplies", part.ShowReplies);
            context.Element(partName).SetAttributeValue("ShowTimestamps", part.ShowTimestamps);
            context.Element(partName).SetAttributeValue("ShowTimestampsAsLinks", part.ShowTimestampsAsLinks);
            context.Element(partName).SetAttributeValue("HashTagsFilter", part.HashTagsFilter);
        }
    }
}
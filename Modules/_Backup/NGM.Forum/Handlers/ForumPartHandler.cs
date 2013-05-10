using System.Web.Routing;
using JetBrains.Annotations;
using NGM.Forum.Extensions;
using NGM.Forum.Models;
using NGM.Forum.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace NGM.Forum.Handlers {
    [UsedImplicitly]
    public class ForumPartHandler : ContentHandler {
        private readonly IForumPathConstraint _forumPathConstraint;

        public ForumPartHandler(IRepository<ForumPartRecord> repository, IForumPathConstraint forumPathConstraint) {
            _forumPathConstraint = forumPathConstraint;
            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<ForumPart>((context, forum) => {
                context.Shape.PostCount = forum.PostCount;
            });

            OnPublished<ForumPart>((context, forum) => _forumPathConstraint.AddPath(forum.As<IAliasAspect>().Path));
            OnUnpublished<ForumPart>((context, forum) => _forumPathConstraint.RemovePath(forum.As<IAliasAspect>().Path));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var forum = context.ContentItem.As<ForumPart>();

            if (forum == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "Forum"},
                {"Action", "Item"},
                {"forumId", context.ContentItem.Id}
            };
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "ForumAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "ForumAdmin"},
                {"Action", "Edit"},
                {"forumId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "ForumAdmin"},
                {"Action", "Remove"},
                {"forumId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "ForumAdmin"},
                {"Action", "Item"},
                {"forumId", context.ContentItem.Id}
            };
        }
    }
}
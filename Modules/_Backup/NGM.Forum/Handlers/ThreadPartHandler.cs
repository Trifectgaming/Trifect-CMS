using System.Linq;
using System.Web.Routing;
using JetBrains.Annotations;
using NGM.Forum.Extensions;
using NGM.Forum.Models;
using NGM.Forum.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Security;

namespace NGM.Forum.Handlers {
    [UsedImplicitly]
    public class ThreadPartHandler : ContentHandler {
        private readonly IPostService _postService;
        private readonly IThreadService _threadService;
        private readonly IForumService _forumService;
        private readonly IContentManager _contentManager;

        public ThreadPartHandler(IRepository<ThreadPartRecord> repository, 
            IPostService postService,
            IThreadService threadService,
            IForumService forumService,
            IContentManager contentManager) {
            _postService = postService;
            _threadService = threadService;
            _forumService = forumService;
            _contentManager = contentManager;

            Filters.Add(StorageFilter.For(repository));

            OnGetDisplayShape<ThreadPart>(SetModelProperties);
            OnGetEditorShape<ThreadPart>(SetModelProperties);
            OnUpdateEditorShape<ThreadPart>(SetModelProperties);

            OnActivated<ThreadPart>(PropertySetHandlers);
            OnLoading<ThreadPart>((context, part) => LazyLoadHandlers(part));
            OnCreated<ThreadPart>((context, part) => UpdateForumPartCounters(part));
            OnPublished<ThreadPart>((context, part) => UpdateForumPartCounters(part));
            OnUnpublished<ThreadPart>((context, part) => UpdateForumPartCounters(part));
            OnVersioning<ThreadPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));
            OnVersioned<ThreadPart>((context, part, newVersionPart) => UpdateForumPartCounters(newVersionPart));
            OnRemoved<ThreadPart>((context, part) => UpdateForumPartCounters(part));
            
            OnRemoved<ForumPart>((context, b) => 
                _threadService
                    .Get(context.ContentItem.As<ForumPart>())
                    .ToList()
                    .ForEach(thread => context.ContentManager.Remove(thread.ContentItem)));


        }

        private void SetModelProperties(BuildShapeContext context, ThreadPart threadPart) {
            context.Shape.Forum = threadPart.ForumPart;
            context.Shape.StickyClass = threadPart.IsSticky ? "Sticky" : string.Empty;
        }

        private void UpdateForumPartCounters(ThreadPart threadPart) {
            var commonPart = threadPart.As<CommonPart>();
            if (commonPart != null &&
                commonPart.Record.Container != null) {

                ForumPart forumPart = threadPart.ForumPart ??
                                      _forumService.Get(commonPart.Record.Container.Id, VersionOptions.Published).As<ForumPart>();

                forumPart.ContentItem.ContentManager.Flush();

                forumPart.ThreadCount = _threadService.Get(forumPart, VersionOptions.Published).Count();
                forumPart.PostCount = _threadService
                    .Get(forumPart, VersionOptions.Published)
                    .Sum(publishedThreadPart => _postService
                        .Get(publishedThreadPart, VersionOptions.Published)
                        .Count());
            }
        }

        protected void LazyLoadHandlers(ThreadPart part) {
            // add handlers that will load content for id's just-in-time
            part.ClosedByField.Loader(() => _contentManager.Get<IUser>(part.Record.ClosedById));
        }

        protected static void PropertySetHandlers(ActivatedContentContext context, ThreadPart part) {
            // add handlers that will update records when part properties are set

            part.ClosedByField.Setter(user => {
                part.Record.ClosedById = user == null
                    ? 0
                    : user.ContentItem.Id;
                return user;
            });

            // Force call to setter if we had already set a value
            if (part.ClosedByField.Value != null)
                part.ClosedByField.Value = part.ClosedByField.Value;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var thread = context.ContentItem.As<ThreadPart>();

            if (thread == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "Thread"},
                {"Action", "Item"},
                {"forumId", thread.ForumPart.ContentItem.Id},
                {"threadId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", Constants.LocalArea},
                {"Controller", "ThreadAdmin"},
                {"Action", "Item"},
                {"threadId", context.ContentItem.Id}
            };
        }
    }
}
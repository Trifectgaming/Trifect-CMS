﻿using System.Linq;
using System.Web.Mvc;
using NGM.Forum.Models;
using NGM.Forum.Services;
using NGM.Forum.Services.Sorting;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;

namespace NGM.Forum.Controllers {
    [Themed]
    public class ForumController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IForumService _forumService;
        private readonly IThreadService _threadService;
        private readonly ISiteService _siteService;

        public ForumController(IOrchardServices orchardServices, 
            IForumService forumService,
            IThreadService threadService,
            ISiteService siteService,
            IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _forumService = forumService;
            _threadService = threadService;
            _siteService = siteService;

            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult List() {
            var forums = _forumService.Get().Select(fbc => _orchardServices.ContentManager.BuildDisplay(fbc, "Summary"));

            var list = Shape.List();
            list.AddRange(forums);

            dynamic viewModel = Shape.ViewModel()
                .ContentItems(list);

            return View((object)viewModel);
        }

        public ActionResult Item(int forumId, PagerParameters pagerParameters, SortingParameters sortingParameters) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewForum, T("Not allowed to view forum")))
                return new HttpUnauthorizedResult();

            var forumPart = _forumService.Get(forumId, VersionOptions.Published).As<ForumPart>();
            if (forumPart == null)
                return HttpNotFound();

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            
            var threads = _threadService
                .Get(forumPart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Published)
                .Select(b => _orchardServices.ContentManager.BuildDisplay(b, "Summary"));

            dynamic forum = _orchardServices.ContentManager.BuildDisplay(forumPart);

            var list = Shape.List();
            list.AddRange(threads);
            forum.Content.Add(Shape.Parts_Forums_Thread_List(ContentPart: forumPart, ContentItems: list), "5");

            var totalItemCount = forumPart.ThreadCount;
            forum.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            return new ShapeResult(this, forum);
        }
    }
}
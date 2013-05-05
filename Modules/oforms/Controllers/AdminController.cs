using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard;
using oforms.Models;
using oforms.Services;
using Orchard.Data;
using System.Text;
using oforms.ViewModels;
using Orchard.Core.Contents.Controllers;
using Orchard.UI.Admin;

namespace oforms.Controllers
{
    [Admin, ValidateInput(false)]
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IOrchardServices _services;
        private readonly ISiteService _siteService;
        private readonly IOFormService _formService;
        private readonly ISerialService _serial;
        private readonly IRepository<OFormResultRecord> _resultsRepo;
        private readonly IRepository<OFormFileRecord> _fileRepo;

        public AdminController(IOrchardServices services, 
            IShapeFactory shapeFactory, 
            ISiteService siteService,
            IOFormService formService, 
            ISerialService serial,
            IRepository<OFormResultRecord> resultsRepo,
            IRepository<OFormFileRecord> fileRepo)
        {
            this._services = services;
            _siteService = siteService;
            this._formService = formService;
            this._serial = serial;
            this.Shape = shapeFactory;
            _resultsRepo = resultsRepo;
            this._fileRepo = fileRepo;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index()
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list users")))
                return new HttpUnauthorizedResult();
            var forms = _services.ContentManager.Query<OFormPart, OFormPartRecord>(VersionOptions.Latest).List();
            ViewBag.ValidSn = _serial.IsSerialValid();
            return View(forms.ToList());
        }

        public ActionResult Create(string template)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to create forms")))
                return new HttpUnauthorizedResult();

            var form = _services.ContentManager.New<OFormPart>("OForm");
            if (form == null)
                return HttpNotFound();
            
            if (!string.IsNullOrEmpty(template)) {
            	OFormTemplateHelper.PreFillForm(template, form, _services);
            }

            dynamic model = _services.ContentManager.BuildEditor(form);
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST()
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to create forms")))
                return new HttpUnauthorizedResult();
            
            var form = _services.ContentManager.New<OFormPart>("OForm");
            _services.ContentManager.Create(form, VersionOptions.Draft);
            dynamic model = _services.ContentManager.UpdateEditor(form, this);

            if (!ModelState.IsValid)
            {
                _services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _services.Notifier.Information(T("Form {0} created successfully", form.Name));

            if (!String.IsNullOrEmpty(Request.Form["submit.Publish"]))
            {
                _services.ContentManager.Publish(form.ContentItem);
            }

            if (!String.IsNullOrEmpty(Request.Form["submit.Apply"]))
            {
                return RedirectToAction("Edit", new { form.Id });
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not allowed to edit form")))
                return new HttpUnauthorizedResult();

            var form = _services.ContentManager.Get<OFormPart>(id, VersionOptions.Latest);
            if (form == null)
                return HttpNotFound();

            dynamic model = _services.ContentManager.BuildEditor(form);
            ViewBag.IsPublished = form.IsPublished;
            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Couldn't edit form")))
                return new HttpUnauthorizedResult();

            var form = _services.ContentManager.Get<OFormPart>(id, VersionOptions.Latest);
            dynamic model = _services.ContentManager.UpdateEditor(form, this);
            if (!ModelState.IsValid)
            {
                _services.TransactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            _services.Notifier.Information(T("Form {0} updated successfully", form.Name));

            if (!String.IsNullOrEmpty(Request.Form["submit.Publish"])) {
                _services.ContentManager.Publish(form.ContentItem);
            }

            if (!String.IsNullOrEmpty(Request.Form["submit.Apply"])) {
                return RedirectToAction("Edit", new { form.Id });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Couldn't delete form")))
                return new HttpUnauthorizedResult();

            var form = this._services.ContentManager.Get(id, VersionOptions.Latest);
            if (form == null)
                return HttpNotFound();

            _services.ContentManager.Remove(form);

            _services.Notifier.Information(T("Form {0} deleted successfully", form.As<OFormPart>().Name));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Publish(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Couldn't delete form")))
                return new HttpUnauthorizedResult();

            var form = this._services.ContentManager.Get(id, VersionOptions.Latest);
            if (form == null)
                return HttpNotFound();

            _services.ContentManager.Publish(form);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Unpublish(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Couldn't delete form")))
                return new HttpUnauthorizedResult();

            var form = this._services.ContentManager.Get(id, VersionOptions.Latest);
            if (form == null)
                return HttpNotFound();

            _services.ContentManager.Unpublish(form);

            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        public void AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

    }
}
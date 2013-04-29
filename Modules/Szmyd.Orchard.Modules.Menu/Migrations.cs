using System.Linq;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Environment;
using Orchard.Environment.Features;

namespace Szmyd.Orchard.Modules.Menu
{
    public class Migrations : DataMigrationImpl
    {
        private readonly Work<IRepository<ContentTypeDefinitionRecord>> _typeDefinitionRepository;
        private readonly Work<IFeatureManager> _features;

        public Migrations(Work<IRepository<ContentTypeDefinitionRecord>> typeDefinitionRepository, Work<IFeatureManager> features) {
            _typeDefinitionRepository = typeDefinitionRepository;
            _features = features;
        }

        public int Create()
        {
            /* Menu item record */
            SchemaBuilder.CreateTable("AdvancedMenuItemPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Text")
                .Column<string>("Url")
                .Column<string>("Position", c => c.WithDefault("1"))
                .Column<string>("MenuName", c => c.WithDefault("main"))
                .Column<string>("SubTitle", c => c.WithLength(1000))
                .Column<string>("Classes", c => c.WithLength(1000))
                .Column<bool>("DisplayText", c => c.WithDefault(true))
                .Column<bool>("DisplayHref", c => c.WithDefault(true))
                .Column<string>("RequiresPermission")
                .Column<int>("RelatedContentId")
                .Column<string>("AuthenticationLevel", c => c.WithDefault("Any"))
                .Column<bool>("IncludeReturnUrl", c => c.WithDefault(false)));

            /* Menu record */
            SchemaBuilder.CreateTable("AdvancedMenuPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Name"));

            /* Menu widget record */
            SchemaBuilder.CreateTable("MenuWidgetPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("MenuName")
                .Column<int>("Mode")
                .Column<string>("RootNode")
                .Column<bool>("WrapChildrenInDivs")
                .Column<int>("Levels")
                .Column<bool>("CutOrFlattenLower"));

            /* Record for storing styled menu info */
            SchemaBuilder.CreateTable("MenuStylingPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("BackColor", c => c.WithDefault("#FFFFFF"))
                .Column<string>("ForeColor", c => c.WithDefault("#000000"))
                .Column<string>("SelectedBackColor", c => c.WithDefault("#DDDDDD"))
                .Column<string>("SelectedForeColor", c => c.WithDefault("#000000"))
                .Column<string>("HoverBackColor", c => c.WithDefault("#EEEEEE"))
                .Column<string>("HoverForeColor", c => c.WithDefault("#000000"))
                .Column<string>("Style", c => c.WithDefault("SuperfishHorizontal")));

            
            ContentDefinitionManager.AlterTypeDefinition("Page", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterTypeDefinition("BlogPost", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterTypeDefinition("Blog", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterPartDefinition("MenuRelationPart", builder => builder.Attachable());

            /* Custom menu item */
            ContentDefinitionManager.AlterTypeDefinition("SimpleMenuItem",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("AdvancedMenuItemPart")
                    .WithPart("IdentityPart")
                );

            /* Custom menu item with template */
            ContentDefinitionManager.AlterTypeDefinition("TemplatedMenuItem",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("AdvancedMenuItemPart")
                    .WithPart("BodyPart")
                    .WithPart("IdentityPart")
                );

            /* Remove the previous name of this part and alter parts' definitions */
            ContentDefinitionManager.AlterTypeDefinition("MenuWidget",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("MenuWidgetPart")
                    .WithPart("WidgetPart")
                    .WithPart("IdentityPart")
                    .WithSetting("Stereotype", "Widget")
                );

            /* Defining Superfish-styled menu widget */
            ContentDefinitionManager.AlterTypeDefinition("StyledMenuWidget",
               cfg => cfg
                   .WithPart("CommonPart")
                   .WithPart("MenuStylingPart")
                   .WithPart("MenuWidgetPart")
                   .WithPart("WidgetPart")
                   .WithPart("IdentityPart")
                   .WithSetting("Stereotype", "Widget"));

            /* Custom menu */
            ContentDefinitionManager.AlterTypeDefinition("Menu",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("AdvancedMenuPart")
                    .WithPart("IdentityPart")
                    .Creatable());

            return 8;
        }

        public int UpdateFrom1()
        {
            return 2;
        }

        public int UpdateFrom2()
        {
            /* Changes in 1.2 */
            /************************************************************/
            /* Settings are obsolete */
            SchemaBuilder.DropTable("MenuSettingsPartRecord");

            /* Menu item record */
            SchemaBuilder.CreateTable("AdvancedMenuItemPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Text")
                .Column<string>("Url", c => c.Nullable())
                .Column<string>("Position", c => c.WithDefault("1"))
                .Column<string>("MenuName", c => c.WithDefault("main").NotNull())
                .Column<string>("SubTitle", c => c.WithLength(1000).Nullable())
                .Column<string>("Classes", c => c.WithLength(1000).Nullable())
                .Column<bool>("DisplayText", c => c.WithDefault(true))
                .Column<bool>("DisplayHref", c => c.WithDefault(true))
                .Column<int>("RelatedContentId", c => c.Nullable()));

            /* Menu record */
            SchemaBuilder.CreateTable("AdvancedMenuPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Name", c => c.Unique()));

            /* Menu widget record */
            SchemaBuilder.CreateTable("MenuWidgetPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("MenuName")
                .Column<int>("Mode")
                .Column<string>("RootNode")
                .Column<bool>("WrapChildrenInDivs")
                .Column<int>("Levels")
                .Column<bool>("CutOrFlattenLower"));

            /* Record for storing styled menu info */
            SchemaBuilder.CreateTable("MenuStylingPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("BackColor", c => c.WithDefault("#FFFFFF"))
                .Column<string>("ForeColor", c => c.WithDefault("#000000"))
                .Column<string>("SelectedBackColor", c => c.WithDefault("#DDDDDD"))
                .Column<string>("SelectedForeColor", c => c.WithDefault("#000000"))
                .Column<string>("HoverBackColor", c => c.WithDefault("#EEEEEE"))
                .Column<string>("HoverForeColor", c => c.WithDefault("#000000"))
                .Column<string>("Style", c => c.WithDefault("SuperfishHorizontal")));

            return 3;
        }

        public int UpdateFrom3() {

            /* Removing the default parts so they won't mess up */
            ContentDefinitionManager.AlterTypeDefinition("Page", cfg => cfg.RemovePart("MenuPart"));

            /* Custom menu item */
            ContentDefinitionManager.AlterTypeDefinition("SimpleMenuItem",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("AdvancedMenuItemPart")
                );

            /* Custom menu item with template */
            ContentDefinitionManager.AlterTypeDefinition("TemplatedMenuItem",
                cfg => cfg
                    .WithPart("AdvancedMenuItemPart")
                    .WithPart("BodyPart")
                );

            /* Remove the previous name of this part and alter parts' definitions */
            ContentDefinitionManager.AlterTypeDefinition("MenuWidget",
                cfg => cfg
                    .RemovePart("SubMenuPart")
                    .WithPart("MenuWidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            /* Defining Superfish-styled menu widget */
            ContentDefinitionManager.AlterTypeDefinition("StyledMenuWidget",
               cfg => cfg
                   .WithPart("MenuStylingPart")
                   .WithPart("MenuWidgetPart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget"));

            /* Custom menu */
            ContentDefinitionManager.AlterTypeDefinition("Menu",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("AdvancedMenuPart")
                    .Creatable());

            return 4;
        }

        public int UpdateFrom4() {
            ContentDefinitionManager.AlterTypeDefinition("Page", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterTypeDefinition("BlogPost", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterTypeDefinition("Blog", cfg => cfg.RemovePart("MenuPart").WithPart("MenuRelationPart"));
            ContentDefinitionManager.AlterPartDefinition("MenuRelationPart", builder => builder.Attachable());
            return 5;
        }

        public int UpdateFrom5()
        {
            /* Removing items from here - moving to other migrations */
            if (!_features.Value.GetEnabledFeatures().Any(f => f.Id == "Szmyd.Menu.Breadcrumbs")) {
                ContentDefinitionManager.AlterTypeDefinition("BreadcrumbsWidget", cfg => { });
                var bWidget = _typeDefinitionRepository.Value.Get(r => r.Name == "BreadcrumbsWidget");
                if (bWidget != null)
                    _typeDefinitionRepository.Value.Delete(bWidget);
                _typeDefinitionRepository.Value.Flush();
            }

            if (!_features.Value.GetEnabledFeatures().Any(f => f.Id == "Szmyd.Menu.Breadcrumbs")) {
                ContentDefinitionManager.AlterTypeDefinition("RecentlySeenWidget", cfg => { });
                var rWidget = _typeDefinitionRepository.Value.Get(r => r.Name == "RecentlySeenWidget");
                if (rWidget != null)
                    _typeDefinitionRepository.Value.Delete(rWidget);
                _typeDefinitionRepository.Value.Flush();
            }
            return 6;
        }

        public int UpdateFrom6() {
            SchemaBuilder.AlterTable("AdvancedMenuItemPartRecord", table => table.AddColumn<string>("RequiresPermission"));
            return 7;
        }

        public int UpdateFrom7() {
            SchemaBuilder.AlterTable("AdvancedMenuItemPartRecord", table =>
                table.AddColumn<string>("AuthenticationLevel", c => c.WithDefault("Any")));
            SchemaBuilder.AlterTable("AdvancedMenuItemPartRecord", table =>
                table.AddColumn<bool>("IncludeReturnUrl", c => c.WithDefault(false)));
            return 8;
        }
    }
}

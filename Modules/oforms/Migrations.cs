using System.Data;
using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Indexing;
using oforms.Models;

namespace oforms {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            // Creating table OFormPartRecord
            SchemaBuilder.CreateTable("OFormPartRecord", table => table
                .ContentPartRecord()
                .Column("Name", DbType.String, x => x.Unique())
                .Column("Method", DbType.String)
                .Column("InnerHtml", DbType.String)
                .Column("Action", DbType.String)
                .Column("RedirectUrl", DbType.String)
                .Column("CanUploadFiles", DbType.Boolean)
                .Column("UploadFileSizeLimit", DbType.Int64)
                .Column("UploadFileType", DbType.String)
                .Column("UseCaptcha", DbType.Boolean)
                .Column("SendEmail", DbType.Boolean)
                .Column("EmailFromName", DbType.String)
                .Column("EmailFrom", DbType.String)
                .Column("EmailSubject", DbType.String)
                .Column("EmailSendTo", DbType.String)
                .Column("EmailTemplate", DbType.String)
            );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("OFormPartRecord",
                table =>
                {
                    table.AddColumn("SaveResultsToDB", DbType.Boolean);
                    table.AddColumn("ValRequiredFields", DbType.String);
                    table.AddColumn("ValNumbersOnly", DbType.String);
                    table.AddColumn("ValLettersOnly", DbType.String);
                    table.AddColumn("ValLettersAndNumbersOnly", DbType.String);
                    table.AddColumn("ValDate", DbType.String);
                    table.AddColumn("ValEmail", DbType.String);
                    table.AddColumn("ValUrl", DbType.String);
                }

           );
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("OFormPartRecord",
                table =>
                {
                    table.AlterColumn("InnerHtml", x => x.WithType(DbType.String).Unlimited());
                    table.AlterColumn("EmailTemplate", x => x.WithType(DbType.String).Unlimited());
                }
           );

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.CreateTable("OFormResultRecord",
            table => table
                .Column<int>("Id", x => x.PrimaryKey().Identity())
                .Column<string>("Xml", x => x.Unlimited())
                .Column<int>("OFormPartRecord_Id")
            );

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable("OFormResultRecord",
            table => {
                    table.AddColumn("CreatedDate", DbType.DateTime);
                    table.AddColumn("Ip", DbType.String);
                }
            );

            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable("OFormPartRecord",
                table =>
                {
                    table.AlterColumn("ValRequiredFields", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValNumbersOnly", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValLettersOnly", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValLettersAndNumbersOnly", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValDate", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValEmail", x => x.WithType(DbType.String).WithLength(800));
                    table.AlterColumn("ValUrl", x => x.WithType(DbType.String).WithLength(800));
                }
            );

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.CreateTable("OFormFileRecord",
                table =>
                {
                    table.Column<int>("Id", x => x.PrimaryKey().Identity());
                    table.Column<int>("OFormResultRecord_Id");
                    table.Column<string>("FieldName");
                    table.Column<string>("OriginalName");
                    table.Column<string>("ContentType");
                    table.Column<byte[]>("Bytes", x => x.WithType(DbType.Binary).Unlimited());
                    table.Column<long>("Size");
                }
            );

            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable("OFormResultRecord",
                table =>
                {
                    table.AddColumn<string>("CreatedBy");
                }
            );

            SchemaBuilder.AlterTable("OFormPartRecord",
                table =>
                {
                    table.AddColumn<string>("SuccessMessage", x => x.WithType(DbType.String).WithLength(500)
                        .WithDefault("Form submitted successfully"));
                }
            );

            return 8;
        }

        public int UpdateFrom8()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(OFormPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("OForm",
                cfg => cfg
                    .WithPart(typeof(OFormPart).Name)
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-from'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("MenuPart"));

            return 9;
        }

        public int UpdateFrom9()
        {
            ContentDefinitionManager.AlterTypeDefinition("OFormWidget", cfg => cfg
                .WithPart(typeof(OFormPart).Name)
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));
            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable("OFormPartRecord",
                table =>
                {
                    table.AddColumn<string>("Target");
                }
            );

            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.CreateTable("OFormSettingsPartRecord",
                table => table.ContentPartRecord()
                    .Column<string>("SerialKey")
                );

            return 12;
        }
    }
}
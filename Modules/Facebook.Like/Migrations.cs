using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Facebook.Like.Models;

namespace Facebook.Like {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("LikePartRecord", table => table
                .ContentPartRecord()
                .Column("Show_Faces", DbType.Boolean)
                .Column("Width", DbType.Int32)
                .Column("Font", DbType.String)
                .Column("Colorsheme", DbType.String)
                .Column("LayoutStyle", DbType.String)
                .Column("Verb", DbType.String)
            );

            ContentDefinitionManager.AlterPartDefinition(
                typeof(LikePart).Name, cfg => cfg.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            // Create a new widget content type with our map
            ContentDefinitionManager.AlterTypeDefinition("FacebookLikeWidget", cfg => cfg
                .WithPart("LikePart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));

            return 2;
        }
    }
}
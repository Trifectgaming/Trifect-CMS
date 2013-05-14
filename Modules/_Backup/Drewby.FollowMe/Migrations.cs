using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Drewby.FollowMe
{
    public class Migrations : DataMigrationImpl {

        public int Create() {
			// Creating table FollowMeRecord
			SchemaBuilder.CreateTable("FollowMeRecord", table => table
				.ContentPartRecord()
				.Column("TwitterUrl", DbType.String)
				.Column("FacebookUrl", DbType.String)
				.Column("EmailUrl", DbType.String)
				.Column("RssUrl", DbType.String)
				.Column("FlickrUrl", DbType.String)
				.Column("YouTubeUrl", DbType.String)
			);

            ContentDefinitionManager.AlterTypeDefinition("FollowMeWidget", cfg => cfg
                .WithPart("FollowMePart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget")
                .WithSetting("Description", "Displays icons and links to your profile pages on popular social websites.")
                );

            return 1;
        }
    }
}
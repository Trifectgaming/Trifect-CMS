using System;
using System.Collections.Generic;
using System.Data;
using LatestTwitter.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace LatestTwitter {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            // Creating table TwitterWidgetRecord
            SchemaBuilder.CreateTable("TwitterWidgetRecord", table => table
				.ContentPartRecord()
				.Column("Username", DbType.String)
                .Column("Count", DbType.Int32)
			);

            ContentDefinitionManager.AlterPartDefinition(typeof(TwitterWidgetPart).Name,
                builder => builder.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("TwitterWidget", cfg => cfg
                .WithPart("TwitterWidgetPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("CacheMinutes", DbType.Int32)
            );

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowAvatars", DbType.Boolean)
            );
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowTimestamps", DbType.Boolean)
            );
            
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowMentionsAsLinks", DbType.Boolean)
            );
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowHashtagsAsLinks", DbType.Boolean)
            );
            
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowUsername", DbType.Boolean)
            );

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("ShowTimestampsAsLinks", DbType.Boolean)
            );

            return 7;
        }
        
        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
                .AddColumn("HashTagsFilter", DbType.String)
            );

            return 8;
        }

		public int UpdateFrom8()
		{
			SchemaBuilder.AlterTable("TwitterWidgetRecord", table => table
				.AddColumn("ShowReplies", DbType.Boolean)
			);
			return 9;
		}
    }
}
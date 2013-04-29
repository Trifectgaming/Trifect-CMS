using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Facebook.Like.Models;

namespace Facebook.Like.Handlers
{
    public class LikeHandler : ContentHandler
    {
        public LikeHandler(IRepository<LikePartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
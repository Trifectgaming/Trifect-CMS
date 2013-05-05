using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Drewby.FollowMe.Models;
using Orchard.Data;

namespace Drewby.FollowMe.Handlers
{
    public class FollowMeHandler : ContentHandler
    {
        public FollowMeHandler(IRepository<FollowMeRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
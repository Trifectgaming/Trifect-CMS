using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using oforms.Models;
using Orchard;

namespace oforms.Services
{
    public interface ISerialService : IDependency {
        bool IsSerialValid();
    }
}

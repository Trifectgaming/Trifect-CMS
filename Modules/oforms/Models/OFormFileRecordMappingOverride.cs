using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;

namespace oforms.Models {
    public class OFormFileRecordMappingOverride : IAutoMappingOverride<OFormFileRecord> {
        public void Override(AutoMapping<OFormFileRecord> mapping) {
            mapping.Map(x => x.Bytes).CustomType(NHibernateUtil.BinaryBlob.Name);
        }
    }
}
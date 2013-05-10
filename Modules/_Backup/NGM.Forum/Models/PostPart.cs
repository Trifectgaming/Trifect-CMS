﻿using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace NGM.Forum.Models {
    public class PostPart : ContentPart<PostPartRecord> {
        public int? RepliedOn {
            get { return Record.RepliedOn; }
            set { Record.RepliedOn = value; }
        }

        public string Text {
            get { return Record.Text; }
            set { this.Record.Text = value; }
        }

        public string Format {
            get { return Record.Format; }
            set { this.Record.Format = value; }
        }

        public ThreadPart ThreadPart {
            get { return this.As<ICommonPart>().Container.As<ThreadPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public bool IsParentThread() {
            return RepliedOn == null;
        }
    }

    public class PostPartRecord : ContentPartRecord {
        public virtual int? RepliedOn { get; set; }

        [StringLengthMax]
        public virtual string Text { get; set; }

        public virtual string Format { get; set; }
    }
}
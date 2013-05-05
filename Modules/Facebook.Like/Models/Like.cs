using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Facebook.Like.Models
{
    public class LikePartRecord : ContentPartRecord
    {
        public virtual bool Show_Faces { get; set; }
        public virtual int Width { get; set; }
        public virtual string Font { get; set; }
        public virtual string Colorsheme { get; set; }
        public virtual string LayoutStyle { get; set; }
        public virtual string Verb { get; set; }

        public LikePartRecord()
        {
            Show_Faces = false;
            Font = "Lucida Grande";
            Width = 450;
            Colorsheme = "light";
            LayoutStyle = "standard";
            Verb = "like";
        }
    }

    public class LikePart : ContentPart<LikePartRecord> {

        [Required]
        public bool Show_Faces
        {
            get { return Record.Show_Faces; }
            set { Record.Show_Faces = value; }
        }
        
        [Required]
        public int Width
        {
            get { return Record.Width; }
            set { Record.Width = value; }
        }
        
        public string Font
        {
            get { return Record.Font; }
            set { Record.Font = value; }
        }
        
        [Required]
        public string Colorsheme
        {
            get { return Record.Colorsheme; }
            set { Record.Colorsheme = value; }
        }

        [Required]
        public string LayoutStyle
        {
            get { return Record.LayoutStyle; }
            set { Record.LayoutStyle = value; }
        }

        [Required]
        public string Verb
        {
            get { return Record.Verb; }
            set { Record.Verb = value; }
        }
    }
}
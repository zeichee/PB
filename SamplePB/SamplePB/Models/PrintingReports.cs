using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SamplePB.Models
{
    public class PrintingReports
    {
        public byte[] RenderedBytes   { get; set; }
        public String MimeType { get; set; }
    }
}
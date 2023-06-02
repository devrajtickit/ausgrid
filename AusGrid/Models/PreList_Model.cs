using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ausgrid.Models
{
    public class PreList_Model
    {
        public string Reference { get; set; }
        [DataType(DataType.Date)]
        public DateTime RegDate { get; set; }
        public string SubsName { get; set; }
        public int PositionNo { get; set; }

        public int SampledBy { get; set; }
        public string ClientRef { get; set; }
        public string Status { get; set; }
    }
}
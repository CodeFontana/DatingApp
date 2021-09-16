using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public  class PhotoDownloadModel
    {
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}

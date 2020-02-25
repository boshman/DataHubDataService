using System;

namespace DataHubDataService.Models
{
    public class DataHubFile 
    {
        public int AgencyID { get; set; }
        public string Path { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Published { get; set; }
        public string ObjectType { get; set; } // File or Folder
        public string DisplayName { get; set; }

        public static int CompareFiles(DataHubFile a, DataHubFile b)
        {
            return a.Path.CompareTo(b.Path);
        }
    }
}
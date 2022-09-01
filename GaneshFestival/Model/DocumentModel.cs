namespace GaneshFestival.Model
{
    public class DocumentModel
    {
        public string FolderName { get; set; }
        public string DocumentType { get; set; }
        public IFormFile UploadDocPath { get; set; }

    }

    public class DocumentResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string DocumentType { get; set; }
        public object FilePath { get; set; }
       
    }

    public class DocumentResponse1
    {
       public string FolderName { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
       // public string DocumentType { get; set; }
        public object FilePath { get; set; }

    }
}

namespace Merit.BarCodeScanner.Models
{
    public class ImportRequest
    {
        public string PathFolder { get; set; }

        public string FileName { get; set; }

        public string FileNameNew { get; set; }

        public string PathFileTest { get; set; }

        public string PathFile
        {
            get { return string.IsNullOrEmpty(PathFileTest) ? string.Format("{0}\\{1}", PathFolder, FileName) : PathFileTest; }
        }
    }
}

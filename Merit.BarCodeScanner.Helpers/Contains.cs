namespace Merit.BarCodeScanner.Helpers
{
    public class Contains
    {
        public enum FileStatus
        {
            EXSIT,
            NOTEXSIT,
            SUCCESSFULL,
            UNSUCCESSFULL,
            EXTENSION,
            NOTEXTENSION
        }

        public enum ItemStatus
        {
            UNDEFINED,
            EMPLOYEE,
            PALLET,
            DELIVER,
            ENDBLOCK,
            DATAERROR,
            BLANK,
            DATETIMEERROR,
            FOMATERROR
        }

        public enum RowType
        {
            PALLET,
            DESTINATION,
            EMPLOYEE,
            BLANK,
            ENDBLOCK,
            UNDEFINED,
            EXCEPTION,
            FOMATERROR
        }
    }
}


namespace DoaMais.Application.Models
{
    public class ReportViewModel
    {
        public byte[] FileData { get; set; }

        public ReportViewModel(byte[] fileData)
        {
            FileData = fileData;
        }

        public static ReportViewModel FromEntity(byte[] fileData)
        {
            return new ReportViewModel(fileData);
        }
    }
}

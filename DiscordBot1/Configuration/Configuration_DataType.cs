using HeartFlame.Reporting;

namespace HeartFlame.Configuration
{
    public class Configuration_DataType
    {
        public string Token { get; set; }
        public string CommandPrefix { get; set; }
        public string Game { get; set; }
        public ReportingDataType Reporting { get; set; }

        public Configuration_DataType()
        {
            Reporting = new ReportingDataType();
        }
    }
}
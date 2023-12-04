using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json.Nodes;
using System.Data.SqlClient;

namespace TN8_to_MES
{
    public partial class Form1 : Form
    {
        public partial class ResultDataCV
        {
            public string currentResultIdCV = string.Empty;
            public string currentProgramversion = string.Empty;
            public string dev_id = "DIAP080";
            public string send_date = string.Empty;
            public string send_serial = string.Empty;
            public string data_size = "44";
            public string data_type = "QR";
            public string send_data = string.Empty;
            public string create_time = string.Empty;
            public string create_user = string.Empty;
            public string vin = string.Empty;
            public string torque = string.Empty;
            public string resultEvaluation = string.Empty;
        }

        public partial class ResultDataGH
        {
            public string currentResultIdGH = string.Empty;
            public string currentProgramversion = string.Empty;
            public string dev_id = "DIAP080";
            public string send_date = string.Empty;
            public string send_serial = string.Empty;
            public string data_size = "44";
            public string data_type = "QR";
            public string send_data = string.Empty;
            public string create_time = string.Empty;
            public string create_user = string.Empty;
            public string vin = string.Empty;
            public string torque = string.Empty;
            public string resultEvaluation = string.Empty;
        }

        int startmemory = 0;

        SqlConnection cnn;
        SqlCommand command;
        SqlDataReader reader;

        string connetionString = @"Data Source=127.0.0.1;Initial Catalog=API_Test;User ID=test;Password=test";

        string query = string.Empty;

        public partial class Data1
        {
            [JsonProperty("data")]
            public Data2[] Data { get; set; }

            [JsonProperty("paging")]
            public Paging Paging { get; set; }
        }

        public partial class Data2
        {
            [JsonProperty("dataModelTitle")]
            public string DataModelTitle { get; set; }

            [JsonProperty("dataModelVersion")]
            public string DataModelVersion { get; set; }

            [JsonProperty("resultMetaData")]
            public ResultMetaData ResultMetaData { get; set; }

            [JsonProperty("resultContent")]
            public ResultContent[] ResultContent { get; set; }
        }

        public partial class ResultContent
        {
            [JsonProperty("dataModelTitle")]
            public string DataModelTitle { get; set; }

            [JsonProperty("dataModelVersion")]
            public string DataModelVersion { get; set; }

            [JsonProperty("overallResultValues")]
            public OverallResultValue[] OverallResultValues { get; set; }
        }

        public partial class OverallResultValue
        {
            [JsonProperty("physicalQuantity")]
            public long PhysicalQuantity { get; set; }

            [JsonProperty("value")]
            public double Value { get; set; }

            [JsonProperty("resultEvaluation")]
            public string ResultEvaluation { get; set; }

            [JsonProperty("valueTag")]
            public long ValueTag { get; set; }

            [JsonProperty("lowLimit")]
            public double LowLimit { get; set; }

            [JsonProperty("highLimit")]
            public long HighLimit { get; set; }

            [JsonProperty("targetValue", NullValueHandling = NullValueHandling.Ignore)]
            public string TargetValue { get; set; }
        }

        public partial class ResultMetaData
        {
            [JsonProperty("sequenceNumber")]
            public long SequenceNumber { get; set; }

            [JsonProperty("tags")]
            public Tag[] Tags { get; set; }

            [JsonProperty("reporterAssetId")]
            public Guid ReporterAssetId { get; set; }

            [JsonProperty("generatorAssetId")]
            public Guid GeneratorAssetId { get; set; }

            [JsonProperty("programId")]
            public string ProgramId { get; set; }

            [JsonProperty("programVersionId")]
            public string programVersionId { get; set; }
            public long ProgramVersionId { get; set; }

            [JsonProperty("resultId")]
            public Guid ResultId { get; set; }

            [JsonProperty("creationTime")]
            public string CreationTime { get; set; }

            [JsonProperty("resultEvaluation")]
            public string ResultEvaluation { get; set; }

            [JsonProperty("resultEvaluationCode")]
            public long ResultEvaluationCode { get; set; }

            [JsonProperty("resultEvaluationDetails")]
            public ResultEvaluationDetails ResultEvaluationDetails { get; set; }

            [JsonProperty("acdc_JobStatus")]
            public long AcdcJobStatus { get; set; }
        }

        public partial class ResultEvaluationDetails
        {
            [JsonProperty("locale")]
            public string Locale { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }
        }

        public partial class Tag
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("tagType")]
            public long TagType { get; set; }
        }

        public partial class Paging
        {
            [JsonProperty("count")]
            public long Count { get; set; }

            [JsonProperty("next")]
            public Uri Next { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }



        private async void Start_btn_Click(object sender, EventArgs e)
        {
            timer1.Start();
            //startmemory = 1;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            textBox1.Text += "tick done\r\n";

            ResultDataGH resultDataGH = new ResultDataGH();
            ResultDataCV resultDataCV = new ResultDataCV();

            //============================= CV
            var httpClient = new HttpClient();
            var respCV = await httpClient.GetAsync("http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-2-1&limit=1");
            string strCV = await respCV.Content.ReadAsStringAsync();
            textBox1.Text += "P1 DONE\r\n";
            var dataCV = JsonConvert.DeserializeObject<Data1>(strCV);
            textBox1.Text += dataCV.Data[0].ResultMetaData.Tags[0].Value + "\r\n";
             
            //============================= GH
            var httpClient2 = new HttpClient();
            var respGH = await httpClient2.GetAsync("http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-1-1&limit=1");
            string strGH = await respGH.Content.ReadAsStringAsync();
            textBox1.Text += "P2 DONE\r\n";
            var dataGH = JsonConvert.DeserializeObject<Data1>(strGH);
            textBox1.Text += dataGH.Data[0].ResultMetaData.Tags[0].Value + "\r\n";


            //============================= GH
            resultDataGH.currentResultIdGH = dataGH.Data[0].ResultMetaData.ResultId.ToString();
            resultDataGH.currentProgramversion = dataGH.Data[0].ResultMetaData.programVersionId;

            resultDataGH.vin = dataGH.Data[0].ResultMetaData.Tags[0].Value;
            resultDataGH.send_date = dataGH.Data[0].ResultMetaData.CreationTime.Replace("-", string.Empty);
            resultDataGH.send_date = resultDataGH.send_date.Replace(":", string.Empty);
            resultDataGH.send_date = resultDataGH.send_date.Replace("T", string.Empty);
            resultDataGH.resultEvaluation = dataGH.Data[0].ResultContent[0].OverallResultValues[0].ResultEvaluation;
            resultDataGH.torque = dataGH.Data[0].ResultContent[0].OverallResultValues[0].TargetValue.ToString();

            resultDataGH.send_data = resultDataGH.vin +
                ";" + resultDataGH.send_date.Substring(0, 8) +
                ";" + resultDataGH.send_date.Substring(8, 6) +
                ";" + resultDataGH.dev_id +
                ";" + resultDataGH.resultEvaluation +
                ";" + resultDataGH.torque + ";";

            textBox1.Text += "\r\nGH:" + resultDataGH.send_data;


            //============================= CV
            resultDataCV.currentResultIdCV = dataCV.Data[0].ResultMetaData.ResultId.ToString();
            resultDataCV.currentProgramversion = dataCV.Data[0].ResultMetaData.programVersionId;

            resultDataCV.vin = dataCV.Data[0].ResultMetaData.Tags[0].Value;
            resultDataCV.send_date = dataCV.Data[0].ResultMetaData.CreationTime.Replace("-", string.Empty);
            resultDataCV.send_date = resultDataCV.send_date.Replace(":", string.Empty);
            resultDataCV.send_date = resultDataCV.send_date.Replace("T", string.Empty);
            resultDataCV.resultEvaluation = dataCV.Data[0].ResultContent[0].OverallResultValues[0].ResultEvaluation;
            resultDataCV.torque = dataCV.Data[0].ResultContent[0].OverallResultValues[0].TargetValue.ToString();

            resultDataCV.send_data = resultDataCV.vin +
                ";" + resultDataCV.send_date.Substring(0, 8) +
                ";" + resultDataCV.send_date.Substring(8, 6) +
                ";" + resultDataCV.dev_id +
                ";" + resultDataCV.resultEvaluation +
                ";" + resultDataCV.torque + ";";

            textBox1.Text += "\r\nCV:" + resultDataCV.send_data;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            startmemory = 0;
        }
    }
}
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json.Nodes;
using System.Data.SqlClient;

namespace TN8_to_MES
{
    public partial class Form1 : Form
    {
        string currentResultIdGH = string.Empty;
        string currentResultIdCV = string.Empty;
        string APIRequest = string.Empty;
        int startmemory = 0;

        SqlConnection cnn;
        SqlCommand command;
        SqlDataReader reader;

        string connetionString = @"Data Source=127.0.0.1;Initial Catalog=API_Test;User ID=test;Password=test";

        string query = "INSERT INTO [dbo].[Q_QUALITY_IF]\r\n           ([DEV_ID]\r\n           ,[SEND_DATE]\r\n           ,[SEND_SERIAL]\r\n           ,[DATA_SIZE]\r\n           ,[DATA_TYPE]\r\n           ,[SEND_DATA]\r\n           ,[CREATE_TIME]\r\n           ,[CREATE_USER])\r\n     VALUES\r\n           ('DIAP080'\r\n           ,'20231201113946'\r\n           ,'1'\r\n           ,'42'\r\n           ,'QR'\r\n           ,'GHF 546502;20231201;113923;DIAP080;OK;48.70;'\r\n           ,'20231201113946'\r\n           ,'GHF 546502')\r\n";

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
            public long? TargetValue { get; set; }
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
            public DateTimeOffset CreationTime { get; set; }

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



        private void Start_btn_Click(object sender, EventArgs e)
        {
            timer1.Start();
            startmemory = 1;
        }

        public async void API_Job()
        {
            //string fileName = @"C:\Users\a00542721\OneDrive - ONEVIRTUALOFFICE\Documents\my\Workspaces\ws-VSCommunity\TN8_to_MES_adapter\Notes\Result-GH.txt";
            //string jsontxt = File.ReadAllText(fileName);
            //var data = JsonConvert.DeserializeObject<Data1>(jsontxt);


        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            checkBox1.Checked = true;
            textBox1.Text = "T STOPPED";

            var httpClient = new HttpClient();
            var respCV = await httpClient.GetAsync("http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-2-1&limit=1");
            string strCV = await respCV.Content.ReadAsStringAsync();

            textBox1.Text = "P1 DONE";

            var httpClient2 = new HttpClient();
            var respGH = await httpClient2.GetAsync("http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-1-1&limit=1");
            string strGH = await respGH.Content.ReadAsStringAsync();

            var dataCV = JsonConvert.DeserializeObject<Data1>(strCV);
            var dataGH = JsonConvert.DeserializeObject<Data1>(strGH);

            textBox1.Text = "Creation Time: " + dataGH.Data[0].ResultMetaData.CreationTime.ToString() + "\r\n";
            textBox1.Text += "ResultId: " + dataGH.Data[0].ResultMetaData.ResultId.ToString() + "\r\n";

            textBox1.Text += "Creation Time: " + dataCV.Data[0].ResultMetaData.CreationTime.ToString() + "\r\n";
            textBox1.Text += "ResultId: " + dataCV.Data[0].ResultMetaData.ResultId.ToString() + "\r\n";

            currentResultIdGH = dataGH.Data[0].ResultMetaData.ResultId.ToString();
            currentResultIdCV = dataCV.Data[0].ResultMetaData.ResultId.ToString();

            if (startmemory == 1)
                timer1.Start();

            if (Start_btn.BackColor == Color.Red)
                Start_btn.BackColor = Color.White;
            else
                Start_btn.BackColor = Color.Red;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            startmemory = 0;
        }
    }
}
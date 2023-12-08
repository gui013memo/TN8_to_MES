using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json.Nodes;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static TN8_to_MES.Form1;
using static System.Windows.Forms.Design.AxImporter;
using EngineNumber_checker;

namespace TN8_to_MES
{
    public partial class Form1 : Form
    {
        public partial class ResultData
        {
            public string currentResultId = string.Empty;
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

        string connectionString = @"Data Source=10.127.56.238;Initial Catalog=ACDC_Test;User ID=ACDC_Test_app;Password=app";

        Logger logger = new Logger();

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
            logger.Log("TN8_to_ME-app Opened");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIsClosing);
        }

        private void FormIsClosing(object sender, FormClosingEventArgs e) //This is called when app is closed
        {
            logger.Log("TN8_to_ME-app Closed");
        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            if (GH_checkBox.Checked || CV_checkBox.Checked || JB_checkBox.Checked)
            {
                TimerCV.Start();
                TimerGH.Start();
                TimerJB.Start();
                startmemory = 1;
                logger.Log("TN8_to_MES-app started");
            }
        }

        private string CheckLastResultId(string resultId)
        {
            string output = string.Empty;

            string query = @"SELECT [CREATE_USER], [RESULT_ID]
                            FROM[ACDC_Test].[dbo].[RESULTID]
                            where RESULT_ID = '" + resultId + "'";

            cnn = new SqlConnection(connectionString);
            cnn.Open();

            command = new SqlCommand(query, cnn);
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                output = reader.GetString(1);
            }

            reader.Close();
            command.Dispose();
            cnn.Close();

            return output;
        }

        private void InsertOnResultIdTable(ResultData resultData)
        {
            try
            {
                string sql = "insert into [dbo].[RESULTID] (CREATE_USER,RESULT_ID,PROGRAM_VERSION) " +
                    "values (@CREATE_USER,@RESULT_ID,@PROGRAM_VERSION)";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CREATE_USER", resultData.vin);
                        cmd.Parameters.AddWithValue("@RESULT_ID", resultData.currentResultId);
                        cmd.Parameters.AddWithValue("@PROGRAM_VERSION", resultData.currentProgramversion);

                        string rowsAffected = cmd.ExecuteNonQuery().ToString();
                        logger.Log("[RESULTID] SQL Insert done, Rows affected: " +
                            rowsAffected + "\r\nVIN: " + resultData.vin +
                            "\r\nResultId: " + resultData.currentResultId +
                            "\r\n [RESULTID]TRANSACTION: " + cmd.CommandText + "\r\n");
                    }
                }
            }
            catch (SqlException ex)
            {
                string msg = "[RESULTID] Insert Error:";
                msg += ex.Message;
                textBox1.Text += "\r\n***** [RESULTID] SQL ERROR *****:\r\n" + msg + "\r\n";
                logger.Log("\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n" +
                    "\r\nVIN: " + resultData.vin +
                            "\r\nResultId: " + resultData.currentResultId);
            }
        }

        private async void TimerCV_Tick(object sender, EventArgs e)
        {
            if (!CV_checkBox.Checked)
                TimerCV.Stop();

            if (startmemory == 1 && CV_checkBox.Checked)
            {
                TimerCV.Stop();
                textBox1.Text += "Timer CV tick occurred \r\n";
                logger.Log("Timer CV tick occurred");

                TimerCV_btn.BackColor = Color.Green;

                ResultData ResultDataCV = new ResultData();

                string httpRequest = "http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-2-1&limit=1";
                var httpClientCV = new HttpClient();
                var respCV = await httpClientCV.GetAsync(httpRequest);
                string strCV = await respCV.Content.ReadAsStringAsync();

                if (strCV.Contains("error"))
                {
                    logger.Log("Result ran into a error: \r\n" +
                        "RAW JSON: " + strCV);
                    textBox1.Text = "Error on get results";
                    TimerCV.Start();
                }
                else
                {
                    var dataCV = JsonConvert.DeserializeObject<Data1>(strCV);
                    ResultDataCV.currentResultId = dataCV.Data[0].ResultMetaData.ResultId.ToString();
                    ResultDataCV.currentProgramversion = dataCV.Data[0].ResultMetaData.programVersionId;
                    ResultDataCV.vin = dataCV.Data[0].ResultMetaData.Tags[0].Value;

                    textBox1.Text += "CV Request done: " + httpRequest + "\r\nResultId: " + ResultDataCV.currentResultId
                        + "   VIN: " + dataCV.Data[0].ResultMetaData.Tags[0].Value;
                    logger.Log("CV Request done: " + httpRequest + "\r\nResultId: " + ResultDataCV.currentResultId
                        + "   VIN: " + dataCV.Data[0].ResultMetaData.Tags[0].Value);

                    string resultCompare = "";
                    resultCompare = CheckLastResultId(ResultDataCV.currentResultId);

                    if (ResultDataCV.currentResultId == resultCompare)
                    {
                        textBox1.Text += "\r\nResultId repeated (" + ResultDataCV.vin + "), tightening disregarded" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare;

                        logger.Log("ResultId REPEATED  (" + ResultDataCV.currentResultId +
                            "\r\n(VIN: " + dataCV.Data[0].ResultMetaData.Tags[0].Value + ")" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare + "tightening disregarded");
                    }
                    else
                    {
                        InsertOnResultIdTable(ResultDataCV);

                        textBox1.Text += "\r\nNEW ResultId to INSERT (" + ResultDataCV.currentResultId + ")" +
                            "\r\n(VIN: " + dataCV.Data[0].ResultMetaData.Tags[0].Value + ")";
                        logger.Log("NEW ResultId to INSERT (" + ResultDataCV.currentResultId + ")" +
                            "\r\n(VIN: " + dataCV.Data[0].ResultMetaData.Tags[0].Value + ")");

                        ResultDataCV.vin = dataCV.Data[0].ResultMetaData.Tags[0].Value;
                        ResultDataCV.send_date = dataCV.Data[0].ResultMetaData.CreationTime.Replace("-", string.Empty);
                        ResultDataCV.send_date = ResultDataCV.send_date.Replace(":", string.Empty);
                        ResultDataCV.send_date = ResultDataCV.send_date.Replace("T", string.Empty);
                        ResultDataCV.send_date = ResultDataCV.send_date.Substring(0, 14);
                        ResultDataCV.resultEvaluation = dataCV.Data[0].ResultContent[0].OverallResultValues[0].ResultEvaluation;
                        ResultDataCV.torque = dataCV.Data[0].ResultContent[0].OverallResultValues[0].Value.ToString();

                        ResultDataCV.send_data = ResultDataCV.vin +
                            ";" + ResultDataCV.send_date.Substring(0, 8) +
                            ";" + ResultDataCV.send_date.Substring(8, 6) +
                            ";" + ResultDataCV.dev_id +
                            ";" + ResultDataCV.resultEvaluation +
                            ";" + ResultDataCV.torque + ";";

                        textBox1.Text += "\r\n      CV send data: " + ResultDataCV.send_data + "\r\n";
                        textBox1.Text += "\r\n      CV send date: " + ResultDataCV.send_date + "\r\n";
                        logger.Log("CV SEND_DATA to SQL Server: \r\n" + ResultDataCV.send_data);

                        try
                        {
                            string sql = "insert into [dbo].[Q_QUALITY_IF] (DEV_ID,SEND_DATE,SEND_SERIAL,DATA_SIZE,DATA_TYPE," +
                                "SEND_DATA,CREATE_TIME,CREATE_USER,RESULT_ID,PROGRAM_VERSION) " +
                                "values (@DEV_ID,@SEND_DATE,@SEND_SERIAL,@DATA_SIZE,@DATA_TYPE,@SEND_DATA,@CREATE_TIME," +
                                "@CREATE_USER,@RESULT_ID,@PROGRAM_VERSION)";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@DEV_ID", "DIAP080");
                                    cmd.Parameters.AddWithValue("@SEND_DATE", ResultDataCV.send_date);
                                    cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                    cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                    cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                    cmd.Parameters.AddWithValue("@SEND_DATA", ResultDataCV.send_data);
                                    cmd.Parameters.AddWithValue("@CREATE_TIME", ResultDataCV.send_date);
                                    cmd.Parameters.AddWithValue("@CREATE_USER", ResultDataCV.vin);
                                    cmd.Parameters.AddWithValue("@RESULT_ID", ResultDataCV.currentResultId);
                                    cmd.Parameters.AddWithValue("@PROGRAM_VERSION", ResultDataCV.currentProgramversion);

                                    string rowsAffected = cmd.ExecuteNonQuery().ToString();
                                    textBox1.Text += "\r\n" + "[QUALITY_IF] SQL insert done, Rows affected: " + rowsAffected + "\r\n";
                                    logger.Log("SQL Insert done, Rows affected: " + rowsAffected
                                        + "\r\n TRANSACTION: " + cmd.CommandText + "\r\n");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            string msg = "Q_QUALITY_IF Insert Error:";
                            msg += ex.Message;
                            textBox1.Text += "\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n";
                            logger.Log("\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n");
                        }
                    }

                    textBox1.Text += "\r\nEnd of CV Timer tick\r\n---------------------\r\n";
                    logger.Log("End of CV Timer tick");
                    TimerCV_btn.BackColor = Color.White;

                    if (startmemory == 1 && CV_checkBox.Checked)
                        TimerCV.Start();
                }
            }
        }

        private async void TimerGH_Tick(object sender, EventArgs e)
        {
            if (!GH_checkBox.Checked)
                TimerGH.Stop();

            if (startmemory == 1 && GH_checkBox.Checked)
            {
                TimerGH.Stop();
                textBox1.Text += "Timer GH tick occurred \r\n";
                logger.Log("Timer GH tick occurred");

                TimerGH_btn.BackColor = Color.Green;

                ResultData ResultDataGH = new ResultData();

                string httpRequest = "http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-1-1&limit=1";
                var httpClientGH = new HttpClient();
                var respGH = await httpClientGH.GetAsync(httpRequest);
                string strGH = await respGH.Content.ReadAsStringAsync();

                if (strGH.Contains("error"))
                {
                    logger.Log("Result ran into a error: \r\n" +
                        "RAW JSON: " + strGH);
                    textBox1.Text = "Error on get results";
                    TimerGH.Start();
                }
                else
                {
                    var dataGH = JsonConvert.DeserializeObject<Data1>(strGH);

                    ResultDataGH.currentResultId = dataGH.Data[0].ResultMetaData.ResultId.ToString();
                    ResultDataGH.currentProgramversion = dataGH.Data[0].ResultMetaData.programVersionId;
                    ResultDataGH.vin = dataGH.Data[0].ResultMetaData.Tags[0].Value;

                    textBox1.Text += "GH Request done: " + httpRequest + "\r\nResultId: " + ResultDataGH.currentResultId
                        + "   VIN: " + dataGH.Data[0].ResultMetaData.Tags[0].Value;
                    logger.Log("GH Request done: " + httpRequest + "\r\nResultId: " + ResultDataGH.currentResultId
                        + "   VIN: " + dataGH.Data[0].ResultMetaData.Tags[0].Value);

                    string resultCompare = "";
                    resultCompare = CheckLastResultId(ResultDataGH.currentResultId);

                    if (ResultDataGH.currentResultId == resultCompare)
                    {
                        textBox1.Text += "\r\nResultId repeated (" + ResultDataGH.vin + "), tightening disregarded" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare;

                        logger.Log("ResultId repeated(" + ResultDataGH.currentResultId +
                            "\r\n(VIN: " + dataGH.Data[0].ResultMetaData.Tags[0].Value + ")" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare);
                    }
                    else
                    {
                        InsertOnResultIdTable(ResultDataGH);

                        textBox1.Text += "\r\nNEW ResultId to INSERT (" + ResultDataGH.currentResultId + ")" +
                            "\r\n(VIN: " + dataGH.Data[0].ResultMetaData.Tags[0].Value + ")";
                        logger.Log("NEW ResultId to INSERT (" + ResultDataGH.currentResultId + ")" +
                            "\r\n(VIN: " + dataGH.Data[0].ResultMetaData.Tags[0].Value + ")");

                        ResultDataGH.vin = dataGH.Data[0].ResultMetaData.Tags[0].Value;
                        ResultDataGH.send_date = dataGH.Data[0].ResultMetaData.CreationTime.Replace("-", string.Empty);
                        ResultDataGH.send_date = ResultDataGH.send_date.Replace(":", string.Empty);
                        ResultDataGH.send_date = ResultDataGH.send_date.Replace("T", string.Empty);
                        ResultDataGH.send_date = ResultDataGH.send_date.Substring(0, 14);
                        ResultDataGH.resultEvaluation = dataGH.Data[0].ResultContent[0].OverallResultValues[0].ResultEvaluation;
                        ResultDataGH.torque = dataGH.Data[0].ResultContent[0].OverallResultValues[0].Value.ToString();

                        ResultDataGH.send_data = ResultDataGH.vin +
                            ";" + ResultDataGH.send_date.Substring(0, 8) +
                            ";" + ResultDataGH.send_date.Substring(8, 6) +
                            ";" + ResultDataGH.dev_id +
                            ";" + ResultDataGH.resultEvaluation +
                            ";" + ResultDataGH.torque + ";";

                        textBox1.Text += "\r\n      GH send data: " + ResultDataGH.send_data + "\r\n";
                        textBox1.Text += "\r\n      GH send date: " + ResultDataGH.send_date + "\r\n";
                        logger.Log("GH SEND_DATA to SQL Server: \r\n" + ResultDataGH.send_data);

                        try
                        {
                            string sql = "insert into [dbo].[Q_QUALITY_IF] (DEV_ID,SEND_DATE,SEND_SERIAL,DATA_SIZE,DATA_TYPE," +
                                "SEND_DATA,CREATE_TIME,CREATE_USER,RESULT_ID,PROGRAM_VERSION) " +
                                "values (@DEV_ID,@SEND_DATE,@SEND_SERIAL,@DATA_SIZE,@DATA_TYPE,@SEND_DATA,@CREATE_TIME," +
                                "@CREATE_USER,@RESULT_ID,@PROGRAM_VERSION)";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@DEV_ID", "DIAP080");
                                    cmd.Parameters.AddWithValue("@SEND_DATE", ResultDataGH.send_date);
                                    cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                    cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                    cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                    cmd.Parameters.AddWithValue("@SEND_DATA", ResultDataGH.send_data);
                                    cmd.Parameters.AddWithValue("@CREATE_TIME", ResultDataGH.send_date);
                                    cmd.Parameters.AddWithValue("@CREATE_USER", ResultDataGH.vin);
                                    cmd.Parameters.AddWithValue("@RESULT_ID", ResultDataGH.currentResultId);
                                    cmd.Parameters.AddWithValue("@PROGRAM_VERSION", ResultDataGH.currentProgramversion);

                                    string rowsAffected = cmd.ExecuteNonQuery().ToString();
                                    textBox1.Text += "\r\n" + "[QUALITY_IF] SQL insert done, Rows affected: " + rowsAffected + "\r\n";
                                    logger.Log("SQL Insert done, Rows affected: " + rowsAffected
                                        + "\r\n TRANSACTION: " + cmd.CommandText + "\r\n");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            string msg = "Q_QUALITY_IF Insert Error:";
                            msg += ex.Message;
                            textBox1.Text += "\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n";
                            logger.Log("\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n");
                        }
                    }

                    textBox1.Text += "\r\nEnd of GH Timer tick\r\n---------------------\r\n";
                    logger.Log("End of GH Timer tick");
                    TimerGH_btn.BackColor = Color.White;

                    if (startmemory == 1 && GH_checkBox.Checked)
                        TimerGH.Start();
                }
            }
        }

        private async void TimerJB_Tick(object sender, EventArgs e)
        {
            if (!JB_checkBox.Checked)
                TimerJB.Stop();

            if (startmemory == 1 && JB_checkBox.Checked)
            {
                TimerJB.Stop();
                textBox1.Text += "Timer JB tick occurred \r\n";
                logger.Log("Timer JB tick occurred");

                TimerGH_btn.BackColor = Color.Green;

                ResultData ResultDataJB = new ResultData();

                string httpRequest = "http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-3-1&limit=1";
                var httpClientJB = new HttpClient();
                var respJB = await httpClientJB.GetAsync(httpRequest);
                string strJB = await respJB.Content.ReadAsStringAsync();

                if (strJB.Contains("error"))
                {
                    logger.Log("Result ran into a error: \r\n" +
                        "RAW JSON: " + strJB);
                    textBox1.Text = "Error on get results";
                    TimerJB.Start();
                }
                else
                {
                    var dataJB = JsonConvert.DeserializeObject<Data1>(strJB);

                    ResultDataJB.currentResultId = dataJB.Data[0].ResultMetaData.ResultId.ToString();
                    ResultDataJB.currentProgramversion = dataJB.Data[0].ResultMetaData.programVersionId;
                    ResultDataJB.vin = dataJB.Data[0].ResultMetaData.Tags[0].Value;

                    textBox1.Text += "JB Request done: " + httpRequest + "\r\nResultId: " + ResultDataJB.currentResultId
                        + "   VIN: " + dataJB.Data[0].ResultMetaData.Tags[0].Value;
                    logger.Log("JB Request done: " + httpRequest + "\r\nResultId: " + ResultDataJB.currentResultId
                        + "   VIN: " + dataJB.Data[0].ResultMetaData.Tags[0].Value);

                    string resultCompare = "";
                    resultCompare = CheckLastResultId(ResultDataJB.currentResultId);

                    if (ResultDataJB.currentResultId == resultCompare)
                    {
                        textBox1.Text += "\r\nResultId repeated (" + ResultDataJB.vin + "), tiJBtening disregarded" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare;

                        logger.Log("ResultId repeated(" + ResultDataJB.currentResultId +
                            "\r\n(VIN: " + dataJB.Data[0].ResultMetaData.Tags[0].Value + ")" +
                            "\r\nResultId on [RESULTID]Table: " + resultCompare);
                    }
                    else
                    {
                        InsertOnResultIdTable(ResultDataJB);

                        textBox1.Text += "\r\nNEW ResultId to INSERT (" + ResultDataJB.currentResultId + ")" +
                            "\r\n(VIN: " + dataJB.Data[0].ResultMetaData.Tags[0].Value + ")";
                        logger.Log("NEW ResultId to INSERT (" + ResultDataJB.currentResultId + ")" +
                            "\r\n(VIN: " + dataJB.Data[0].ResultMetaData.Tags[0].Value + ")");

                        ResultDataJB.vin = dataJB.Data[0].ResultMetaData.Tags[0].Value;
                        ResultDataJB.send_date = dataJB.Data[0].ResultMetaData.CreationTime.Replace("-", string.Empty);
                        ResultDataJB.send_date = ResultDataJB.send_date.Replace(":", string.Empty);
                        ResultDataJB.send_date = ResultDataJB.send_date.Replace("T", string.Empty);
                        ResultDataJB.send_date = ResultDataJB.send_date.Substring(0, 14);
                        ResultDataJB.resultEvaluation = dataJB.Data[0].ResultContent[0].OverallResultValues[0].ResultEvaluation;
                        ResultDataJB.torque = dataJB.Data[0].ResultContent[0].OverallResultValues[0].Value.ToString();

                        ResultDataJB.send_data = ResultDataJB.vin +
                            ";" + ResultDataJB.send_date.Substring(0, 8) +
                            ";" + ResultDataJB.send_date.Substring(8, 6) +
                            ";" + ResultDataJB.dev_id +
                            ";" + ResultDataJB.resultEvaluation +
                            ";" + ResultDataJB.torque + ";";

                        textBox1.Text += "\r\n      JB send data: " + ResultDataJB.send_data + "\r\n";
                        textBox1.Text += "\r\n      JB send date: " + ResultDataJB.send_date + "\r\n";
                        logger.Log("JB SEND_DATA to SQL Server: \r\n" + ResultDataJB.send_data);

                        try
                        {
                            string sql = "insert into [dbo].[Q_QUALITY_IF] (DEV_ID,SEND_DATE,SEND_SERIAL,DATA_SIZE,DATA_TYPE," +
                                "SEND_DATA,CREATE_TIME,CREATE_USER,RESULT_ID,PROGRAM_VERSION) " +
                                "values (@DEV_ID,@SEND_DATE,@SEND_SERIAL,@DATA_SIZE,@DATA_TYPE,@SEND_DATA,@CREATE_TIME," +
                                "@CREATE_USER,@RESULT_ID,@PROGRAM_VERSION)";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@DEV_ID", "DIAP080");
                                    cmd.Parameters.AddWithValue("@SEND_DATE", ResultDataJB.send_date);
                                    cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                    cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                    cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                    cmd.Parameters.AddWithValue("@SEND_DATA", ResultDataJB.send_data);
                                    cmd.Parameters.AddWithValue("@CREATE_TIME", ResultDataJB.send_date);
                                    cmd.Parameters.AddWithValue("@CREATE_USER", ResultDataJB.vin);
                                    cmd.Parameters.AddWithValue("@RESULT_ID", ResultDataJB.currentResultId);
                                    cmd.Parameters.AddWithValue("@PROGRAM_VERSION", ResultDataJB.currentProgramversion);

                                    string rowsAffected = cmd.ExecuteNonQuery().ToString();
                                    textBox1.Text += "\r\n" + "[QUALITY_IF] SQL insert done, Rows affected: " + rowsAffected + "\r\n";
                                    logger.Log("SQL Insert done, Rows affected: " + rowsAffected
                                        + "\r\n TRANSACTION: " + cmd.CommandText + "\r\n");
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            string msg = "Q_QUALITY_IF Insert Error:";
                            msg += ex.Message;
                            textBox1.Text += "\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n";
                            logger.Log("\r\n***** SQL ERROR *****:\r\n" + msg + "\r\n");
                        }
                    }

                    textBox1.Text += "\r\nEnd of JB Timer tick\r\n---------------------\r\n";
                    logger.Log("End of JB Timer tick");
                    TimerJB_btn.BackColor = Color.White;

                    if (startmemory == 1 && JB_checkBox.Checked)
                        TimerJB.Start();
                }
                
            }
        }

        private void Stop_btn_Click(object sender, EventArgs e)
        {
            startmemory = 0;
            TimerCV.Stop();
            TimerGH.Stop();
            TimerJB.Stop();
            textBox1.Text += "App stopped";
            logger.Log("TN8_to_MES-App stopped");
        }

        private void TimerCV_btn_Click(object sender, EventArgs e)
        {
            //TimerCV.Start();
        }

        private void TimerGH_btn_Click(object sender, EventArgs e)
        {
            //TimerGH.Start();
        }

        private void TimerJB_btn_Click(object sender, EventArgs e)
        {
            //TimerJB.Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 2500)
                textBox1.Text = "";
        }
    }
}
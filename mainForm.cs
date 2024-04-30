using Newtonsoft.Json;
using System.Data.SqlClient;

namespace TN8_to_MES
{
    public partial class mainForm : Form
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

        int startMemory = 0;

        int ghTimerWaiter = 0;
        int cvTimerWaiter = 0;
        int jbTimerWaiter = 0;

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

            [JsonProperty("Errors", NullValueHandling = NullValueHandling.Ignore)]
            public Errors[] Errors { get; set; }
        }

        public partial class Errors
        {
            [JsonProperty("errorType")]
            public long ErrorType { get; set; }

            [JsonProperty("errorId")]
            //[JsonConverter(typeof(ParseStringConverter))]
            public long ErrorId { get; set; }

            [JsonProperty("errorMessage")]
            public ResultEvaluationDetails ErrorMessage { get; set; }
        }

        public partial class ErrorMessage
        {
            [JsonProperty("locale")]
            public long locale { get; set; }

            [JsonProperty("text")]
            public string text { get; set; }
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

            [JsonProperty("violationType", NullValueHandling = NullValueHandling.Ignore)]
            public long? ViolationType { get; set; }
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

        public mainForm()
        {
            InitializeComponent();
            logger.Log("TN8_to_ME-app Opened");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormIsClosing);
        }

        private void FormIsClosing(object sender, FormClosingEventArgs e) //This is called when app is closed
        {
            logger.Log("TN8_to_ME-app Closed");
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (ghCheckBox.Checked || cvCheckBox.Checked || jbCheckBox.Checked)
            {
                logger.Log("TN8_to_MES-app started");

                startMemory = 1;

                timerCV.Start();
                timerGH.Start();
                timerJB.Start();

                //TimerJB_Tick(null, null);
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            startMemory = 0;
            timerCV.Stop();
            timerGH.Stop();
            timerJB.Stop();

            consoleTextBox.Text += "App stopped";
            logger.Log("TN8_to_MES-App stopped");
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
                        logger.Log("Inserted (" + rowsAffected + ") rows on [RESULTID] table: " +
                                "\r\nVIN: " + resultData.vin +
                                "\r\nResultId: " + resultData.currentResultId +
                                "\r\nProgramVersion: " + resultData.currentProgramversion);
                    }
                }
            }
            catch (SqlException ex)
            {
                logger.Log("[RESULTID] Insert Error:" +
                       "\r\nVIN: " + resultData.vin +
                       "\r\nResultId: " + resultData.currentResultId +
                       "\r\nProgramVersion: " + resultData.currentProgramversion +
                       "\r\nEXCEPTION MESSAGE:" +
                       "\r\n" + ex.Message);

                consoleTextBox.Text += "@@Error inserting on [RESULTID]";
            }
        }

        private async void GetUnifyResults (string httpRequest, System.Windows.Forms.Timer timer)
        {
            //public async Task<bool> doAsyncOperation()
            //{
            //    // do work
            //    return true;
            //}

            //bool result = await doAsyncOperation();

            ResultData[] ResultData = new ResultData[10];

            var httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage;
            string httpResponseMessageContent;

            try
            {
                httpResponseMessage = await httpClient.GetAsync(httpRequest);
                httpResponseMessageContent = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception exc)
            {
                logger.Log("Error on http request: " + exc.ToString() + "\r\n exiting from TimerJB_Tick function... \r\n");

                jbTimerWaiter = 4;
                timer.Start();
                return;
            }

            //LOCAL TEST CODE
            //{
            // string str = File.ReadAllText(@"C:\Users\gui01\OneDrive\Documents\Drive\Drive\Workspaces\ws-VS2022\C#\TN8_to_MES-master\TN8_to_MES\Req-responses\t1.json", Encoding.UTF8);
            //string httpRequest = "local TEST-NO-REQ-DONE";
            //}
            //LOCAL TEST CODE


            /*----------------------------*/

            if (httpResponseMessageContent.Contains(@"""title"": ""An error has occurred."""))
            {
                logger.Log("Request with error on response: \r\n" +
                    "RAW JSON: " + httpResponseMessageContent);
                consoleTextBox.Text = "Error on get results from JB";

                jbTimerWaiter = 4;
                timerJB.Start();
                return;
            }
            else
            {
                var jsonData = JsonConvert.DeserializeObject<Data1>(httpResponseMessageContent);

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        ResultData[i] = new ResultData();

                        ResultData[i].currentResultId = jsonData.Data[i].ResultMetaData.ResultId.ToString();
                        ResultData[i].currentProgramversion = jsonData.Data[i].ResultMetaData.programVersionId.ToString();
                        ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                        logger.Log("JB Request done: " + httpRequest +
                        "\r\nResultId: " + ResultData[i].currentResultId +
                        "\r\nVIN: " + ResultData[i].vin +
                        "\r\nProgramVersion: " + ResultData[i].currentProgramversion);
                    }
                    catch (Exception exc)
                    {
                        logger.Log("@@ ERROR @@ ON DATA TRANSFER BETWEEN JSON DATA AND RESULTDATA[:" + i.ToString() + "\r\n" +
                                "RAW JSON: " + httpResponseMessageContent + "\r\nException message: \r\n" + exc.Message + "\r\n exiting from TimerJB_Tick function... \r\n");
                        //TimerJB.Start();
                        //return;
                    }

                }

                for (int i = 0; i < 10; i++)
                {
                    string resultCompare = CheckLastResultId(ResultData[i].currentResultId);

                    if (ResultData[i].currentResultId == resultCompare)
                    {
                        // Repeated result
                        logger.Log("COMPARATION EVALUATION: --REPEATED RESULT ON TAG: " + ResultData[i].vin +
                                "\r\nResultId from http request: " + ResultData[i].currentResultId +
                                "\r\nResultId on [RESULTID] table: " + resultCompare);

                        consoleTextBox.Text += "\r\n--REPEATED RESULT ON TAG: " + ResultData[i].vin;
                    }
                    else
                    {
                        // New result to insert
                        logger.Log("COMPARATION EVALUATION: ++NEW RESULT ON TAG: " + ResultData[i].vin +
                                "\r\nResultId from http request: " + ResultData[i].vin +
                                "\r\nResultId on [RESULTID] table: " + resultCompare);

                        consoleTextBox.Text += "\r\n++NEW RESULT ON TAG: " + ResultData[i].vin;

                        InsertOnResultIdTable(ResultData[i]);

                        // #START of formatting for MES format -------------
                        {
                            ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                            //
                            ResultData[i].send_date = jsonData.Data[i].ResultMetaData.CreationTime.Replace("-", string.Empty);
                            ResultData[i].send_date = ResultData[i].send_date.Replace(":", string.Empty);
                            ResultData[i].send_date = ResultData[i].send_date.Replace("T", string.Empty);
                            ResultData[i].send_date = ResultData[i].send_date.Substring(0, 14);

                            // This ternary validation is necessary because when a result is NOK the string on ResultEvaluation is: NotOK (It must be "OK" or "NG")
                            ResultData[i].resultEvaluation = jsonData.Data[i].ResultMetaData.ResultEvaluation == "OK" ? "OK" : "NG";

                            ResultData[i].torque = jsonData.Data[i].ResultContent[0].OverallResultValues[0].Value.ToString();
                            ResultData[i].torque = ResultData[i].torque.Replace(".", ",");

                            ResultData[i].send_data = ResultData[i].vin +
                                ";" + ResultData[i].send_date.Substring(0, 8) +
                                ";" + ResultData[i].send_date.Substring(8, 6) +
                                ";" + ResultData[i].dev_id +
                                ";" + ResultData[i].resultEvaluation +
                                ";" + ResultData[i].torque + ";";
                        }
                        // #END of formatting for MES format


                        // Inserting on Q_QUALITY_IF -------------
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
                                    cmd.Parameters.AddWithValue("@SEND_DATE", ResultData[i].send_date);
                                    cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                    cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                    cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                    cmd.Parameters.AddWithValue("@SEND_DATA", ResultData[i].send_data);
                                    cmd.Parameters.AddWithValue("@CREATE_TIME", ResultData[i].send_date);
                                    cmd.Parameters.AddWithValue("@CREATE_USER", ResultData[i].vin);
                                    cmd.Parameters.AddWithValue("@RESULT_ID", ResultData[i].currentResultId);
                                    cmd.Parameters.AddWithValue("@PROGRAM_VERSION", "");

                                    string rowsAffected = cmd.ExecuteNonQuery().ToString();

                                    logger.Log("Inserted (" + rowsAffected + ") rows on [Q_QUALITY_IF] table: " +
                                            "\r\nVIN: " + ResultData[i].vin +
                                            "\r\nResultId: " + ResultData[i].currentResultId +
                                            "\r\nSendData: " + ResultData[i].send_data);

                                    consoleTextBox.Text += "\r\nInserted on [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "(" + ResultData[i].resultEvaluation + ")";
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            logger.Log("[Q_QUALITY_IF] Insert Error:" +
                                   "\r\nVIN: " + ResultData[i].vin +
                                   "\r\nResultId: " + ResultData[i].currentResultId +
                                   "\r\nSendData: " + ResultData[i].send_data +
                                   "\r\nEXCEPTION MESSAGE:" +
                                   "\r\n" + ex.Message);

                            consoleTextBox.Text += "@@ ERROR @@ INSERTING ON [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "\r\n exiting from TimerJB_Tick function... \r\n";
                            //TimerJB.Start();
                            //return; 
                        }
                    }
                }

                consoleTextBox.Text += "\r\nEnd of JB Timer tick\r\n---------------------\r\n";
                logger.Log("End of JB Timer tick");
                timerJBButton.BackColor = Color.White;

                if (startMemory == 1 && jbCheckBox.Checked)
                    timerJB.Start();
            }
        }

        private async void TimerCV_Tick(object sender, EventArgs e)
        {
            if (cvTimerWaiter > 0)
            {
                consoleTextBox.Text += "\r\n CV Waiter: " + cvTimerWaiter.ToString();
                logger.Log("CV Waiter:" + cvTimerWaiter.ToString());
                cvTimerWaiter--;
                return;
            }

            if (startMemory == 1)
            {
                timerCV.Stop();
                consoleTextBox.Text += "Timer CV tick occurred \r\n";
                logger.Log("Timer CV tick occurred");

                timerCVButton.BackColor = Color.Green;

                ResultData[] ResultData = new ResultData[10];

                //Current quantity of results = 3 <-------
                string httpRequest = "http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-2-1&limit=10";
                var httpClient = new HttpClient();
                HttpResponseMessage resp;
                string str;

                try
                {
                    resp = await httpClient.GetAsync(httpRequest);
                    str = await resp.Content.ReadAsStringAsync();
                }
                catch (Exception exc)
                {
                    logger.Log("Error on http request: " + exc.ToString() + "\r\n exiting from TimerCV_Tick function... \r\n");

                    cvTimerWaiter = 4;
                    timerCV.Start();
                    return;
                }



                //LOCAL TEST CODE
                {
                    //string str = File.ReadAllText(@"C:\Users\gui01\OneDrive\Documents\Drive\Drive\Workspaces\ws-VS2022\C#\TN8_to_MES-master\TN8_to_MES\Req-responses\1OK_1NOK.json", Encoding.UTF8);
                    //string httpRequest = "local TEST-NO-REQ-DONE";
                }
                //LOCAL TEST CODE

                /*--------------------------------*/

                if (str.Contains(@"""title"": ""An error has occurred."""))
                {
                    logger.Log("Request with error on response: \r\n" +
                        "RAW JSON: " + str);
                    consoleTextBox.Text = "Error on get results from CV";

                    cvTimerWaiter = 4;
                    timerCV.Start();
                    return;
                }
                else
                {
                    var jsonData = JsonConvert.DeserializeObject<Data1>(str);

                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            ResultData[i] = new ResultData();

                            ResultData[i].currentResultId = jsonData.Data[i].ResultMetaData.ResultId.ToString();
                            ResultData[i].currentProgramversion = jsonData.Data[i].ResultMetaData.programVersionId.ToString();
                            ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                            logger.Log("CV Request done: " + httpRequest +
                            "\r\nResultId: " + ResultData[i].currentResultId +
                            "\r\nVIN: " + ResultData[i].vin +
                            "\r\nProgramVersion: " + ResultData[i].currentProgramversion);
                        }
                        catch (Exception exc)
                        {
                            logger.Log("@@ ERROR @@ ON DATA TRANSFER BETWEEN JSON DATA AND RESULTDATA[:" + i.ToString() + "\r\n" +
                                    "RAW JSON: " + str + "\r\nException message: \r\n" + exc.Message + "\r\n exiting from TimerCV_Tick function... \r\n");
                            //TimerCV.Start();
                            //return;
                        }

                    }

                    for (int i = 0; i < 10; i++)
                    {
                        string resultCompare = CheckLastResultId(ResultData[i].currentResultId);

                        if (ResultData[i].currentResultId == resultCompare)
                        {
                            // Repeated result
                            logger.Log("COMPARATION EVALUATION: --REPEATED RESULT ON TAG: " + ResultData[i].vin +
                                    "\r\nResultId from http request: " + ResultData[i].currentResultId +
                                    "\r\nResultId on [RESULTID] table: " + resultCompare);

                            consoleTextBox.Text += "\r\n--REPEATED RESULT ON TAG: " + ResultData[i].vin;
                        }
                        else
                        {
                            // New result to insert
                            logger.Log("COMPARATION EVALUATION: ++NEW RESULT ON TAG: " + ResultData[i].vin +
                                    "\r\nResultId from http request: " + ResultData[i].vin +
                                    "\r\nResultId on [RESULTID] table: " + resultCompare);

                            consoleTextBox.Text += "\r\n++NEW RESULT ON TAG: " + ResultData[i].vin;

                            InsertOnResultIdTable(ResultData[i]);

                            // #START of formatting for MES format -------------
                            {
                                ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                                //
                                ResultData[i].send_date = jsonData.Data[i].ResultMetaData.CreationTime.Replace("-", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date.Replace(":", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date.Replace("T", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date[..14];

                                // This ternary validation is necessary because when a result is NOK the string on ResultEvaluation is: NotOK (It must be "OK" or "NG")
                                ResultData[i].resultEvaluation = jsonData.Data[i].ResultMetaData.ResultEvaluation == "OK" ? "OK" : "NG";

                                ResultData[i].torque = jsonData.Data[i].ResultContent[0].OverallResultValues[0].Value.ToString();
                                ResultData[i].torque = ResultData[i].torque.Replace(".", ",");

                                ResultData[i].send_data = ResultData[i].vin +
                                    ";" + ResultData[i].send_date.Substring(0, 8) +
                                    ";" + ResultData[i].send_date.Substring(8, 6) +
                                    ";" + ResultData[i].dev_id +
                                    ";" + ResultData[i].resultEvaluation +
                                    ";" + ResultData[i].torque + ";";
                            }
                            // #END of formatting for MES format


                            // Inserting on Q_QUALITY_IF -------------
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
                                        cmd.Parameters.AddWithValue("@SEND_DATE", ResultData[i].send_date);
                                        cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                        cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                        cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                        cmd.Parameters.AddWithValue("@SEND_DATA", ResultData[i].send_data);
                                        cmd.Parameters.AddWithValue("@CREATE_TIME", ResultData[i].send_date);
                                        cmd.Parameters.AddWithValue("@CREATE_USER", ResultData[i].vin);
                                        cmd.Parameters.AddWithValue("@RESULT_ID", ResultData[i].currentResultId);
                                        cmd.Parameters.AddWithValue("@PROGRAM_VERSION", "");

                                        string rowsAffected = cmd.ExecuteNonQuery().ToString();

                                        logger.Log("Inserted (" + rowsAffected + ") rows on [Q_QUALITY_IF] table: " +
                                                "\r\nVIN: " + ResultData[i].vin +
                                                "\r\nResultId: " + ResultData[i].currentResultId +
                                                "\r\nSendData: " + ResultData[i].send_data);

                                        consoleTextBox.Text += "\r\nInserted on [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "(" + ResultData[i].resultEvaluation + ")";
                                    }
                                }
                            }
                            catch (SqlException ex)
                            {
                                logger.Log("[Q_QUALITY_IF] Insert Error:" +
                                       "\r\nVIN: " + ResultData[i].vin +
                                       "\r\nResultId: " + ResultData[i].currentResultId +
                                       "\r\nSendData: " + ResultData[i].send_data +
                                       "\r\nEXCEPTION MESSAGE:" +
                                       "\r\n" + ex.Message);

                                consoleTextBox.Text += "@@ ERROR @@ INSERTING ON [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "\r\n exiting from TimerCV_Tick function... \r\n";
                                //TimerCV.Start();
                                //return;
                            }
                        }
                    }

                    consoleTextBox.Text += "\r\nEnd of CV Timer tick\r\n---------------------\r\n";
                    logger.Log("End of CV Timer tick");
                    timerCVButton.BackColor = Color.White;

                    if (startMemory == 1 && cvCheckBox.Checked)
                        timerCV.Start();
                }
            }
        }

        private async void TimerGH_Tick(object sender, EventArgs e)
        {
            if (ghTimerWaiter > 0)
            {
                consoleTextBox.Text += "\r\n GH Waiter: " + ghTimerWaiter.ToString();
                logger.Log("GH Waiter:" + ghTimerWaiter.ToString());
                ghTimerWaiter--;
                return;
            }

            if (startMemory == 1 && ghCheckBox.Checked)
            {
                timerGH.Stop();
                consoleTextBox.Text += "Timer GH tick occurred \r\n";
                logger.Log("Timer GH tick occurred");

                timerGHButton.BackColor = Color.Green;

                ResultData[] ResultData = new ResultData[10];

                //Current quantity of results = 3 <-------
                string httpRequest = "http://127.0.0.1:7110/api/v3/results/tightening?programId=0050D604FB07-1-1&limit=10";
                var httpClient = new HttpClient();
                HttpResponseMessage resp;
                string str;

                try
                {
                    resp = await httpClient.GetAsync(httpRequest);
                    str = await resp.Content.ReadAsStringAsync();
                }
                catch (Exception exc)
                {
                    logger.Log("Error on http request: " + exc.ToString() + "\r\n exiting from TimerGH_Tick function... \r\n");

                    ghTimerWaiter = 4;
                    timerGH.Start();
                    return;
                }



                //LOCAL TEST CODE
                {
                    //string str = File.ReadAllText(@"C:\Users\gui01\OneDrive\Documents\Drive\Drive\Workspaces\ws-VS2022\C#\TN8_to_MES-master\TN8_to_MES\Req-responses\1OK_1NOK.json", Encoding.UTF8);
                    //string httpRequest = "local TEST-NO-REQ-DONE";
                }
                //LOCAL TEST CODE



                if (str.Contains(@"""title"": ""An error has occurred."""))
                {
                    logger.Log("Request with error on response: \r\n" +
                        "RAW JSON: " + str);
                    consoleTextBox.Text = "Error on get results from GH";

                    ghTimerWaiter = 4;
                    timerGH.Start();
                    return;
                }
                else
                {
                    var jsonData = JsonConvert.DeserializeObject<Data1>(str);

                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            ResultData[i] = new ResultData();

                            ResultData[i].currentResultId = jsonData.Data[i].ResultMetaData.ResultId.ToString();
                            ResultData[i].currentProgramversion = jsonData.Data[i].ResultMetaData.programVersionId.ToString();
                            ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                            logger.Log("GH Request done: " + httpRequest +
                            "\r\nResultId: " + ResultData[i].currentResultId +
                            "\r\nVIN: " + ResultData[i].vin +
                            "\r\nProgramVersion: " + ResultData[i].currentProgramversion);
                        }
                        catch (Exception exc)
                        {
                            logger.Log("@@ ERROR @@ ON DATA TRANSFER BETWEEN JSON DATA AND RESULTDATA[:" + i.ToString() + "\r\n" +
                                    "RAW JSON: " + str + "\r\nException message: \r\n" + exc.Message + "\r\n exiting from TimerGH_Tick function... \r\n");

                            ghTimerWaiter = 4;
                            timerGH.Start();
                            return;
                        }

                    }

                    for (int i = 0; i < 10; i++)
                    {
                        string resultCompare = CheckLastResultId(ResultData[i].currentResultId);

                        if (ResultData[i].currentResultId == resultCompare)
                        {
                            // Repeated result
                            logger.Log("COMPARATION EVALUATION: --REPEATED RESULT ON TAG: " + ResultData[i].vin +
                                    "\r\nResultId from http request: " + ResultData[i].currentResultId +
                                    "\r\nResultId on [RESULTID] table: " + resultCompare);

                            consoleTextBox.Text += "\r\n--REPEATED RESULT ON TAG: " + ResultData[i].vin;
                        }
                        else
                        {
                            // New result to insert
                            logger.Log("COMPARATION EVALUATION: ++NEW RESULT ON TAG: " + ResultData[i].vin +
                                    "\r\nResultId from http request: " + ResultData[i].vin +
                                    "\r\nResultId on [RESULTID] table: " + resultCompare);

                            consoleTextBox.Text += "\r\n++NEW RESULT ON TAG: " + ResultData[i].vin;

                            InsertOnResultIdTable(ResultData[i]);

                            // #START of formatting for MES format -------------
                            {
                                ResultData[i].vin = jsonData.Data[i].ResultMetaData.Tags[0].Value;

                                //
                                ResultData[i].send_date = jsonData.Data[i].ResultMetaData.CreationTime.Replace("-", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date.Replace(":", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date.Replace("T", string.Empty);
                                ResultData[i].send_date = ResultData[i].send_date.Substring(0, 14);

                                // This ternary validation is necessary because when a result is NOK the string on ResultEvaluation is: NotOK (It must be "OK" or "NG")
                                ResultData[i].resultEvaluation = jsonData.Data[i].ResultMetaData.ResultEvaluation == "OK" ? "OK" : "NG";

                                ResultData[i].torque = jsonData.Data[i].ResultContent[0].OverallResultValues[0].Value.ToString();
                                ResultData[i].torque = ResultData[i].torque.Replace(".", ",");

                                ResultData[i].send_data = ResultData[i].vin +
                                    ";" + ResultData[i].send_date.Substring(0, 8) +
                                    ";" + ResultData[i].send_date.Substring(8, 6) +
                                    ";" + ResultData[i].dev_id +
                                    ";" + ResultData[i].resultEvaluation +
                                    ";" + ResultData[i].torque + ";";
                            }
                            // #END of formatting for MES format


                            // Inserting on Q_QUALITY_IF -------------
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
                                        cmd.Parameters.AddWithValue("@SEND_DATE", ResultData[i].send_date);
                                        cmd.Parameters.AddWithValue("@SEND_SERIAL", "1");
                                        cmd.Parameters.AddWithValue("@DATA_SIZE", "44");
                                        cmd.Parameters.AddWithValue("@DATA_TYPE", "QR");
                                        cmd.Parameters.AddWithValue("@SEND_DATA", ResultData[i].send_data);
                                        cmd.Parameters.AddWithValue("@CREATE_TIME", ResultData[i].send_date);
                                        cmd.Parameters.AddWithValue("@CREATE_USER", ResultData[i].vin);
                                        cmd.Parameters.AddWithValue("@RESULT_ID", ResultData[i].currentResultId);
                                        cmd.Parameters.AddWithValue("@PROGRAM_VERSION", "");

                                        string rowsAffected = cmd.ExecuteNonQuery().ToString();

                                        logger.Log("Inserted (" + rowsAffected + ") rows on [Q_QUALITY_IF] table: " +
                                                "\r\nVIN: " + ResultData[i].vin +
                                                "\r\nResultId: " + ResultData[i].currentResultId +
                                                "\r\nSendData: " + ResultData[i].send_data);

                                        consoleTextBox.Text += "\r\nInserted on [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "(" + ResultData[i].resultEvaluation + ")";
                                    }
                                }
                            }
                            catch (SqlException ex)
                            {
                                logger.Log("[Q_QUALITY_IF] Insert Error:" +
                                       "\r\nVIN: " + ResultData[i].vin +
                                       "\r\nResultId: " + ResultData[i].currentResultId +
                                       "\r\nSendData: " + ResultData[i].send_data +
                                       "\r\nEXCEPTION MESSAGE:" +
                                       "\r\n" + ex.Message);

                                consoleTextBox.Text += "@@ ERROR @@ INSERTING ON [Q_QUALITY_IF] TAG: " + ResultData[i].vin + "\r\n exiting from TimerGH_Tick function... \r\n";
                                //TimerGH.Start();
                                //return;
                            }
                        }
                    }

                    consoleTextBox.Text += "\r\nEnd of GH Timer tick\r\n---------------------\r\n";
                    logger.Log("End of GH Timer tick");
                    timerGHButton.BackColor = Color.White;

                    if (startMemory == 1 && ghCheckBox.Checked)
                        timerGH.Start();
                }
            }
        }

        private async void TimerJB_Tick(object sender, EventArgs e)
        {
            if (jbTimerWaiter > 0)
            {
                consoleTextBox.Text += "\r\nTimer JB tick occurred -> JB Waiter: " + jbTimerWaiter.ToString();
                logger.Log("Timer JB tick occurred -> JB Waiter: " + jbTimerWaiter.ToString());
                jbTimerWaiter--;
                return;
            }

            timerJB.Stop();
            consoleTextBox.Text += "Timer JB tick occurred \r\n";
            logger.Log("Timer JB tick occurred");

            timerJBButton.BackColor = Color.Green;


            

        }

        private void ConsoleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (consoleTextBox.TextLength > 2500)
                consoleTextBox.Text = "";
        }

        private void JBCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!jbCheckBox.Checked)
                timerJB.Stop();
        }

        private void GHCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ghCheckBox.Checked)
                timerGH.Stop();
        }

        private void CVCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!cvCheckBox.Checked)
                timerCV.Stop();
        }

        private void timerCVButton_Click(object sender, EventArgs e)
        {
            //TimerCV.Start();
        }

        private void timerGHButton_Click(object sender, EventArgs e)
        {
            //TimerGH.Start();
        }

        private void timerJBButton_Click(object sender, EventArgs e)
        {
            //TimerJB.Start();
        }


    }
}

            query = "INSERT INTO [dbo].[Q_QUALITY_IF]\r\n" +
     "([DEV_ID]\r\n" +
     ",[SEND_DATE]\r\n" +
     ",[SEND_SERIAL]\r\n" +
     ",[DATA_SIZE]\r\n" +
     ",[DATA_TYPE]\r\n" +
     ",[SEND_DATA]\r\n" +
     ",[CREATE_TIME]\r\n" +
     ",[CREATE_USER]\r\n" +
     ",[RESULT_ID]\r\n" +
     ",[PROGRAM_VERSION])\r\n" +
     "VALUES\r\n" +
     "('DIAP080'\r\n" +
     "," + ResultData.send_date +
     ",'1'\r\n" +
     ",'44'\r\n" +
     ",'QR'\r\n" +
     "," + ResultData.send_data + "\r\n" +
     "," + ResultData.send_date + "\r\n" +
     "," + ResultData.vin + "\r\n" +
     "," + ResultData.currentResultIdGH + "\r\n" +
     "," + ResultData.currentProgramversion;
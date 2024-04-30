namespace TN8_to_MES
{
    partial class mainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            startButton = new Button();
            consoleTextBox = new TextBox();
            timerCV = new System.Windows.Forms.Timer(components);
            stopButton = new Button();
            timerGH = new System.Windows.Forms.Timer(components);
            timerJB = new System.Windows.Forms.Timer(components);
            timerCVButton = new Button();
            timerGHButton = new Button();
            timerJBButton = new Button();
            enableRequestLabel = new Label();
            cvCheckBox = new CheckBox();
            ghCheckBox = new CheckBox();
            jbCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Location = new Point(597, 481);
            startButton.Name = "startButton";
            startButton.Size = new Size(192, 52);
            startButton.TabIndex = 0;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += StartButton_Click;
            // 
            // consoleTextBox
            // 
            consoleTextBox.Location = new Point(23, 12);
            consoleTextBox.Multiline = true;
            consoleTextBox.Name = "consoleTextBox";
            consoleTextBox.ScrollBars = ScrollBars.Vertical;
            consoleTextBox.Size = new Size(546, 521);
            consoleTextBox.TabIndex = 1;
            consoleTextBox.TextChanged += ConsoleTextBox_TextChanged;
            // 
            // timerCV
            // 
            timerCV.Interval = 60000;
            timerCV.Tick += TimerCV_Tick;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(597, 423);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(192, 52);
            stopButton.TabIndex = 3;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += StopButton_Click;
            // 
            // timerGH
            // 
            timerGH.Interval = 30000;
            timerGH.Tick += TimerGH_Tick;
            // 
            // timerJB
            // 
            timerJB.Interval = 35000;
            timerJB.Tick += TimerJB_Tick;
            // 
            // timerCVButton
            // 
            timerCVButton.Location = new Point(590, 35);
            timerCVButton.Name = "timerCVButton";
            timerCVButton.Size = new Size(191, 60);
            timerCVButton.TabIndex = 4;
            timerCVButton.Text = "Requesting CV ";
            timerCVButton.UseVisualStyleBackColor = true;
            timerCVButton.Click += timerCVButton_Click;
            // 
            // timerGHButton
            // 
            timerGHButton.Location = new Point(590, 109);
            timerGHButton.Name = "timerGHButton";
            timerGHButton.Size = new Size(191, 60);
            timerGHButton.TabIndex = 5;
            timerGHButton.Text = "Requesting GH";
            timerGHButton.UseVisualStyleBackColor = true;
            timerGHButton.Click += timerGHButton_Click;
            // 
            // timerJBButton
            // 
            timerJBButton.Location = new Point(590, 189);
            timerJBButton.Name = "timerJBButton";
            timerJBButton.Size = new Size(191, 60);
            timerJBButton.TabIndex = 6;
            timerJBButton.Text = "Requesting JB";
            timerJBButton.UseVisualStyleBackColor = true;
            timerJBButton.Click += timerJBButton_Click;
            // 
            // enableRequestLabel
            // 
            enableRequestLabel.AutoSize = true;
            enableRequestLabel.Location = new Point(629, 284);
            enableRequestLabel.Name = "enableRequestLabel";
            enableRequestLabel.Size = new Size(110, 20);
            enableRequestLabel.TabIndex = 7;
            enableRequestLabel.Text = "Enable request:";
            // 
            // cvCheckBox
            // 
            cvCheckBox.AutoSize = true;
            cvCheckBox.Checked = true;
            cvCheckBox.CheckState = CheckState.Checked;
            cvCheckBox.Location = new Point(629, 316);
            cvCheckBox.Name = "cvCheckBox";
            cvCheckBox.Size = new Size(126, 24);
            cvCheckBox.TabIndex = 8;
            cvCheckBox.Text = "CV (Old Creta)";
            cvCheckBox.UseVisualStyleBackColor = true;
            cvCheckBox.CheckedChanged += CVCheckBox_CheckedChanged;
            // 
            // ghCheckBox
            // 
            ghCheckBox.AutoSize = true;
            ghCheckBox.Checked = true;
            ghCheckBox.CheckState = CheckState.Checked;
            ghCheckBox.Location = new Point(629, 347);
            ghCheckBox.Name = "ghCheckBox";
            ghCheckBox.Size = new Size(102, 24);
            ghCheckBox.TabIndex = 9;
            ghCheckBox.Text = "GH (HB20)";
            ghCheckBox.UseVisualStyleBackColor = true;
            ghCheckBox.CheckedChanged += GHCheckBox_CheckedChanged;
            // 
            // jbCheckBox
            // 
            jbCheckBox.AutoSize = true;
            jbCheckBox.Checked = true;
            jbCheckBox.CheckState = CheckState.Checked;
            jbCheckBox.Location = new Point(629, 376);
            jbCheckBox.Name = "jbCheckBox";
            jbCheckBox.Size = new Size(128, 24);
            jbCheckBox.TabIndex = 10;
            jbCheckBox.Text = "JB (New Creta)";
            jbCheckBox.UseVisualStyleBackColor = true;
            jbCheckBox.CheckedChanged += JBCheckBox_CheckedChanged;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 545);
            Controls.Add(jbCheckBox);
            Controls.Add(ghCheckBox);
            Controls.Add(cvCheckBox);
            Controls.Add(enableRequestLabel);
            Controls.Add(timerJBButton);
            Controls.Add(timerGHButton);
            Controls.Add(timerCVButton);
            Controls.Add(stopButton);
            Controls.Add(consoleTextBox);
            Controls.Add(startButton);
            Name = "mainForm";
            Text = "TN8_to_MES - v1.5.1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button startButton;
        private TextBox consoleTextBox;
        private System.Windows.Forms.Timer timerCV;
        private Button stopButton;
        private System.Windows.Forms.Timer timerGH;
        private System.Windows.Forms.Timer timerJB;
        private Button timerCVButton;
        private Button timerGHButton;
        private Button timerJBButton;
        private Label enableRequestLabel;
        private CheckBox cvCheckBox;
        private CheckBox ghCheckBox;
        private CheckBox jbCheckBox;
    }
}
namespace TN8_to_MES
{
    partial class Form1
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
            Start_btn = new Button();
            textBox1 = new TextBox();
            TimerCV = new System.Windows.Forms.Timer(components);
            Stop_btn = new Button();
            TimerGH = new System.Windows.Forms.Timer(components);
            TimerJB = new System.Windows.Forms.Timer(components);
            TimerCV_btn = new Button();
            TimerGH_btn = new Button();
            TimerJB_btn = new Button();
            label1 = new Label();
            CV_checkBox = new CheckBox();
            GH_checkBox = new CheckBox();
            JB_checkBox = new CheckBox();
            SuspendLayout();
            // 
            // Start_btn
            // 
            Start_btn.Location = new Point(522, 361);
            Start_btn.Margin = new Padding(3, 2, 3, 2);
            Start_btn.Name = "Start_btn";
            Start_btn.Size = new Size(168, 39);
            Start_btn.TabIndex = 0;
            Start_btn.Text = "Start";
            Start_btn.UseVisualStyleBackColor = true;
            Start_btn.Click += Start_btn_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(20, 9);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(478, 392);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // TimerCV
            // 
            TimerCV.Interval = 6000;
            TimerCV.Tick += TimerCV_Tick;
            // 
            // Stop_btn
            // 
            Stop_btn.Location = new Point(522, 317);
            Stop_btn.Margin = new Padding(3, 2, 3, 2);
            Stop_btn.Name = "Stop_btn";
            Stop_btn.Size = new Size(168, 39);
            Stop_btn.TabIndex = 3;
            Stop_btn.Text = "Stop";
            Stop_btn.UseVisualStyleBackColor = true;
            Stop_btn.Click += Stop_btn_Click;
            // 
            // TimerGH
            // 
            TimerGH.Interval = 3000;
            TimerGH.Tick += TimerGH_Tick;
            // 
            // TimerJB
            // 
            TimerJB.Interval = 9000;
            TimerJB.Tick += TimerJB_Tick;
            // 
            // TimerCV_btn
            // 
            TimerCV_btn.Location = new Point(516, 26);
            TimerCV_btn.Margin = new Padding(3, 2, 3, 2);
            TimerCV_btn.Name = "TimerCV_btn";
            TimerCV_btn.Size = new Size(173, 45);
            TimerCV_btn.TabIndex = 4;
            TimerCV_btn.Text = "Requesting CV ";
            TimerCV_btn.UseVisualStyleBackColor = true;
            TimerCV_btn.Click += TimerCV_btn_Click;
            // 
            // TimerGH_btn
            // 
            TimerGH_btn.Location = new Point(516, 82);
            TimerGH_btn.Margin = new Padding(3, 2, 3, 2);
            TimerGH_btn.Name = "TimerGH_btn";
            TimerGH_btn.Size = new Size(167, 45);
            TimerGH_btn.TabIndex = 5;
            TimerGH_btn.Text = "Requesting GH";
            TimerGH_btn.UseVisualStyleBackColor = true;
            TimerGH_btn.Click += TimerGH_btn_Click;
            // 
            // TimerJB_btn
            // 
            TimerJB_btn.Location = new Point(516, 142);
            TimerJB_btn.Margin = new Padding(3, 2, 3, 2);
            TimerJB_btn.Name = "TimerJB_btn";
            TimerJB_btn.Size = new Size(167, 45);
            TimerJB_btn.TabIndex = 6;
            TimerJB_btn.Text = "Requesting JB";
            TimerJB_btn.UseVisualStyleBackColor = true;
            TimerJB_btn.Click += TimerJB_btn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(550, 213);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 7;
            label1.Text = "Enable request:";
            label1.Click += label1_Click;
            // 
            // CV_checkBox
            // 
            CV_checkBox.AutoSize = true;
            CV_checkBox.Checked = true;
            CV_checkBox.CheckState = CheckState.Checked;
            CV_checkBox.Location = new Point(550, 237);
            CV_checkBox.Margin = new Padding(3, 2, 3, 2);
            CV_checkBox.Name = "CV_checkBox";
            CV_checkBox.Size = new Size(102, 19);
            CV_checkBox.TabIndex = 8;
            CV_checkBox.Text = "CV (Old Creta)";
            CV_checkBox.UseVisualStyleBackColor = true;
            // 
            // GH_checkBox
            // 
            GH_checkBox.AutoSize = true;
            GH_checkBox.Checked = true;
            GH_checkBox.CheckState = CheckState.Checked;
            GH_checkBox.Location = new Point(550, 260);
            GH_checkBox.Margin = new Padding(3, 2, 3, 2);
            GH_checkBox.Name = "GH_checkBox";
            GH_checkBox.Size = new Size(82, 19);
            GH_checkBox.TabIndex = 9;
            GH_checkBox.Text = "GH (HB20)";
            GH_checkBox.UseVisualStyleBackColor = true;
            // 
            // JB_checkBox
            // 
            JB_checkBox.AutoSize = true;
            JB_checkBox.Checked = true;
            JB_checkBox.CheckState = CheckState.Checked;
            JB_checkBox.Location = new Point(550, 282);
            JB_checkBox.Margin = new Padding(3, 2, 3, 2);
            JB_checkBox.Name = "JB_checkBox";
            JB_checkBox.Size = new Size(103, 19);
            JB_checkBox.TabIndex = 10;
            JB_checkBox.Text = "JB (New Creta)";
            JB_checkBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 409);
            Controls.Add(JB_checkBox);
            Controls.Add(GH_checkBox);
            Controls.Add(CV_checkBox);
            Controls.Add(label1);
            Controls.Add(TimerJB_btn);
            Controls.Add(TimerGH_btn);
            Controls.Add(TimerCV_btn);
            Controls.Add(Stop_btn);
            Controls.Add(textBox1);
            Controls.Add(Start_btn);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Start_btn;
        private TextBox textBox1;
        private System.Windows.Forms.Timer TimerCV;
        private Button Stop_btn;
        private System.Windows.Forms.Timer TimerGH;
        private System.Windows.Forms.Timer TimerJB;
        private Button TimerCV_btn;
        private Button TimerGH_btn;
        private Button TimerJB_btn;
        private Label label1;
        private CheckBox CV_checkBox;
        private CheckBox GH_checkBox;
        private CheckBox JB_checkBox;
    }
}
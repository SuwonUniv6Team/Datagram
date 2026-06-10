namespace Datagram
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.picMain = new System.Windows.Forms.PictureBox();
            this.prgAngle = new System.Windows.Forms.ProgressBar();
            this.prgThrottle = new System.Windows.Forms.ProgressBar();
            this.lblMain = new System.Windows.Forms.Label();
            this.lblAngleName = new System.Windows.Forms.Label();
            this.lblThrottleName = new System.Windows.Forms.Label();
            this.lstFrames = new System.Windows.Forms.ListBox();
            this.trackFrame = new System.Windows.Forms.TrackBar();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnTrain = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnplay = new System.Windows.Forms.Button();
            this.btnpurse = new System.Windows.Forms.Button();
            this.btnnext = new System.Windows.Forms.Button();
            this.btntrace = new System.Windows.Forms.Button();
            this.cbboxspeed = new System.Windows.Forms.ComboBox();
            this.gbfilter = new System.Windows.Forms.GroupBox();
            this.cbboxtub = new System.Windows.Forms.ComboBox();
            this.cbox1 = new System.Windows.Forms.CheckBox();
            this.cbox3 = new System.Windows.Forms.CheckBox();
            this.lbl1 = new System.Windows.Forms.Label();
            this.nud1 = new System.Windows.Forms.NumericUpDown();
            this.lbl2 = new System.Windows.Forms.Label();
            this.cbox2 = new System.Windows.Forms.CheckBox();
            this.btnreset = new System.Windows.Forms.Button();
            this.cbox4 = new System.Windows.Forms.CheckBox();
            this.btnGraph = new System.Windows.Forms.Button();
            this.nudEpochs = new System.Windows.Forms.NumericUpDown();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnRangeSelect = new System.Windows.Forms.Button();
            this.BtnRestore = new System.Windows.Forms.Button();
            this.btnAIreview = new System.Windows.Forms.Button();
            this.lstDeleteRanges = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrame)).BeginInit();
            this.gbfilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEpochs)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPath.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPath.ForeColor = System.Drawing.Color.White;
            this.txtPath.Location = new System.Drawing.Point(140, 22);
            this.txtPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(882, 27);
            this.txtPath.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(1068, 10);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(102, 50);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "폴더 선택";
            this.btnLoad.UseVisualStyleBackColor = false;
            // 
            // picMain
            // 
            this.picMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.picMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMain.Location = new System.Drawing.Point(9, 68);
            this.picMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(997, 283);
            this.picMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMain.TabIndex = 2;
            this.picMain.TabStop = false;
            // 
            // prgAngle
            // 
            this.prgAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prgAngle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.prgAngle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(100)))));
            this.prgAngle.Location = new System.Drawing.Point(1068, 81);
            this.prgAngle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.prgAngle.Name = "prgAngle";
            this.prgAngle.Size = new System.Drawing.Size(219, 34);
            this.prgAngle.TabIndex = 3;
            // 
            // prgThrottle
            // 
            this.prgThrottle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prgThrottle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.prgThrottle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.prgThrottle.Location = new System.Drawing.Point(1068, 135);
            this.prgThrottle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.prgThrottle.Name = "prgThrottle";
            this.prgThrottle.Size = new System.Drawing.Size(219, 33);
            this.prgThrottle.TabIndex = 4;
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.ForeColor = System.Drawing.Color.LightGray;
            this.lblMain.Location = new System.Drawing.Point(4, 20);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(123, 25);
            this.lblMain.TabIndex = 5;
            this.lblMain.Text = "Catalog Path:";
            // 
            // lblAngleName
            // 
            this.lblAngleName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAngleName.AutoSize = true;
            this.lblAngleName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAngleName.ForeColor = System.Drawing.Color.LightGray;
            this.lblAngleName.Location = new System.Drawing.Point(1012, 92);
            this.lblAngleName.Name = "lblAngleName";
            this.lblAngleName.Size = new System.Drawing.Size(41, 15);
            this.lblAngleName.TabIndex = 6;
            this.lblAngleName.Text = "Angle:";
            // 
            // lblThrottleName
            // 
            this.lblThrottleName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblThrottleName.AutoSize = true;
            this.lblThrottleName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThrottleName.ForeColor = System.Drawing.Color.LightGray;
            this.lblThrottleName.Location = new System.Drawing.Point(1012, 148);
            this.lblThrottleName.Name = "lblThrottleName";
            this.lblThrottleName.Size = new System.Drawing.Size(52, 15);
            this.lblThrottleName.TabIndex = 7;
            this.lblThrottleName.Text = "Throttle:";
            // 
            // lstFrames
            // 
            this.lstFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFrames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstFrames.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstFrames.ForeColor = System.Drawing.Color.White;
            this.lstFrames.FormattingEnabled = true;
            this.lstFrames.ItemHeight = 14;
            this.lstFrames.Location = new System.Drawing.Point(1015, 186);
            this.lstFrames.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lstFrames.Name = "lstFrames";
            this.lstFrames.Size = new System.Drawing.Size(273, 158);
            this.lstFrames.TabIndex = 8;
            // 
            // trackFrame
            // 
            this.trackFrame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackFrame.Location = new System.Drawing.Point(442, 355);
            this.trackFrame.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackFrame.Name = "trackFrame";
            this.trackFrame.Size = new System.Drawing.Size(844, 45);
            this.trackFrame.TabIndex = 9;
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(302, 33);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(107, 47);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "⫷ 필터 적용";
            this.btnFilter.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(450, 438);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(125, 30);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "🗑 삭제";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnTrain
            // 
            this.btnTrain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(100)))));
            this.btnTrain.FlatAppearance.BorderSize = 0;
            this.btnTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTrain.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrain.ForeColor = System.Drawing.Color.White;
            this.btnTrain.Location = new System.Drawing.Point(1117, 445);
            this.btnTrain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(158, 48);
            this.btnTrain.TabIndex = 12;
            this.btnTrain.Text = "▶ AI 학습 시작";
            this.btnTrain.UseVisualStyleBackColor = false;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))));
            this.txtLog.Location = new System.Drawing.Point(0, 517);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(1298, 125);
            this.txtLog.TabIndex = 13;
            this.txtLog.Text = "";
            // 
            // btnplay
            // 
            this.btnplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnplay.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnplay.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnplay.Location = new System.Drawing.Point(561, 392);
            this.btnplay.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnplay.Name = "btnplay";
            this.btnplay.Size = new System.Drawing.Size(65, 39);
            this.btnplay.TabIndex = 14;
            this.btnplay.Text = "▶";
            this.btnplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnplay.UseVisualStyleBackColor = true;
            // 
            // btnpurse
            // 
            this.btnpurse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnpurse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnpurse.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnpurse.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnpurse.Location = new System.Drawing.Point(636, 392);
            this.btnpurse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnpurse.Name = "btnpurse";
            this.btnpurse.Size = new System.Drawing.Size(61, 39);
            this.btnpurse.TabIndex = 15;
            this.btnpurse.Text = "⏸";
            this.btnpurse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnpurse.UseVisualStyleBackColor = true;
            // 
            // btnnext
            // 
            this.btnnext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnnext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnnext.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnnext.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnnext.Location = new System.Drawing.Point(708, 392);
            this.btnnext.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnnext.Name = "btnnext";
            this.btnnext.Size = new System.Drawing.Size(101, 39);
            this.btnnext.TabIndex = 16;
            this.btnnext.Text = "▶▶";
            this.btnnext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnnext.UseVisualStyleBackColor = true;
            // 
            // btntrace
            // 
            this.btntrace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btntrace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btntrace.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btntrace.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.btntrace.Location = new System.Drawing.Point(454, 392);
            this.btntrace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btntrace.Name = "btntrace";
            this.btntrace.Size = new System.Drawing.Size(96, 39);
            this.btntrace.TabIndex = 17;
            this.btntrace.Text = "◀◀";
            this.btntrace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btntrace.UseVisualStyleBackColor = true;
            // 
            // cbboxspeed
            // 
            this.cbboxspeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbboxspeed.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbboxspeed.FormattingEnabled = true;
            this.cbboxspeed.Location = new System.Drawing.Point(820, 391);
            this.cbboxspeed.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbboxspeed.Name = "cbboxspeed";
            this.cbboxspeed.Size = new System.Drawing.Size(106, 40);
            this.cbboxspeed.TabIndex = 18;
            // 
            // gbfilter
            // 
            this.gbfilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbfilter.Controls.Add(this.cbboxtub);
            this.gbfilter.Controls.Add(this.cbox1);
            this.gbfilter.Controls.Add(this.cbox3);
            this.gbfilter.Controls.Add(this.lbl1);
            this.gbfilter.Controls.Add(this.nud1);
            this.gbfilter.Controls.Add(this.lbl2);
            this.gbfilter.Controls.Add(this.cbox2);
            this.gbfilter.Controls.Add(this.btnreset);
            this.gbfilter.Controls.Add(this.btnFilter);
            this.gbfilter.Controls.Add(this.cbox4);
            this.gbfilter.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbfilter.ForeColor = System.Drawing.SystemColors.Control;
            this.gbfilter.Location = new System.Drawing.Point(21, 354);
            this.gbfilter.Name = "gbfilter";
            this.gbfilter.Size = new System.Drawing.Size(415, 158);
            this.gbfilter.TabIndex = 19;
            this.gbfilter.TabStop = false;
            this.gbfilter.Text = "데이터 필터링";
            // 
            // cbboxtub
            // 
            this.cbboxtub.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cbboxtub.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbboxtub.ForeColor = System.Drawing.Color.White;
            this.cbboxtub.FormattingEnabled = true;
            this.cbboxtub.Location = new System.Drawing.Point(175, 84);
            this.cbboxtub.Name = "cbboxtub";
            this.cbboxtub.Size = new System.Drawing.Size(121, 25);
            this.cbboxtub.TabIndex = 16;
            // 
            // cbox1
            // 
            this.cbox1.AutoSize = true;
            this.cbox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbox1.ForeColor = System.Drawing.Color.White;
            this.cbox1.Location = new System.Drawing.Point(6, 34);
            this.cbox1.Name = "cbox1";
            this.cbox1.Size = new System.Drawing.Size(126, 21);
            this.cbox1.TabIndex = 12;
            this.cbox1.Text = "직진 데이터 제외";
            this.cbox1.UseVisualStyleBackColor = true;
            // 
            // cbox3
            // 
            this.cbox3.AutoSize = true;
            this.cbox3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbox3.ForeColor = System.Drawing.Color.White;
            this.cbox3.Location = new System.Drawing.Point(6, 84);
            this.cbox3.Name = "cbox3";
            this.cbox3.Size = new System.Drawing.Size(173, 21);
            this.cbox3.TabIndex = 15;
            this.cbox3.Text = "특정 주행 회차(Tub) 선택";
            this.cbox3.UseVisualStyleBackColor = true;
            this.cbox3.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1.ForeColor = System.Drawing.Color.White;
            this.lbl1.Location = new System.Drawing.Point(136, 55);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(68, 17);
            this.lbl1.TabIndex = 0;
            this.lbl1.Text = "( |Angle| ≥";
            // 
            // nud1
            // 
            this.nud1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.nud1.DecimalPlaces = 1;
            this.nud1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nud1.ForeColor = System.Drawing.Color.White;
            this.nud1.Location = new System.Drawing.Point(204, 55);
            this.nud1.Name = "nud1";
            this.nud1.Size = new System.Drawing.Size(43, 25);
            this.nud1.TabIndex = 1;
            this.nud1.Value = new decimal(new int[] {
            6,
            0,
            0,
            65536});
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl2.ForeColor = System.Drawing.Color.White;
            this.lbl2.Location = new System.Drawing.Point(250, 55);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(12, 17);
            this.lbl2.TabIndex = 2;
            this.lbl2.Text = ")";
            // 
            // cbox2
            // 
            this.cbox2.AutoSize = true;
            this.cbox2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbox2.ForeColor = System.Drawing.Color.White;
            this.cbox2.Location = new System.Drawing.Point(6, 57);
            this.cbox2.Name = "cbox2";
            this.cbox2.Size = new System.Drawing.Size(139, 21);
            this.cbox2.TabIndex = 14;
            this.cbox2.Text = "급커브 구간만 보기";
            this.cbox2.UseVisualStyleBackColor = true;
            // 
            // btnreset
            // 
            this.btnreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnreset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnreset.FlatAppearance.BorderSize = 0;
            this.btnreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnreset.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnreset.ForeColor = System.Drawing.Color.White;
            this.btnreset.Location = new System.Drawing.Point(302, 98);
            this.btnreset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(107, 44);
            this.btnreset.TabIndex = 11;
            this.btnreset.Text = "⫷필터 초기화";
            this.btnreset.UseVisualStyleBackColor = false;
            // 
            // cbox4
            // 
            this.cbox4.AutoSize = true;
            this.cbox4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbox4.ForeColor = System.Drawing.Color.White;
            this.cbox4.Location = new System.Drawing.Point(6, 111);
            this.cbox4.Name = "cbox4";
            this.cbox4.Size = new System.Drawing.Size(130, 21);
            this.cbox4.TabIndex = 13;
            this.cbox4.Text = "정지 및 후진 제외";
            this.cbox4.UseVisualStyleBackColor = true;
            // 
            // btnGraph
            // 
            this.btnGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGraph.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnGraph.FlatAppearance.BorderSize = 0;
            this.btnGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraph.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGraph.ForeColor = System.Drawing.Color.White;
            this.btnGraph.Location = new System.Drawing.Point(948, 391);
            this.btnGraph.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(158, 40);
            this.btnGraph.TabIndex = 12;
            this.btnGraph.Text = "📈 그래프";
            this.btnGraph.UseVisualStyleBackColor = false;
            this.btnGraph.Click += new System.EventHandler(this.BtnGraph_Click);
            // 
            // nudEpochs
            // 
            this.nudEpochs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nudEpochs.Location = new System.Drawing.Point(1063, 457);
            this.nudEpochs.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudEpochs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEpochs.Name = "nudEpochs";
            this.nudEpochs.Size = new System.Drawing.Size(43, 21);
            this.nudEpochs.TabIndex = 20;
            this.nudEpochs.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCount.ForeColor = System.Drawing.SystemColors.Control;
            this.lblCount.Location = new System.Drawing.Point(916, 449);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(145, 28);
            this.lblCount.TabIndex = 21;
            this.lblCount.Text = "AI 학습 횟수 : ";
            this.lblCount.Click += new System.EventHandler(this.lblCount_Click);
            // 
            // btnRangeSelect
            // 
            this.btnRangeSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRangeSelect.BackColor = System.Drawing.Color.Teal;
            this.btnRangeSelect.FlatAppearance.BorderSize = 0;
            this.btnRangeSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRangeSelect.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRangeSelect.ForeColor = System.Drawing.Color.White;
            this.btnRangeSelect.Location = new System.Drawing.Point(584, 445);
            this.btnRangeSelect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRangeSelect.Name = "btnRangeSelect";
            this.btnRangeSelect.Size = new System.Drawing.Size(85, 48);
            this.btnRangeSelect.TabIndex = 22;
            this.btnRangeSelect.Text = "범위 선택";
            this.btnRangeSelect.UseVisualStyleBackColor = false;
            // 
            // BtnRestore
            // 
            this.BtnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnRestore.BackColor = System.Drawing.Color.Gray;
            this.BtnRestore.FlatAppearance.BorderSize = 0;
            this.BtnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnRestore.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRestore.ForeColor = System.Drawing.Color.White;
            this.BtnRestore.Location = new System.Drawing.Point(450, 472);
            this.BtnRestore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnRestore.Name = "BtnRestore";
            this.BtnRestore.Size = new System.Drawing.Size(125, 30);
            this.BtnRestore.TabIndex = 11;
            this.BtnRestore.Text = "↩ 복원";
            this.BtnRestore.UseVisualStyleBackColor = false;
            this.BtnRestore.Click += new System.EventHandler(this.BtnRestore_Click);
            // 
            // btnAIreview
            // 
            this.btnAIreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAIreview.BackColor = System.Drawing.Color.Indigo;
            this.btnAIreview.FlatAppearance.BorderSize = 0;
            this.btnAIreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAIreview.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAIreview.ForeColor = System.Drawing.Color.White;
            this.btnAIreview.Location = new System.Drawing.Point(1117, 391);
            this.btnAIreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAIreview.Name = "btnAIreview";
            this.btnAIreview.Size = new System.Drawing.Size(158, 40);
            this.btnAIreview.TabIndex = 12;
            this.btnAIreview.Text = "AI 학습 검증";
            this.btnAIreview.UseVisualStyleBackColor = false;
            this.btnAIreview.Click += new System.EventHandler(this.BtnGraph_Click);
            // 
            // lstDeleteRanges
            // 
            this.lstDeleteRanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDeleteRanges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.lstDeleteRanges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstDeleteRanges.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstDeleteRanges.ForeColor = System.Drawing.Color.White;
            this.lstDeleteRanges.FormattingEnabled = true;
            this.lstDeleteRanges.Location = new System.Drawing.Point(675, 438);
            this.lstDeleteRanges.Name = "lstDeleteRanges";
            this.lstDeleteRanges.Size = new System.Drawing.Size(238, 67);
            this.lstDeleteRanges.TabIndex = 29;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1298, 642);
            this.Controls.Add(this.lstDeleteRanges);
            this.Controls.Add(this.btnRangeSelect);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.nudEpochs);
            this.Controls.Add(this.cbboxspeed);
            this.Controls.Add(this.btntrace);
            this.Controls.Add(this.btnnext);
            this.Controls.Add(this.btnpurse);
            this.Controls.Add(this.btnplay);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnAIreview);
            this.Controls.Add(this.btnGraph);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.BtnRestore);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.trackFrame);
            this.Controls.Add(this.lstFrames);
            this.Controls.Add(this.lblThrottleName);
            this.Controls.Add(this.lblAngleName);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.prgThrottle);
            this.Controls.Add(this.prgAngle);
            this.Controls.Add(this.picMain);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.gbfilter);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(877, 568);
            this.Name = "Form1";
            this.Text = "Datagram";
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrame)).EndInit();
            this.gbfilter.ResumeLayout(false);
            this.gbfilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEpochs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.PictureBox picMain;
        private System.Windows.Forms.ProgressBar prgAngle;
        private System.Windows.Forms.ProgressBar prgThrottle;
        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.Label lblAngleName;
        private System.Windows.Forms.Label lblThrottleName;
        private System.Windows.Forms.ListBox lstFrames;
        private System.Windows.Forms.TrackBar trackFrame;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnTrain;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnplay;
        private System.Windows.Forms.Button btnpurse;
        private System.Windows.Forms.Button btnnext;
        private System.Windows.Forms.Button btntrace;
        private System.Windows.Forms.ComboBox cbboxspeed;
        private System.Windows.Forms.GroupBox gbfilter;
        private System.Windows.Forms.Button btnreset;
        private System.Windows.Forms.ComboBox cbboxtub;
        private System.Windows.Forms.CheckBox cbox1;
        private System.Windows.Forms.CheckBox cbox3;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.NumericUpDown nud1;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.CheckBox cbox2;
        private System.Windows.Forms.CheckBox cbox4;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.NumericUpDown nudEpochs;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnRangeSelect;
        private System.Windows.Forms.Button BtnRestore;
        private System.Windows.Forms.Button btnAIreview;
        private System.Windows.Forms.ListBox lstDeleteRanges;
    }
}


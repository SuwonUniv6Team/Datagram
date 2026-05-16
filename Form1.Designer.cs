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
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.Font = new System.Drawing.Font("굴림", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtPath.Location = new System.Drawing.Point(143, 23);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(808, 39);
            this.txtPath.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnLoad.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoad.Location = new System.Drawing.Point(1021, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(116, 62);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "폴더 열기";
            this.btnLoad.UseVisualStyleBackColor = false;
            // 
            // picMain
            // 
            this.picMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMain.Location = new System.Drawing.Point(12, 82);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(939, 380);
            this.picMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMain.TabIndex = 2;
            this.picMain.TabStop = false;
            // 
            // prgAngle
            // 
            this.prgAngle.Location = new System.Drawing.Point(1021, 101);
            this.prgAngle.Name = "prgAngle";
            this.prgAngle.Size = new System.Drawing.Size(250, 42);
            this.prgAngle.TabIndex = 3;
            // 
            // prgThrottle
            // 
            this.prgThrottle.Location = new System.Drawing.Point(1021, 169);
            this.prgThrottle.Name = "prgThrottle";
            this.prgThrottle.Size = new System.Drawing.Size(250, 41);
            this.prgThrottle.TabIndex = 4;
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMain.Location = new System.Drawing.Point(12, 32);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(117, 20);
            this.lblMain.TabIndex = 5;
            this.lblMain.Text = "Catalog Path";
            // 
            // lblAngleName
            // 
            this.lblAngleName.AutoSize = true;
            this.lblAngleName.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAngleName.Location = new System.Drawing.Point(957, 115);
            this.lblAngleName.Name = "lblAngleName";
            this.lblAngleName.Size = new System.Drawing.Size(48, 15);
            this.lblAngleName.TabIndex = 6;
            this.lblAngleName.Text = "Angle:";
            // 
            // lblThrottleName
            // 
            this.lblThrottleName.AutoSize = true;
            this.lblThrottleName.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblThrottleName.Location = new System.Drawing.Point(957, 185);
            this.lblThrottleName.Name = "lblThrottleName";
            this.lblThrottleName.Size = new System.Drawing.Size(60, 15);
            this.lblThrottleName.TabIndex = 7;
            this.lblThrottleName.Text = "Throttle:";
            // 
            // lstFrames
            // 
            this.lstFrames.FormattingEnabled = true;
            this.lstFrames.ItemHeight = 15;
            this.lstFrames.Location = new System.Drawing.Point(960, 233);
            this.lstFrames.Name = "lstFrames";
            this.lstFrames.Size = new System.Drawing.Size(311, 229);
            this.lstFrames.TabIndex = 8;
            // 
            // trackFrame
            // 
            this.trackFrame.Location = new System.Drawing.Point(12, 468);
            this.trackFrame.Name = "trackFrame";
            this.trackFrame.Size = new System.Drawing.Size(1259, 56);
            this.trackFrame.TabIndex = 9;
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.LightGray;
            this.btnFilter.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFilter.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnFilter.Location = new System.Drawing.Point(12, 530);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(153, 60);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "데이터 필터링";
            this.btnFilter.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.LightGray;
            this.btnDelete.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDelete.ForeColor = System.Drawing.Color.Red;
            this.btnDelete.Location = new System.Drawing.Point(171, 530);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(143, 60);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "프레임 삭제";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnTrain
            // 
            this.btnTrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnTrain.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTrain.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnTrain.Location = new System.Drawing.Point(1041, 530);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(219, 60);
            this.btnTrain.TabIndex = 12;
            this.btnTrain.Text = "AI 학습 시작";
            this.btnTrain.UseVisualStyleBackColor = false;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 614);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(1248, 106);
            this.txtLog.TabIndex = 13;
            this.txtLog.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1283, 732);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnFilter);
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
            this.Name = "Form1";
            this.Text = "Datagram";
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrame)).EndInit();
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
    }
}


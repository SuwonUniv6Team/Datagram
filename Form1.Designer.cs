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
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrame)).BeginInit();
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
            this.txtPath.Size = new System.Drawing.Size(707, 27);
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
            this.btnLoad.Location = new System.Drawing.Point(893, 10);
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
            this.picMain.Location = new System.Drawing.Point(10, 66);
            this.picMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(822, 304);
            this.picMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMain.TabIndex = 2;
            this.picMain.TabStop = false;
            // 
            // prgAngle
            // 
            this.prgAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prgAngle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.prgAngle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(100)))));
            this.prgAngle.Location = new System.Drawing.Point(893, 81);
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
            this.prgThrottle.Location = new System.Drawing.Point(893, 135);
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
            this.lblAngleName.Location = new System.Drawing.Point(837, 92);
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
            this.lblThrottleName.Location = new System.Drawing.Point(837, 148);
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
            this.lstFrames.Location = new System.Drawing.Point(840, 186);
            this.lstFrames.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lstFrames.Name = "lstFrames";
            this.lstFrames.Size = new System.Drawing.Size(273, 172);
            this.lstFrames.TabIndex = 8;
            // 
            // trackFrame
            // 
            this.trackFrame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackFrame.Location = new System.Drawing.Point(10, 374);
            this.trackFrame.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackFrame.Name = "trackFrame";
            this.trackFrame.Size = new System.Drawing.Size(1102, 45);
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
            this.btnFilter.Location = new System.Drawing.Point(10, 441);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(134, 48);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "⫷ 데이터 필터링";
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
            this.btnDelete.Location = new System.Drawing.Point(149, 441);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(125, 48);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "🗑 프레임 삭제";
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
            this.btnTrain.Location = new System.Drawing.Point(905, 441);
            this.btnTrain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(192, 48);
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
            this.txtLog.Location = new System.Drawing.Point(0, 500);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(1123, 86);
            this.txtLog.TabIndex = 13;
            this.txtLog.Text = "";
            // 
            // btnplay
            // 
            this.btnplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnplay.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnplay.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnplay.Location = new System.Drawing.Point(388, 406);
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
            this.btnpurse.Location = new System.Drawing.Point(470, 406);
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
            this.btnnext.Location = new System.Drawing.Point(549, 406);
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
            this.btntrace.Location = new System.Drawing.Point(313, 406);
            this.btntrace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btntrace.Name = "btntrace";
            this.btntrace.Size = new System.Drawing.Size(56, 39);
            this.btntrace.TabIndex = 17;
            this.btntrace.Text = "◀";
            this.btntrace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btntrace.UseVisualStyleBackColor = true;
            // 
            // cbboxspeed
            // 
            this.cbboxspeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbboxspeed.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbboxspeed.FormattingEnabled = true;
            this.cbboxspeed.Location = new System.Drawing.Point(675, 406);
            this.cbboxspeed.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbboxspeed.Name = "cbboxspeed";
            this.cbboxspeed.Size = new System.Drawing.Size(106, 40);
            this.cbboxspeed.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(1123, 586);
            this.Controls.Add(this.cbboxspeed);
            this.Controls.Add(this.btntrace);
            this.Controls.Add(this.btnnext);
            this.Controls.Add(this.btnpurse);
            this.Controls.Add(this.btnplay);
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
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(877, 568);
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
        private System.Windows.Forms.Button btnplay;
        private System.Windows.Forms.Button btnpurse;
        private System.Windows.Forms.Button btnnext;
        private System.Windows.Forms.Button btntrace;
        private System.Windows.Forms.ComboBox cbboxspeed;
    }
}


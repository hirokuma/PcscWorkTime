namespace WorkTime
{
    partial class WorkTime
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRwDetect = new System.Windows.Forms.Button();
            this.comboRw = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.groupControl = new System.Windows.Forms.GroupBox();
            this.buttonWatch = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.textStatus = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonRwDetect
            // 
            this.buttonRwDetect.Location = new System.Drawing.Point(295, 10);
            this.buttonRwDetect.Name = "buttonRwDetect";
            this.buttonRwDetect.Size = new System.Drawing.Size(75, 23);
            this.buttonRwDetect.TabIndex = 0;
            this.buttonRwDetect.Text = "R/W detect";
            this.buttonRwDetect.UseVisualStyleBackColor = true;
            this.buttonRwDetect.Click += new System.EventHandler(this.buttonRwDetect_Click);
            // 
            // comboRw
            // 
            this.comboRw.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRw.FormattingEnabled = true;
            this.comboRw.Location = new System.Drawing.Point(12, 12);
            this.comboRw.Name = "comboRw";
            this.comboRw.Size = new System.Drawing.Size(277, 20);
            this.comboRw.TabIndex = 1;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Enabled = false;
            this.buttonConnect.Location = new System.Drawing.Point(12, 38);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(177, 23);
            this.buttonConnect.TabIndex = 3;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // groupControl
            // 
            this.groupControl.Location = new System.Drawing.Point(12, 120);
            this.groupControl.Name = "groupControl";
            this.groupControl.Size = new System.Drawing.Size(358, 141);
            this.groupControl.TabIndex = 4;
            this.groupControl.TabStop = false;
            this.groupControl.Text = "Status";
            // 
            // buttonWatch
            // 
            this.buttonWatch.Location = new System.Drawing.Point(196, 38);
            this.buttonWatch.Name = "buttonWatch";
            this.buttonWatch.Size = new System.Drawing.Size(168, 23);
            this.buttonWatch.TabIndex = 5;
            this.buttonWatch.Text = "Watch";
            this.buttonWatch.UseVisualStyleBackColor = true;
            this.buttonWatch.Click += new System.EventHandler(this.buttonWatch_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // textStatus
            // 
            this.textStatus.Location = new System.Drawing.Point(18, 79);
            this.textStatus.Name = "textStatus";
            this.textStatus.ReadOnly = true;
            this.textStatus.Size = new System.Drawing.Size(346, 19);
            this.textStatus.TabIndex = 0;
            // 
            // WorkTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 273);
            this.Controls.Add(this.textStatus);
            this.Controls.Add(this.buttonWatch);
            this.Controls.Add(this.groupControl);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.comboRw);
            this.Controls.Add(this.buttonRwDetect);
            this.Name = "WorkTime";
            this.Text = "WorkTime";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRwDetect;
        private System.Windows.Forms.ComboBox comboRw;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.GroupBox groupControl;
        private System.Windows.Forms.Button buttonWatch;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.TextBox textStatus;
    }
}


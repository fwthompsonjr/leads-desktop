namespace LegalLead.PublicData.Search
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ButtonDentonSetting = new System.Windows.Forms.Button();
            dteStart = new System.Windows.Forms.DateTimePicker();
            dteEnding = new System.Windows.Forms.DateTimePicker();
            button1 = new System.Windows.Forms.Button();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tsWebDriver = new System.Windows.Forms.ToolStripDropDownButton();
            tsDropFileList = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            tsUserName = new System.Windows.Forms.ToolStripStatusLabel();
            cboWebsite = new Classes.ComboBoxEx();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            txConsole = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            cboSearchType = new Classes.ComboBoxEx();
            labelCboCaseType = new System.Windows.Forms.Label();
            cboCaseType = new Classes.ComboBoxEx();
            label6 = new System.Windows.Forms.Label();
            cboCourts = new Classes.ComboBoxEx();
            tableLayoutPanel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(ButtonDentonSetting, 0, 6);
            tableLayoutPanel1.Controls.Add(dteStart, 1, 1);
            tableLayoutPanel1.Controls.Add(dteEnding, 1, 2);
            tableLayoutPanel1.Controls.Add(button1, 1, 6);
            tableLayoutPanel1.Controls.Add(statusStrip1, 0, 8);
            tableLayoutPanel1.Controls.Add(cboWebsite, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(txConsole, 0, 7);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(cboSearchType, 1, 3);
            tableLayoutPanel1.Controls.Add(labelCboCaseType, 0, 4);
            tableLayoutPanel1.Controls.Add(cboCaseType, 1, 4);
            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(cboCourts, 1, 5);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            tableLayoutPanel1.Size = new System.Drawing.Size(869, 734);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // ButtonDentonSetting
            // 
            ButtonDentonSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            ButtonDentonSetting.Location = new System.Drawing.Point(4, 377);
            ButtonDentonSetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            ButtonDentonSetting.Name = "ButtonDentonSetting";
            ButtonDentonSetting.Size = new System.Drawing.Size(152, 52);
            ButtonDentonSetting.TabIndex = 14;
            ButtonDentonSetting.Text = "Settings";
            ButtonDentonSetting.UseVisualStyleBackColor = true;
            ButtonDentonSetting.Click += ButtonDentonSetting_Click;
            // 
            // dteStart
            // 
            dteStart.Dock = System.Windows.Forms.DockStyle.Fill;
            dteStart.Location = new System.Drawing.Point(164, 74);
            dteStart.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            dteStart.Name = "dteStart";
            dteStart.Size = new System.Drawing.Size(701, 27);
            dteStart.TabIndex = 2;
            // 
            // dteEnding
            // 
            dteEnding.Dock = System.Windows.Forms.DockStyle.Fill;
            dteEnding.Location = new System.Drawing.Point(164, 136);
            dteEnding.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            dteEnding.Name = "dteEnding";
            dteEnding.Size = new System.Drawing.Size(701, 27);
            dteEnding.TabIndex = 3;
            // 
            // button1
            // 
            button1.Dock = System.Windows.Forms.DockStyle.Fill;
            button1.Location = new System.Drawing.Point(164, 377);
            button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(701, 52);
            button1.TabIndex = 4;
            button1.Text = "Get Data";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // statusStrip1
            // 
            tableLayoutPanel1.SetColumnSpan(statusStrip1, 2);
            statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsWebDriver, tsDropFileList, toolStripStatus, tsStatusLabel, tsUserName });
            statusStrip1.Location = new System.Drawing.Point(0, 663);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            statusStrip1.Size = new System.Drawing.Size(869, 46);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsWebDriver
            // 
            tsWebDriver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsWebDriver.Name = "tsWebDriver";
            tsWebDriver.Size = new System.Drawing.Size(53, 44);
            tsWebDriver.Text = "Web";
            tsWebDriver.ToolTipText = "Select Browser";
            // 
            // tsDropFileList
            // 
            tsDropFileList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsDropFileList.Image = (System.Drawing.Image)resources.GetObject("tsDropFileList.Image");
            tsDropFileList.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsDropFileList.Name = "tsDropFileList";
            tsDropFileList.Size = new System.Drawing.Size(155, 44);
            tsDropFileList.Text = "Previous File Results";
            // 
            // toolStripStatus
            // 
            toolStripStatus.Name = "toolStripStatus";
            toolStripStatus.Size = new System.Drawing.Size(50, 40);
            toolStripStatus.Text = "Ready";
            // 
            // tsStatusLabel
            // 
            tsStatusLabel.Name = "tsStatusLabel";
            tsStatusLabel.Size = new System.Drawing.Size(0, 40);
            // 
            // tsUserName
            // 
            tsUserName.Name = "tsUserName";
            tsUserName.Size = new System.Drawing.Size(552, 40);
            tsUserName.Spring = true;
            tsUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            tsUserName.Visible = false;
            // 
            // cboWebsite
            // 
            cboWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
            cboWebsite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cboWebsite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboWebsite.FormattingEnabled = true;
            cboWebsite.Location = new System.Drawing.Point(164, 12);
            cboWebsite.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            cboWebsite.Name = "cboWebsite";
            cboWebsite.Size = new System.Drawing.Size(701, 28);
            cboWebsite.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 15);
            label1.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(62, 20);
            label1.TabIndex = 0;
            label1.Text = "Website";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(13, 77);
            label2.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(76, 20);
            label2.TabIndex = 1;
            label2.Text = "Start Date";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 139);
            label3.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(70, 20);
            label3.TabIndex = 7;
            label3.Text = "End Date";
            // 
            // txConsole
            // 
            tableLayoutPanel1.SetColumnSpan(txConsole, 2);
            txConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            txConsole.Location = new System.Drawing.Point(4, 439);
            txConsole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txConsole.Multiline = true;
            txConsole.Name = "txConsole";
            txConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txConsole.Size = new System.Drawing.Size(861, 219);
            txConsole.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(13, 201);
            label4.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(88, 20);
            label4.TabIndex = 9;
            label4.Text = "Search Type";
            // 
            // cboSearchType
            // 
            cboSearchType.Dock = System.Windows.Forms.DockStyle.Fill;
            cboSearchType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cboSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboSearchType.FormattingEnabled = true;
            cboSearchType.Location = new System.Drawing.Point(164, 198);
            cboSearchType.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            cboSearchType.Name = "cboSearchType";
            cboSearchType.Size = new System.Drawing.Size(701, 28);
            cboSearchType.TabIndex = 10;
            // 
            // labelCboCaseType
            // 
            labelCboCaseType.AutoSize = true;
            labelCboCaseType.Location = new System.Drawing.Point(13, 263);
            labelCboCaseType.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            labelCboCaseType.Name = "labelCboCaseType";
            labelCboCaseType.Size = new System.Drawing.Size(75, 20);
            labelCboCaseType.TabIndex = 11;
            labelCboCaseType.Text = "Case Type";
            // 
            // cboCaseType
            // 
            cboCaseType.Dock = System.Windows.Forms.DockStyle.Fill;
            cboCaseType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cboCaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboCaseType.FormattingEnabled = true;
            cboCaseType.Location = new System.Drawing.Point(164, 260);
            cboCaseType.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            cboCaseType.Name = "cboCaseType";
            cboCaseType.Size = new System.Drawing.Size(701, 28);
            cboCaseType.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(13, 325);
            label6.Margin = new System.Windows.Forms.Padding(13, 15, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(75, 20);
            label6.TabIndex = 11;
            label6.Text = "Case Type";
            // 
            // cboCourts
            // 
            cboCourts.Dock = System.Windows.Forms.DockStyle.Fill;
            cboCourts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cboCourts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboCourts.FormattingEnabled = true;
            cboCourts.Location = new System.Drawing.Point(164, 322);
            cboCourts.Margin = new System.Windows.Forms.Padding(4, 12, 4, 5);
            cboCourts.Name = "cboCourts";
            cboCourts.Size = new System.Drawing.Size(701, 28);
            cboCourts.TabIndex = 13;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(869, 734);
            Controls.Add(tableLayoutPanel1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "FormMain";
            Text = "Document Search";
            Load += FormMain_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.DateTimePicker dteStart;
        internal System.Windows.Forms.DateTimePicker dteEnding;
        internal System.Windows.Forms.Button button1;
        internal System.Windows.Forms.StatusStrip statusStrip1;
        internal LegalLead.PublicData.Search.Classes.ComboBoxEx cboWebsite;
        internal System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txConsole;
        internal System.Windows.Forms.Label label4;
        internal LegalLead.PublicData.Search.Classes.ComboBoxEx cboSearchType;
        internal System.Windows.Forms.Label labelCboCaseType;
        internal LegalLead.PublicData.Search.Classes.ComboBoxEx cboCaseType;
        internal System.Windows.Forms.Label label6;
        internal LegalLead.PublicData.Search.Classes.ComboBoxEx cboCourts;
        internal System.Windows.Forms.Button ButtonDentonSetting;
        internal System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        internal System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.ToolStripDropDownButton tsDropFileList;
        private System.Windows.Forms.ToolStripDropDownButton tsWebDriver;
        private System.Windows.Forms.ToolStripStatusLabel tsUserName;
    }
}


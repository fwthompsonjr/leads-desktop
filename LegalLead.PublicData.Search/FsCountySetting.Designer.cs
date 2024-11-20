namespace LegalLead.PublicData.Search
{
    partial class FsCountySetting
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnSubmit = new System.Windows.Forms.Button();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            ColCounty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ColEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ColUserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ColPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ColButton = new System.Windows.Forms.DataGridViewButtonColumn();
            lbStatus = new System.Windows.Forms.Label();
            txUserName = new System.Windows.Forms.TextBox();
            txPassword = new System.Windows.Forms.MaskedTextBox();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 0, 2);
            tableLayoutPanel1.Controls.Add(dataGridView1, 0, 1);
            tableLayoutPanel1.Controls.Add(lbStatus, 0, 3);
            tableLayoutPanel1.Controls.Add(txUserName, 1, 2);
            tableLayoutPanel1.Controls.Add(txPassword, 2, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new System.Drawing.Size(800, 521);
            tableLayoutPanel1.TabIndex = 2;
            tableLayoutPanel1.Tag = "UserName";
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(10, 410);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(120, 27);
            btnSubmit.TabIndex = 42;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += BtnSubmit_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ColCounty, ColEnabled, ColUserID, ColPassword, ColButton });
            tableLayoutPanel1.SetColumnSpan(dataGridView1, 5);
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(3, 23);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 24;
            dataGridView1.Size = new System.Drawing.Size(794, 375);
            dataGridView1.TabIndex = 41;
            // 
            // ColCounty
            // 
            ColCounty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            ColCounty.DataPropertyName = "CountyName";
            ColCounty.HeaderText = "County";
            ColCounty.Name = "ColCounty";
            ColCounty.ReadOnly = true;
            ColCounty.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColEnabled
            // 
            ColEnabled.DataPropertyName = "IsEnabled";
            ColEnabled.HeaderText = "Enabled";
            ColEnabled.Name = "ColEnabled";
            ColEnabled.ReadOnly = true;
            // 
            // ColUserID
            // 
            ColUserID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            ColUserID.DataPropertyName = "UserName";
            ColUserID.HeaderText = "User Name";
            ColUserID.Name = "ColUserID";
            ColUserID.ReadOnly = true;
            ColUserID.Width = 90;
            // 
            // ColPassword
            // 
            ColPassword.DataPropertyName = "UserPassword";
            ColPassword.HeaderText = "Password";
            ColPassword.Name = "ColPassword";
            ColPassword.ReadOnly = true;
            // 
            // ColButton
            // 
            ColButton.DataPropertyName = "ButtonText";
            ColButton.HeaderText = "Change";
            ColButton.Name = "ColButton";
            ColButton.ReadOnly = true;
            ColButton.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbStatus, 5);
            lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lbStatus.Location = new System.Drawing.Point(10, 455);
            lbStatus.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(780, 27);
            lbStatus.TabIndex = 14;
            lbStatus.Text = "Ready";
            // 
            // txUserName
            // 
            txUserName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txUserName.Location = new System.Drawing.Point(150, 412);
            txUserName.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            txUserName.MaxLength = 250;
            txUserName.Name = "txUserName";
            txUserName.Size = new System.Drawing.Size(180, 23);
            txUserName.TabIndex = 43;
            // 
            // txPassword
            // 
            txPassword.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txPassword.Location = new System.Drawing.Point(350, 412);
            txPassword.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            txPassword.Name = "txPassword";
            txPassword.PasswordChar = '*';
            txPassword.Size = new System.Drawing.Size(180, 23);
            txPassword.TabIndex = 44;
            // 
            // FsCountySetting
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 521);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FsCountySetting";
            ShowInTaskbar = false;
            Text = "FsCountySetting";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCounty;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColUserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColPassword;
        private System.Windows.Forms.DataGridViewButtonColumn ColButton;
        private System.Windows.Forms.TextBox txUserName;
        private System.Windows.Forms.MaskedTextBox txPassword;
    }
}
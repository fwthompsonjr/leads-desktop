namespace LegalLead.PublicData.Search
{
    partial class FsOfflineHistory
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
            btnReturn = new System.Windows.Forms.Button();
            lbStatus = new System.Windows.Forms.Label();
            grid = new System.Windows.Forms.DataGridView();
            lbRecordCount = new System.Windows.Forms.Label();
            lbInvoiceName = new System.Windows.Forms.Label();
            lbProcessed = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 0, 2);
            tableLayoutPanel1.Controls.Add(btnReturn, 0, 2);
            tableLayoutPanel1.Controls.Add(lbStatus, 0, 3);
            tableLayoutPanel1.Controls.Add(grid, 0, 1);
            tableLayoutPanel1.Controls.Add(lbRecordCount, 4, 2);
            tableLayoutPanel1.Controls.Add(lbInvoiceName, 3, 2);
            tableLayoutPanel1.Controls.Add(lbProcessed, 4, 3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            tableLayoutPanel1.TabIndex = 6;
            tableLayoutPanel1.Tag = "UserName";
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(10, 369);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(120, 27);
            btnSubmit.TabIndex = 42;
            btnSubmit.Text = "Reload";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += BtnSubmit_Click;
            // 
            // btnReturn
            // 
            btnReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            btnReturn.Location = new System.Drawing.Point(150, 369);
            btnReturn.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new System.Drawing.Size(120, 27);
            btnReturn.TabIndex = 42;
            btnReturn.Text = "Return";
            btnReturn.UseVisualStyleBackColor = true;
            btnReturn.Click += Button_Click;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbStatus, 4);
            lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lbStatus.Location = new System.Drawing.Point(10, 414);
            lbStatus.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(580, 27);
            lbStatus.TabIndex = 14;
            lbStatus.Text = "Ready";
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(grid, 6);
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.Location = new System.Drawing.Point(3, 11);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            grid.Size = new System.Drawing.Size(794, 346);
            grid.TabIndex = 45;
            // 
            // lbRecordCount
            // 
            lbRecordCount.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbRecordCount.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbRecordCount, 2);
            lbRecordCount.Location = new System.Drawing.Point(603, 375);
            lbRecordCount.Name = "lbRecordCount";
            lbRecordCount.Size = new System.Drawing.Size(194, 15);
            lbRecordCount.TabIndex = 47;
            lbRecordCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbInvoiceName
            // 
            lbInvoiceName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbInvoiceName.AutoSize = true;
            lbInvoiceName.Location = new System.Drawing.Point(423, 375);
            lbInvoiceName.Name = "lbInvoiceName";
            lbInvoiceName.Size = new System.Drawing.Size(174, 15);
            lbInvoiceName.TabIndex = 50;
            // 
            // lbProcessed
            // 
            lbProcessed.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbProcessed, 2);
            lbProcessed.Dock = System.Windows.Forms.DockStyle.Fill;
            lbProcessed.Location = new System.Drawing.Point(610, 414);
            lbProcessed.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbProcessed.Name = "lbProcessed";
            lbProcessed.Size = new System.Drawing.Size(180, 27);
            lbProcessed.TabIndex = 51;
            // 
            // FsOfflineHistory
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "FsOfflineHistory";
            Text = "FsOfllineHistory";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Label lbRecordCount;
        private System.Windows.Forms.Label lbInvoiceName;
        private System.Windows.Forms.Label lbProcessed;
        private System.Windows.Forms.Button btnReturn;
    }
}
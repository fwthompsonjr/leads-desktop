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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FsOfflineHistory));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnSubmit = new System.Windows.Forms.Button();
            lbStatus = new System.Windows.Forms.Label();
            grid = new System.Windows.Forms.DataGridView();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            tssbFilterCounty = new System.Windows.Forms.ToolStripSplitButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tssbSort = new System.Windows.Forms.ToolStripSplitButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tslbDataStatus = new System.Windows.Forms.ToolStripLabel();
            lbRecordCount = new System.Windows.Forms.Label();
            btnDownload = new System.Windows.Forms.Button();
            lbInvoiceName = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            toolStrip1.SuspendLayout();
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
            tableLayoutPanel1.Controls.Add(btnSubmit, 0, 4);
            tableLayoutPanel1.Controls.Add(lbStatus, 0, 5);
            tableLayoutPanel1.Controls.Add(grid, 0, 2);
            tableLayoutPanel1.Controls.Add(toolStrip1, 0, 0);
            tableLayoutPanel1.Controls.Add(lbRecordCount, 4, 4);
            tableLayoutPanel1.Controls.Add(btnDownload, 1, 4);
            tableLayoutPanel1.Controls.Add(lbInvoiceName, 3, 4);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            tableLayoutPanel1.TabIndex = 6;
            tableLayoutPanel1.Tag = "UserName";
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(10, 339);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(120, 27);
            btnSubmit.TabIndex = 42;
            btnSubmit.Text = "Reload";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbStatus, 6);
            lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lbStatus.Location = new System.Drawing.Point(10, 384);
            lbStatus.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(780, 27);
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
            grid.Location = new System.Drawing.Point(3, 38);
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            grid.Size = new System.Drawing.Size(794, 269);
            grid.TabIndex = 45;
            // 
            // toolStrip1
            // 
            toolStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(toolStrip1, 6);
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssbFilterCounty, toolStripSeparator1, tssbSort, toolStripSeparator2, tslbDataStatus });
            toolStrip1.Location = new System.Drawing.Point(0, 2);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 46;
            toolStrip1.Text = "toolStrip1";
            // 
            // tssbFilterCounty
            // 
            tssbFilterCounty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tssbFilterCounty.Image = (System.Drawing.Image)resources.GetObject("tssbFilterCounty.Image");
            tssbFilterCounty.ImageTransparentColor = System.Drawing.Color.White;
            tssbFilterCounty.Name = "tssbFilterCounty";
            tssbFilterCounty.Size = new System.Drawing.Size(32, 22);
            tssbFilterCounty.Text = "Filter";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            toolStripSeparator1.Visible = false;
            // 
            // tssbSort
            // 
            tssbSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tssbSort.Image = (System.Drawing.Image)resources.GetObject("tssbSort.Image");
            tssbSort.ImageTransparentColor = System.Drawing.Color.White;
            tssbSort.Name = "tssbSort";
            tssbSort.Size = new System.Drawing.Size(32, 22);
            tssbSort.Text = "Sort";
            tssbSort.Visible = false;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tslbDataStatus
            // 
            tslbDataStatus.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            tslbDataStatus.Name = "tslbDataStatus";
            tslbDataStatus.Size = new System.Drawing.Size(50, 22);
            tslbDataStatus.Text = "Loading";
            // 
            // lbRecordCount
            // 
            lbRecordCount.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbRecordCount.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbRecordCount, 2);
            lbRecordCount.Location = new System.Drawing.Point(603, 345);
            lbRecordCount.Name = "lbRecordCount";
            lbRecordCount.Size = new System.Drawing.Size(194, 15);
            lbRecordCount.TabIndex = 47;
            lbRecordCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnDownload
            // 
            btnDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            btnDownload.Enabled = false;
            btnDownload.Location = new System.Drawing.Point(150, 339);
            btnDownload.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new System.Drawing.Size(120, 27);
            btnDownload.TabIndex = 49;
            btnDownload.Text = "Download Records";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Visible = false;
            // 
            // lbInvoiceName
            // 
            lbInvoiceName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbInvoiceName.AutoSize = true;
            lbInvoiceName.Location = new System.Drawing.Point(423, 345);
            lbInvoiceName.Name = "lbInvoiceName";
            lbInvoiceName.Size = new System.Drawing.Size(174, 15);
            lbInvoiceName.TabIndex = 50;
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
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton tssbFilterCounty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSplitButton tssbSort;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel tslbDataStatus;
        private System.Windows.Forms.Label lbRecordCount;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label lbInvoiceName;
    }
}
namespace LegalLead.PublicData.Search
{
    partial class FsInvoiceHistory
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FsInvoiceHistory));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnSubmit = new System.Windows.Forms.Button();
            lbStatus = new System.Windows.Forms.Label();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            tssbFilterCounty = new System.Windows.Forms.ToolStripSplitButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tssbSort = new System.Windows.Forms.ToolStripSplitButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tslbDataStatus = new System.Windows.Forms.ToolStripLabel();
            lbRecordCount = new System.Windows.Forms.Label();
            imageList1 = new System.Windows.Forms.ImageList(components);
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 0, 4);
            tableLayoutPanel1.Controls.Add(lbStatus, 0, 5);
            tableLayoutPanel1.Controls.Add(dataGridView1, 0, 2);
            tableLayoutPanel1.Controls.Add(toolStrip1, 0, 0);
            tableLayoutPanel1.Controls.Add(lbRecordCount, 3, 4);
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
            tableLayoutPanel1.TabIndex = 5;
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
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbStatus, 5);
            lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lbStatus.Location = new System.Drawing.Point(10, 384);
            lbStatus.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(780, 27);
            lbStatus.TabIndex = 14;
            lbStatus.Text = "Ready";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(dataGridView1, 5);
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(3, 38);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new System.Drawing.Size(794, 269);
            dataGridView1.TabIndex = 45;
            // 
            // toolStrip1
            // 
            toolStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.SetColumnSpan(toolStrip1, 5);
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
            toolStripSeparator2.Size = new System.Drawing.Size(6, 10);
            // 
            // tslbDataStatus
            // 
            tslbDataStatus.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            tslbDataStatus.Name = "tslbDataStatus";
            tslbDataStatus.Size = new System.Drawing.Size(50, 7);
            tslbDataStatus.Text = "Loading";
            // 
            // lbRecordCount
            // 
            lbRecordCount.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbRecordCount.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbRecordCount, 2);
            lbRecordCount.Location = new System.Drawing.Point(483, 345);
            lbRecordCount.Name = "lbRecordCount";
            lbRecordCount.Size = new System.Drawing.Size(314, 15);
            lbRecordCount.TabIndex = 47;
            lbRecordCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imageList1.ImageSize = new System.Drawing.Size(16, 16);
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FsInvoiceHistory
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "FsInvoiceHistory";
            Text = "FsInvoiceHistory";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton tssbFilterCounty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSplitButton tssbSort;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel tslbDataStatus;
        private System.Windows.Forms.Label lbRecordCount;
        private System.Windows.Forms.ImageList imageList1;
    }
}
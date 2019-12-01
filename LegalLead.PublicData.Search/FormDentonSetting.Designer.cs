namespace LegalLead.PublicData.Search
{
    partial class FormDentonSetting
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCaseSearchType = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboCaseSearchType = new System.Windows.Forms.ComboBox();
            this.cboCourtListA = new System.Windows.Forms.ComboBox();
            this.cboCourtListB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboDistrictSearchType = new System.Windows.Forms.ComboBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelCaseSearchType, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cboCaseSearchType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cboCourtListA, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cboCourtListB, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cboDistrictSearchType, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonSave, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(483, 313);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelCaseSearchType
            // 
            this.labelCaseSearchType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelCaseSearchType.AutoSize = true;
            this.labelCaseSearchType.Location = new System.Drawing.Point(3, 22);
            this.labelCaseSearchType.Name = "labelCaseSearchType";
            this.labelCaseSearchType.Size = new System.Drawing.Size(125, 17);
            this.labelCaseSearchType.TabIndex = 0;
            this.labelCaseSearchType.Tag = "JP District";
            this.labelCaseSearchType.Text = "Case Search Type";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 17);
            this.label2.TabIndex = 1;
            this.label2.Tag = "JP";
            this.label2.Text = "JP and County Courts";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 17);
            this.label3.TabIndex = 2;
            this.label3.Tag = "District";
            this.label3.Text = "District Courts";
            // 
            // cboCaseSearchType
            // 
            this.cboCaseSearchType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCaseSearchType.FormattingEnabled = true;
            this.cboCaseSearchType.Location = new System.Drawing.Point(168, 18);
            this.cboCaseSearchType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCaseSearchType.Name = "cboCaseSearchType";
            this.cboCaseSearchType.Size = new System.Drawing.Size(312, 24);
            this.cboCaseSearchType.TabIndex = 3;
            this.cboCaseSearchType.Tag = "JP District";
            this.cboCaseSearchType.SelectedIndexChanged += new System.EventHandler(this.CboCaseSearchType_SelectedIndexChanged);
            // 
            // cboCourtListA
            // 
            this.cboCourtListA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCourtListA.FormattingEnabled = true;
            this.cboCourtListA.Location = new System.Drawing.Point(168, 80);
            this.cboCourtListA.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCourtListA.Name = "cboCourtListA";
            this.cboCourtListA.Size = new System.Drawing.Size(312, 24);
            this.cboCourtListA.TabIndex = 4;
            this.cboCourtListA.Tag = "JP";
            // 
            // cboCourtListB
            // 
            this.cboCourtListB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCourtListB.FormattingEnabled = true;
            this.cboCourtListB.Location = new System.Drawing.Point(168, 142);
            this.cboCourtListB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboCourtListB.Name = "cboCourtListB";
            this.cboCourtListB.Size = new System.Drawing.Size(312, 24);
            this.cboCourtListB.TabIndex = 5;
            this.cboCourtListB.Tag = "District";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 208);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 17);
            this.label1.TabIndex = 6;
            this.label1.Tag = "District";
            this.label1.Text = "District Search Type";
            // 
            // cboDistrictSearchType
            // 
            this.cboDistrictSearchType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDistrictSearchType.FormattingEnabled = true;
            this.cboDistrictSearchType.Location = new System.Drawing.Point(168, 204);
            this.cboDistrictSearchType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboDistrictSearchType.Name = "cboDistrictSearchType";
            this.cboDistrictSearchType.Size = new System.Drawing.Size(312, 24);
            this.cboDistrictSearchType.TabIndex = 7;
            this.cboDistrictSearchType.Tag = "District";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonSave.Location = new System.Drawing.Point(169, 265);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 28);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormDentonSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 313);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormDentonSetting";
            this.Text = "Denton County Settings";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelCaseSearchType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboCaseSearchType;
        private System.Windows.Forms.ComboBox cboCourtListA;
        private System.Windows.Forms.ComboBox cboCourtListB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDistrictSearchType;
        private System.Windows.Forms.Button buttonSave;
    }
}
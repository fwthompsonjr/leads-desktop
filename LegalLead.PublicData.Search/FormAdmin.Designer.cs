namespace LegalLead.PublicData.Search
{
    partial class FormAdmin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdmin));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            toolStrip = new System.Windows.Forms.ToolStrip();
            tsReturn = new System.Windows.Forms.ToolStripButton();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tsContext = new System.Windows.Forms.ToolStripStatusLabel();
            tsSeparator = new System.Windows.Forms.ToolStripStatusLabel();
            tsPosition = new System.Windows.Forms.ToolStripStatusLabel();
            tsButtonSetBilling = new System.Windows.Forms.ToolStripSplitButton();
            testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            prodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer = new System.Windows.Forms.SplitContainer();
            gridUsers = new System.Windows.Forms.DataGridView();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            cboUserAction = new System.Windows.Forms.ComboBox();
            lbAction = new System.Windows.Forms.Label();
            grid = new System.Windows.Forms.DataGridView();
            label1 = new System.Windows.Forms.Label();
            lbUserName = new System.Windows.Forms.Label();
            buttonSaveChanges = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUsers).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(toolStrip, 0, 0);
            tableLayoutPanel1.Controls.Add(statusStrip1, 0, 2);
            tableLayoutPanel1.Controls.Add(splitContainer, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tableLayoutPanel1.Size = new System.Drawing.Size(700, 474);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip
            // 
            toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsReturn });
            toolStrip.Location = new System.Drawing.Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new System.Drawing.Size(700, 30);
            toolStrip.TabIndex = 0;
            toolStrip.Text = "toolStrip";
            // 
            // tsReturn
            // 
            tsReturn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsReturn.Image = (System.Drawing.Image)resources.GetObject("tsReturn.Image");
            tsReturn.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsReturn.Name = "tsReturn";
            tsReturn.Padding = new System.Windows.Forms.Padding(2);
            tsReturn.Size = new System.Drawing.Size(50, 27);
            tsReturn.Text = "Return";
            tsReturn.Click += TsReturn_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsContext, tsSeparator, tsPosition, tsButtonSetBilling });
            statusStrip1.Location = new System.Drawing.Point(0, 452);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            statusStrip1.Size = new System.Drawing.Size(700, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsContext
            // 
            tsContext.Name = "tsContext";
            tsContext.Padding = new System.Windows.Forms.Padding(2);
            tsContext.Size = new System.Drawing.Size(53, 17);
            tsContext.Text = "Context";
            // 
            // tsSeparator
            // 
            tsSeparator.Margin = new System.Windows.Forms.Padding(2);
            tsSeparator.Name = "tsSeparator";
            tsSeparator.Size = new System.Drawing.Size(10, 18);
            tsSeparator.Text = "|";
            // 
            // tsPosition
            // 
            tsPosition.Name = "tsPosition";
            tsPosition.Padding = new System.Windows.Forms.Padding(2);
            tsPosition.Size = new System.Drawing.Size(54, 17);
            tsPosition.Text = "Position";
            // 
            // tsButtonSetBilling
            // 
            tsButtonSetBilling.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsButtonSetBilling.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { testToolStripMenuItem, prodToolStripMenuItem });
            tsButtonSetBilling.Image = (System.Drawing.Image)resources.GetObject("tsButtonSetBilling.Image");
            tsButtonSetBilling.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsButtonSetBilling.Name = "tsButtonSetBilling";
            tsButtonSetBilling.Size = new System.Drawing.Size(75, 20);
            tsButtonSetBilling.Text = "Set Billing";
            tsButtonSetBilling.ToolTipText = "Set Billing Mode";
            tsButtonSetBilling.Visible = false;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            testToolStripMenuItem.Tag = "TEST";
            testToolStripMenuItem.Text = "TEST";
            // 
            // prodToolStripMenuItem
            // 
            prodToolStripMenuItem.Name = "prodToolStripMenuItem";
            prodToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            prodToolStripMenuItem.Tag = "PROD";
            prodToolStripMenuItem.Text = "LIVE";
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(3, 32);
            splitContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.AutoScroll = true;
            splitContainer.Panel1.Controls.Add(gridUsers);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.AutoScroll = true;
            splitContainer.Panel2.Controls.Add(tableLayoutPanel2);
            splitContainer.Size = new System.Drawing.Size(694, 418);
            splitContainer.SplitterDistance = 295;
            splitContainer.SplitterWidth = 3;
            splitContainer.TabIndex = 2;
            // 
            // gridUsers
            // 
            gridUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            gridUsers.Location = new System.Drawing.Point(0, 0);
            gridUsers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            gridUsers.Name = "gridUsers";
            gridUsers.RowHeadersWidth = 51;
            gridUsers.Size = new System.Drawing.Size(694, 295);
            gridUsers.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel2.Controls.Add(cboUserAction, 1, 0);
            tableLayoutPanel2.Controls.Add(lbAction, 0, 0);
            tableLayoutPanel2.Controls.Add(grid, 0, 2);
            tableLayoutPanel2.Controls.Add(label1, 0, 1);
            tableLayoutPanel2.Controls.Add(lbUserName, 1, 1);
            tableLayoutPanel2.Controls.Add(buttonSaveChanges, 2, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(694, 120);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // cboUserAction
            // 
            cboUserAction.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel2.SetColumnSpan(cboUserAction, 2);
            cboUserAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboUserAction.FormattingEnabled = true;
            cboUserAction.Location = new System.Drawing.Point(91, 2);
            cboUserAction.Margin = new System.Windows.Forms.Padding(3, 2, 3, 4);
            cboUserAction.Name = "cboUserAction";
            cboUserAction.Size = new System.Drawing.Size(600, 23);
            cboUserAction.TabIndex = 0;
            // 
            // lbAction
            // 
            lbAction.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbAction.AutoSize = true;
            lbAction.Location = new System.Drawing.Point(3, 3);
            lbAction.Name = "lbAction";
            lbAction.Size = new System.Drawing.Size(82, 15);
            lbAction.TabIndex = 1;
            lbAction.Text = "Select view:";
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel2.SetColumnSpan(grid, 3);
            grid.Dock = System.Windows.Forms.DockStyle.Fill;
            grid.Location = new System.Drawing.Point(4, 56);
            grid.Margin = new System.Windows.Forms.Padding(4);
            grid.Name = "grid";
            grid.RowHeadersWidth = 51;
            grid.Size = new System.Drawing.Size(686, 60);
            grid.TabIndex = 2;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 29);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(82, 15);
            label1.TabIndex = 3;
            label1.Text = "User:";
            // 
            // lbUserName
            // 
            lbUserName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbUserName.AutoSize = true;
            lbUserName.Location = new System.Drawing.Point(91, 29);
            lbUserName.Name = "lbUserName";
            lbUserName.Size = new System.Drawing.Size(520, 15);
            lbUserName.TabIndex = 4;
            lbUserName.Text = "-";
            // 
            // buttonSaveChanges
            // 
            buttonSaveChanges.Location = new System.Drawing.Point(617, 25);
            buttonSaveChanges.Name = "buttonSaveChanges";
            buttonSaveChanges.Size = new System.Drawing.Size(74, 23);
            buttonSaveChanges.TabIndex = 5;
            buttonSaveChanges.Text = "Save";
            buttonSaveChanges.UseVisualStyleBackColor = true;
            buttonSaveChanges.Visible = false;
            // 
            // FormAdmin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(700, 474);
            Controls.Add(tableLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "FormAdmin";
            Text = "FormAdmin";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUsers).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripButton tsReturn;
        private System.Windows.Forms.ToolStripStatusLabel tsContext;
        private System.Windows.Forms.ToolStripStatusLabel tsSeparator;
        private System.Windows.Forms.ToolStripStatusLabel tsPosition;
        private System.Windows.Forms.DataGridView gridUsers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox cboUserAction;
        private System.Windows.Forms.Label lbAction;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbUserName;
        private System.Windows.Forms.Button buttonSaveChanges;
        private System.Windows.Forms.ToolStripSplitButton tsButtonSetBilling;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prodToolStripMenuItem;
    }
}
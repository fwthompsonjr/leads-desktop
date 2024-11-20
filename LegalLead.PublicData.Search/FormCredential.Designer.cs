namespace LegalLead.PublicData.Search
{
    partial class FormCredential
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCredential));
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnSubmit = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            cboWebsite = new System.Windows.Forms.ComboBox();
            tbxUser = new System.Windows.Forms.TextBox();
            tbxPwd = new System.Windows.Forms.MaskedTextBox();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 1, 3);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(cboWebsite, 1, 0);
            tableLayoutPanel1.Controls.Add(tbxUser, 1, 1);
            tableLayoutPanel1.Controls.Add(tbxPwd, 1, 2);
            tableLayoutPanel1.Controls.Add(dataGridView1, 1, 4);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(564, 369);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(119, 150);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(378, 29);
            btnSubmit.TabIndex = 9;
            btnSubmit.Text = "Change Password";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 9);
            label1.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(49, 15);
            label1.TabIndex = 0;
            label1.Text = "Website";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 56);
            label2.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 15);
            label2.TabIndex = 1;
            label2.Text = "User Id:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 103);
            label3.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(60, 15);
            label3.TabIndex = 2;
            label3.Text = "Password:";
            // 
            // cboWebsite
            // 
            cboWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
            cboWebsite.FormattingEnabled = true;
            cboWebsite.Location = new System.Drawing.Point(119, 9);
            cboWebsite.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            cboWebsite.Name = "cboWebsite";
            cboWebsite.Size = new System.Drawing.Size(378, 23);
            cboWebsite.TabIndex = 3;
            // 
            // tbxUser
            // 
            tbxUser.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxUser.Location = new System.Drawing.Point(119, 56);
            tbxUser.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxUser.Name = "tbxUser";
            tbxUser.Size = new System.Drawing.Size(378, 23);
            tbxUser.TabIndex = 4;
            // 
            // tbxPwd
            // 
            tbxPwd.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxPwd.Location = new System.Drawing.Point(119, 103);
            tbxPwd.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxPwd.Name = "tbxPwd";
            tbxPwd.PasswordChar = '*';
            tbxPwd.Size = new System.Drawing.Size(378, 23);
            tbxPwd.TabIndex = 5;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView1.Location = new System.Drawing.Point(112, 191);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 24;
            dataGridView1.Size = new System.Drawing.Size(392, 175);
            dataGridView1.TabIndex = 11;
            // 
            // FormCredential
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(564, 369);
            Controls.Add(tableLayoutPanel1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            Name = "FormCredential";
            ShowInTaskbar = false;
            Text = "Manage Credential";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboWebsite;
        private System.Windows.Forms.TextBox tbxUser;
        private System.Windows.Forms.MaskedTextBox tbxPwd;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
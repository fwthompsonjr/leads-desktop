namespace LegalLead.PublicData.Search
{
    partial class FormLogin
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
            label3 = new System.Windows.Forms.Label();
            tbxUser = new System.Windows.Forms.TextBox();
            tbxPwd = new System.Windows.Forms.MaskedTextBox();
            label2 = new System.Windows.Forms.Label();
            labelSts = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 1, 2);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(tbxUser, 1, 0);
            tableLayoutPanel1.Controls.Add(tbxPwd, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(labelSts, 1, 3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            tableLayoutPanel1.Size = new System.Drawing.Size(382, 233);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(111, 138);
            btnSubmit.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(235, 39);
            btnSubmit.TabIndex = 9;
            btnSubmit.Text = "Login";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += BtnSubmit_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(11, 75);
            label3.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(73, 20);
            label3.TabIndex = 2;
            label3.Text = "Password:";
            // 
            // tbxUser
            // 
            tbxUser.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxUser.Location = new System.Drawing.Point(111, 12);
            tbxUser.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            tbxUser.Name = "tbxUser";
            tbxUser.Size = new System.Drawing.Size(235, 27);
            tbxUser.TabIndex = 4;
            // 
            // tbxPwd
            // 
            tbxPwd.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxPwd.Location = new System.Drawing.Point(111, 75);
            tbxPwd.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            tbxPwd.Name = "tbxPwd";
            tbxPwd.PasswordChar = '*';
            tbxPwd.Size = new System.Drawing.Size(235, 27);
            tbxPwd.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(11, 12);
            label2.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(58, 20);
            label2.TabIndex = 1;
            label2.Text = "User Id:";
            // 
            // labelSts
            // 
            labelSts.AutoSize = true;
            labelSts.Location = new System.Drawing.Point(103, 189);
            labelSts.Name = "labelSts";
            labelSts.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            labelSts.Size = new System.Drawing.Size(165, 30);
            labelSts.TabIndex = 10;
            labelSts.Text = "Please enter credentials";
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(382, 233);
            Controls.Add(tableLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Login";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxUser;
        private System.Windows.Forms.MaskedTextBox tbxPwd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSts;
    }
}
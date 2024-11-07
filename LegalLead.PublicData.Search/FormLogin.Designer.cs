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
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            tableLayoutPanel1.Controls.Add(btnSubmit, 1, 2);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(tbxUser, 1, 0);
            tableLayoutPanel1.Controls.Add(tbxPwd, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            tableLayoutPanel1.Size = new System.Drawing.Size(364, 161);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(90, 103);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(207, 29);
            btnSubmit.TabIndex = 9;
            btnSubmit.Text = "Login";
            btnSubmit.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 56);
            label3.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(60, 15);
            label3.TabIndex = 2;
            label3.Text = "Password:";
            // 
            // tbxUser
            // 
            tbxUser.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxUser.Location = new System.Drawing.Point(90, 9);
            tbxUser.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxUser.Name = "tbxUser";
            tbxUser.Size = new System.Drawing.Size(207, 23);
            tbxUser.TabIndex = 4;
            // 
            // tbxPwd
            // 
            tbxPwd.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxPwd.Location = new System.Drawing.Point(90, 56);
            tbxPwd.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxPwd.Name = "tbxPwd";
            tbxPwd.PasswordChar = '*';
            tbxPwd.Size = new System.Drawing.Size(207, 23);
            tbxPwd.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 9);
            label2.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 15);
            label2.TabIndex = 1;
            label2.Text = "User Id:";
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(364, 161);
            Controls.Add(tableLayoutPanel1);
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
    }
}
﻿namespace LegalLead.PublicData.Search
{
    partial class FormCreateAccount
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
            tbxConfirmPwd = new System.Windows.Forms.MaskedTextBox();
            txUserName = new System.Windows.Forms.TextBox();
            btnSubmit = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            tbxPwd = new System.Windows.Forms.MaskedTextBox();
            label4 = new System.Windows.Forms.Label();
            lbStatus = new System.Windows.Forms.Label();
            txEmail = new System.Windows.Forms.TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(tbxConfirmPwd, 1, 3);
            tableLayoutPanel1.Controls.Add(txUserName, 1, 0);
            tableLayoutPanel1.Controls.Add(btnSubmit, 1, 4);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(tbxPwd, 1, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(lbStatus, 0, 5);
            tableLayoutPanel1.Controls.Add(txEmail, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            tableLayoutPanel1.TabIndex = 2;
            tableLayoutPanel1.Tag = "UserName";
            // 
            // tbxConfirmPwd
            // 
            tbxConfirmPwd.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxConfirmPwd.Location = new System.Drawing.Point(150, 144);
            tbxConfirmPwd.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxConfirmPwd.Name = "tbxConfirmPwd";
            tbxConfirmPwd.PasswordChar = '*';
            tbxConfirmPwd.Size = new System.Drawing.Size(560, 23);
            tbxConfirmPwd.TabIndex = 30;
            tbxConfirmPwd.Tag = "ConfirmPassword";
            // 
            // txUserName
            // 
            txUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            txUserName.Location = new System.Drawing.Point(150, 10);
            txUserName.Margin = new System.Windows.Forms.Padding(10);
            txUserName.MaxLength = 255;
            txUserName.Name = "txUserName";
            txUserName.Size = new System.Drawing.Size(560, 23);
            txUserName.TabIndex = 1;
            // 
            // btnSubmit
            // 
            btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            btnSubmit.Location = new System.Drawing.Point(150, 189);
            btnSubmit.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(560, 37);
            btnSubmit.TabIndex = 40;
            btnSubmit.Text = "Create Account";
            btnSubmit.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 9);
            label1.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 15);
            label1.TabIndex = 0;
            label1.Text = "User Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(10, 54);
            label2.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 1;
            label2.Text = "Email:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 99);
            label3.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(60, 15);
            label3.TabIndex = 2;
            label3.Text = "Password:";
            // 
            // tbxPwd
            // 
            tbxPwd.Dock = System.Windows.Forms.DockStyle.Fill;
            tbxPwd.Location = new System.Drawing.Point(150, 99);
            tbxPwd.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            tbxPwd.Name = "tbxPwd";
            tbxPwd.PasswordChar = '*';
            tbxPwd.Size = new System.Drawing.Size(560, 23);
            tbxPwd.TabIndex = 20;
            tbxPwd.Tag = "NewPassword";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(10, 144);
            label4.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(107, 15);
            label4.TabIndex = 13;
            label4.Text = "Confirm Password:";
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(lbStatus, 3);
            lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lbStatus.Location = new System.Drawing.Point(10, 244);
            lbStatus.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(780, 167);
            lbStatus.TabIndex = 14;
            lbStatus.Text = "Ready";
            // 
            // txEmail
            // 
            txEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            txEmail.Location = new System.Drawing.Point(150, 55);
            txEmail.Margin = new System.Windows.Forms.Padding(10);
            txEmail.MaxLength = 255;
            txEmail.Name = "txEmail";
            txEmail.Size = new System.Drawing.Size(560, 23);
            txEmail.TabIndex = 2;
            // 
            // FormCreateAccount
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "FormCreateAccount";
            Text = "FormCreateAccount";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MaskedTextBox tbxConfirmPwd;
        private System.Windows.Forms.TextBox txUserName;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox tbxPwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.TextBox txEmail;
    }
}
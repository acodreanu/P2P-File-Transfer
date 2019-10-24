namespace WindowsFormsApp1
{
    partial class Form1
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
            this.IP_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Chat_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Send_button = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.FileTransferWorker = new System.ComponentModel.BackgroundWorker();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.encrypt_checkbox = new System.Windows.Forms.CheckBox();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.pass_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // IP_textBox
            // 
            this.IP_textBox.Location = new System.Drawing.Point(68, 31);
            this.IP_textBox.Name = "IP_textBox";
            this.IP_textBox.Size = new System.Drawing.Size(100, 20);
            this.IP_textBox.TabIndex = 0;
            this.IP_textBox.TextChanged += new System.EventHandler(this.IP_textBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(401, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Not Connected";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(183, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(325, 34);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 5;
            // 
            // Chat_textBox
            // 
            this.Chat_textBox.AcceptsReturn = true;
            this.Chat_textBox.AcceptsTab = true;
            this.Chat_textBox.Location = new System.Drawing.Point(68, 57);
            this.Chat_textBox.Multiline = true;
            this.Chat_textBox.Name = "Chat_textBox";
            this.Chat_textBox.Size = new System.Drawing.Size(100, 20);
            this.Chat_textBox.TabIndex = 6;
            this.Chat_textBox.TextChanged += new System.EventHandler(this.Chat_textBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Chat";
            // 
            // Send_button
            // 
            this.Send_button.Location = new System.Drawing.Point(183, 55);
            this.Send_button.Name = "Send_button";
            this.Send_button.Size = new System.Drawing.Size(75, 23);
            this.Send_button.TabIndex = 8;
            this.Send_button.Text = "Send";
            this.Send_button.UseVisualStyleBackColor = true;
            this.Send_button.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.AcceptsReturn = true;
            this.textBox3.AcceptsTab = true;
            this.textBox3.Location = new System.Drawing.Point(30, 84);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(450, 133);
            this.textBox3.TabIndex = 9;
            this.textBox3.TextChanged += new System.EventHandler(this.BigChat_textBox_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(405, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "File Transfer";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.FileTransfer_button_Click);
            // 
            // FileTransferWorker
            // 
            this.FileTransferWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FileTransferWorker_DoWork);
            this.FileTransferWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.FileTransferWorker_ProgressChanged);
            this.FileTransferWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FileTransferWorker_Completed);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(382, 34);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(0, 13);
            this.ProgressLabel.TabIndex = 11;
            this.ProgressLabel.Visible = false;
            // 
            // encrypt_checkbox
            // 
            this.encrypt_checkbox.AutoSize = true;
            this.encrypt_checkbox.Location = new System.Drawing.Point(293, 33);
            this.encrypt_checkbox.Name = "encrypt_checkbox";
            this.encrypt_checkbox.Size = new System.Drawing.Size(106, 17);
            this.encrypt_checkbox.TabIndex = 12;
            this.encrypt_checkbox.Text = "Encrypt";
            this.encrypt_checkbox.UseVisualStyleBackColor = true;
            this.encrypt_checkbox.CheckedChanged += new System.EventHandler(this.encrypt_checkbox_CheckedChanged);
            // 
            // password_textBox
            // 
            this.password_textBox.Location = new System.Drawing.Point(293, 57);
            this.password_textBox.Name = "password_textBox";
            this.password_textBox.Size = new System.Drawing.Size(100, 20);
            this.password_textBox.TabIndex = 13;
            this.password_textBox.Visible = false;
            this.password_textBox.TextChanged += new System.EventHandler(this.password_textBox_TextChanged);
            // 
            // pass_label
            // 
            this.pass_label.AutoSize = true;
            this.pass_label.Location = new System.Drawing.Point(258, 60);
            this.pass_label.Name = "pass_label";
            this.pass_label.Size = new System.Drawing.Size(29, 13);
            this.pass_label.TabIndex = 14;
            this.pass_label.Text = "pass";
            this.pass_label.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 229);
            this.Controls.Add(this.pass_label);
            this.Controls.Add(this.password_textBox);
            this.Controls.Add(this.encrypt_checkbox);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.Send_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Chat_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.IP_textBox);
            this.Name = "Form1";
            this.Text = "P2P File Transfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IP_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Chat_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Send_button;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.BackgroundWorker FileTransferWorker;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.CheckBox encrypt_checkbox;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.Label pass_label;
    }
}


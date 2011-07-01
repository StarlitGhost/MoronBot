namespace MoronBot
{
    partial class formMoronBot
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
            this.txtProgLog = new System.Windows.Forms.TextBox();
            this.splitPanelTextFields = new System.Windows.Forms.SplitContainer();
            this.txtIRC = new System.Windows.Forms.TextBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelChannelsAndUsers = new System.Windows.Forms.Panel();
            this.splitLists = new System.Windows.Forms.SplitContainer();
            this.listChannels = new System.Windows.Forms.ListBox();
            this.listUsers = new System.Windows.Forms.ListBox();
            this.splitPanelTextFields.Panel1.SuspendLayout();
            this.splitPanelTextFields.Panel2.SuspendLayout();
            this.splitPanelTextFields.SuspendLayout();
            this.panelChannelsAndUsers.SuspendLayout();
            this.splitLists.Panel1.SuspendLayout();
            this.splitLists.Panel2.SuspendLayout();
            this.splitLists.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtProgLog
            // 
            this.txtProgLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProgLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtProgLog.Font = new System.Drawing.Font("Inconsolata", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProgLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtProgLog.Location = new System.Drawing.Point(3, 3);
            this.txtProgLog.Multiline = true;
            this.txtProgLog.Name = "txtProgLog";
            this.txtProgLog.ReadOnly = true;
            this.txtProgLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProgLog.Size = new System.Drawing.Size(573, 130);
            this.txtProgLog.TabIndex = 0;
            this.txtProgLog.Text = "<Raw IRC>";
            this.txtProgLog.WordWrap = false;
            this.txtProgLog.TextChanged += new System.EventHandler(this.txtProgLog_TextChanged);
            // 
            // splitPanelTextFields
            // 
            this.splitPanelTextFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitPanelTextFields.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitPanelTextFields.Location = new System.Drawing.Point(12, 12);
            this.splitPanelTextFields.Name = "splitPanelTextFields";
            this.splitPanelTextFields.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitPanelTextFields.Panel1
            // 
            this.splitPanelTextFields.Panel1.Controls.Add(this.txtProgLog);
            // 
            // splitPanelTextFields.Panel2
            // 
            this.splitPanelTextFields.Panel2.Controls.Add(this.txtIRC);
            this.splitPanelTextFields.Panel2.Controls.Add(this.txtInput);
            this.splitPanelTextFields.Size = new System.Drawing.Size(579, 391);
            this.splitPanelTextFields.SplitterDistance = 136;
            this.splitPanelTextFields.TabIndex = 1;
            // 
            // txtIRC
            // 
            this.txtIRC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIRC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtIRC.Font = new System.Drawing.Font("Inconsolata", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIRC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtIRC.Location = new System.Drawing.Point(3, 3);
            this.txtIRC.Multiline = true;
            this.txtIRC.Name = "txtIRC";
            this.txtIRC.ReadOnly = true;
            this.txtIRC.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtIRC.Size = new System.Drawing.Size(573, 219);
            this.txtIRC.TabIndex = 1;
            this.txtIRC.Text = "<IRC>";
            this.txtIRC.TextChanged += new System.EventHandler(this.txtIRC_TextChanged);
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtInput.Font = new System.Drawing.Font("Inconsolata", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtInput.Location = new System.Drawing.Point(4, 228);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(572, 23);
            this.txtInput.TabIndex = 0;
            this.txtInput.Text = "<Input>";
            this.txtInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyUp);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(3, 361);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(161, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(161, 23);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // panelChannelsAndUsers
            // 
            this.panelChannelsAndUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChannelsAndUsers.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelChannelsAndUsers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelChannelsAndUsers.Controls.Add(this.splitLists);
            this.panelChannelsAndUsers.Controls.Add(this.btnClear);
            this.panelChannelsAndUsers.Controls.Add(this.btnExit);
            this.panelChannelsAndUsers.Location = new System.Drawing.Point(597, 12);
            this.panelChannelsAndUsers.Name = "panelChannelsAndUsers";
            this.panelChannelsAndUsers.Size = new System.Drawing.Size(171, 391);
            this.panelChannelsAndUsers.TabIndex = 2;
            // 
            // splitLists
            // 
            this.splitLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitLists.Location = new System.Drawing.Point(4, 33);
            this.splitLists.Name = "splitLists";
            this.splitLists.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitLists.Panel1
            // 
            this.splitLists.Panel1.Controls.Add(this.listChannels);
            // 
            // splitLists.Panel2
            // 
            this.splitLists.Panel2.Controls.Add(this.listUsers);
            this.splitLists.Size = new System.Drawing.Size(160, 322);
            this.splitLists.SplitterDistance = 101;
            this.splitLists.TabIndex = 2;
            // 
            // listChannels
            // 
            this.listChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listChannels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listChannels.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listChannels.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listChannels.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.listChannels.FormattingEnabled = true;
            this.listChannels.ItemHeight = 14;
            this.listChannels.Location = new System.Drawing.Point(3, 3);
            this.listChannels.Name = "listChannels";
            this.listChannels.Size = new System.Drawing.Size(154, 88);
            this.listChannels.TabIndex = 2;
            this.listChannels.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listChannels_DrawItem);
            this.listChannels.SelectedIndexChanged += new System.EventHandler(this.listChannels_SelectedIndexChanged);
            // 
            // listUsers
            // 
            this.listUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listUsers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listUsers.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.listUsers.FormattingEnabled = true;
            this.listUsers.ItemHeight = 14;
            this.listUsers.Location = new System.Drawing.Point(4, 4);
            this.listUsers.Name = "listUsers";
            this.listUsers.Size = new System.Drawing.Size(153, 200);
            this.listUsers.TabIndex = 0;
            this.listUsers.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listUsers_DrawItem);
            // 
            // formMoronBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(780, 415);
            this.Controls.Add(this.panelChannelsAndUsers);
            this.Controls.Add(this.splitPanelTextFields);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "formMoronBot";
            this.Text = "MoronBot";
            this.Load += new System.EventHandler(this.formMoronBot_Load);
            this.splitPanelTextFields.Panel1.ResumeLayout(false);
            this.splitPanelTextFields.Panel1.PerformLayout();
            this.splitPanelTextFields.Panel2.ResumeLayout(false);
            this.splitPanelTextFields.Panel2.PerformLayout();
            this.splitPanelTextFields.ResumeLayout(false);
            this.panelChannelsAndUsers.ResumeLayout(false);
            this.splitLists.Panel1.ResumeLayout(false);
            this.splitLists.Panel2.ResumeLayout(false);
            this.splitLists.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtProgLog;
        private System.Windows.Forms.SplitContainer splitPanelTextFields;
        private System.Windows.Forms.TextBox txtIRC;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelChannelsAndUsers;
        private System.Windows.Forms.ListBox listChannels;
        private System.Windows.Forms.SplitContainer splitLists;
        private System.Windows.Forms.ListBox listUsers;
    }
}


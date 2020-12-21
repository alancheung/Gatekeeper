namespace GatekeeperCSharp
{
    partial class GatekeeperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GatekeeperForm));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.AdminTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.Admin_ExitButton = new System.Windows.Forms.Button();
            this.Admin_DebugButton = new System.Windows.Forms.Button();
            this.AddPasswordButton = new System.Windows.Forms.Button();
            this.LoadPasswordButton = new System.Windows.Forms.Button();
            this.TriggerRfidButton = new System.Windows.Forms.Button();
            this.RightTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.ZeroButton = new System.Windows.Forms.Button();
            this.AdminButton = new System.Windows.Forms.Button();
            this.OneButton = new System.Windows.Forms.Button();
            this.TwoButton = new System.Windows.Forms.Button();
            this.ThreeButton = new System.Windows.Forms.Button();
            this.FourButton = new System.Windows.Forms.Button();
            this.FiveButton = new System.Windows.Forms.Button();
            this.SixButton = new System.Windows.Forms.Button();
            this.SevenButton = new System.Windows.Forms.Button();
            this.EightButton = new System.Windows.Forms.Button();
            this.NineButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.AddCardButton = new System.Windows.Forms.Button();
            this.MainPanel.SuspendLayout();
            this.AdminTablePanel.SuspendLayout();
            this.RightTablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.AdminTablePanel);
            this.MainPanel.Controls.Add(this.RightTablePanel);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(800, 494);
            this.MainPanel.TabIndex = 0;
            // 
            // AdminTablePanel
            // 
            this.AdminTablePanel.ColumnCount = 4;
            this.AdminTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AdminTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AdminTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AdminTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.AdminTablePanel.Controls.Add(this.Admin_ExitButton, 0, 0);
            this.AdminTablePanel.Controls.Add(this.Admin_DebugButton, 1, 0);
            this.AdminTablePanel.Controls.Add(this.AddPasswordButton, 0, 4);
            this.AdminTablePanel.Controls.Add(this.TriggerRfidButton, 3, 0);
            this.AdminTablePanel.Controls.Add(this.LoadPasswordButton, 0, 3);
            this.AdminTablePanel.Controls.Add(this.AddCardButton, 1, 4);
            this.AdminTablePanel.Location = new System.Drawing.Point(0, 0);
            this.AdminTablePanel.Name = "AdminTablePanel";
            this.AdminTablePanel.RowCount = 5;
            this.AdminTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.AdminTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.AdminTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.AdminTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.AdminTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.AdminTablePanel.Size = new System.Drawing.Size(400, 480);
            this.AdminTablePanel.TabIndex = 1;
            this.AdminTablePanel.Visible = false;
            // 
            // Admin_ExitButton
            // 
            this.Admin_ExitButton.BackColor = System.Drawing.Color.Red;
            this.Admin_ExitButton.Location = new System.Drawing.Point(3, 3);
            this.Admin_ExitButton.Name = "Admin_ExitButton";
            this.Admin_ExitButton.Size = new System.Drawing.Size(94, 90);
            this.Admin_ExitButton.TabIndex = 1;
            this.Admin_ExitButton.Text = "Final Exit";
            this.Admin_ExitButton.UseVisualStyleBackColor = false;
            this.Admin_ExitButton.Click += new System.EventHandler(this.Admin_ExitButton_Click);
            // 
            // Admin_DebugButton
            // 
            this.Admin_DebugButton.BackColor = System.Drawing.Color.Transparent;
            this.Admin_DebugButton.Location = new System.Drawing.Point(103, 3);
            this.Admin_DebugButton.Name = "Admin_DebugButton";
            this.Admin_DebugButton.Size = new System.Drawing.Size(94, 90);
            this.Admin_DebugButton.TabIndex = 0;
            this.Admin_DebugButton.Text = "Debug";
            this.Admin_DebugButton.UseVisualStyleBackColor = false;
            this.Admin_DebugButton.Click += new System.EventHandler(this.Admin_DebugButton_Click);
            // 
            // AddPasswordButton
            // 
            this.AddPasswordButton.Location = new System.Drawing.Point(3, 387);
            this.AddPasswordButton.Name = "AddPasswordButton";
            this.AddPasswordButton.Size = new System.Drawing.Size(94, 90);
            this.AddPasswordButton.TabIndex = 2;
            this.AddPasswordButton.Text = "Add New Password";
            this.AddPasswordButton.UseVisualStyleBackColor = true;
            this.AddPasswordButton.Click += new System.EventHandler(this.Admin_AddNewPasswordButton_Click);
            // 
            // LoadPasswordButton
            // 
            this.LoadPasswordButton.Location = new System.Drawing.Point(3, 291);
            this.LoadPasswordButton.Name = "LoadPasswordButton";
            this.LoadPasswordButton.Size = new System.Drawing.Size(94, 90);
            this.LoadPasswordButton.TabIndex = 4;
            this.LoadPasswordButton.Text = "Load Passwords";
            this.LoadPasswordButton.UseVisualStyleBackColor = true;
            this.LoadPasswordButton.Click += new System.EventHandler(this.Admin_LoadPasswordButton_Click);
            // 
            // TriggerRfidButton
            // 
            this.TriggerRfidButton.Location = new System.Drawing.Point(303, 3);
            this.TriggerRfidButton.Name = "TriggerRfidButton";
            this.TriggerRfidButton.Size = new System.Drawing.Size(94, 90);
            this.TriggerRfidButton.TabIndex = 5;
            this.TriggerRfidButton.Text = "Trigger RFID";
            this.TriggerRfidButton.UseVisualStyleBackColor = true;
            this.TriggerRfidButton.Click += new System.EventHandler(this.Admin_TriggerRfidButton_Click);
            // 
            // RightTablePanel
            // 
            this.RightTablePanel.ColumnCount = 3;
            this.RightTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.RightTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.RightTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.RightTablePanel.Controls.Add(this.SubmitButton, 0, 5);
            this.RightTablePanel.Controls.Add(this.ClearButton, 0, 4);
            this.RightTablePanel.Controls.Add(this.ZeroButton, 1, 4);
            this.RightTablePanel.Controls.Add(this.AdminButton, 2, 4);
            this.RightTablePanel.Controls.Add(this.OneButton, 0, 3);
            this.RightTablePanel.Controls.Add(this.TwoButton, 1, 3);
            this.RightTablePanel.Controls.Add(this.ThreeButton, 2, 3);
            this.RightTablePanel.Controls.Add(this.FourButton, 0, 2);
            this.RightTablePanel.Controls.Add(this.FiveButton, 1, 2);
            this.RightTablePanel.Controls.Add(this.SixButton, 2, 2);
            this.RightTablePanel.Controls.Add(this.SevenButton, 0, 1);
            this.RightTablePanel.Controls.Add(this.EightButton, 1, 1);
            this.RightTablePanel.Controls.Add(this.NineButton, 2, 1);
            this.RightTablePanel.Controls.Add(this.StatusLabel, 0, 0);
            this.RightTablePanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.RightTablePanel.Location = new System.Drawing.Point(400, 0);
            this.RightTablePanel.Name = "RightTablePanel";
            this.RightTablePanel.RowCount = 6;
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.RightTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.RightTablePanel.Size = new System.Drawing.Size(400, 480);
            this.RightTablePanel.TabIndex = 0;
            // 
            // SubmitButton
            // 
            this.SubmitButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.RightTablePanel.SetColumnSpan(this.SubmitButton, 3);
            this.SubmitButton.Location = new System.Drawing.Point(3, 407);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(394, 71);
            this.SubmitButton.TabIndex = 0;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = false;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClearButton.Location = new System.Drawing.Point(3, 330);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(127, 70);
            this.ClearButton.TabIndex = 1;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = false;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // ZeroButton
            // 
            this.ZeroButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ZeroButton.Location = new System.Drawing.Point(136, 330);
            this.ZeroButton.Name = "ZeroButton";
            this.ZeroButton.Size = new System.Drawing.Size(127, 70);
            this.ZeroButton.TabIndex = 2;
            this.ZeroButton.Text = "0";
            this.ZeroButton.UseVisualStyleBackColor = false;
            this.ZeroButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // AdminButton
            // 
            this.AdminButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.AdminButton.Location = new System.Drawing.Point(269, 330);
            this.AdminButton.Name = "AdminButton";
            this.AdminButton.Size = new System.Drawing.Size(128, 70);
            this.AdminButton.TabIndex = 3;
            this.AdminButton.Text = "Admin";
            this.AdminButton.UseVisualStyleBackColor = false;
            this.AdminButton.Click += new System.EventHandler(this.AdminButton_Click);
            // 
            // OneButton
            // 
            this.OneButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.OneButton.Location = new System.Drawing.Point(3, 253);
            this.OneButton.Name = "OneButton";
            this.OneButton.Size = new System.Drawing.Size(127, 70);
            this.OneButton.TabIndex = 4;
            this.OneButton.Text = "1";
            this.OneButton.UseVisualStyleBackColor = false;
            this.OneButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // TwoButton
            // 
            this.TwoButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.TwoButton.Location = new System.Drawing.Point(136, 253);
            this.TwoButton.Name = "TwoButton";
            this.TwoButton.Size = new System.Drawing.Size(127, 70);
            this.TwoButton.TabIndex = 5;
            this.TwoButton.Text = "2";
            this.TwoButton.UseVisualStyleBackColor = false;
            this.TwoButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // ThreeButton
            // 
            this.ThreeButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ThreeButton.Location = new System.Drawing.Point(269, 253);
            this.ThreeButton.Name = "ThreeButton";
            this.ThreeButton.Size = new System.Drawing.Size(128, 70);
            this.ThreeButton.TabIndex = 6;
            this.ThreeButton.Text = "3";
            this.ThreeButton.UseVisualStyleBackColor = false;
            this.ThreeButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // FourButton
            // 
            this.FourButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.FourButton.Location = new System.Drawing.Point(3, 176);
            this.FourButton.Name = "FourButton";
            this.FourButton.Size = new System.Drawing.Size(127, 70);
            this.FourButton.TabIndex = 7;
            this.FourButton.Text = "4";
            this.FourButton.UseVisualStyleBackColor = false;
            this.FourButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // FiveButton
            // 
            this.FiveButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.FiveButton.Location = new System.Drawing.Point(136, 176);
            this.FiveButton.Name = "FiveButton";
            this.FiveButton.Size = new System.Drawing.Size(127, 70);
            this.FiveButton.TabIndex = 8;
            this.FiveButton.Text = "5";
            this.FiveButton.UseVisualStyleBackColor = false;
            this.FiveButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // SixButton
            // 
            this.SixButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.SixButton.Location = new System.Drawing.Point(269, 176);
            this.SixButton.Name = "SixButton";
            this.SixButton.Size = new System.Drawing.Size(128, 70);
            this.SixButton.TabIndex = 9;
            this.SixButton.Text = "6";
            this.SixButton.UseVisualStyleBackColor = false;
            this.SixButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // SevenButton
            // 
            this.SevenButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.SevenButton.Location = new System.Drawing.Point(3, 99);
            this.SevenButton.Name = "SevenButton";
            this.SevenButton.Size = new System.Drawing.Size(127, 70);
            this.SevenButton.TabIndex = 10;
            this.SevenButton.Text = "7";
            this.SevenButton.UseVisualStyleBackColor = false;
            this.SevenButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // EightButton
            // 
            this.EightButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.EightButton.Location = new System.Drawing.Point(136, 99);
            this.EightButton.Name = "EightButton";
            this.EightButton.Size = new System.Drawing.Size(127, 70);
            this.EightButton.TabIndex = 11;
            this.EightButton.Text = "8";
            this.EightButton.UseVisualStyleBackColor = false;
            this.EightButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // NineButton
            // 
            this.NineButton.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.NineButton.Location = new System.Drawing.Point(269, 99);
            this.NineButton.Name = "NineButton";
            this.NineButton.Size = new System.Drawing.Size(128, 70);
            this.NineButton.TabIndex = 12;
            this.NineButton.Text = "9";
            this.NineButton.UseVisualStyleBackColor = false;
            this.NineButton.Click += new System.EventHandler(this.NumberButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.StatusLabel.CausesValidation = false;
            this.RightTablePanel.SetColumnSpan(this.StatusLabel, 3);
            this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(3, 0);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(394, 96);
            this.StatusLabel.TabIndex = 13;
            this.StatusLabel.Text = "Initializing...";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AddCardButton
            // 
            this.AddCardButton.Location = new System.Drawing.Point(103, 387);
            this.AddCardButton.Name = "AddCardButton";
            this.AddCardButton.Size = new System.Drawing.Size(94, 90);
            this.AddCardButton.TabIndex = 6;
            this.AddCardButton.Text = "Add New RFID";
            this.AddCardButton.UseVisualStyleBackColor = true;
            this.AddCardButton.Click += new System.EventHandler(this.Admin_AddCardButton_Click);
            // 
            // GatekeeperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(812, 494);
            this.Controls.Add(this.MainPanel);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GatekeeperForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gatekeeper - DEBUG";
            this.MainPanel.ResumeLayout(false);
            this.AdminTablePanel.ResumeLayout(false);
            this.RightTablePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.TableLayoutPanel RightTablePanel;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button ZeroButton;
        private System.Windows.Forms.Button AdminButton;
        private System.Windows.Forms.Button OneButton;
        private System.Windows.Forms.Button TwoButton;
        private System.Windows.Forms.Button ThreeButton;
        private System.Windows.Forms.Button FourButton;
        private System.Windows.Forms.Button FiveButton;
        private System.Windows.Forms.Button SixButton;
        private System.Windows.Forms.Button SevenButton;
        private System.Windows.Forms.Button EightButton;
        private System.Windows.Forms.Button NineButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.TableLayoutPanel AdminTablePanel;
        private System.Windows.Forms.Button Admin_DebugButton;
        private System.Windows.Forms.Button Admin_ExitButton;
        private System.Windows.Forms.Button AddPasswordButton;
        private System.Windows.Forms.Button LoadPasswordButton;
        private System.Windows.Forms.Button TriggerRfidButton;
        private System.Windows.Forms.Button AddCardButton;
    }
}


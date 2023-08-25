
namespace neuopc
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.DAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.UAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainListView = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.type = new System.Windows.Forms.ColumnHeader();
            this.rights = new System.Windows.Forms.ColumnHeader();
            this.value = new System.Windows.Forms.ColumnHeader();
            this.quality = new System.Windows.Forms.ColumnHeader();
            this.error = new System.Windows.Forms.ColumnHeader();
            this.timestamp = new System.Windows.Forms.ColumnHeader();
            this.handle = new System.Windows.Forms.ColumnHeader();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPageSetting = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.DADomainTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.DAPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DAUserTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.UALabel = new System.Windows.Forms.Label();
            this.CheckBox = new System.Windows.Forms.CheckBox();
            this.SwitchButton = new System.Windows.Forms.Button();
            this.UAPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.UAPortTextBox = new System.Windows.Forms.TextBox();
            this.UAUserTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DAHostComboBox = new System.Windows.Forms.ComboBox();
            this.DAServerComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TestButton = new System.Windows.Forms.Button();
            this.TabPageData = new System.Windows.Forms.TabPage();
            this.TabPageLog = new System.Windows.Forms.TabPage();
            this.LogListView = new System.Windows.Forms.ListView();
            this.filename = new System.Windows.Forms.ColumnHeader();
            this.time = new System.Windows.Forms.ColumnHeader();
            this.length = new System.Windows.Forms.ColumnHeader();
            this.TabPageAbout = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabPageSetting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TabPageData.SuspendLayout();
            this.TabPageLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DAStatusLabel,
            this.UAStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 672);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1128, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // DAStatusLabel
            // 
            this.DAStatusLabel.Name = "DAStatusLabel";
            this.DAStatusLabel.Size = new System.Drawing.Size(0, 15);
            // 
            // UAStatusLabel
            // 
            this.UAStatusLabel.Name = "UAStatusLabel";
            this.UAStatusLabel.Size = new System.Drawing.Size(0, 15);
            // 
            // MainListView
            // 
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.type,
            this.rights,
            this.value,
            this.quality,
            this.error,
            this.timestamp,
            this.handle});
            this.MainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainListView.HideSelection = false;
            this.MainListView.Location = new System.Drawing.Point(3, 3);
            this.MainListView.Name = "MainListView";
            this.MainListView.Size = new System.Drawing.Size(1114, 626);
            this.MainListView.TabIndex = 7;
            this.MainListView.UseCompatibleStateImageBehavior = false;
            this.MainListView.View = System.Windows.Forms.View.Details;
            this.MainListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainListView_MouseDoubleClick);
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 250;
            // 
            // type
            // 
            this.type.Text = "Type";
            this.type.Width = 100;
            // 
            // rights
            // 
            this.rights.Text = "Rights";
            this.rights.Width = 100;
            // 
            // value
            // 
            this.value.Text = "Value";
            this.value.Width = 250;
            // 
            // quality
            // 
            this.quality.Text = "Quality";
            this.quality.Width = 80;
            // 
            // error
            // 
            this.error.Text = "Error";
            this.error.Width = 80;
            // 
            // timestamp
            // 
            this.timestamp.Text = "Timestamp";
            this.timestamp.Width = 200;
            // 
            // handle
            // 
            this.handle.Text = "Index";
            this.handle.Width = 0;
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Text = "neuopc";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.Click += new System.EventHandler(this.NotifyIcon_Click);
            // 
            // TabControl
            // 
            this.TabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.TabControl.Controls.Add(this.TabPageSetting);
            this.TabControl.Controls.Add(this.TabPageData);
            this.TabControl.Controls.Add(this.TabPageLog);
            this.TabControl.Controls.Add(this.TabPageAbout);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1128, 672);
            this.TabControl.TabIndex = 8;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // TabPageSetting
            // 
            this.TabPageSetting.Controls.Add(this.groupBox1);
            this.TabPageSetting.Location = new System.Drawing.Point(4, 36);
            this.TabPageSetting.Name = "TabPageSetting";
            this.TabPageSetting.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageSetting.Size = new System.Drawing.Size(1120, 632);
            this.TabPageSetting.TabIndex = 0;
            this.TabPageSetting.Text = "Setting";
            this.TabPageSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.DADomainTextBox);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.DAPasswordTextBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.DAUserTextBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.SaveButton);
            this.groupBox1.Controls.Add(this.UALabel);
            this.groupBox1.Controls.Add(this.CheckBox);
            this.groupBox1.Controls.Add(this.SwitchButton);
            this.groupBox1.Controls.Add(this.UAPasswordTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.UAPortTextBox);
            this.groupBox1.Controls.Add(this.UAUserTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.DAHostComboBox);
            this.groupBox1.Controls.Add(this.DAServerComboBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TestButton);
            this.groupBox1.Location = new System.Drawing.Point(9, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1102, 618);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label14.Location = new System.Drawing.Point(730, 302);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(18, 24);
            this.label14.TabIndex = 47;
            this.label14.Text = "*";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label13.Location = new System.Drawing.Point(730, 210);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(20, 24);
            this.label13.TabIndex = 46;
            this.label13.Text = "  ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label12.Location = new System.Drawing.Point(730, 162);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(20, 24);
            this.label12.TabIndex = 45;
            this.label12.Text = "  ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label11.Location = new System.Drawing.Point(730, 117);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 24);
            this.label11.TabIndex = 44;
            this.label11.Text = "  ";
            // 
            // DADomainTextBox
            // 
            this.DADomainTextBox.Location = new System.Drawing.Point(226, 207);
            this.DADomainTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.DADomainTextBox.Name = "DADomainTextBox";
            this.DADomainTextBox.Size = new System.Drawing.Size(498, 30);
            this.DADomainTextBox.TabIndex = 43;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 210);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(157, 24);
            this.label10.TabIndex = 42;
            this.label10.Text = "OPC DA Domain:";
            // 
            // DAPasswordTextBox
            // 
            this.DAPasswordTextBox.Location = new System.Drawing.Point(226, 159);
            this.DAPasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.DAPasswordTextBox.Name = "DAPasswordTextBox";
            this.DAPasswordTextBox.Size = new System.Drawing.Size(498, 30);
            this.DAPasswordTextBox.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 162);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(170, 24);
            this.label9.TabIndex = 40;
            this.label9.Text = "OPC DA Password:";
            // 
            // DAUserTextBox
            // 
            this.DAUserTextBox.Location = new System.Drawing.Point(226, 117);
            this.DAUserTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.DAUserTextBox.Name = "DAUserTextBox";
            this.DAUserTextBox.Size = new System.Drawing.Size(498, 30);
            this.DAUserTextBox.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 120);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(127, 24);
            this.label8.TabIndex = 38;
            this.label8.Text = "OPC DA User:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label7.Location = new System.Drawing.Point(730, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 24);
            this.label7.TabIndex = 37;
            this.label7.Text = "*";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label6.Location = new System.Drawing.Point(730, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 24);
            this.label6.TabIndex = 36;
            this.label6.Text = "*";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(564, 429);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(160, 34);
            this.SaveButton.TabIndex = 35;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // UALabel
            // 
            this.UALabel.AutoSize = true;
            this.UALabel.Location = new System.Drawing.Point(226, 120);
            this.UALabel.Name = "UALabel";
            this.UALabel.Size = new System.Drawing.Size(0, 24);
            this.UALabel.TabIndex = 34;
            // 
            // CheckBox
            // 
            this.CheckBox.AutoSize = true;
            this.CheckBox.Location = new System.Drawing.Point(209, 433);
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Size = new System.Drawing.Size(178, 28);
            this.CheckBox.TabIndex = 33;
            this.CheckBox.Text = "Auto connection";
            this.CheckBox.UseVisualStyleBackColor = true;
            // 
            // SwitchButton
            // 
            this.SwitchButton.Location = new System.Drawing.Point(393, 429);
            this.SwitchButton.Name = "SwitchButton";
            this.SwitchButton.Size = new System.Drawing.Size(160, 34);
            this.SwitchButton.TabIndex = 32;
            this.SwitchButton.Text = "Start";
            this.SwitchButton.UseVisualStyleBackColor = true;
            this.SwitchButton.Click += new System.EventHandler(this.SwitchButton_Click);
            // 
            // UAPasswordTextBox
            // 
            this.UAPasswordTextBox.Location = new System.Drawing.Point(226, 384);
            this.UAPasswordTextBox.Name = "UAPasswordTextBox";
            this.UAPasswordTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAPasswordTextBox.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 390);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 24);
            this.label5.TabIndex = 30;
            this.label5.Text = "OPC UA Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 302);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 24);
            this.label3.TabIndex = 27;
            this.label3.Text = "OPC UA Port:";
            // 
            // UAPortTextBox
            // 
            this.UAPortTextBox.Location = new System.Drawing.Point(226, 302);
            this.UAPortTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.UAPortTextBox.Name = "UAPortTextBox";
            this.UAPortTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAPortTextBox.TabIndex = 25;
            // 
            // UAUserTextBox
            // 
            this.UAUserTextBox.Location = new System.Drawing.Point(226, 344);
            this.UAUserTextBox.Name = "UAUserTextBox";
            this.UAUserTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAUserTextBox.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 347);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 24);
            this.label4.TabIndex = 28;
            this.label4.Text = "OPC UA User:";
            // 
            // DAHostComboBox
            // 
            this.DAHostComboBox.FormattingEnabled = true;
            this.DAHostComboBox.Location = new System.Drawing.Point(226, 23);
            this.DAHostComboBox.Name = "DAHostComboBox";
            this.DAHostComboBox.Size = new System.Drawing.Size(498, 32);
            this.DAHostComboBox.TabIndex = 0;
            this.DAHostComboBox.DropDown += new System.EventHandler(this.DAHostComboBox_DropDown);
            // 
            // DAServerComboBox
            // 
            this.DAServerComboBox.FormattingEnabled = true;
            this.DAServerComboBox.Location = new System.Drawing.Point(226, 65);
            this.DAServerComboBox.Name = "DAServerComboBox";
            this.DAServerComboBox.Size = new System.Drawing.Size(498, 32);
            this.DAServerComboBox.TabIndex = 1;
            this.DAServerComboBox.DropDown += new System.EventHandler(this.DAServerComboBox_DropDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 69);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "OPC DA Server:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "OPC DA Host:";
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(564, 253);
            this.TestButton.Margin = new System.Windows.Forms.Padding(2);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(160, 34);
            this.TestButton.TabIndex = 2;
            this.TestButton.Text = "Connection Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // TabPageData
            // 
            this.TabPageData.Controls.Add(this.MainListView);
            this.TabPageData.Location = new System.Drawing.Point(4, 36);
            this.TabPageData.Name = "TabPageData";
            this.TabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageData.Size = new System.Drawing.Size(1120, 632);
            this.TabPageData.TabIndex = 1;
            this.TabPageData.Text = "Data view";
            this.TabPageData.UseVisualStyleBackColor = true;
            // 
            // TabPageLog
            // 
            this.TabPageLog.Controls.Add(this.LogListView);
            this.TabPageLog.Location = new System.Drawing.Point(4, 36);
            this.TabPageLog.Name = "TabPageLog";
            this.TabPageLog.Size = new System.Drawing.Size(1120, 632);
            this.TabPageLog.TabIndex = 3;
            this.TabPageLog.Text = "Log";
            this.TabPageLog.UseVisualStyleBackColor = true;
            // 
            // LogListView
            // 
            this.LogListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filename,
            this.time,
            this.length});
            this.LogListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogListView.HideSelection = false;
            this.LogListView.Location = new System.Drawing.Point(0, 0);
            this.LogListView.Name = "LogListView";
            this.LogListView.Size = new System.Drawing.Size(1120, 632);
            this.LogListView.TabIndex = 0;
            this.LogListView.UseCompatibleStateImageBehavior = false;
            this.LogListView.View = System.Windows.Forms.View.Details;
            this.LogListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LogListView_MouseDoubleClick);
            // 
            // filename
            // 
            this.filename.Text = "Name";
            this.filename.Width = 300;
            // 
            // time
            // 
            this.time.Text = "Time";
            this.time.Width = 200;
            // 
            // length
            // 
            this.length.Text = "Length";
            this.length.Width = 200;
            // 
            // TabPageAbout
            // 
            this.TabPageAbout.Location = new System.Drawing.Point(4, 36);
            this.TabPageAbout.Name = "TabPageAbout";
            this.TabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageAbout.Size = new System.Drawing.Size(1120, 632);
            this.TabPageAbout.TabIndex = 2;
            this.TabPageAbout.Text = "About";
            this.TabPageAbout.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 694);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NeuOPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.TabControl.ResumeLayout(false);
            this.TabPageSetting.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.TabPageData.ResumeLayout(false);
            this.TabPageLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel DAStatusLabel;
        private System.Windows.Forms.ListView MainListView;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.ColumnHeader handle;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader quality;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.ColumnHeader rights;
        private System.Windows.Forms.ColumnHeader error;
        private System.Windows.Forms.ColumnHeader timestamp;
        private System.Windows.Forms.ColumnHeader type;
        private System.Windows.Forms.ToolStripStatusLabel UAStatusLabel;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage TabPageSetting;
        private System.Windows.Forms.TabPage TabPageData;
        private System.Windows.Forms.TabPage TabPageAbout;
        private System.Windows.Forms.TabPage TabPageWriteLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SwitchButton;
        private System.Windows.Forms.TextBox UAPasswordTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UAPortTextBox;
        private System.Windows.Forms.TextBox UAUserTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox DAHostComboBox;
        private System.Windows.Forms.ComboBox DAServerComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.CheckBox CheckBox;
        private System.Windows.Forms.Label UALabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TabPage TabPageLog;
        private System.Windows.Forms.ListView LogListView;
        private System.Windows.Forms.ColumnHeader filename;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.ColumnHeader length;
        private System.Windows.Forms.TextBox DAPasswordTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox DAUserTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox DADomainTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
    }
}


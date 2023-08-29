
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
            components = new System.ComponentModel.Container();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            DAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            UAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            MainListView = new System.Windows.Forms.ListView();
            name = new System.Windows.Forms.ColumnHeader();
            type = new System.Windows.Forms.ColumnHeader();
            rights = new System.Windows.Forms.ColumnHeader();
            value = new System.Windows.Forms.ColumnHeader();
            quality = new System.Windows.Forms.ColumnHeader();
            error = new System.Windows.Forms.ColumnHeader();
            timestamp = new System.Windows.Forms.ColumnHeader();
            handle = new System.Windows.Forms.ColumnHeader();
            NotifyIcon = new System.Windows.Forms.NotifyIcon(components);
            TabControl = new System.Windows.Forms.TabControl();
            TabPageSetting = new System.Windows.Forms.TabPage();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label14 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            DADomainTextBox = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            DAPasswordTextBox = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            DAUserTextBox = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            SaveButton = new System.Windows.Forms.Button();
            UALabel = new System.Windows.Forms.Label();
            CheckBox = new System.Windows.Forms.CheckBox();
            SwitchButton = new System.Windows.Forms.Button();
            UAPasswordTextBox = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            UAPortTextBox = new System.Windows.Forms.TextBox();
            UAUserTextBox = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            DAHostComboBox = new System.Windows.Forms.ComboBox();
            DAServerComboBox = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            TestButton = new System.Windows.Forms.Button();
            TabPageData = new System.Windows.Forms.TabPage();
            TabPageLog = new System.Windows.Forms.TabPage();
            richTextBoxLogControl1 = new Serilog.Sinks.WinForms.Core.RichTextBoxLogControl();
            TabPageAbout = new System.Windows.Forms.TabPage();
            statusStrip1.SuspendLayout();
            TabControl.SuspendLayout();
            TabPageSetting.SuspendLayout();
            groupBox1.SuspendLayout();
            TabPageData.SuspendLayout();
            TabPageLog.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { DAStatusLabel, UAStatusLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 672);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            statusStrip1.Size = new System.Drawing.Size(1128, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // DAStatusLabel
            // 
            DAStatusLabel.Name = "DAStatusLabel";
            DAStatusLabel.Size = new System.Drawing.Size(0, 15);
            // 
            // UAStatusLabel
            // 
            UAStatusLabel.Name = "UAStatusLabel";
            UAStatusLabel.Size = new System.Drawing.Size(0, 15);
            // 
            // MainListView
            // 
            MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { name, type, rights, value, quality, error, timestamp, handle });
            MainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            MainListView.Location = new System.Drawing.Point(3, 3);
            MainListView.Name = "MainListView";
            MainListView.Size = new System.Drawing.Size(1114, 626);
            MainListView.TabIndex = 7;
            MainListView.UseCompatibleStateImageBehavior = false;
            MainListView.View = System.Windows.Forms.View.Details;
            MainListView.MouseDoubleClick += MainListView_MouseDoubleClick;
            // 
            // name
            // 
            name.Text = "Name";
            name.Width = 250;
            // 
            // type
            // 
            type.Text = "Type";
            type.Width = 100;
            // 
            // rights
            // 
            rights.Text = "Rights";
            rights.Width = 100;
            // 
            // value
            // 
            value.Text = "Value";
            value.Width = 250;
            // 
            // quality
            // 
            quality.Text = "Quality";
            quality.Width = 80;
            // 
            // error
            // 
            error.Text = "Error";
            error.Width = 80;
            // 
            // timestamp
            // 
            timestamp.Text = "Timestamp";
            timestamp.Width = 200;
            // 
            // handle
            // 
            handle.Text = "Index";
            handle.Width = 0;
            // 
            // NotifyIcon
            // 
            NotifyIcon.Text = "neuopc";
            NotifyIcon.Visible = true;
            NotifyIcon.Click += NotifyIcon_Click;
            // 
            // TabControl
            // 
            TabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            TabControl.Controls.Add(TabPageSetting);
            TabControl.Controls.Add(TabPageData);
            TabControl.Controls.Add(TabPageLog);
            TabControl.Controls.Add(TabPageAbout);
            TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            TabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TabControl.Location = new System.Drawing.Point(0, 0);
            TabControl.Name = "TabControl";
            TabControl.SelectedIndex = 0;
            TabControl.Size = new System.Drawing.Size(1128, 672);
            TabControl.TabIndex = 8;
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            // 
            // TabPageSetting
            // 
            TabPageSetting.Controls.Add(groupBox1);
            TabPageSetting.Location = new System.Drawing.Point(4, 36);
            TabPageSetting.Name = "TabPageSetting";
            TabPageSetting.Padding = new System.Windows.Forms.Padding(3);
            TabPageSetting.Size = new System.Drawing.Size(1120, 632);
            TabPageSetting.TabIndex = 0;
            TabPageSetting.Text = "Setting";
            TabPageSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(DADomainTextBox);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(DAPasswordTextBox);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(DAUserTextBox);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(SaveButton);
            groupBox1.Controls.Add(UALabel);
            groupBox1.Controls.Add(CheckBox);
            groupBox1.Controls.Add(SwitchButton);
            groupBox1.Controls.Add(UAPasswordTextBox);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(UAPortTextBox);
            groupBox1.Controls.Add(UAUserTextBox);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(DAHostComboBox);
            groupBox1.Controls.Add(DAServerComboBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(TestButton);
            groupBox1.Location = new System.Drawing.Point(9, 7);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(1102, 618);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label14.Location = new System.Drawing.Point(730, 302);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(18, 24);
            label14.TabIndex = 47;
            label14.Text = "*";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label13.Location = new System.Drawing.Point(730, 210);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(20, 24);
            label13.TabIndex = 46;
            label13.Text = "  ";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label12.Location = new System.Drawing.Point(730, 162);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(20, 24);
            label12.TabIndex = 45;
            label12.Text = "  ";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label11.Location = new System.Drawing.Point(730, 117);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(20, 24);
            label11.TabIndex = 44;
            label11.Text = "  ";
            // 
            // DADomainTextBox
            // 
            DADomainTextBox.Location = new System.Drawing.Point(226, 207);
            DADomainTextBox.Margin = new System.Windows.Forms.Padding(2);
            DADomainTextBox.Name = "DADomainTextBox";
            DADomainTextBox.Size = new System.Drawing.Size(498, 30);
            DADomainTextBox.TabIndex = 43;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(10, 210);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(157, 24);
            label10.TabIndex = 42;
            label10.Text = "OPC DA Domain:";
            // 
            // DAPasswordTextBox
            // 
            DAPasswordTextBox.Location = new System.Drawing.Point(226, 159);
            DAPasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
            DAPasswordTextBox.Name = "DAPasswordTextBox";
            DAPasswordTextBox.Size = new System.Drawing.Size(498, 30);
            DAPasswordTextBox.TabIndex = 41;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(10, 162);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(170, 24);
            label9.TabIndex = 40;
            label9.Text = "OPC DA Password:";
            // 
            // DAUserTextBox
            // 
            DAUserTextBox.Location = new System.Drawing.Point(226, 117);
            DAUserTextBox.Margin = new System.Windows.Forms.Padding(2);
            DAUserTextBox.Name = "DAUserTextBox";
            DAUserTextBox.Size = new System.Drawing.Size(498, 30);
            DAUserTextBox.TabIndex = 39;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(8, 120);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(127, 24);
            label8.TabIndex = 38;
            label8.Text = "OPC DA User:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label7.Location = new System.Drawing.Point(730, 68);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(18, 24);
            label7.TabIndex = 37;
            label7.Text = "*";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            label6.Location = new System.Drawing.Point(730, 26);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(18, 24);
            label6.TabIndex = 36;
            label6.Text = "*";
            // 
            // SaveButton
            // 
            SaveButton.Location = new System.Drawing.Point(564, 429);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(160, 34);
            SaveButton.TabIndex = 35;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // UALabel
            // 
            UALabel.AutoSize = true;
            UALabel.Location = new System.Drawing.Point(226, 120);
            UALabel.Name = "UALabel";
            UALabel.Size = new System.Drawing.Size(0, 24);
            UALabel.TabIndex = 34;
            // 
            // CheckBox
            // 
            CheckBox.AutoSize = true;
            CheckBox.Location = new System.Drawing.Point(209, 433);
            CheckBox.Name = "CheckBox";
            CheckBox.Size = new System.Drawing.Size(178, 28);
            CheckBox.TabIndex = 33;
            CheckBox.Text = "Auto connection";
            CheckBox.UseVisualStyleBackColor = true;
            // 
            // SwitchButton
            // 
            SwitchButton.Location = new System.Drawing.Point(393, 429);
            SwitchButton.Name = "SwitchButton";
            SwitchButton.Size = new System.Drawing.Size(160, 34);
            SwitchButton.TabIndex = 32;
            SwitchButton.Text = "Start";
            SwitchButton.UseVisualStyleBackColor = true;
            SwitchButton.Click += SwitchButton_Click;
            // 
            // UAPasswordTextBox
            // 
            UAPasswordTextBox.Location = new System.Drawing.Point(226, 384);
            UAPasswordTextBox.Name = "UAPasswordTextBox";
            UAPasswordTextBox.Size = new System.Drawing.Size(498, 30);
            UAPasswordTextBox.TabIndex = 29;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 390);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(169, 24);
            label5.TabIndex = 30;
            label5.Text = "OPC UA Password:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(8, 302);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(124, 24);
            label3.TabIndex = 27;
            label3.Text = "OPC UA Port:";
            // 
            // UAPortTextBox
            // 
            UAPortTextBox.Location = new System.Drawing.Point(226, 302);
            UAPortTextBox.Margin = new System.Windows.Forms.Padding(2);
            UAPortTextBox.Name = "UAPortTextBox";
            UAPortTextBox.Size = new System.Drawing.Size(498, 30);
            UAPortTextBox.TabIndex = 25;
            // 
            // UAUserTextBox
            // 
            UAUserTextBox.Location = new System.Drawing.Point(226, 344);
            UAUserTextBox.Name = "UAUserTextBox";
            UAUserTextBox.Size = new System.Drawing.Size(498, 30);
            UAUserTextBox.TabIndex = 26;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(8, 347);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(126, 24);
            label4.TabIndex = 28;
            label4.Text = "OPC UA User:";
            // 
            // DAHostComboBox
            // 
            DAHostComboBox.FormattingEnabled = true;
            DAHostComboBox.Location = new System.Drawing.Point(226, 23);
            DAHostComboBox.Name = "DAHostComboBox";
            DAHostComboBox.Size = new System.Drawing.Size(498, 32);
            DAHostComboBox.TabIndex = 0;
            DAHostComboBox.DropDown += DAHostComboBox_DropDown;
            // 
            // DAServerComboBox
            // 
            DAServerComboBox.FormattingEnabled = true;
            DAServerComboBox.Location = new System.Drawing.Point(226, 65);
            DAServerComboBox.Name = "DAServerComboBox";
            DAServerComboBox.Size = new System.Drawing.Size(498, 32);
            DAServerComboBox.TabIndex = 1;
            DAServerComboBox.DropDown += DAServerComboBox_DropDown;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 69);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(142, 24);
            label2.TabIndex = 6;
            label2.Text = "OPC DA Server:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 27);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(129, 24);
            label1.TabIndex = 3;
            label1.Text = "OPC DA Host:";
            // 
            // TestButton
            // 
            TestButton.Location = new System.Drawing.Point(564, 253);
            TestButton.Margin = new System.Windows.Forms.Padding(2);
            TestButton.Name = "TestButton";
            TestButton.Size = new System.Drawing.Size(160, 34);
            TestButton.TabIndex = 2;
            TestButton.Text = "Connection Test";
            TestButton.UseVisualStyleBackColor = true;
            TestButton.Click += TestButton_Click;
            // 
            // TabPageData
            // 
            TabPageData.Controls.Add(MainListView);
            TabPageData.Location = new System.Drawing.Point(4, 36);
            TabPageData.Name = "TabPageData";
            TabPageData.Padding = new System.Windows.Forms.Padding(3);
            TabPageData.Size = new System.Drawing.Size(1120, 632);
            TabPageData.TabIndex = 1;
            TabPageData.Text = "Tag list";
            TabPageData.UseVisualStyleBackColor = true;
            // 
            // TabPageLog
            // 
            TabPageLog.Controls.Add(richTextBoxLogControl1);
            TabPageLog.Location = new System.Drawing.Point(4, 36);
            TabPageLog.Name = "TabPageLog";
            TabPageLog.Size = new System.Drawing.Size(1120, 632);
            TabPageLog.TabIndex = 3;
            TabPageLog.Text = "Log";
            TabPageLog.UseVisualStyleBackColor = true;
            // 
            // richTextBoxLogControl1
            // 
            richTextBoxLogControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxLogControl1.ForContext = "";
            richTextBoxLogControl1.Location = new System.Drawing.Point(0, 0);
            richTextBoxLogControl1.Name = "richTextBoxLogControl1";
            richTextBoxLogControl1.ReadOnly = true;
            richTextBoxLogControl1.Size = new System.Drawing.Size(1120, 632);
            richTextBoxLogControl1.TabIndex = 0;
            richTextBoxLogControl1.Text = "";
            // 
            // TabPageAbout
            // 
            TabPageAbout.Location = new System.Drawing.Point(4, 36);
            TabPageAbout.Name = "TabPageAbout";
            TabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            TabPageAbout.Size = new System.Drawing.Size(1120, 632);
            TabPageAbout.TabIndex = 2;
            TabPageAbout.Text = "About";
            TabPageAbout.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1128, 694);
            Controls.Add(TabControl);
            Controls.Add(statusStrip1);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "NeuOPC";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            TabControl.ResumeLayout(false);
            TabPageSetting.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            TabPageData.ResumeLayout(false);
            TabPageLog.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private Serilog.Sinks.WinForms.Core.RichTextBoxLogControl richTextBoxLogControl1;
    }
}


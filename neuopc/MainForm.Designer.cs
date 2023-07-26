
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
            this.TabPageAbout = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabPageSetting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TabPageData.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DAStatusLabel,
            this.UAStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
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
            this.MainListView.Size = new System.Drawing.Size(770, 413);
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
            this.TabControl.Controls.Add(this.TabPageAbout);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(784, 459);
            this.TabControl.TabIndex = 8;
            // 
            // TabPageSetting
            // 
            this.TabPageSetting.Controls.Add(this.groupBox1);
            this.TabPageSetting.Location = new System.Drawing.Point(4, 36);
            this.TabPageSetting.Name = "TabPageSetting";
            this.TabPageSetting.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageSetting.Size = new System.Drawing.Size(776, 419);
            this.TabPageSetting.TabIndex = 0;
            this.TabPageSetting.Text = "Setting";
            this.TabPageSetting.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.groupBox1.Size = new System.Drawing.Size(758, 405);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(560, 326);
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
            this.UALabel.Location = new System.Drawing.Point(227, 132);
            this.UALabel.Name = "UALabel";
            this.UALabel.Size = new System.Drawing.Size(0, 24);
            this.UALabel.TabIndex = 34;
            // 
            // CheckBox
            // 
            this.CheckBox.AutoSize = true;
            this.CheckBox.Location = new System.Drawing.Point(210, 330);
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Size = new System.Drawing.Size(178, 28);
            this.CheckBox.TabIndex = 33;
            this.CheckBox.Text = "Auto connection";
            this.CheckBox.UseVisualStyleBackColor = true;
            // 
            // SwitchButton
            // 
            this.SwitchButton.Location = new System.Drawing.Point(394, 326);
            this.SwitchButton.Name = "SwitchButton";
            this.SwitchButton.Size = new System.Drawing.Size(160, 34);
            this.SwitchButton.TabIndex = 32;
            this.SwitchButton.Text = "Start";
            this.SwitchButton.UseVisualStyleBackColor = true;
            this.SwitchButton.Click += new System.EventHandler(this.SwitchButton_Click);
            // 
            // UAPasswordTextBox
            // 
            this.UAPasswordTextBox.Location = new System.Drawing.Point(222, 280);
            this.UAPasswordTextBox.Name = "UAPasswordTextBox";
            this.UAPasswordTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAPasswordTextBox.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 287);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 24);
            this.label5.TabIndex = 30;
            this.label5.Text = "OPCUA Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 199);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 24);
            this.label3.TabIndex = 27;
            this.label3.Text = "OPCUA Port:";
            // 
            // UAPortTextBox
            // 
            this.UAPortTextBox.Location = new System.Drawing.Point(222, 195);
            this.UAPortTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.UAPortTextBox.Name = "UAPortTextBox";
            this.UAPortTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAPortTextBox.TabIndex = 25;
            // 
            // UAUserTextBox
            // 
            this.UAUserTextBox.Location = new System.Drawing.Point(222, 240);
            this.UAUserTextBox.Name = "UAUserTextBox";
            this.UAUserTextBox.Size = new System.Drawing.Size(498, 30);
            this.UAUserTextBox.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 244);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 24);
            this.label4.TabIndex = 28;
            this.label4.Text = "OPCUA User:";
            // 
            // DAHostComboBox
            // 
            this.DAHostComboBox.FormattingEnabled = true;
            this.DAHostComboBox.Location = new System.Drawing.Point(227, 35);
            this.DAHostComboBox.Name = "DAHostComboBox";
            this.DAHostComboBox.Size = new System.Drawing.Size(498, 32);
            this.DAHostComboBox.TabIndex = 0;
            this.DAHostComboBox.DropDown += new System.EventHandler(this.DAHostComboBox_DropDown);
            // 
            // DAServerComboBox
            // 
            this.DAServerComboBox.FormattingEnabled = true;
            this.DAServerComboBox.Location = new System.Drawing.Point(227, 77);
            this.DAServerComboBox.Name = "DAServerComboBox";
            this.DAServerComboBox.Size = new System.Drawing.Size(498, 32);
            this.DAServerComboBox.TabIndex = 1;
            this.DAServerComboBox.DropDown += new System.EventHandler(this.DAServerComboBox_DropDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 81);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "OPCDA Server:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "OPCDA Host:";
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(565, 127);
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
            this.TabPageData.Size = new System.Drawing.Size(776, 419);
            this.TabPageData.TabIndex = 1;
            this.TabPageData.Text = "Data view";
            this.TabPageData.UseVisualStyleBackColor = true;
            // 
            // TabPageAbout
            // 
            this.TabPageAbout.Location = new System.Drawing.Point(4, 36);
            this.TabPageAbout.Name = "TabPageAbout";
            this.TabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageAbout.Size = new System.Drawing.Size(776, 419);
            this.TabPageAbout.TabIndex = 2;
            this.TabPageAbout.Text = "About";
            this.TabPageAbout.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 481);
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
    }
}


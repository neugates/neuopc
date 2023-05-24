
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ReadButton = new System.Windows.Forms.Button();
            this.UAPortTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.DAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.UAStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.UAPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.UAUserTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DAHostComboBox = new System.Windows.Forms.ComboBox();
            this.DAServerComboBox = new System.Windows.Forms.ComboBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(924, 30);
            this.ReadButton.Margin = new System.Windows.Forms.Padding(2);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(112, 34);
            this.ReadButton.TabIndex = 2;
            this.ReadButton.Text = "Connect";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // UAPortTextBox
            // 
            this.UAPortTextBox.Location = new System.Drawing.Point(108, 72);
            this.UAPortTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.UAPortTextBox.Name = "UAPortTextBox";
            this.UAPortTextBox.Size = new System.Drawing.Size(200, 30);
            this.UAPortTextBox.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1128, 32);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 28);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.documentToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(67, 28);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // documentToolStripMenuItem
            // 
            this.documentToolStripMenuItem.Name = "documentToolStripMenuItem";
            this.documentToolStripMenuItem.Size = new System.Drawing.Size(200, 34);
            this.documentToolStripMenuItem.Text = "Document";
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.UAPasswordTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.UAUserTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.DAHostComboBox);
            this.groupBox1.Controls.Add(this.DAServerComboBox);
            this.groupBox1.Controls.Add(this.RunButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ReadButton);
            this.groupBox1.Controls.Add(this.UAPortTextBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 38);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1102, 121);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // UAPasswordTextBox
            // 
            this.UAPasswordTextBox.Location = new System.Drawing.Point(739, 72);
            this.UAPasswordTextBox.Name = "UAPasswordTextBox";
            this.UAPasswordTextBox.Size = new System.Drawing.Size(180, 30);
            this.UAPasswordTextBox.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(607, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 24);
            this.label5.TabIndex = 15;
            this.label5.Text = "UA Password:";
            // 
            // UAUserTextBox
            // 
            this.UAUserTextBox.Location = new System.Drawing.Point(421, 72);
            this.UAUserTextBox.Name = "UAUserTextBox";
            this.UAUserTextBox.Size = new System.Drawing.Size(180, 30);
            this.UAUserTextBox.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(313, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 24);
            this.label4.TabIndex = 13;
            this.label4.Text = "UA User:";
            // 
            // DAHostComboBox
            // 
            this.DAHostComboBox.FormattingEnabled = true;
            this.DAHostComboBox.Location = new System.Drawing.Point(108, 32);
            this.DAHostComboBox.Name = "DAHostComboBox";
            this.DAHostComboBox.Size = new System.Drawing.Size(200, 32);
            this.DAHostComboBox.TabIndex = 0;
            this.DAHostComboBox.DropDown += new System.EventHandler(this.DAHostComboBox_DropDown);
            // 
            // DAServerComboBox
            // 
            this.DAServerComboBox.FormattingEnabled = true;
            this.DAServerComboBox.Location = new System.Drawing.Point(421, 32);
            this.DAServerComboBox.Name = "DAServerComboBox";
            this.DAServerComboBox.Size = new System.Drawing.Size(498, 32);
            this.DAServerComboBox.TabIndex = 1;
            this.DAServerComboBox.DropDown += new System.EventHandler(this.DAServerComboBox_DropDown);
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(924, 71);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(112, 34);
            this.RunButton.TabIndex = 6;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 76);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "UA Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "DA Server:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "DA Host:";
            // 
            // MainListView
            // 
            this.MainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.type,
            this.rights,
            this.value,
            this.quality,
            this.error,
            this.timestamp,
            this.handle});
            this.MainListView.HideSelection = false;
            this.MainListView.Location = new System.Drawing.Point(13, 166);
            this.MainListView.Name = "MainListView";
            this.MainListView.Size = new System.Drawing.Size(1103, 491);
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
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "neuopc";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.Click += new System.EventHandler(this.NotifyIcon_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 694);
            this.Controls.Add(this.MainListView);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "neuopc";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.TextBox UAPortTextBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel DAStatusLabel;
        private System.Windows.Forms.ListView MainListView;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.ComboBox DAServerComboBox;
        private System.Windows.Forms.ComboBox DAHostComboBox;
        private System.Windows.Forms.ColumnHeader handle;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader quality;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.ColumnHeader rights;
        private System.Windows.Forms.ColumnHeader error;
        private System.Windows.Forms.ColumnHeader timestamp;
        private System.Windows.Forms.ColumnHeader type;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox UAUserTextBox;
        private System.Windows.Forms.TextBox UAPasswordTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripStatusLabel UAStatusLabel;
    }
}


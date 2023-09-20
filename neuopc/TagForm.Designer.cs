namespace neuopc
{
    partial class TagForm
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
            label1 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            comboBox1 = new System.Windows.Forms.ComboBox();
            SaveButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 25);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(117, 24);
            label1.TabIndex = 0;
            label1.Text = "Tag Address";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(135, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(316, 30);
            textBox1.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(24, 74);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(105, 24);
            label2.TabIndex = 2;
            label2.Text = "Value Type";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new System.Drawing.Point(135, 66);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(316, 32);
            comboBox1.TabIndex = 3;
            // 
            // SaveButton
            // 
            SaveButton.Location = new System.Drawing.Point(479, 64);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new System.Drawing.Size(112, 34);
            SaveButton.TabIndex = 4;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // TagForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(603, 130);
            Controls.Add(SaveButton);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "TagForm";
            Text = "TagForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button SaveButton;
    }
}
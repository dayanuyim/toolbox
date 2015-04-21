namespace TTToolbox
{
    partial class FieldsDialog
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
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.grpbox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_cancel.Location = new System.Drawing.Point(257, 120);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(55, 23);
            this.button_cancel.TabIndex = 7;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_ok
            // 
            this.button_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_ok.Location = new System.Drawing.Point(187, 120);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(55, 23);
            this.button_ok.TabIndex = 6;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // grpbox
            // 
            this.grpbox.Location = new System.Drawing.Point(12, 8);
            this.grpbox.Name = "grpbox";
            this.grpbox.Size = new System.Drawing.Size(300, 103);
            this.grpbox.TabIndex = 0;
            this.grpbox.TabStop = false;
            // 
            // FieldsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.OldLace;
            this.ClientSize = new System.Drawing.Size(328, 158);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.grpbox);
            this.Controls.Add(this.button_cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldsDialog";
            this.Text = "Add/Modify";
            this.Load += new System.EventHandler(this.ModifyDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.GroupBox grpbox;
    }
}
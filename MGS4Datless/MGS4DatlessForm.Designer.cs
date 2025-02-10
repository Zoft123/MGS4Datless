using System.Drawing;
using System.Windows.Forms;

namespace MGSDatless
{
    partial class MGS4DatlessForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtMGSPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnUndat;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RichTextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGS4DatlessForm));
            this.txtMGSPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnUndat = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtMGSPath
            // 
            this.txtMGSPath.Location = new System.Drawing.Point(10, 10);
            this.txtMGSPath.Name = "txtMGSPath";
            this.txtMGSPath.ReadOnly = true;
            this.txtMGSPath.Size = new System.Drawing.Size(429, 20);
            this.txtMGSPath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(444, 10);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(64, 22);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnUndat
            // 
            this.btnUndat.Location = new System.Drawing.Point(10, 43);
            this.btnUndat.Name = "btnUndat";
            this.btnUndat.Size = new System.Drawing.Size(64, 26);
            this.btnUndat.TabIndex = 2;
            this.btnUndat.Text = "Undat";
            this.btnUndat.UseVisualStyleBackColor = true;
            this.btnUndat.Click += new System.EventHandler(this.btnUndat_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(10, 347);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(498, 20);
            this.progressBar.TabIndex = 3;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(10, 87);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(499, 252);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // MGS4DatlessForm
            // 
            this.AcceptButton = this.btnUndat;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 377);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUndat);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtMGSPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MGS4DatlessForm";
            this.Text = "MGS4Datless Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
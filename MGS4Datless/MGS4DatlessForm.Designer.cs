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
            txtMGSPath = new TextBox();
            btnBrowse = new Button();
            btnUndat = new Button();
            progressBar = new ProgressBar();
            txtLog = new RichTextBox();
            SuspendLayout();
            // 
            // txtMGSPath
            // 
            txtMGSPath.Location = new Point(12, 12);
            txtMGSPath.Name = "txtMGSPath";
            txtMGSPath.ReadOnly = true;
            txtMGSPath.Size = new Size(500, 23);
            txtMGSPath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(518, 11);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 25);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnUndat
            // 
            btnUndat.Location = new Point(12, 50);
            btnUndat.Name = "btnUndat";
            btnUndat.Size = new Size(75, 30);
            btnUndat.TabIndex = 2;
            btnUndat.Text = "Undat";
            btnUndat.UseVisualStyleBackColor = true;
            btnUndat.Click += btnUndat_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 400);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(581, 23);
            progressBar.TabIndex = 3;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 100);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(581, 290);
            txtLog.TabIndex = 4;
            txtLog.Text = "";
            // 
            // MGS4DatlessForm
            // 
            AcceptButton = btnUndat;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(605, 435);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnUndat);
            Controls.Add(btnBrowse);
            Controls.Add(txtMGSPath);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MGS4DatlessForm";
            Text = "MGS4Datless Tool";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
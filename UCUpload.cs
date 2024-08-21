
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Attendance
{
  public class UCUpload : UserControl
  {
    private IContainer components;
    private Label label1;

    public UCUpload() => this.InitializeComponent();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.label1 = new Label();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Location = new Point(34, 34);
      this.label1.Name = "label1";
      this.label1.Size = new Size(77, 12);
      this.label1.TabIndex = 2;
      this.label1.Text = "===Upload===";
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.label1);
      this.Name = nameof (UCUpload);
      this.Size = new Size(544, 445);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

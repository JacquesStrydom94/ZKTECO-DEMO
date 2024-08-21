
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Utils;

namespace Attendance
{
  public class UCCommInfo : UserControl
  {
    private bool _IsPause;
    private IContainer components;
    private RichTextBox rtxtCommInfo;
    private Button btnClear;
    private Button btnPause;
    private Label lblVersion;
    private Panel panel1;
    private Panel pnlTop;

    public UCCommInfo() => this.InitializeComponent();

    public void ShowVersion(string verString)
    {
      verString = string.IsNullOrEmpty(verString) ? "1.0.0.0" : verString;
      this.lblVersion.Text = string.Format("Version: {0}   ", (object) verString);
    }

    public void AddCommInfo(string info, int Mode)
    {
      if (this._IsPause)
        return;
      string str = Tools.GetDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss:fff");
      if (Mode == 0)
        info = string.Format("Sever Receive Data:  {0}\r\n{1}\r\n", (object) str, (object) info.TrimEnd(new char[1]));
      else if (1 == Mode)
        info = string.Format("Sever Send Data:  {0}\r\n{1}\r\n", (object) str, (object) info);
      else if (3 == Mode)
        info = string.Format("Sever Start:  {0}\r\n{1}\r\n", (object) str, (object) info);
      else if (4 == Mode)
        info = string.Format("Sever Stop:  {0}\r\n{1}\r\n", (object) str, (object) info);
      this.rtxtCommInfo.AppendText(info);
      ServerLogToFile.WriteLogs(info);
    }

    private void btnClear_Click(object sender, EventArgs e) => this.rtxtCommInfo.Clear();

    private void btnPause_Click(object sender, EventArgs e)
    {
      this._IsPause = !this._IsPause;
      this.btnPause.Text = this._IsPause ? "Resume" : "Pause";
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.rtxtCommInfo = new RichTextBox();
      this.btnClear = new Button();
      this.btnPause = new Button();
      this.lblVersion = new Label();
      this.panel1 = new Panel();
      this.pnlTop = new Panel();
      this.panel1.SuspendLayout();
      this.pnlTop.SuspendLayout();
      this.SuspendLayout();
      this.rtxtCommInfo.BackColor = Color.White;
      this.rtxtCommInfo.Dock = DockStyle.Fill;
      this.rtxtCommInfo.Location = new Point(0, 0);
      this.rtxtCommInfo.Name = "rtxtCommInfo";
      this.rtxtCommInfo.ReadOnly = true;
      this.rtxtCommInfo.Size = new Size(862, 210);
      this.rtxtCommInfo.TabIndex = 0;
      this.rtxtCommInfo.Text = "";
      this.btnClear.BackColor = Color.FromArgb(37, 190, 167);
      this.btnClear.Cursor = Cursors.Hand;
      this.btnClear.FlatStyle = FlatStyle.Flat;
      this.btnClear.Font = new Font("Arial", 12f);
      this.btnClear.ForeColor = Color.White;
      this.btnClear.Location = new Point(28, 6);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new Size(75, 30);
      this.btnClear.TabIndex = 1;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = false;
      this.btnClear.Click += new EventHandler(this.btnClear_Click);
      this.btnPause.BackColor = Color.FromArgb(37, 190, 167);
      this.btnPause.Cursor = Cursors.Hand;
      this.btnPause.FlatStyle = FlatStyle.Flat;
      this.btnPause.Font = new Font("Arial", 12f);
      this.btnPause.ForeColor = Color.White;
      this.btnPause.Location = new Point(135, 6);
      this.btnPause.Name = "btnPause";
      this.btnPause.Size = new Size(75, 30);
      this.btnPause.TabIndex = 2;
      this.btnPause.Text = "Pause";
      this.btnPause.UseVisualStyleBackColor = false;
      this.btnPause.Click += new EventHandler(this.btnPause_Click);
      this.lblVersion.Dock = DockStyle.Right;
      this.lblVersion.Font = new Font("Arial", 10f);
      this.lblVersion.Location = new Point(720, 0);
      this.lblVersion.Margin = new Padding(3, 15, 3, 0);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new Size(142, 45);
      this.lblVersion.TabIndex = 3;
      this.lblVersion.Text = "Version: 1.0.0.0";
      this.lblVersion.TextAlign = ContentAlignment.MiddleCenter;
      this.panel1.BackColor = Color.White;
      this.panel1.Controls.Add((Control) this.btnClear);
      this.panel1.Controls.Add((Control) this.lblVersion);
      this.panel1.Controls.Add((Control) this.btnPause);
      this.panel1.Dock = DockStyle.Bottom;
      this.panel1.Location = new Point(0, 210);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(862, 45);
      this.panel1.TabIndex = 4;
      this.pnlTop.Controls.Add((Control) this.rtxtCommInfo);
      this.pnlTop.Dock = DockStyle.Fill;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(862, 210);
      this.pnlTop.TabIndex = 5;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlTop);
      this.Controls.Add((Control) this.panel1);
      this.Name = nameof (UCCommInfo);
      this.Size = new Size(862, (int) byte.MaxValue);
      this.panel1.ResumeLayout(false);
      this.pnlTop.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}

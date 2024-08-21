
using Attendance.Properties;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Utils;

namespace Attendance
{
  public class UCOperateLog : UserControl
  {
    private OpLogBll _bll = new OpLogBll();
    private IContainer components;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private DateTimePicker dtpEndTime;
    private DateTimePicker dtpStartTime;
    private ComboBox cmb_DevSN;
    private Label lblDevSN;
    private Label lblEndTime;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private Label lblStartTime;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private Panel pnlData;
    private Label lblModuleName;
    private Panel pnlTop;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
    private DataGridView dgvOperate;
    private PictureBox pb_Search;
    private Button btn_Clear;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colTime;
    private DataGridViewTextBoxColumn colSN;
    private DataGridViewTextBoxColumn colAdmin;
    private DataGridViewTextBoxColumn colType;
    private DataGridViewTextBoxColumn colParam1;
    private DataGridViewTextBoxColumn colParam2;
    private DataGridViewTextBoxColumn colParam3;
    private DataGridViewTextBoxColumn colParam4;

    public UCOperateLog() => this.InitializeComponent();

    private void UCOperateLog_Load(object sender, EventArgs e)
    {
      this.dgvOperate.AutoGenerateColumns = false;
      Control.CheckForIllegalCrossThreadCalls = false;
      DateTime dateTimeNow = Tools.GetDateTimeNow();
      this.dtpStartTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0);
      this.dtpEndTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 23, 59, 59);
      this.GetAllDevSNToCmbo();
      this.LoadDefaultData();
    }

    private void LoadDefaultData()
    {
      string devsn = this.cmb_DevSN.Text.Trim();
      try
      {
        this.dgvOperate.DataSource = (object) this._bll.GetOplogByTime(this.dtpStartTime.Value, this.dtpEndTime.Value, devsn);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load operatelog info error:" + ex.ToString());
      }
    }

    public void RefreshData() => this.LoadDefaultData();

    private void GetAllDevSNToCmbo()
    {
      this.cmb_DevSN.Items.Clear();
      this.cmb_DevSN.Items.Add((object) "");
      try
      {
        List<string> allDevSn = new DeviceBll().GetAllDevSN();
        for (int index = 0; index < allDevSn.Count; ++index)
          this.cmb_DevSN.Items.Add((object) allDevSn[index]);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("GetAllDevSNToCmbo error:" + ex.ToString());
      }
    }

    private void btnGetPeriodLog_Click(object sender, EventArgs e) => this.LoadDefaultData();

    private void btnClearOpLog_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to delete all data?", "Tip", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK || this._bll.ClearAll() <= 0)
        return;
      this.dgvOperate.DataSource = (object) null;
    }

    private void dgvOperate_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
      using (SolidBrush solidBrush1 = new SolidBrush(this.dgvOperate.RowHeadersDefaultCellStyle.ForeColor))
      {
        Graphics graphics = e.Graphics;
        string s = Convert.ToString(e.RowIndex + 1, (IFormatProvider) CultureInfo.CurrentUICulture);
        Font font = e.InheritedRowStyle.Font;
        SolidBrush solidBrush2 = solidBrush1;
        Rectangle rowBounds = e.RowBounds;
        double x = (double) (rowBounds.Location.X + 20);
        rowBounds = e.RowBounds;
        double y = (double) (rowBounds.Location.Y + 4);
        graphics.DrawString(s, font, (Brush) solidBrush2, (float) x, (float) y);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle4 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle5 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle6 = new DataGridViewCellStyle();
      this.dtpEndTime = new DateTimePicker();
      this.dtpStartTime = new DateTimePicker();
      this.cmb_DevSN = new ComboBox();
      this.lblDevSN = new Label();
      this.lblEndTime = new Label();
      this.lblStartTime = new Label();
      this.pnlData = new Panel();
      this.dgvOperate = new DataGridView();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colTime = new DataGridViewTextBoxColumn();
      this.colSN = new DataGridViewTextBoxColumn();
      this.colAdmin = new DataGridViewTextBoxColumn();
      this.colType = new DataGridViewTextBoxColumn();
      this.colParam1 = new DataGridViewTextBoxColumn();
      this.colParam2 = new DataGridViewTextBoxColumn();
      this.colParam3 = new DataGridViewTextBoxColumn();
      this.colParam4 = new DataGridViewTextBoxColumn();
      this.lblModuleName = new Label();
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.btn_Clear = new Button();
      this.pb_Search = new PictureBox();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvOperate).BeginInit();
      this.pnlTop.SuspendLayout();
      ((ISupportInitialize) this.pb_Search).BeginInit();
      this.SuspendLayout();
      this.dtpEndTime.CalendarFont = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpEndTime.Font = new Font("Arial", 10f);
      this.dtpEndTime.Format = DateTimePickerFormat.Custom;
      this.dtpEndTime.Location = new Point(376, 34);
      this.dtpEndTime.Name = "dtpEndTime";
      this.dtpEndTime.RightToLeft = RightToLeft.No;
      this.dtpEndTime.ShowUpDown = true;
      this.dtpEndTime.Size = new Size(145, 23);
      this.dtpEndTime.TabIndex = 65;
      this.dtpStartTime.CalendarFont = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpStartTime.Font = new Font("Arial", 10f);
      this.dtpStartTime.Format = DateTimePickerFormat.Custom;
      this.dtpStartTime.Location = new Point(212, 34);
      this.dtpStartTime.Name = "dtpStartTime";
      this.dtpStartTime.RightToLeft = RightToLeft.No;
      this.dtpStartTime.ShowUpDown = true;
      this.dtpStartTime.Size = new Size(145, 23);
      this.dtpStartTime.TabIndex = 64;
      this.cmb_DevSN.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmb_DevSN.FormattingEnabled = true;
      this.cmb_DevSN.Location = new Point(46, 35);
      this.cmb_DevSN.Name = "cmb_DevSN";
      this.cmb_DevSN.Size = new Size(121, 20);
      this.cmb_DevSN.TabIndex = 63;
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 12f);
      this.lblDevSN.Location = new Point(14, 36);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(30, 18);
      this.lblDevSN.TabIndex = 62;
      this.lblDevSN.Text = "SN";
      this.lblEndTime.AutoSize = true;
      this.lblEndTime.Location = new Point(358, 39);
      this.lblEndTime.Name = "lblEndTime";
      this.lblEndTime.Size = new Size(17, 12);
      this.lblEndTime.TabIndex = 56;
      this.lblEndTime.Text = "--";
      this.lblStartTime.AutoSize = true;
      this.lblStartTime.Font = new Font("Arial", 12f);
      this.lblStartTime.Location = new Point(169, 36);
      this.lblStartTime.Name = "lblStartTime";
      this.lblStartTime.Size = new Size(42, 18);
      this.lblStartTime.TabIndex = 55;
      this.lblStartTime.Text = "Time";
      this.pnlData.Controls.Add((Control) this.dgvOperate);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 65);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(866, 471);
      this.pnlData.TabIndex = 5;
      this.dgvOperate.AllowUserToAddRows = false;
      this.dgvOperate.AllowUserToDeleteRows = false;
      this.dgvOperate.AllowUserToResizeRows = false;
      this.dgvOperate.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvOperate.BackgroundColor = Color.White;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvOperate.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvOperate.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colTime, (DataGridViewColumn) this.colSN, (DataGridViewColumn) this.colAdmin, (DataGridViewColumn) this.colType, (DataGridViewColumn) this.colParam1, (DataGridViewColumn) this.colParam2, (DataGridViewColumn) this.colParam3, (DataGridViewColumn) this.colParam4);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvOperate.DefaultCellStyle = gridViewCellStyle2;
      this.dgvOperate.Dock = DockStyle.Fill;
      this.dgvOperate.Location = new Point(0, 0);
      this.dgvOperate.MultiSelect = false;
      this.dgvOperate.Name = "dgvOperate";
      this.dgvOperate.ReadOnly = true;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvOperate.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvOperate.RowHeadersVisible = false;
      this.dgvOperate.RowTemplate.Height = 23;
      this.dgvOperate.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvOperate.Size = new Size(866, 471);
      this.dgvOperate.TabIndex = 1;
      this.dgvOperate.RowPostPaint += new DataGridViewRowPostPaintEventHandler(this.dgvOperate_RowPostPaint);
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle4;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colIndex.Width = 50;
      this.colTime.DataPropertyName = "OpTime";
      gridViewCellStyle5.Format = "yyyy-MM-dd HH:mm:ss";
      gridViewCellStyle5.NullValue = (object) null;
      this.colTime.DefaultCellStyle = gridViewCellStyle5;
      this.colTime.HeaderText = "Date Time";
      this.colTime.MinimumWidth = 80;
      this.colTime.Name = "colTime";
      this.colTime.ReadOnly = true;
      this.colSN.DataPropertyName = "DeviceID";
      this.colSN.HeaderText = "DevSN";
      this.colSN.MinimumWidth = 80;
      this.colSN.Name = "colSN";
      this.colSN.ReadOnly = true;
      this.colAdmin.DataPropertyName = "Operator";
      this.colAdmin.HeaderText = "Admin";
      this.colAdmin.MinimumWidth = 80;
      this.colAdmin.Name = "colAdmin";
      this.colAdmin.ReadOnly = true;
      this.colType.DataPropertyName = "OpType";
      this.colType.HeaderText = "Op Type";
      this.colType.MinimumWidth = 60;
      this.colType.Name = "colType";
      this.colType.ReadOnly = true;
      this.colParam1.DataPropertyName = "Obj1";
      this.colParam1.HeaderText = "Param1";
      this.colParam1.MinimumWidth = 100;
      this.colParam1.Name = "colParam1";
      this.colParam1.ReadOnly = true;
      this.colParam2.DataPropertyName = "Obj2";
      this.colParam2.HeaderText = "Param2";
      this.colParam2.MinimumWidth = 80;
      this.colParam2.Name = "colParam2";
      this.colParam2.ReadOnly = true;
      this.colParam3.DataPropertyName = "Obj3";
      this.colParam3.HeaderText = "Param3";
      this.colParam3.MinimumWidth = 80;
      this.colParam3.Name = "colParam3";
      this.colParam3.ReadOnly = true;
      this.colParam4.DataPropertyName = "Obj4";
      this.colParam4.HeaderText = "Param4";
      this.colParam4.MinimumWidth = 80;
      this.colParam4.Name = "colParam4";
      this.colParam4.ReadOnly = true;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(92, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "OperateLog";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.btn_Clear);
      this.pnlTop.Controls.Add((Control) this.pb_Search);
      this.pnlTop.Controls.Add((Control) this.dtpEndTime);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Controls.Add((Control) this.dtpStartTime);
      this.pnlTop.Controls.Add((Control) this.cmb_DevSN);
      this.pnlTop.Controls.Add((Control) this.lblEndTime);
      this.pnlTop.Controls.Add((Control) this.lblDevSN);
      this.pnlTop.Controls.Add((Control) this.lblStartTime);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(866, 65);
      this.pnlTop.TabIndex = 3;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 69;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.btn_Clear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btn_Clear.BackColor = Color.FromArgb(37, 190, 167);
      this.btn_Clear.Cursor = Cursors.Hand;
      this.btn_Clear.FlatStyle = FlatStyle.Flat;
      this.btn_Clear.Font = new Font("Arial", 12f);
      this.btn_Clear.ForeColor = Color.White;
      this.btn_Clear.Location = new Point(753, 30);
      this.btn_Clear.Name = "btn_Clear";
      this.btn_Clear.Size = new Size(75, 30);
      this.btn_Clear.TabIndex = 68;
      this.btn_Clear.Text = "Clear";
      this.btn_Clear.UseVisualStyleBackColor = false;
      this.btn_Clear.Click += new EventHandler(this.btnClearOpLog_Click);
      this.pb_Search.Cursor = Cursors.Hand;
      this.pb_Search.Image = (Image) Resources.sousuo2;
      this.pb_Search.Location = new Point(532, 32);
      this.pb_Search.Name = "pb_Search";
      this.pb_Search.Size = new Size(27, 27);
      this.pb_Search.TabIndex = 67;
      this.pb_Search.TabStop = false;
      this.pb_Search.Click += new EventHandler(this.btnGetPeriodLog_Click);
      this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn1.DefaultCellStyle = gridViewCellStyle6;
      this.dataGridViewTextBoxColumn1.Frozen = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 50;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn1.Width = 75;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "DeviceID";
      this.dataGridViewTextBoxColumn2.HeaderText = "DeviceID";
      this.dataGridViewTextBoxColumn2.MinimumWidth = 100;
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 102;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "PIN";
      this.dataGridViewTextBoxColumn3.HeaderText = "UserID";
      this.dataGridViewTextBoxColumn3.MinimumWidth = 20;
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 76;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Status";
      this.dataGridViewTextBoxColumn4.HeaderText = "AttState";
      this.dataGridViewTextBoxColumn4.MinimumWidth = 55;
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 75;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "Verify";
      this.dataGridViewTextBoxColumn5.HeaderText = "VerifyMode";
      this.dataGridViewTextBoxColumn5.MinimumWidth = 80;
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.Width = 80;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "AttTime";
      this.dataGridViewTextBoxColumn6.HeaderText = "AttTime";
      this.dataGridViewTextBoxColumn6.MinimumWidth = 150;
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.Width = 150;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "WorkCode";
      this.dataGridViewTextBoxColumn7.HeaderText = "WorkCode";
      this.dataGridViewTextBoxColumn7.MinimumWidth = 80;
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.Width = 80;
      this.dataGridViewTextBoxColumn8.DataPropertyName = "Obj4";
      this.dataGridViewTextBoxColumn8.HeaderText = "Param4";
      this.dataGridViewTextBoxColumn8.MinimumWidth = 24;
      this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
      this.dataGridViewTextBoxColumn8.ReadOnly = true;
      this.dataGridViewTextBoxColumn8.Width = 24;
      this.dataGridViewTextBoxColumn9.DataPropertyName = "DeviceID";
      this.dataGridViewTextBoxColumn9.HeaderText = "DevSN";
      this.dataGridViewTextBoxColumn9.MinimumWidth = 40;
      this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
      this.dataGridViewTextBoxColumn9.ReadOnly = true;
      this.dataGridViewTextBoxColumn9.Width = 40;
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.Width = 50;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCOperateLog);
      this.Size = new Size(866, 536);
      this.Load += new EventHandler(this.UCOperateLog_Load);
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvOperate).EndInit();
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      ((ISupportInitialize) this.pb_Search).EndInit();
      this.ResumeLayout(false);
    }
  }
}

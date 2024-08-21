
using Attendance.Properties;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Utils;

namespace Attendance
{
  public class UCAttendance : UserControl
  {
    private DeviceBll _bllDevice = new DeviceBll();
    private AttLogBll _bll = new AttLogBll();
    private WorkCodeBll _workCodeBll = new WorkCodeBll();
    private DataTable _dt;
    private IContainer components;
    private Panel pnlTop;
    private Panel pnlData;
    private Label lblModuleName;
    private DataGridView dgvAttendance;
    private ComboBox cmbDevSN;
    private Label lblDevSN;
    private TextBox txtUserID;
    private Label lblUserID;
    private Label lblStartTime;
    private Label lblEndTime;
    private Button btnClearListAttLog;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DateTimePicker dtpEndTime;
    private DateTimePicker dtpStartTime;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private PictureBox pb_Search;
    private Panel pnlControl;
    private PictureBox picAttpho;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colDeviceID;
    private DataGridViewTextBoxColumn colUserID;
    private DataGridViewTextBoxColumn colAttState;
    private DataGridViewTextBoxColumn colVerifyMode;
    private DataGridViewTextBoxColumn colAttTime;
    private DataGridViewTextBoxColumn colWorkName;

    public UCAttendance() => this.InitializeComponent();

    private void UCAttendance_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvAttendance.AutoGenerateColumns = false;
      this.LoadDefaultData();
    }

    private void LoadDefaultData()
    {
      DateTime dateTimeNow = Tools.GetDateTimeNow();
      this.dtpStartTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0);
      this.dtpEndTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 23, 59, 59);
      this.GetAllDevSNToCmbo();
      this.LoadAttlogData();
    }

    private void GetAllDevSNToCmbo()
    {
      this.cmbDevSN.Items.Clear();
      this.cmbDevSN.Items.Add((object) "");
      try
      {
        List<string> allDevSn = this._bllDevice.GetAllDevSN();
        for (int index = 0; index < allDevSn.Count; ++index)
          this.cmbDevSN.Items.Add((object) allDevSn[index]);
      }
      catch (Exception ex)
      {
      }
    }

    public void AddNewRow(AttLogModel attLogModel)
    {
      if (!(attLogModel.AttTime >= this.dtpStartTime.Value) || !(attLogModel.AttTime <= this.dtpEndTime.Value))
        return;
      DataRow row = this._dt.NewRow();
      row["PIN"] = (object) attLogModel.PIN;
      row["AttTime"] = (object) attLogModel.AttTime;
      row["Status"] = (object) attLogModel.Status;
      row["Verify"] = (object) attLogModel.Verify;
      row["WorkCode"] = (object) attLogModel.WorkCode;
      row["DeviceID"] = (object) attLogModel.DeviceID;
      row["MaskFlag"] = (object) attLogModel.MaskFlag;
      row["Temperature"] = (object) attLogModel.Temperature;
      row["WorkName"] = string.IsNullOrEmpty(attLogModel.WorkCode) ? (object) "" : (object) this._workCodeBll.GetByWorkCode(attLogModel.WorkCode).WorkName;
      this._dt.Rows.InsertAt(row, 0);
    }

    private void LoadAttlogData()
    {
      string userid = this.txtUserID.Text.Trim();
      string devsn = this.cmbDevSN.Text.Trim();
      try
      {
        this._dt = this._bll.GetByTime(this.dtpStartTime.Value, this.dtpEndTime.Value, userid, devsn);
        this.dgvAttendance.DataSource = (object) this._dt;
        this.dgvAttendance.Update();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load attlog info error:" + ex.ToString());
      }
    }

    private void btnGetAttLog_Click(object sender, EventArgs e) => this.LoadAttlogData();

    private void btnClearListAttLog_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to delete all data?", "Tip", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK || this._bll.ClearAll() <= 0)
        return;
      this.dgvAttendance.DataSource = (object) null;
      this.dgvAttendance.Update();
    }

    private void dgvAttendance_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
      using (SolidBrush solidBrush1 = new SolidBrush(this.dgvAttendance.RowHeadersDefaultCellStyle.ForeColor))
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

    private void dgvAttendance_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvAttendance.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvAttendance.CurrentRow == null)
        return;
      DataGridViewRow currentRow = this.dgvAttendance.CurrentRow;
      string str1 = Convert.ToDateTime(currentRow.Cells["colAttTime"].Value.ToString()).ToString("yyyyMMddHHmmss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      string str2 = currentRow.Cells["colUserID"].Value.ToString();
      string str3 = Directory.GetCurrentDirectory() + "\\Capture\\" + str1 + "-" + str2 + ".jpg";
      this.picAttpho.Image = File.Exists(str3) ? Image.FromFile(str3) : this.picAttpho.ErrorImage;
      this.picAttpho.SizeMode = PictureBoxSizeMode.StretchImage;
      this.picAttpho.Update();
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
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.pb_Search = new PictureBox();
      this.txtUserID = new TextBox();
      this.lblUserID = new Label();
      this.btnClearListAttLog = new Button();
      this.dtpEndTime = new DateTimePicker();
      this.dtpStartTime = new DateTimePicker();
      this.lblModuleName = new Label();
      this.cmbDevSN = new ComboBox();
      this.lblDevSN = new Label();
      this.lblEndTime = new Label();
      this.lblStartTime = new Label();
      this.pnlData = new Panel();
      this.dgvAttendance = new DataGridView();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colDeviceID = new DataGridViewTextBoxColumn();
      this.colUserID = new DataGridViewTextBoxColumn();
      this.colAttState = new DataGridViewTextBoxColumn();
      this.colVerifyMode = new DataGridViewTextBoxColumn();
      this.colAttTime = new DataGridViewTextBoxColumn();
      this.colWorkName = new DataGridViewTextBoxColumn();
      this.pnlControl = new Panel();
      this.picAttpho = new PictureBox();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.pnlTop.SuspendLayout();
      ((ISupportInitialize) this.pb_Search).BeginInit();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvAttendance).BeginInit();
      this.pnlControl.SuspendLayout();
      ((ISupportInitialize) this.picAttpho).BeginInit();
      this.SuspendLayout();
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.pb_Search);
      this.pnlTop.Controls.Add((Control) this.txtUserID);
      this.pnlTop.Controls.Add((Control) this.lblUserID);
      this.pnlTop.Controls.Add((Control) this.btnClearListAttLog);
      this.pnlTop.Controls.Add((Control) this.dtpEndTime);
      this.pnlTop.Controls.Add((Control) this.dtpStartTime);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Controls.Add((Control) this.cmbDevSN);
      this.pnlTop.Controls.Add((Control) this.lblDevSN);
      this.pnlTop.Controls.Add((Control) this.lblEndTime);
      this.pnlTop.Controls.Add((Control) this.lblStartTime);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(885, 65);
      this.pnlTop.TabIndex = 0;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 5);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 69;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.pb_Search.Cursor = Cursors.Hand;
      this.pb_Search.Image = (Image) Resources.sousuo2;
      this.pb_Search.Location = new Point(744, 31);
      this.pb_Search.Name = "pb_Search";
      this.pb_Search.Size = new Size(27, 27);
      this.pb_Search.TabIndex = 68;
      this.pb_Search.TabStop = false;
      this.pb_Search.Click += new EventHandler(this.btnGetAttLog_Click);
      this.txtUserID.Location = new Point(619, 34);
      this.txtUserID.Name = "txtUserID";
      this.txtUserID.Size = new Size(121, 21);
      this.txtUserID.TabIndex = 61;
      this.lblUserID.AutoSize = true;
      this.lblUserID.Font = new Font("Arial", 12f);
      this.lblUserID.Location = new Point(559, 35);
      this.lblUserID.Name = "lblUserID";
      this.lblUserID.Size = new Size(56, 18);
      this.lblUserID.TabIndex = 60;
      this.lblUserID.Text = "UserID";
      this.btnClearListAttLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.btnClearListAttLog.BackColor = Color.FromArgb(37, 190, 167);
      this.btnClearListAttLog.Cursor = Cursors.Hand;
      this.btnClearListAttLog.FlatStyle = FlatStyle.Flat;
      this.btnClearListAttLog.Font = new Font("Arial", 12f);
      this.btnClearListAttLog.ForeColor = Color.White;
      this.btnClearListAttLog.Location = new Point(781, 31);
      this.btnClearListAttLog.Name = "btnClearListAttLog";
      this.btnClearListAttLog.Size = new Size(82, 27);
      this.btnClearListAttLog.TabIndex = 52;
      this.btnClearListAttLog.Text = "Clear";
      this.btnClearListAttLog.UseVisualStyleBackColor = false;
      this.btnClearListAttLog.Click += new EventHandler(this.btnClearListAttLog_Click);
      this.dtpEndTime.CalendarFont = new Font("宋体", 10f);
      this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpEndTime.Font = new Font("Arial", 9f);
      this.dtpEndTime.Format = DateTimePickerFormat.Custom;
      this.dtpEndTime.Location = new Point(419, 34);
      this.dtpEndTime.Name = "dtpEndTime";
      this.dtpEndTime.RightToLeft = RightToLeft.No;
      this.dtpEndTime.ShowUpDown = true;
      this.dtpEndTime.Size = new Size(136, 21);
      this.dtpEndTime.TabIndex = 65;
      this.dtpStartTime.CalendarFont = new Font("宋体", 10f);
      this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpStartTime.Font = new Font("Arial", 9f);
      this.dtpStartTime.Format = DateTimePickerFormat.Custom;
      this.dtpStartTime.Location = new Point(251, 34);
      this.dtpStartTime.Name = "dtpStartTime";
      this.dtpStartTime.RightToLeft = RightToLeft.No;
      this.dtpStartTime.ShowUpDown = true;
      this.dtpStartTime.Size = new Size(136, 21);
      this.dtpStartTime.TabIndex = 64;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(15, 6);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(87, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "Attendance";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.cmbDevSN.Font = new Font("Arial", 9f);
      this.cmbDevSN.FormattingEnabled = true;
      this.cmbDevSN.Location = new Point(47, 33);
      this.cmbDevSN.Name = "cmbDevSN";
      this.cmbDevSN.Size = new Size(121, 23);
      this.cmbDevSN.TabIndex = 63;
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 12f);
      this.lblDevSN.Location = new Point(13, 35);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(30, 18);
      this.lblDevSN.TabIndex = 62;
      this.lblDevSN.Text = "SN";
      this.lblEndTime.AutoSize = true;
      this.lblEndTime.Font = new Font("宋体", 12f);
      this.lblEndTime.Location = new Point(391, 36);
      this.lblEndTime.Name = "lblEndTime";
      this.lblEndTime.Size = new Size(24, 16);
      this.lblEndTime.TabIndex = 56;
      this.lblEndTime.Text = "--";
      this.lblStartTime.AutoSize = true;
      this.lblStartTime.Font = new Font("Arial", 12f);
      this.lblStartTime.Location = new Point(172, 35);
      this.lblStartTime.Name = "lblStartTime";
      this.lblStartTime.Size = new Size(75, 18);
      this.lblStartTime.TabIndex = 55;
      this.lblStartTime.Text = "StartTime";
      this.pnlData.Controls.Add((Control) this.dgvAttendance);
      this.pnlData.Controls.Add((Control) this.pnlControl);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 65);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(885, 498);
      this.pnlData.TabIndex = 2;
      this.dgvAttendance.AllowUserToAddRows = false;
      this.dgvAttendance.AllowUserToDeleteRows = false;
      this.dgvAttendance.AllowUserToResizeRows = false;
      this.dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvAttendance.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvAttendance.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvAttendance.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colDeviceID, (DataGridViewColumn) this.colUserID, (DataGridViewColumn) this.colAttState, (DataGridViewColumn) this.colVerifyMode, (DataGridViewColumn) this.colAttTime, (DataGridViewColumn) this.colWorkName);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvAttendance.DefaultCellStyle = gridViewCellStyle2;
      this.dgvAttendance.Dock = DockStyle.Fill;
      this.dgvAttendance.Location = new Point(0, 0);
      this.dgvAttendance.MultiSelect = false;
      this.dgvAttendance.Name = "dgvAttendance";
      this.dgvAttendance.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvAttendance.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvAttendance.RowHeadersVisible = false;
      this.dgvAttendance.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvAttendance.RowTemplate.Height = 23;
      this.dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvAttendance.Size = new Size(600, 498);
      this.dgvAttendance.TabIndex = 1;
      this.dgvAttendance.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvAttendance_CellMouseClick);
      this.dgvAttendance.RowPostPaint += new DataGridViewRowPostPaintEventHandler(this.dgvAttendance_RowPostPaint);
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle4;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.Width = 50;
      this.colDeviceID.DataPropertyName = "DeviceID";
      this.colDeviceID.HeaderText = "DeviceID";
      this.colDeviceID.MinimumWidth = 100;
      this.colDeviceID.Name = "colDeviceID";
      this.colDeviceID.ReadOnly = true;
      this.colUserID.DataPropertyName = "PIN";
      this.colUserID.HeaderText = "UserID";
      this.colUserID.MinimumWidth = 20;
      this.colUserID.Name = "colUserID";
      this.colUserID.ReadOnly = true;
      this.colAttState.DataPropertyName = "Status";
      this.colAttState.HeaderText = "AttState";
      this.colAttState.MinimumWidth = 55;
      this.colAttState.Name = "colAttState";
      this.colAttState.ReadOnly = true;
      this.colVerifyMode.DataPropertyName = "Verify";
      this.colVerifyMode.HeaderText = "VerifyMode";
      this.colVerifyMode.MinimumWidth = 80;
      this.colVerifyMode.Name = "colVerifyMode";
      this.colVerifyMode.ReadOnly = true;
      this.colAttTime.DataPropertyName = "AttTime";
      gridViewCellStyle5.Format = "yyyy-MM-dd HH:mm:ss";
      gridViewCellStyle5.NullValue = (object) null;
      this.colAttTime.DefaultCellStyle = gridViewCellStyle5;
      this.colAttTime.HeaderText = "AttTime";
      this.colAttTime.MinimumWidth = 150;
      this.colAttTime.Name = "colAttTime";
      this.colAttTime.ReadOnly = true;
      this.colWorkName.DataPropertyName = "workname";
      this.colWorkName.HeaderText = "WorkName";
      this.colWorkName.MinimumWidth = 80;
      this.colWorkName.Name = "colWorkName";
      this.colWorkName.ReadOnly = true;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.picAttpho);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Location = new Point(600, 0);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(285, 498);
      this.pnlControl.TabIndex = 5;
      this.picAttpho.ErrorImage = (Image) Resources.imgNoPhoto;
      this.picAttpho.Image = (Image) Resources.imgNoPhoto;
      this.picAttpho.InitialImage = (Image) null;
      this.picAttpho.Location = new Point(23, 52);
      this.picAttpho.Name = "picAttpho";
      this.picAttpho.Size = new Size(232, 290);
      this.picAttpho.SizeMode = PictureBoxSizeMode.StretchImage;
      this.picAttpho.TabIndex = 49;
      this.picAttpho.TabStop = false;
      this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn1.Frozen = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 50;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 75;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "DeviceID";
      this.dataGridViewTextBoxColumn2.HeaderText = "DeviceID";
      this.dataGridViewTextBoxColumn2.MinimumWidth = 100;
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 131;
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
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.Width = 50;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCAttendance);
      this.Size = new Size(885, 563);
      this.Load += new EventHandler(this.UCAttendance_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      ((ISupportInitialize) this.pb_Search).EndInit();
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvAttendance).EndInit();
      this.pnlControl.ResumeLayout(false);
      ((ISupportInitialize) this.picAttpho).EndInit();
      this.ResumeLayout(false);
    }
  }
}

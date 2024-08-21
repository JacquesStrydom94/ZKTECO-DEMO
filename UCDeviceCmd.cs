
using Attendance.Properties;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Utils;


namespace Attendance
{
  public class UCDeviceCmd : UserControl
  {
    private DeviceBll _bllDevice = new DeviceBll();
    private DeviceCmdBll _bll = new DeviceCmdBll();
    private IContainer components;
    private Panel pnlTop;
    private Panel pnlControl;
    private Panel pnlData;
    private Label lblModuleName;
    private DataGridView dgvDeviceCmd;
    private ComboBox cmbDevSN;
    private Label lblDevSN;
    private Label lblMsg;
    private Label lblStartTime;
    private Label lblEndTime;
    private Button btnGet;
    private Button btnGetAll;
    private Button btnClearList;
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
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colDevSN;
    private DataGridViewTextBoxColumn colCommitTime;
    private DataGridViewTextBoxColumn colContent;
    private DataGridViewTextBoxColumn colTransTime;
    private DataGridViewTextBoxColumn colResponseTime;
    private DataGridViewTextBoxColumn colReturnValue;

    public UCDeviceCmd() => this.InitializeComponent();

    private void UCDeviceCmd_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvDeviceCmd.AutoGenerateColumns = false;
      this.LoadDefaultData();
      this.LoadGridViewData();
    }

    private void btnGet_Click(object sender, EventArgs e) => this.LoadGridViewData();

    private void LoadGridViewData()
    {
      string devSN = this.cmbDevSN.Text.Trim();
      try
      {
        this.dgvDeviceCmd.DataSource = (object) this._bll.GetByTime(this.dtpStartTime.Value, this.dtpEndTime.Value, devSN);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load attlog info error:" + ex.ToString());
      }
    }

    private void btnGetAll_Click(object sender, EventArgs e)
    {
      try
      {
        this.dgvDeviceCmd.DataSource = (object) this._bll.GetAll();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load DeviceCmd info error:" + ex.ToString());
      }
    }

    private void btnClearList_Click(object sender, EventArgs e)
    {
      if (this._bll.ClearAll() <= 0)
        return;
      this.dgvDeviceCmd.DataSource = (object) null;
      this.lblMsg.Visible = true;
      this.lblMsg.Text = "Clear all cmd success";
    }

    private void LoadDefaultData()
    {
      DateTime dateTimeNow = Tools.GetDateTimeNow();
      this.dtpStartTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0);
      this.dtpEndTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 23, 59, 59);
      this.GetAllDevSNToCmbo();
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

    private void dgvDeviceCmd_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvDeviceCmd.Rows[e.Row.Index].Cells["colIndex"].Value = (object) (e.Row.Index + 1);
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
      DataGridViewCellStyle gridViewCellStyle7 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle8 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle9 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle10 = new DataGridViewCellStyle();
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.lblModuleName = new Label();
      this.pnlControl = new Panel();
      this.dtpEndTime = new DateTimePicker();
      this.dtpStartTime = new DateTimePicker();
      this.cmbDevSN = new ComboBox();
      this.lblDevSN = new Label();
      this.lblMsg = new Label();
      this.lblStartTime = new Label();
      this.lblEndTime = new Label();
      this.btnGet = new Button();
      this.btnGetAll = new Button();
      this.btnClearList = new Button();
      this.pnlData = new Panel();
      this.dgvDeviceCmd = new DataGridView();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colDevSN = new DataGridViewTextBoxColumn();
      this.colCommitTime = new DataGridViewTextBoxColumn();
      this.colContent = new DataGridViewTextBoxColumn();
      this.colTransTime = new DataGridViewTextBoxColumn();
      this.colResponseTime = new DataGridViewTextBoxColumn();
      this.colReturnValue = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.pnlTop.SuspendLayout();
      this.pnlControl.SuspendLayout();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvDeviceCmd).BeginInit();
      this.SuspendLayout();
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(885, 30);
      this.pnlTop.TabIndex = 0;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 4;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(91, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "DeviceCmd";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.dtpEndTime);
      this.pnlControl.Controls.Add((Control) this.dtpStartTime);
      this.pnlControl.Controls.Add((Control) this.cmbDevSN);
      this.pnlControl.Controls.Add((Control) this.lblDevSN);
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Controls.Add((Control) this.lblStartTime);
      this.pnlControl.Controls.Add((Control) this.lblEndTime);
      this.pnlControl.Controls.Add((Control) this.btnGet);
      this.pnlControl.Controls.Add((Control) this.btnGetAll);
      this.pnlControl.Controls.Add((Control) this.btnClearList);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Location = new Point(630, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size((int) byte.MaxValue, 533);
      this.pnlControl.TabIndex = 1;
      this.dtpEndTime.CalendarFont = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpEndTime.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpEndTime.Format = DateTimePickerFormat.Custom;
      this.dtpEndTime.Location = new Point(95, 128);
      this.dtpEndTime.Name = "dtpEndTime";
      this.dtpEndTime.RightToLeft = RightToLeft.No;
      this.dtpEndTime.ShowUpDown = true;
      this.dtpEndTime.Size = new Size(136, 21);
      this.dtpEndTime.TabIndex = 65;
      this.dtpStartTime.CalendarFont = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpStartTime.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpStartTime.Format = DateTimePickerFormat.Custom;
      this.dtpStartTime.Location = new Point(95, 98);
      this.dtpStartTime.Name = "dtpStartTime";
      this.dtpStartTime.RightToLeft = RightToLeft.No;
      this.dtpStartTime.ShowUpDown = true;
      this.dtpStartTime.Size = new Size(136, 21);
      this.dtpStartTime.TabIndex = 64;
      this.cmbDevSN.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbDevSN.FormattingEnabled = true;
      this.cmbDevSN.Location = new Point(95, 68);
      this.cmbDevSN.Name = "cmbDevSN";
      this.cmbDevSN.Size = new Size(136, 20);
      this.cmbDevSN.TabIndex = 63;
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 10f);
      this.lblDevSN.Location = new Point(19, 70);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(26, 16);
      this.lblDevSN.TabIndex = 62;
      this.lblDevSN.Text = "SN";
      this.lblMsg.AutoSize = true;
      this.lblMsg.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(26, 40);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(23, 12);
      this.lblMsg.TabIndex = 59;
      this.lblMsg.Text = "msg";
      this.lblMsg.Visible = false;
      this.lblStartTime.AutoSize = true;
      this.lblStartTime.Font = new Font("Arial", 10f);
      this.lblStartTime.Location = new Point(19, 100);
      this.lblStartTime.Name = "lblStartTime";
      this.lblStartTime.Size = new Size(68, 16);
      this.lblStartTime.TabIndex = 55;
      this.lblStartTime.Text = "StartTime";
      this.lblEndTime.AutoSize = true;
      this.lblEndTime.Font = new Font("Arial", 10f);
      this.lblEndTime.Location = new Point(19, 130);
      this.lblEndTime.Name = "lblEndTime";
      this.lblEndTime.Size = new Size(63, 16);
      this.lblEndTime.TabIndex = 56;
      this.lblEndTime.Text = "EndTime";
      this.btnGet.BackColor = Color.FromArgb(37, 190, 167);
      this.btnGet.Cursor = Cursors.Hand;
      this.btnGet.FlatStyle = FlatStyle.Flat;
      this.btnGet.Font = new Font("Arial", 12f);
      this.btnGet.ForeColor = Color.White;
      this.btnGet.Location = new Point(46, 187);
      this.btnGet.Name = "btnGet";
      this.btnGet.Size = new Size(157, 30);
      this.btnGet.TabIndex = 54;
      this.btnGet.Text = "Get Period Cmd";
      this.btnGet.UseVisualStyleBackColor = false;
      this.btnGet.Click += new EventHandler(this.btnGet_Click);
      this.btnGetAll.BackColor = Color.FromArgb(37, 190, 167);
      this.btnGetAll.Cursor = Cursors.Hand;
      this.btnGetAll.FlatStyle = FlatStyle.Flat;
      this.btnGetAll.Font = new Font("Arial", 12f);
      this.btnGetAll.ForeColor = Color.White;
      this.btnGetAll.Location = new Point(46, 292);
      this.btnGetAll.Name = "btnGetAll";
      this.btnGetAll.Size = new Size(157, 30);
      this.btnGetAll.TabIndex = 53;
      this.btnGetAll.Text = "Get All Cmd";
      this.btnGetAll.UseVisualStyleBackColor = false;
      this.btnGetAll.Click += new EventHandler(this.btnGetAll_Click);
      this.btnClearList.BackColor = Color.FromArgb(37, 190, 167);
      this.btnClearList.Cursor = Cursors.Hand;
      this.btnClearList.FlatStyle = FlatStyle.Flat;
      this.btnClearList.Font = new Font("Arial", 12f);
      this.btnClearList.ForeColor = Color.White;
      this.btnClearList.Location = new Point(46, 327);
      this.btnClearList.Name = "btnClearList";
      this.btnClearList.Size = new Size(157, 30);
      this.btnClearList.TabIndex = 52;
      this.btnClearList.Text = "Clear All Cmd";
      this.btnClearList.UseVisualStyleBackColor = false;
      this.btnClearList.Click += new EventHandler(this.btnClearList_Click);
      this.pnlData.Controls.Add((Control) this.dgvDeviceCmd);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(630, 533);
      this.pnlData.TabIndex = 2;
      this.dgvDeviceCmd.AllowUserToAddRows = false;
      this.dgvDeviceCmd.AllowUserToDeleteRows = false;
      this.dgvDeviceCmd.AllowUserToResizeRows = false;
      this.dgvDeviceCmd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvDeviceCmd.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvDeviceCmd.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvDeviceCmd.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colDevSN, (DataGridViewColumn) this.colCommitTime, (DataGridViewColumn) this.colContent, (DataGridViewColumn) this.colTransTime, (DataGridViewColumn) this.colResponseTime, (DataGridViewColumn) this.colReturnValue);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvDeviceCmd.DefaultCellStyle = gridViewCellStyle2;
      this.dgvDeviceCmd.Dock = DockStyle.Fill;
      this.dgvDeviceCmd.Location = new Point(0, 0);
      this.dgvDeviceCmd.MultiSelect = false;
      this.dgvDeviceCmd.Name = "dgvDeviceCmd";
      this.dgvDeviceCmd.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvDeviceCmd.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvDeviceCmd.RowHeadersVisible = false;
      this.dgvDeviceCmd.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvDeviceCmd.RowTemplate.Height = 23;
      this.dgvDeviceCmd.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvDeviceCmd.Size = new Size(630, 533);
      this.dgvDeviceCmd.TabIndex = 1;
      this.dgvDeviceCmd.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvDeviceCmd_RowStateChanged);
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle4;
      this.colIndex.FillWeight = 177.665f;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.Width = 50;
      this.colDevSN.DataPropertyName = "DevSN";
      this.colDevSN.FillWeight = 87.05584f;
      this.colDevSN.HeaderText = "DevSN";
      this.colDevSN.Name = "colDevSN";
      this.colDevSN.ReadOnly = true;
      this.colCommitTime.DataPropertyName = "CommitTime";
      gridViewCellStyle5.Format = "yyyy-MM-dd HH:mm:ss";
      this.colCommitTime.DefaultCellStyle = gridViewCellStyle5;
      this.colCommitTime.FillWeight = 87.05584f;
      this.colCommitTime.HeaderText = "CommitTime";
      this.colCommitTime.Name = "colCommitTime";
      this.colCommitTime.ReadOnly = true;
      this.colContent.DataPropertyName = "Content";
      this.colContent.FillWeight = 87.05584f;
      this.colContent.HeaderText = "Content";
      this.colContent.Name = "colContent";
      this.colContent.ReadOnly = true;
      this.colTransTime.DataPropertyName = "TransTime";
      gridViewCellStyle6.Format = "yyyy-MM-dd HH:mm:ss";
      this.colTransTime.DefaultCellStyle = gridViewCellStyle6;
      this.colTransTime.FillWeight = 87.05584f;
      this.colTransTime.HeaderText = "TransTime";
      this.colTransTime.Name = "colTransTime";
      this.colTransTime.ReadOnly = true;
      this.colResponseTime.DataPropertyName = "ResponseTime";
      gridViewCellStyle7.Format = "yyyy-MM-dd HH:mm:ss";
      this.colResponseTime.DefaultCellStyle = gridViewCellStyle7;
      this.colResponseTime.FillWeight = 87.05584f;
      this.colResponseTime.HeaderText = "ResponseTime";
      this.colResponseTime.Name = "colResponseTime";
      this.colResponseTime.ReadOnly = true;
      this.colReturnValue.DataPropertyName = "ReturnValue";
      this.colReturnValue.FillWeight = 87.05584f;
      this.colReturnValue.HeaderText = "ReturnValue";
      this.colReturnValue.Name = "colReturnValue";
      this.colReturnValue.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "DevSN";
      this.dataGridViewTextBoxColumn2.HeaderText = "DevSN";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 90;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "CommitTime";
      gridViewCellStyle8.Format = "yyyy-MM-dd hh:mm:ss";
      this.dataGridViewTextBoxColumn3.DefaultCellStyle = gridViewCellStyle8;
      this.dataGridViewTextBoxColumn3.HeaderText = "CommitTime";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 50;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Content";
      this.dataGridViewTextBoxColumn4.HeaderText = "Content";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 200;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "TransTime";
      gridViewCellStyle9.Format = "yyyy-MM-dd hh:mm:ss";
      this.dataGridViewTextBoxColumn5.DefaultCellStyle = gridViewCellStyle9;
      this.dataGridViewTextBoxColumn5.HeaderText = "TransTime";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.Width = 80;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "ResponseTime";
      gridViewCellStyle10.Format = "yyyy-MM-dd hh:mm:ss";
      this.dataGridViewTextBoxColumn6.DefaultCellStyle = gridViewCellStyle10;
      this.dataGridViewTextBoxColumn6.HeaderText = "ResponseTime";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.Width = 150;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "ReturnValue";
      this.dataGridViewTextBoxColumn7.HeaderText = "ReturnValue";
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
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCDeviceCmd);
      this.Size = new Size(885, 563);
      this.Load += new EventHandler(this.UCDeviceCmd_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvDeviceCmd).EndInit();
      this.ResumeLayout(false);
    }
  }
}

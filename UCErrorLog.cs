
using Attendance.Properties;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Attendance
{
  public class UCErrorLog : UserControl
  {
    private ErrorLogBll _bll = new ErrorLogBll();
    private DataTable _dt = new DataTable();
    private IContainer components;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private Label lblModuleName;
    private Panel pnlData;
    private Panel pnlTop;
    private DataGridView dgvErrorLog;
    private PictureBox pb_Search;
    private ComboBox cmb_DevSN;
    private Label lblDevSN;
    private Button btn_Clear;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn col_DeviceSN;
    private DataGridViewTextBoxColumn colErrorCode;
    private DataGridViewTextBoxColumn colTime;
    private DataGridViewTextBoxColumn colErrorMsg;
    private DataGridViewTextBoxColumn colDataOrigin;
    private DataGridViewTextBoxColumn colCmdId;
    private DataGridViewTextBoxColumn colAdditional;

    public UCErrorLog() => this.InitializeComponent();

    private void UCErrorLog_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvErrorLog.AutoGenerateColumns = false;
      this.GetAllDevSNToCmbo();
      this.LoadDefaultData("");
    }

    private void LoadDefaultData(string SN)
    {
      try
      {
        this._dt = this._bll.GetAll(SN);
        this.dgvErrorLog.DataSource = (object) this._dt;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load attlog info error:" + ex.ToString());
      }
    }

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

    public void AddNewRow(ErrorLogModel errorLogModel)
    {
      if (this._dt.Rows.Count == 0)
      {
        this.LoadDefaultData("");
      }
      else
      {
        DataRow row = this._dt.NewRow();
        row["ErrorCode"] = (object) errorLogModel.ErrCode;
        row["ErrorCode"] = (object) errorLogModel.ErrMsg;
        row["ErrorCode"] = (object) errorLogModel.DataOrigin;
        row["ErrorCode"] = (object) errorLogModel.CmdId;
        row["ErrorCode"] = (object) errorLogModel.Additional;
        row["DeviceID"] = (object) errorLogModel.DeviceID;
        this._dt.Rows.InsertAt(row, 0);
        this.dgvErrorLog.DataSource = (object) this._dt;
      }
    }

    private void pb_Search_Click(object sender, EventArgs e)
    {
      this.LoadDefaultData(this.cmb_DevSN.Text.Trim());
    }

    private void btn_Clear_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to delete all data?", "Tip", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK || this._bll.ClearAll() <= 0)
        return;
      this.dgvErrorLog.DataSource = (object) null;
    }

    private void dgvErrorLog_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvErrorLog.Rows[e.Row.Index].Cells["colIndex"].Value = (object) (e.Row.Index + 1);
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
      this.lblModuleName = new Label();
      this.pnlData = new Panel();
      this.dgvErrorLog = new DataGridView();
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.btn_Clear = new Button();
      this.pb_Search = new PictureBox();
      this.cmb_DevSN = new ComboBox();
      this.lblDevSN = new Label();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.col_DeviceSN = new DataGridViewTextBoxColumn();
      this.colErrorCode = new DataGridViewTextBoxColumn();
      this.colTime = new DataGridViewTextBoxColumn();
      this.colErrorMsg = new DataGridViewTextBoxColumn();
      this.colDataOrigin = new DataGridViewTextBoxColumn();
      this.colCmdId = new DataGridViewTextBoxColumn();
      this.colAdditional = new DataGridViewTextBoxColumn();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvErrorLog).BeginInit();
      this.pnlTop.SuspendLayout();
      ((ISupportInitialize) this.pb_Search).BeginInit();
      this.SuspendLayout();
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(70, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "ErrorLog";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlData.Controls.Add((Control) this.dgvErrorLog);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 65);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(816, 504);
      this.pnlData.TabIndex = 5;
      this.dgvErrorLog.AllowUserToAddRows = false;
      this.dgvErrorLog.AllowUserToDeleteRows = false;
      this.dgvErrorLog.AllowUserToResizeRows = false;
      this.dgvErrorLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvErrorLog.BackgroundColor = Color.White;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvErrorLog.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvErrorLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dgvErrorLog.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.col_DeviceSN, (DataGridViewColumn) this.colErrorCode, (DataGridViewColumn) this.colTime, (DataGridViewColumn) this.colErrorMsg, (DataGridViewColumn) this.colDataOrigin, (DataGridViewColumn) this.colCmdId, (DataGridViewColumn) this.colAdditional);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvErrorLog.DefaultCellStyle = gridViewCellStyle2;
      this.dgvErrorLog.Dock = DockStyle.Fill;
      this.dgvErrorLog.Location = new Point(0, 0);
      this.dgvErrorLog.MultiSelect = false;
      this.dgvErrorLog.Name = "dgvErrorLog";
      this.dgvErrorLog.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvErrorLog.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvErrorLog.RowHeadersVisible = false;
      this.dgvErrorLog.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvErrorLog.RowTemplate.Height = 23;
      this.dgvErrorLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvErrorLog.Size = new Size(816, 504);
      this.dgvErrorLog.TabIndex = 3;
      this.dgvErrorLog.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvErrorLog_RowStateChanged);
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.btn_Clear);
      this.pnlTop.Controls.Add((Control) this.pb_Search);
      this.pnlTop.Controls.Add((Control) this.cmb_DevSN);
      this.pnlTop.Controls.Add((Control) this.lblDevSN);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(816, 65);
      this.pnlTop.TabIndex = 3;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 68;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.btn_Clear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btn_Clear.BackColor = Color.FromArgb(37, 190, 167);
      this.btn_Clear.Cursor = Cursors.Hand;
      this.btn_Clear.FlatStyle = FlatStyle.Flat;
      this.btn_Clear.Font = new Font("Arial", 12f);
      this.btn_Clear.ForeColor = Color.White;
      this.btn_Clear.Location = new Point(701, 29);
      this.btn_Clear.Name = "btn_Clear";
      this.btn_Clear.Size = new Size(75, 30);
      this.btn_Clear.TabIndex = 67;
      this.btn_Clear.Text = "Clear";
      this.btn_Clear.UseVisualStyleBackColor = false;
      this.btn_Clear.Click += new EventHandler(this.btn_Clear_Click);
      this.pb_Search.Cursor = Cursors.Hand;
      this.pb_Search.Image = (Image) Resources.sousuo2;
      this.pb_Search.Location = new Point(187, 31);
      this.pb_Search.Name = "pb_Search";
      this.pb_Search.Size = new Size(27, 27);
      this.pb_Search.TabIndex = 66;
      this.pb_Search.TabStop = false;
      this.pb_Search.Click += new EventHandler(this.pb_Search_Click);
      this.cmb_DevSN.Font = new Font("Arial", 9f);
      this.cmb_DevSN.FormattingEnabled = true;
      this.cmb_DevSN.Location = new Point(48, 33);
      this.cmb_DevSN.Name = "cmb_DevSN";
      this.cmb_DevSN.Size = new Size(121, 23);
      this.cmb_DevSN.TabIndex = 65;
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 12f);
      this.lblDevSN.Location = new Point(14, 35);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(30, 18);
      this.lblDevSN.TabIndex = 64;
      this.lblDevSN.Text = "SN";
      this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn1.DefaultCellStyle = gridViewCellStyle4;
      this.dataGridViewTextBoxColumn1.FillWeight = 106.599f;
      this.dataGridViewTextBoxColumn1.Frozen = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 30;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "LTime";
      gridViewCellStyle5.Format = "yyyy-MM-dd HH:mm:ss";
      this.dataGridViewTextBoxColumn2.DefaultCellStyle = gridViewCellStyle5;
      this.dataGridViewTextBoxColumn2.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn2.HeaderText = "Time";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 120;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "LTypeName";
      gridViewCellStyle6.Format = "yyyy-MM-dd HH:mm:ss";
      this.dataGridViewTextBoxColumn3.DefaultCellStyle = gridViewCellStyle6;
      this.dataGridViewTextBoxColumn3.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn3.HeaderText = "Type";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Visible = false;
      this.dataGridViewTextBoxColumn3.Width = 120;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Content";
      this.dataGridViewTextBoxColumn4.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn4.HeaderText = "Content";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 300;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "DataOrigin";
      this.dataGridViewTextBoxColumn5.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn5.HeaderText = "DataOrigin";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.Width = 115;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "CmdId";
      this.dataGridViewTextBoxColumn6.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn6.HeaderText = "CmdId";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.Width = 115;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "Additional";
      this.dataGridViewTextBoxColumn7.FillWeight = 98.90017f;
      this.dataGridViewTextBoxColumn7.HeaderText = "Additional";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.Width = 115;
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.Width = 50;
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle7;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.Width = 50;
      this.col_DeviceSN.DataPropertyName = "DeviceID";
      this.col_DeviceSN.HeaderText = "DeviceSN";
      this.col_DeviceSN.Name = "col_DeviceSN";
      this.col_DeviceSN.ReadOnly = true;
      this.colErrorCode.DataPropertyName = "ErrCode";
      this.colErrorCode.FillWeight = 87.05584f;
      this.colErrorCode.HeaderText = "ErrorCode";
      this.colErrorCode.Name = "colErrorCode";
      this.colErrorCode.ReadOnly = true;
      this.colTime.DataPropertyName = "Time";
      gridViewCellStyle8.Format = "yyyy-MM-dd HH:mm:ss";
      this.colTime.DefaultCellStyle = gridViewCellStyle8;
      this.colTime.FillWeight = 87.05584f;
      this.colTime.HeaderText = "Time";
      this.colTime.Name = "colTime";
      this.colTime.ReadOnly = true;
      this.colTime.Visible = false;
      this.colErrorMsg.DataPropertyName = "ErrMsg";
      this.colErrorMsg.FillWeight = 87.05584f;
      this.colErrorMsg.HeaderText = "ErrorMsg";
      this.colErrorMsg.Name = "colErrorMsg";
      this.colErrorMsg.ReadOnly = true;
      this.colDataOrigin.DataPropertyName = "DataOrigin";
      this.colDataOrigin.FillWeight = 87.05584f;
      this.colDataOrigin.HeaderText = "DataOrigin";
      this.colDataOrigin.Name = "colDataOrigin";
      this.colDataOrigin.ReadOnly = true;
      this.colCmdId.DataPropertyName = "CmdId";
      this.colCmdId.FillWeight = 87.05584f;
      this.colCmdId.HeaderText = "CmdId";
      this.colCmdId.Name = "colCmdId";
      this.colCmdId.ReadOnly = true;
      this.colAdditional.DataPropertyName = "Additional";
      this.colAdditional.FillWeight = 87.05584f;
      this.colAdditional.HeaderText = "Additional";
      this.colAdditional.Name = "colAdditional";
      this.colAdditional.ReadOnly = true;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCErrorLog);
      this.Size = new Size(816, 569);
      this.Load += new EventHandler(this.UCErrorLog_Load);
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvErrorLog).EndInit();
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      ((ISupportInitialize) this.pb_Search).EndInit();
      this.ResumeLayout(false);
    }
  }
}

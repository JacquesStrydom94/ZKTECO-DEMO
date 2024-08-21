
using Attendance.Properties;
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Utils;


namespace Attendance
{
  public class UCDevice : UserControl
  {
    private Dictionary<string, int> _dicDevInterval = new Dictionary<string, int>();
    private string[] devCmd = new string[100];
    private DeviceBll _bll = new DeviceBll();
    private DataTable _dtDevice;
    private IContainer components;
    private DataGridView dgvDevice;
    private Label lblModuleName;
    private Panel pnlData;
    private Panel pnlControl;
    private Label lblMsg;
    private TextBox txtTransFlag;
    private Label lblTransFlag;
    private Button btnAdd;
    private Button btnDelete;
    private ComboBox cmbTimeZone;
    private Label lblTimeZone;
    private Button btnSave;
    private TextBox txtDevName;
    private Label lblDevName;
    private Label lblDevSN;
    private TextBox txtDevSN;
    private Panel pnlTop;
    private Timer timerGetDevStatus;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
    private Label label1;
    private DataGridViewImageColumn colStatus;
    private DataGridViewTextBoxColumn colDevSN;
    private DataGridViewTextBoxColumn colDevName;
    private DataGridViewTextBoxColumn colMask;
    private DataGridViewTextBoxColumn colTemp;
    private DataGridViewTextBoxColumn colDevIP;
    private DataGridViewTextBoxColumn colFirmwareVersion;
    private DataGridViewTextBoxColumn colTransFlag;
    private DataGridViewTextBoxColumn colTimeZone;

    public UCDevice() => this.InitializeComponent();

    private void UCDevice_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvDevice.AutoGenerateColumns = false;
      this.LoadDevice();
      if (this._dtDevice == null || this._dtDevice.Rows.Count != 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) this._dtDevice.Rows)
        {
          string key = row["DevSN"].ToString();
          Tools.TryConvertToInt32(row["Delay"]);
          if (!this._dicDevInterval.ContainsKey(key))
            this._dicDevInterval.Add(key, 0);
        }
      }
      this.timerGetDevStatus.Enabled = true;
      this.timerGetDevStatus.Interval = 1000;
    }

    private void LoadDevice()
    {
      this._dtDevice = this._bll.GetAll("");
      this.dgvDevice.DataSource = (object) this._dtDevice;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.lblMsg.Visible = false;
      DeviceModel deviceModel = new DeviceModel();
      this.txtDevSN.Text = deviceModel.DevSN;
      this.txtDevName.Text = deviceModel.DevName;
      this.txtTransFlag.Text = deviceModel.TransFlag;
      this.cmbTimeZone.Text = deviceModel.TimeZone;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.txtDevSN.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please input Device SN";
      }
      else
      {
        this.lblMsg.Visible = false;
        DeviceModel device1;
        if ((device1 = this._bll.Get(this.txtDevSN.Text.Trim())) != null)
        {
          device1.TimeZone = this.cmbTimeZone.Text;
          device1.DevName = this.txtDevName.Text;
          device1.TransFlag = this.txtTransFlag.Text.Trim();
          try
          {
            if (this._bll.Update(device1) > 0)
            {
              this.LoadDevice();
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Update device success";
              return;
            }
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Update device fail";
            return;
          }
          catch
          {
          }
        }
        this.lblMsg.Visible = false;
        DeviceModel device2 = new DeviceModel();
        device2.DevSN = this.txtDevSN.Text.Trim();
        device2.TimeZone = this.cmbTimeZone.Text;
        device2.DevName = this.txtDevName.Text;
        device2.TransFlag = this.txtTransFlag.Text.Trim();
        try
        {
          if (this._bll.Add(device2) > 0)
          {
            this.LoadDevice();
            if (!this._dicDevInterval.ContainsKey(device2.DevSN))
              this._dicDevInterval.Add(device2.DevSN, 0);
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Add Device SN " + this.txtDevSN.Text.Trim() + " Success";
          }
          else
          {
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Add Device SN " + this.txtDevSN.Text.Trim() + " Fail";
          }
        }
        catch (Exception ex)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = ex.ToString();
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.txtDevSN.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please Input Device SN first";
      }
      else
      {
        string str = this.txtDevSN.Text.Trim();
        if (this._bll.Get(str) == null)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = "Device SN is not exist";
        }
        else
        {
          this.lblMsg.Visible = false;
          if (this._bll.Delete(str) > 0)
          {
            this.LoadDevice();
            this._dicDevInterval.Remove(str);
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Remove device success";
          }
          else
          {
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Remove device fail";
          }
        }
      }
    }

    private void btnSend_Click(object sender, EventArgs e)
    {
    }

    private void dgvDevice_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvDevice.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvDevice.CurrentRow == null)
        return;
      DataGridViewRow currentRow = this.dgvDevice.CurrentRow;
      this.txtDevSN.Text = currentRow.Cells["colDevSN"].Value.ToString();
      this.txtDevName.Text = currentRow.Cells["colDevName"].Value.ToString();
      this.txtTransFlag.Text = currentRow.Cells["colTransFlag"].Value.ToString();
      this.cmbTimeZone.Text = currentRow.Cells["colTimeZone"].Value.ToString();
    }

    private void timerGetDevStatus_Tick(object sender, EventArgs e)
    {
      try
      {
        for (int index = 0; index < this.dgvDevice.Rows.Count; ++index)
        {
          string key = this.dgvDevice.Rows[index].Cells["colDevSN"].Value.ToString();
          if (this._dicDevInterval.ContainsKey(key))
          {
            if (Tools.TryConvertToInt32(this._dtDevice.Rows[index]["Delay"]) > this._dicDevInterval[key])
            {
              ++this._dicDevInterval[key];
            }
            else
            {
              this._dicDevInterval[key] = 0;
              this.dgvDevice.Rows[index].Cells["colStatus"].Value = (object) Resources.imgDevStatus1;
            }
          }
        }
      }
      catch
      {
      }
    }

    public void UpdateDeviceMask(string devSN, int maskStatus)
    {
      for (int index = 0; index < this.dgvDevice.Rows.Count; ++index)
      {
        string str1 = this.dgvDevice.Rows[index].Cells["colDevSN"].Value.ToString();
        if (!(devSN != str1))
        {
          string str2 = "";
          switch (maskStatus)
          {
            case 0:
              str2 = "No";
              break;
            case 1:
              str2 = "Yes";
              break;
            case 2:
              str2 = "";
              break;
          }
          this.dgvDevice.Rows[index].Cells["colMask"].Value = (object) str2;
        }
      }
    }

    public void UpdateDeviceTemp(string devSN, string tempVal)
    {
      for (int index = 0; index < this.dgvDevice.Rows.Count; ++index)
      {
        string str = this.dgvDevice.Rows[index].Cells["colDevSN"].Value.ToString();
        if (!(devSN != str))
        {
          tempVal = string.IsNullOrEmpty(tempVal) ? "Disable/NotFound" : tempVal;
          this.dgvDevice.Rows[index].Cells["colTemp"].Value = (object) tempVal;
        }
      }
    }

    public void UpdateDevice(DeviceModel dev)
    {
      for (int index = 0; index < this.dgvDevice.Rows.Count; ++index)
      {
        string key = this.dgvDevice.Rows[index].Cells["colDevSN"].Value.ToString();
        if (!(dev.DevSN != key))
        {
          this.dgvDevice.Rows[index].Cells["colStatus"].Value = (object) Resources.imgDevStatus2;
          if (this._dicDevInterval.ContainsKey(key))
            this._dicDevInterval[key] = 0;
          this.dgvDevice.Rows[index].Cells["colDevIP"].Value = (object) dev.DevIP;
          this.dgvDevice.Rows[index].Cells["colFirmwareVersion"].Value = (object) dev.DevFirmwareVersion;
        }
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
      this.components = (IContainer) new System.ComponentModel.Container();
      DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
      this.dgvDevice = new DataGridView();
      this.lblModuleName = new Label();
      this.pnlData = new Panel();
      this.pnlControl = new Panel();
      this.lblMsg = new Label();
      this.txtTransFlag = new TextBox();
      this.lblTransFlag = new Label();
      this.btnAdd = new Button();
      this.btnDelete = new Button();
      this.cmbTimeZone = new ComboBox();
      this.lblTimeZone = new Label();
      this.btnSave = new Button();
      this.txtDevName = new TextBox();
      this.lblDevName = new Label();
      this.lblDevSN = new Label();
      this.txtDevSN = new TextBox();
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.timerGetDevStatus = new Timer(this.components);
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
      this.colStatus = new DataGridViewImageColumn();
      this.colDevSN = new DataGridViewTextBoxColumn();
      this.colDevName = new DataGridViewTextBoxColumn();
      this.colMask = new DataGridViewTextBoxColumn();
      this.colTemp = new DataGridViewTextBoxColumn();
      this.colDevIP = new DataGridViewTextBoxColumn();
      this.colFirmwareVersion = new DataGridViewTextBoxColumn();
      this.colTransFlag = new DataGridViewTextBoxColumn();
      this.colTimeZone = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.dgvDevice).BeginInit();
      this.pnlData.SuspendLayout();
      this.pnlControl.SuspendLayout();
      this.pnlTop.SuspendLayout();
      this.SuspendLayout();
      this.dgvDevice.AllowUserToAddRows = false;
      this.dgvDevice.AllowUserToDeleteRows = false;
      this.dgvDevice.AllowUserToResizeRows = false;
      this.dgvDevice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvDevice.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.ControlText;
      gridViewCellStyle1.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvDevice.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvDevice.Columns.AddRange((DataGridViewColumn) this.colStatus, (DataGridViewColumn) this.colDevSN, (DataGridViewColumn) this.colDevName, (DataGridViewColumn) this.colMask, (DataGridViewColumn) this.colTemp, (DataGridViewColumn) this.colDevIP, (DataGridViewColumn) this.colFirmwareVersion, (DataGridViewColumn) this.colTransFlag, (DataGridViewColumn) this.colTimeZone);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvDevice.DefaultCellStyle = gridViewCellStyle2;
      this.dgvDevice.Dock = DockStyle.Fill;
      this.dgvDevice.Location = new Point(0, 0);
      this.dgvDevice.MultiSelect = false;
      this.dgvDevice.Name = "dgvDevice";
      this.dgvDevice.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvDevice.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvDevice.RowHeadersVisible = false;
      this.dgvDevice.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvDevice.RowTemplate.Height = 23;
      this.dgvDevice.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvDevice.Size = new Size(525, 611);
      this.dgvDevice.TabIndex = 0;
      this.dgvDevice.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvDevice_CellMouseClick);
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(57, 18);
      this.lblModuleName.TabIndex = 1;
      this.lblModuleName.Text = "Device";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlData.Controls.Add((Control) this.dgvDevice);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(525, 611);
      this.pnlData.TabIndex = 2;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Controls.Add((Control) this.txtTransFlag);
      this.pnlControl.Controls.Add((Control) this.lblTransFlag);
      this.pnlControl.Controls.Add((Control) this.btnAdd);
      this.pnlControl.Controls.Add((Control) this.btnDelete);
      this.pnlControl.Controls.Add((Control) this.cmbTimeZone);
      this.pnlControl.Controls.Add((Control) this.lblTimeZone);
      this.pnlControl.Controls.Add((Control) this.btnSave);
      this.pnlControl.Controls.Add((Control) this.txtDevName);
      this.pnlControl.Controls.Add((Control) this.lblDevName);
      this.pnlControl.Controls.Add((Control) this.lblDevSN);
      this.pnlControl.Controls.Add((Control) this.txtDevSN);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Location = new Point(525, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(346, 611);
      this.pnlControl.TabIndex = 3;
      this.lblMsg.AutoSize = true;
      this.lblMsg.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(29, 17);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(23, 12);
      this.lblMsg.TabIndex = 32;
      this.lblMsg.Text = "msg";
      this.lblMsg.Visible = false;
      this.txtTransFlag.Font = new Font("Arial", 9f);
      this.txtTransFlag.Location = new Point(107, 162);
      this.txtTransFlag.Name = "txtTransFlag";
      this.txtTransFlag.Size = new Size(216, 21);
      this.txtTransFlag.TabIndex = 31;
      this.lblTransFlag.AutoSize = true;
      this.lblTransFlag.Font = new Font("Arial", 10f);
      this.lblTransFlag.Location = new Point(28, 164);
      this.lblTransFlag.Name = "lblTransFlag";
      this.lblTransFlag.Size = new Size(67, 16);
      this.lblTransFlag.TabIndex = 30;
      this.lblTransFlag.Text = "Transflag";
      this.btnAdd.BackColor = Color.FromArgb(37, 190, 167);
      this.btnAdd.Cursor = Cursors.Hand;
      this.btnAdd.FlatAppearance.BorderColor = Color.White;
      this.btnAdd.FlatStyle = FlatStyle.Flat;
      this.btnAdd.Font = new Font("Arial", 12f);
      this.btnAdd.ForeColor = Color.White;
      this.btnAdd.ImageIndex = 2;
      this.btnAdd.Location = new Point(31, 210);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new Size(70, 30);
      this.btnAdd.TabIndex = 29;
      this.btnAdd.Text = "New";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
      this.btnDelete.BackColor = Color.FromArgb(37, 190, 167);
      this.btnDelete.Cursor = Cursors.Hand;
      this.btnDelete.FlatAppearance.BorderColor = Color.White;
      this.btnDelete.FlatStyle = FlatStyle.Flat;
      this.btnDelete.Font = new Font("Arial", 12f);
      this.btnDelete.ForeColor = Color.White;
      this.btnDelete.ImageIndex = 3;
      this.btnDelete.Location = new Point(243, 210);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new Size(79, 30);
      this.btnDelete.TabIndex = 25;
      this.btnDelete.Text = "Remove";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
      this.cmbTimeZone.Font = new Font("Arial", 9f);
      this.cmbTimeZone.FormattingEnabled = true;
      this.cmbTimeZone.Items.AddRange(new object[53]
      {
        (object) "-12:30",
        (object) "-12:00",
        (object) "-11:30",
        (object) "-11:00",
        (object) "-10:30",
        (object) "-10:00",
        (object) "-09:30",
        (object) "-09:00",
        (object) "-08:30",
        (object) "-08:00",
        (object) "-07:30",
        (object) "-07:00",
        (object) "-06:30",
        (object) "-06:00",
        (object) "-05:30",
        (object) "-05:00",
        (object) "-04:30",
        (object) "-04:00",
        (object) "-03:30",
        (object) "-03:00",
        (object) "-02:30",
        (object) "-02:00",
        (object) "-01:30",
        (object) "-01:00",
        (object) "-00:30",
        (object) "",
        (object) "+00:30",
        (object) "+01:00",
        (object) "+01:30",
        (object) "+02:00",
        (object) "+02:30",
        (object) "+03:00",
        (object) "+03:30",
        (object) "+04:00",
        (object) "+04:30",
        (object) "+05:00",
        (object) "+05:30",
        (object) "+06:00",
        (object) "+06:30",
        (object) "+07:00",
        (object) "+07:30",
        (object) "+08:00",
        (object) "+08:30",
        (object) "+09:00",
        (object) "+09:30",
        (object) "+10:00",
        (object) "+10:30",
        (object) "+11:00",
        (object) "+11:30",
        (object) "+12:00",
        (object) "+12:30",
        (object) "+13:00",
        (object) "+13:30"
      });
      this.cmbTimeZone.Location = new Point(107, 131);
      this.cmbTimeZone.Name = "cmbTimeZone";
      this.cmbTimeZone.Size = new Size(216, 23);
      this.cmbTimeZone.TabIndex = 24;
      this.lblTimeZone.AutoSize = true;
      this.lblTimeZone.Font = new Font("Arial", 10f);
      this.lblTimeZone.Location = new Point(28, 134);
      this.lblTimeZone.Name = "lblTimeZone";
      this.lblTimeZone.Size = new Size(70, 16);
      this.lblTimeZone.TabIndex = 23;
      this.lblTimeZone.Text = "TimeZone";
      this.btnSave.BackColor = Color.FromArgb(37, 190, 167);
      this.btnSave.Cursor = Cursors.Hand;
      this.btnSave.FlatAppearance.BorderColor = Color.White;
      this.btnSave.FlatStyle = FlatStyle.Flat;
      this.btnSave.Font = new Font("Arial", 12f);
      this.btnSave.ForeColor = Color.White;
      this.btnSave.ImageIndex = 4;
      this.btnSave.Location = new Point(137, 210);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(70, 30);
      this.btnSave.TabIndex = 22;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.txtDevName.Font = new Font("Arial", 9f);
      this.txtDevName.Location = new Point(107, 102);
      this.txtDevName.Name = "txtDevName";
      this.txtDevName.Size = new Size(216, 21);
      this.txtDevName.TabIndex = 20;
      this.lblDevName.AutoSize = true;
      this.lblDevName.Font = new Font("Arial", 10f);
      this.lblDevName.Location = new Point(28, 104);
      this.lblDevName.Name = "lblDevName";
      this.lblDevName.Size = new Size(44, 16);
      this.lblDevName.TabIndex = 19;
      this.lblDevName.Text = "Name";
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 10f);
      this.lblDevSN.Location = new Point(28, 74);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(26, 16);
      this.lblDevSN.TabIndex = 18;
      this.lblDevSN.Text = "SN";
      this.txtDevSN.Font = new Font("Arial", 9f);
      this.txtDevSN.Location = new Point(107, 72);
      this.txtDevSN.Name = "txtDevSN";
      this.txtDevSN.Size = new Size(216, 21);
      this.txtDevSN.TabIndex = 17;
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(871, 30);
      this.pnlTop.TabIndex = 4;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 2;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.timerGetDevStatus.Interval = 1000;
      this.timerGetDevStatus.Tick += new EventHandler(this.timerGetDevStatus_Tick);
      this.dataGridViewImageColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewImageColumn1.Frozen = true;
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.ReadOnly = true;
      this.dataGridViewImageColumn1.Width = 50;
      this.dataGridViewTextBoxColumn1.DataPropertyName = "DevSN";
      this.dataGridViewTextBoxColumn1.HeaderText = "DeviceSN";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn1.Width = 97;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "DevName";
      this.dataGridViewTextBoxColumn2.HeaderText = "DeviceName";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 98;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "DevIP";
      this.dataGridViewTextBoxColumn3.HeaderText = "DeviceIP";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn3.Width = 97;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "DevFirmwareVersion";
      this.dataGridViewTextBoxColumn4.HeaderText = "FirmwareVersion";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn4.Width = 195;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "DevIP";
      this.dataGridViewTextBoxColumn5.HeaderText = "DeviceIP";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn5.Width = 65;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "DevFirmwareVersion";
      this.dataGridViewTextBoxColumn6.HeaderText = "FirmwareVersion";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn6.Width = 65;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "TransFlag";
      this.dataGridViewTextBoxColumn7.HeaderText = "TransFlag";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn7.Visible = false;
      this.dataGridViewTextBoxColumn8.DataPropertyName = "TimeZone";
      this.dataGridViewTextBoxColumn8.HeaderText = "TimeZone";
      this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
      this.dataGridViewTextBoxColumn8.ReadOnly = true;
      this.dataGridViewTextBoxColumn8.Visible = false;
      this.colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.colStatus.Frozen = true;
      this.colStatus.HeaderText = "Status";
      this.colStatus.Image = (Image) Resources.imgDevStatus1;
      this.colStatus.MinimumWidth = 70;
      this.colStatus.Name = "colStatus";
      this.colStatus.ReadOnly = true;
      this.colStatus.Width = 70;
      this.colDevSN.DataPropertyName = "DevSN";
      this.colDevSN.HeaderText = "DeviceSN";
      this.colDevSN.Name = "colDevSN";
      this.colDevSN.ReadOnly = true;
      this.colDevSN.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colDevName.DataPropertyName = "DevName";
      this.colDevName.HeaderText = "DeviceName";
      this.colDevName.Name = "colDevName";
      this.colDevName.ReadOnly = true;
      this.colMask.HeaderText = "Mask";
      this.colMask.Name = "colMask";
      this.colMask.ReadOnly = true;
      this.colMask.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colTemp.HeaderText = "Temperature";
      this.colTemp.Name = "colTemp";
      this.colTemp.ReadOnly = true;
      this.colTemp.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colDevIP.DataPropertyName = "DevIP";
      this.colDevIP.HeaderText = "DeviceIP";
      this.colDevIP.Name = "colDevIP";
      this.colDevIP.ReadOnly = true;
      this.colDevIP.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colFirmwareVersion.DataPropertyName = "DevFirmwareVersion";
      this.colFirmwareVersion.HeaderText = "FirmwareVersion";
      this.colFirmwareVersion.Name = "colFirmwareVersion";
      this.colFirmwareVersion.ReadOnly = true;
      this.colFirmwareVersion.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colTransFlag.DataPropertyName = "TransFlag";
      this.colTransFlag.HeaderText = "TransFlag";
      this.colTransFlag.Name = "colTransFlag";
      this.colTransFlag.ReadOnly = true;
      this.colTransFlag.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colTransFlag.Visible = false;
      this.colTimeZone.DataPropertyName = "TimeZone";
      this.colTimeZone.HeaderText = "TimeZone";
      this.colTimeZone.Name = "colTimeZone";
      this.colTimeZone.ReadOnly = true;
      this.colTimeZone.Visible = false;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCDevice);
      this.Size = new Size(871, 641);
      this.Load += new EventHandler(this.UCDevice_Load);
      ((ISupportInitialize) this.dgvDevice).EndInit();
      this.pnlData.ResumeLayout(false);
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}


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
  public class UCCreateCmd : UserControl
  {
    private DeviceBll _bllDevice = new DeviceBll();
    private DeviceCmdBll _bll = new DeviceCmdBll();
    private string _devSN;
    private Dictionary<string, string> _dicCmd = new Dictionary<string, string>();
    private IContainer components;
    private Panel pnlTop;
    private Panel pnlControl;
    private Panel pnlData;
    private Label lblModuleName;
    private DataGridView dgvDevice;
    private Label lblMsg;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private RadioButton rbtnDelete;
    private RadioButton rbtnQuery;
    private RadioButton rbtnClear;
    private RadioButton rbtnControl;
    private RichTextBox rtxtCmd;
    private ComboBox cmbUpdate;
    private RadioButton rbtnUpdate;
    private Button btnCreate;
    private ComboBox cmbControl;
    private ComboBox cmbClear;
    private ComboBox cmbQuery;
    private ComboBox cmbDelete;
    private RadioButton rbtnUserDefined;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colDevSN;
    private DataGridViewTextBoxColumn colDevName;
    private DataGridViewTextBoxColumn colDevIP;

    public UCCreateCmd() => this.InitializeComponent();

    private void UCCreateCmd_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvDevice.AutoGenerateColumns = false;
      this.LoadDevice();
      this.LoadCmd();
      this.rbtnControl.Checked = true;
    }

    private void LoadDevice()
    {
      DataTable all = this._bllDevice.GetAll("");
      this.dgvDevice.DataSource = (object) all;
      if (all == null || all.Rows.Count <= 0)
        return;
      this._devSN = all.Rows[0]["DevSN"].ToString();
    }

    private void dgvDevice_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvDevice.Rows[e.Row.Index].Cells["colIndex"].Value = (object) (e.Row.Index + 1);
    }

    private void dgvDevice_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvDevice.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvDevice.CurrentRow == null)
        return;
      this._devSN = this.dgvDevice.CurrentRow.Cells["colDevSN"].Value.ToString();
    }

    private void LoadCmd()
    {
      this._dicCmd.Add("Reboot", "REBOOT");
      this._dicCmd.Add("UnLock", "AC_UNLOCK");
      this._dicCmd.Add("UnAlarm", "AC_UNALARM");
      this._dicCmd.Add("Info", "INFO");
      this.cmbControl.Items.AddRange((object[]) new string[4]
      {
        "Reboot",
        "UnLock",
        "UnAlarm",
        "Info"
      });
      this._dicCmd.Add("UpdateUserInfo", "DATA UPDATE USERINFO PIN={0}\tName={1}\tPri={2}\tPasswd={3}\tCard={4}\tGrp={5}\tTZ={6}");
      this._dicCmd.Add("UpdateFingerTmp", "DATA UPDATE FINGERTMP PIN={0}\tFID={1}\tSize={2}\tValid={3}\tTMP={4}");
      this._dicCmd.Add("UpdateFaceTmp", "DATA UPDATE FACE PIN={0}\tFID={1}\tValid={2}\tSize={3}\tTMP={4}");
      this._dicCmd.Add("UpdateFvein", "DATA$ UPDATE FVEIN Pin={0}\tFID={1}\tIndex={2}\tValid={3}\tSize={4}\tTmp={5}");
      this._dicCmd.Add("UpdateBioData", "DATA UPDATE BIODATA Pin={0}\tNo={1}\tIndex={2}\tValid={3}\tDuress={4}\tType={5}\tMajorVer={6}\tMinorVer ={7}\tFormat={8}\tTmp={9}");
      this._dicCmd.Add("UpdateUserPic", "DATA UPDATE USERPIC PIN={0}\tSize={1}\tContent={2}");
      this._dicCmd.Add("UpdateSMS", "DATA UPDATE SMS MSG={0}\tTAG={1}\tUID={2}\tMIN={3}\tStartTime={4}");
      this._dicCmd.Add("UpdateUserSMS", "DATA UPDATE USER_SMS PIN={0}\tUID={1}");
      this._dicCmd.Add("UpdateAdPic", "DATA UPDATE ADPIC Index={0}\tSize={1}\tExtension={2}\tContent={3}");
      this._dicCmd.Add("UpdateWorkCode", "DATA UPDATE WORKCODE PIN={0}\tCODE={1}\tNAME={2}");
      this._dicCmd.Add("UpdateShortcutKey", "DATA UPDATE ShortcutKey KeyID={0}\tKeyFun={1}\tStatusCode=={2}\tShowName={3}\tAutoState={4}\tAutoTime={5}\tSun={6}\tMon={7}\tTue={8}\tWed={9}\tThu={10}\tFri={11}\tSat={12}");
      this._dicCmd.Add("UpdateAccGroup", "DATA UPDATE AccGroup ID={0}\tVerify={1}\tValidHoliday={2}\tTZ={3}");
      this._dicCmd.Add("UpdateAccTimeZone", "DATA UPDATE AccTimeZone UID={0}\tSunStart={1}\tSunEnd={2}\tMonStart={3}\tMonEnd={4}\tTuesStart={5}\tTuesEnd ={6}\tWedStart={7}\tWedEnd={8}\tThursStart={9}\tThursEnd={10}\tFriStart={11}\tFriEnd={12}\tSatStart={13}\tSatEnd={14}");
      this._dicCmd.Add("UpdateAccHoliday", "DATA UPDATE AccHoliday UID={0}\tHolidayName={1}\tStartDate={2}\tEndDate={3}\tTimeZone={4}");
      this._dicCmd.Add("UpdateAccUnLockComb", "DATA UPDATE AccUnLockComb UID={0}\tGroup1={1}\tGroup2={2}\tGroup3={3}\tGroup4={4}\tGroup5={5}");
      this._dicCmd.Add("UpdateBlackList", "DATA UPDATE Blacklist IDNum={0}");
      this.cmbUpdate.Items.AddRange((object[]) new string[16]
      {
        "UpdateUserInfo",
        "UpdateFingerTmp",
        "UpdateFaceTmp",
        "UpdateFvein",
        "UpdateBioData",
        "UpdateUserPic",
        "UpdateSMS",
        "UpdateUserSMS",
        "UpdateAdPic",
        "UpdateWorkCode",
        "UpdateShortcutKey",
        "UpdateAccGroup",
        "UpdateAccTimeZone",
        "UpdateAccHoliday",
        "UpdateAccUnLockComb",
        "UpdateBlackList"
      });
      this._dicCmd.Add("DeleteUser", "DATA DELETE USERINFO PIN={0}");
      this._dicCmd.Add("DeleteFingerTmp1", "DATA DELETE FINGERTMP PIN={0}");
      this._dicCmd.Add("DeleteFingerTmp2", "DATA DELETE FINGERTMP PIN={0}\tFID={1}");
      this._dicCmd.Add("DeleteFace", "DATA DELETE FACE PIN={0}");
      this._dicCmd.Add("DeleteFvein1", "DATA DELETE FVEIN Pin={0}");
      this._dicCmd.Add("DeleteFvein2", "DATA DELETE FVEIN Pin={0}\tFID={1}");
      this._dicCmd.Add("DeleteBioData1", "DATA DELETE BIODATA Pin={0}");
      this._dicCmd.Add("DeleteBioData2", "DATA DELETE BIODATA Pin={0}\tType={1}");
      this._dicCmd.Add("DeleteBioData3", "DATA DELETE BIODATA Pin={0}\tType={1}\tNo={2}");
      this._dicCmd.Add("DeleteUserPic", "DATA DELETE USERPIC PIN={0}");
      this._dicCmd.Add("DeleteBioPhoto", "DATA DELETE BIOPHOTO PIN={0}");
      this._dicCmd.Add("DeleteSMS", "DATA DELETE SMS UID={0}");
      this._dicCmd.Add("DeleteWorkCode", "DATA DELETE WORKCODE CODE={0}");
      this._dicCmd.Add("DeleteAdPic", "DATA DELETE ADPIC Index={0}");
      this.cmbDelete.Items.AddRange((object[]) new string[14]
      {
        "DeleteUser",
        "DeleteFingerTmp1",
        "DeleteFingerTmp2",
        "DeleteFace",
        "DeleteFvein1",
        "DeleteFvein2",
        "DeleteBioData1",
        "DeleteBioData2",
        "DeleteBioData3",
        "DeleteUserPic",
        "DeleteBioPhoto",
        "DeleteSMS",
        "DeleteWorkCode",
        "DeleteAdPic"
      });
      this._dicCmd.Add("QueryAttLog", "DATA QUERY ATTLOG StartTime={0}\tEndTime={1}");
      this._dicCmd.Add("QueryAttPhoto", "DATA QUERY ATTPHOTO StartTime={0}\tEndTime={1}");
      this._dicCmd.Add("QueryUserInfo", "DATA QUERY USERINFO PIN={0}");
      this._dicCmd.Add("QueryFingerTmp", "DATA QUERY FINGERTMP PIN={0}\tFID={1}");
      this._dicCmd.Add("QueryBioData1", "DATA QUERY BIODATA Type={0}");
      this._dicCmd.Add("QueryBioData2", "DATA QUERY BIODATA Type={0}\tPIN={1}");
      this._dicCmd.Add("QueryBioData3", "DATA QUERY BIODATA Type={0}\tPIN={1}\tNo={2}");
      this.cmbQuery.Items.AddRange((object[]) new string[7]
      {
        "QueryAttLog",
        "QueryAttPhoto",
        "QueryUserInfo",
        "QueryFingerTmp",
        "QueryBioData1",
        "QueryBioData2",
        "QueryBioData3"
      });
      this._dicCmd.Add("ClearLog", "CLEAR LOG");
      this._dicCmd.Add("ClearPhoto", "CLEAR PHOTO");
      this._dicCmd.Add("ClearData", "CLEAR DATA");
      this._dicCmd.Add("ClearBioData", "CLEAR BIODATA");
      this.cmbClear.Items.AddRange((object[]) new string[4]
      {
        "ClearLog",
        "ClearPhoto",
        "ClearData",
        "ClearBioData"
      });
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this._devSN))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please select a device";
      }
      else if (string.IsNullOrEmpty(this.rtxtCmd.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Cmd content is empty";
      }
      else if (!this.CheckCmd(this.rtxtCmd.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Cmd content is invalid";
      }
      else if (this._bll.Add(new DeviceCmdModel()
      {
        DevSN = this._devSN,
        Content = this.rtxtCmd.Text,
        CommitTime = Tools.GetDateTimeNow()
      }) > 0)
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Success";
      }
      else
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Fail";
      }
    }

    private bool CheckCmd(string cmd) => cmd.IndexOf('{') < 0 && cmd.IndexOf('}') < 0;

    private void GetCmdContent(ComboBox cmb)
    {
      if (cmb.SelectedIndex < 0)
        return;
      string key = cmb.SelectedItem.ToString();
      if (this._dicCmd.ContainsKey(key))
        this.rtxtCmd.Text = this._dicCmd[key];
      else
        this.rtxtCmd.Text = "";
    }

    private void cmbControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.GetCmdContent((ComboBox) sender);
    }

    private void cmbUpdate_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.GetCmdContent((ComboBox) sender);
    }

    private void cmbDelete_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.GetCmdContent((ComboBox) sender);
    }

    private void cmbQuery_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.GetCmdContent((ComboBox) sender);
    }

    private void cmbClear_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.GetCmdContent((ComboBox) sender);
    }

    private void rbtnUserDefined_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.rtxtCmd.Text = "";
    }

    private void rbtnClear_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.cmbClear.Enabled = true;
    }

    private void rbtnQuery_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.cmbQuery.Enabled = true;
    }

    private void rbtnDelete_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.cmbDelete.Enabled = true;
    }

    private void rbtnUpdate_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.cmbUpdate.Enabled = true;
    }

    private void rbtnControl_CheckedChanged(object sender, EventArgs e)
    {
      this.DisableCombobox();
      this.cmbControl.Enabled = true;
    }

    private void DisableCombobox()
    {
      this.cmbControl.Enabled = false;
      this.cmbUpdate.Enabled = false;
      this.cmbDelete.Enabled = false;
      this.cmbQuery.Enabled = false;
      this.cmbClear.Enabled = false;
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
      this.lblModuleName = new Label();
      this.pnlControl = new Panel();
      this.rbtnUserDefined = new RadioButton();
      this.cmbControl = new ComboBox();
      this.cmbClear = new ComboBox();
      this.cmbQuery = new ComboBox();
      this.cmbDelete = new ComboBox();
      this.rbtnDelete = new RadioButton();
      this.rbtnQuery = new RadioButton();
      this.rbtnClear = new RadioButton();
      this.rbtnControl = new RadioButton();
      this.rtxtCmd = new RichTextBox();
      this.cmbUpdate = new ComboBox();
      this.rbtnUpdate = new RadioButton();
      this.btnCreate = new Button();
      this.lblMsg = new Label();
      this.pnlData = new Panel();
      this.dgvDevice = new DataGridView();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colDevSN = new DataGridViewTextBoxColumn();
      this.colDevName = new DataGridViewTextBoxColumn();
      this.colDevIP = new DataGridViewTextBoxColumn();
      this.pnlTop.SuspendLayout();
      this.pnlControl.SuspendLayout();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvDevice).BeginInit();
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
      this.label1.TabIndex = 3;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(90, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "CreateCmd";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.rbtnUserDefined);
      this.pnlControl.Controls.Add((Control) this.cmbControl);
      this.pnlControl.Controls.Add((Control) this.cmbClear);
      this.pnlControl.Controls.Add((Control) this.cmbQuery);
      this.pnlControl.Controls.Add((Control) this.cmbDelete);
      this.pnlControl.Controls.Add((Control) this.rbtnDelete);
      this.pnlControl.Controls.Add((Control) this.rbtnQuery);
      this.pnlControl.Controls.Add((Control) this.rbtnClear);
      this.pnlControl.Controls.Add((Control) this.rbtnControl);
      this.pnlControl.Controls.Add((Control) this.rtxtCmd);
      this.pnlControl.Controls.Add((Control) this.cmbUpdate);
      this.pnlControl.Controls.Add((Control) this.rbtnUpdate);
      this.pnlControl.Controls.Add((Control) this.btnCreate);
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.pnlControl.Location = new Point(355, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(530, 533);
      this.pnlControl.TabIndex = 1;
      this.rbtnUserDefined.AutoSize = true;
      this.rbtnUserDefined.Font = new Font("Arial", 10f);
      this.rbtnUserDefined.Location = new Point(17, 274);
      this.rbtnUserDefined.Name = "rbtnUserDefined";
      this.rbtnUserDefined.Size = new Size(109, 20);
      this.rbtnUserDefined.TabIndex = 72;
      this.rbtnUserDefined.TabStop = true;
      this.rbtnUserDefined.Text = "User-Defined";
      this.rbtnUserDefined.UseVisualStyleBackColor = true;
      this.rbtnUserDefined.CheckedChanged += new EventHandler(this.rbtnUserDefined_CheckedChanged);
      this.cmbControl.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbControl.FormattingEnabled = true;
      this.cmbControl.Location = new Point(87, 58);
      this.cmbControl.Name = "cmbControl";
      this.cmbControl.Size = new Size(153, 20);
      this.cmbControl.TabIndex = 71;
      this.cmbControl.SelectedIndexChanged += new EventHandler(this.cmbControl_SelectedIndexChanged);
      this.cmbClear.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbClear.FormattingEnabled = true;
      this.cmbClear.Location = new Point(87, 236);
      this.cmbClear.Name = "cmbClear";
      this.cmbClear.Size = new Size(153, 20);
      this.cmbClear.TabIndex = 70;
      this.cmbClear.SelectedIndexChanged += new EventHandler(this.cmbClear_SelectedIndexChanged);
      this.cmbQuery.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbQuery.FormattingEnabled = true;
      this.cmbQuery.Location = new Point(87, 189);
      this.cmbQuery.Name = "cmbQuery";
      this.cmbQuery.Size = new Size(153, 20);
      this.cmbQuery.TabIndex = 69;
      this.cmbQuery.SelectedIndexChanged += new EventHandler(this.cmbQuery_SelectedIndexChanged);
      this.cmbDelete.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbDelete.FormattingEnabled = true;
      this.cmbDelete.Location = new Point(87, 145);
      this.cmbDelete.Name = "cmbDelete";
      this.cmbDelete.Size = new Size(153, 20);
      this.cmbDelete.TabIndex = 68;
      this.cmbDelete.SelectedIndexChanged += new EventHandler(this.cmbDelete_SelectedIndexChanged);
      this.rbtnDelete.AutoSize = true;
      this.rbtnDelete.Font = new Font("Arial", 10f);
      this.rbtnDelete.Location = new Point(17, 145);
      this.rbtnDelete.Name = "rbtnDelete";
      this.rbtnDelete.Size = new Size(67, 20);
      this.rbtnDelete.TabIndex = 67;
      this.rbtnDelete.TabStop = true;
      this.rbtnDelete.Text = "Delete";
      this.rbtnDelete.UseVisualStyleBackColor = true;
      this.rbtnDelete.CheckedChanged += new EventHandler(this.rbtnDelete_CheckedChanged);
      this.rbtnQuery.AutoSize = true;
      this.rbtnQuery.Font = new Font("Arial", 10f);
      this.rbtnQuery.Location = new Point(17, 189);
      this.rbtnQuery.Name = "rbtnQuery";
      this.rbtnQuery.Size = new Size(65, 20);
      this.rbtnQuery.TabIndex = 66;
      this.rbtnQuery.TabStop = true;
      this.rbtnQuery.Text = "Query";
      this.rbtnQuery.UseVisualStyleBackColor = true;
      this.rbtnQuery.CheckedChanged += new EventHandler(this.rbtnQuery_CheckedChanged);
      this.rbtnClear.AutoSize = true;
      this.rbtnClear.Font = new Font("Arial", 10f);
      this.rbtnClear.Location = new Point(17, 236);
      this.rbtnClear.Name = "rbtnClear";
      this.rbtnClear.Size = new Size(60, 20);
      this.rbtnClear.TabIndex = 65;
      this.rbtnClear.TabStop = true;
      this.rbtnClear.Text = "Clear";
      this.rbtnClear.UseVisualStyleBackColor = true;
      this.rbtnClear.CheckedChanged += new EventHandler(this.rbtnClear_CheckedChanged);
      this.rbtnControl.AutoSize = true;
      this.rbtnControl.Font = new Font("Arial", 10f);
      this.rbtnControl.Location = new Point(17, 58);
      this.rbtnControl.Name = "rbtnControl";
      this.rbtnControl.Size = new Size(72, 20);
      this.rbtnControl.TabIndex = 64;
      this.rbtnControl.TabStop = true;
      this.rbtnControl.Text = "Control";
      this.rbtnControl.UseVisualStyleBackColor = true;
      this.rbtnControl.CheckedChanged += new EventHandler(this.rbtnControl_CheckedChanged);
      this.rtxtCmd.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.rtxtCmd.Location = new Point(248, 57);
      this.rtxtCmd.Name = "rtxtCmd";
      this.rtxtCmd.Size = new Size(268, 243);
      this.rtxtCmd.TabIndex = 63;
      this.rtxtCmd.Text = "";
      this.cmbUpdate.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbUpdate.FormattingEnabled = true;
      this.cmbUpdate.Location = new Point(87, 99);
      this.cmbUpdate.Name = "cmbUpdate";
      this.cmbUpdate.Size = new Size(153, 20);
      this.cmbUpdate.TabIndex = 62;
      this.cmbUpdate.SelectedIndexChanged += new EventHandler(this.cmbUpdate_SelectedIndexChanged);
      this.rbtnUpdate.AutoSize = true;
      this.rbtnUpdate.Font = new Font("Arial", 10f);
      this.rbtnUpdate.Location = new Point(17, 99);
      this.rbtnUpdate.Name = "rbtnUpdate";
      this.rbtnUpdate.Size = new Size(71, 20);
      this.rbtnUpdate.TabIndex = 61;
      this.rbtnUpdate.TabStop = true;
      this.rbtnUpdate.Text = "Update";
      this.rbtnUpdate.UseVisualStyleBackColor = true;
      this.rbtnUpdate.CheckedChanged += new EventHandler(this.rbtnUpdate_CheckedChanged);
      this.btnCreate.BackColor = Color.FromArgb(37, 190, 167);
      this.btnCreate.Cursor = Cursors.Hand;
      this.btnCreate.FlatStyle = FlatStyle.Flat;
      this.btnCreate.Font = new Font("Arial", 12f);
      this.btnCreate.ForeColor = Color.White;
      this.btnCreate.Location = new Point(413, 306);
      this.btnCreate.Name = "btnCreate";
      this.btnCreate.Size = new Size(75, 30);
      this.btnCreate.TabIndex = 60;
      this.btnCreate.Text = "Create";
      this.btnCreate.UseVisualStyleBackColor = false;
      this.btnCreate.Click += new EventHandler(this.btnCreate_Click);
      this.lblMsg.AutoSize = true;
      this.lblMsg.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(15, 11);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(23, 12);
      this.lblMsg.TabIndex = 59;
      this.lblMsg.Text = "msg";
      this.lblMsg.Visible = false;
      this.pnlData.Controls.Add((Control) this.dgvDevice);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(355, 533);
      this.pnlData.TabIndex = 2;
      this.dgvDevice.AllowUserToAddRows = false;
      this.dgvDevice.AllowUserToDeleteRows = false;
      this.dgvDevice.AllowUserToResizeRows = false;
      this.dgvDevice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvDevice.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvDevice.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvDevice.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colDevSN, (DataGridViewColumn) this.colDevName, (DataGridViewColumn) this.colDevIP);
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
      this.dgvDevice.Size = new Size(355, 533);
      this.dgvDevice.TabIndex = 1;
      this.dgvDevice.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvDevice_CellMouseClick);
      this.dgvDevice.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvDevice_RowStateChanged);
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn1.DefaultCellStyle = gridViewCellStyle4;
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "DeviceID";
      this.dataGridViewTextBoxColumn2.HeaderText = "DeviceID";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 88;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "PIN";
      this.dataGridViewTextBoxColumn3.HeaderText = "UserID";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 50;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Status";
      this.dataGridViewTextBoxColumn4.HeaderText = "AttState";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 55;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "Verify";
      this.dataGridViewTextBoxColumn5.HeaderText = "VerifyMode";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.Width = 80;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "AttTime";
      this.dataGridViewTextBoxColumn6.HeaderText = "AttTime";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.Width = 150;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "WorkCode";
      this.dataGridViewTextBoxColumn7.HeaderText = "WorkCode";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.Width = 80;
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.Width = 50;
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle5;
      this.colIndex.FillWeight = 101.5228f;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colIndex.Width = 50;
      this.colDevSN.DataPropertyName = "DevSN";
      this.colDevSN.FillWeight = 99.49239f;
      this.colDevSN.HeaderText = "DevSN";
      this.colDevSN.Name = "colDevSN";
      this.colDevSN.ReadOnly = true;
      this.colDevName.DataPropertyName = "DevName";
      this.colDevName.FillWeight = 99.49239f;
      this.colDevName.HeaderText = "DevName";
      this.colDevName.Name = "colDevName";
      this.colDevName.ReadOnly = true;
      this.colDevIP.DataPropertyName = "DevIP";
      this.colDevIP.FillWeight = 99.49239f;
      this.colDevIP.HeaderText = "DevIP";
      this.colDevIP.Name = "colDevIP";
      this.colDevIP.ReadOnly = true;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCCreateCmd);
      this.Size = new Size(885, 563);
      this.Load += new EventHandler(this.UCCreateCmd_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvDevice).EndInit();
      this.ResumeLayout(false);
    }
  }
}

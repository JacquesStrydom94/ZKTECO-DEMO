
using Attendance.Properties;
using BLL;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Utils;


namespace Attendance
{
  public class UCSms : UserControl
  {
    private DeviceBll _bllDevice = new DeviceBll();
    private SmsBll _bll = new SmsBll();
    private DataTable _dt = new DataTable();
    private IContainer components;
    private Panel pnlTop;
    private Panel pnlControl;
    private Panel pnlData;
    private Label lblModuleName;
    private DataGridView dgvSms;
    private ComboBox cmbDevSN;
    private Label lblDevSN;
    private TextBox txtUserID;
    private Label lblUserID;
    private Label lblMsg;
    private Label lblBeginTime;
    private DataGridViewImageColumn dataGridViewImageColumn1;
    private DateTimePicker dtpBeginTime;
    private TextBox txtContent;
    private Label lblContent;
    private Label lblValidTime;
    private ComboBox cmbType;
    private Label lblType;
    private Button btnUpload;
    private Button btnDelete;
    private Button btnSave;
    private Button btnAdd;
    private TextBox txtSmsID;
    private Label lblSmdID;
    private Label lblMinute;
    private NumericUpDown numValidTime;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colSmsID;
    private DataGridViewTextBoxColumn colType;
    private DataGridViewTextBoxColumn colValidTime;
    private DataGridViewTextBoxColumn colBeginTime;
    private DataGridViewTextBoxColumn colContent;
    private DataGridViewTextBoxColumn colUserID;

    public UCSms() => this.InitializeComponent();

    private void UCSms_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvSms.AutoGenerateColumns = false;
      this.LoadDefaultData(sender, e);
    }

    public void LoadAllSMS()
    {
      this.dgvSms.DataSource = (object) null;
      this._dt = this._bll.GetAll("");
      this.dgvSms.DataSource = (object) this._dt;
    }

    private void LoadDefaultData(object sender, EventArgs e)
    {
      this.cmbType.DataSource = (object) new ArrayList()
      {
        (object) new DictionaryEntry((object) "253", (object) "Common"),
        (object) new DictionaryEntry((object) "254", (object) "User"),
        (object) new DictionaryEntry((object) "255", (object) "Reserved")
      };
      this.cmbType.DisplayMember = "Value";
      this.cmbType.ValueMember = "Key";
      DateTime dateTimeNow = Tools.GetDateTimeNow();
      this.dtpBeginTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0);
      this.GetAllDevSNToCmbo(sender, e);
      this.LoadAllSMS();
    }

    private void GetAllDevSNToCmbo(object sender, EventArgs e)
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

    private void dgvSms_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvSms.Rows[e.Row.Index].Cells["colIndex"].Value = (object) (e.Row.Index + 1);
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.lblMsg.Visible = false;
      this.cmbType.SelectedIndex = 0;
      this.txtSmsID.Text = string.Empty;
      this.numValidTime.Value = 0M;
      DateTime dateTimeNow = Tools.GetDateTimeNow();
      this.dtpBeginTime.Value = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 0, 0, 0);
      this.txtContent.Text = string.Empty;
      this.txtUserID.Text = "";
      this.txtUserID.Enabled = false;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      int int32 = Tools.TryConvertToInt32(this.txtSmsID.Text, 0);
      if (int32 == 0)
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "SmdID is null or error.";
      }
      else if (string.IsNullOrEmpty(this.txtContent.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Content is Empty.";
      }
      else if (this.cmbType.SelectedValue == (object) "254" && string.IsNullOrEmpty(this.txtUserID.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "UserID is Empty.";
      }
      else
      {
        this.lblMsg.Visible = false;
        SMSModel model = this._bll.Get(this.txtSmsID.Text.Trim()) ?? new SMSModel();
        model.Type = Tools.TryConvertToInt32(this.cmbType.SelectedValue);
        model.SMSId = int32;
        model.ValidTime = Convert.ToInt32(this.numValidTime.Value);
        model.BeginTime = this.dtpBeginTime.Value;
        model.Content = this.txtContent.Text.Trim();
        model.UserID = this.txtUserID.Text.Trim();
        try
        {
          if (model.ID == 0)
          {
            if (this._bll.Add(model) > 0)
            {
              this.LoadAllSMS();
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add successful.";
            }
            else
            {
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add fail.";
            }
          }
          else if (this._bll.Update(model) > 0)
          {
            this.LoadAllSMS();
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Update successful.";
          }
          else
          {
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Update fail.";
          }
        }
        catch (Exception ex)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = ex.ToString();
        }
      }
    }

    private void btnUpload_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.cmbDevSN.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "DevSN is Empty.";
      }
      else if (string.IsNullOrEmpty(this.txtSmsID.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "SmdID is Empty.";
      }
      else if (string.IsNullOrEmpty(this.txtContent.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Content is Empty.";
      }
      else
      {
        this.lblMsg.Visible = false;
        DeviceCmdModel dCmd = new DeviceCmdModel();
        dCmd.DevSN = this.cmbDevSN.Text;
        dCmd.CommitTime = Tools.GetDateTimeNow();
        string str1 = this.txtContent.Text.Trim();
        string str2 = this.cmbType.SelectedValue.ToString();
        string str3 = this.txtSmsID.Text.Trim();
        int int32 = Tools.TryConvertToInt32((object) this.numValidTime);
        string str4 = this.dtpBeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (str2 == "254")
        {
          string str5 = this.txtUserID.Text.Trim();
          dCmd.Content = string.Format("DATA UPDATE USER_SMS PIN={0}\tUID={1}", (object) str5, (object) str3);
        }
        else
          dCmd.Content = string.Format("DATA UPDATE SMS MSG={0}\tTAG={1}\tUID={2}\tMIN={3}\tStartTime={4}", (object) str1, (object) str2, (object) str3, (object) int32, (object) str4);
        this.lblMsg.Visible = true;
        if (new DeviceCmdBll().Add(dCmd) >= 0)
          this.lblMsg.Text = "Operate successful.";
        else
          this.lblMsg.Text = "Operate fail.";
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.txtSmsID.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please input SmdID.";
      }
      else
      {
        this.lblMsg.Visible = false;
        if (this._bll.Get(this.txtSmsID.Text) == null)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = "The SMS is not exist.";
        }
        else
        {
          this.lblMsg.Visible = false;
          if (this._bll.Delete(this.txtSmsID.Text) > 0)
          {
            this.LoadAllSMS();
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Operate successful.";
          }
          else
          {
            this.lblMsg.Visible = true;
            this.lblMsg.Text = "Operate fail.";
          }
        }
      }
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.cmbType.SelectedValue != null && this.cmbType.SelectedValue.ToString() == "254")
      {
        this.txtUserID.Enabled = true;
      }
      else
      {
        this.txtUserID.Enabled = false;
        this.txtUserID.Text = "";
      }
    }

    private void dgvSms_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvSms.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvSms.CurrentRow == null)
        return;
      DataGridViewRow currentRow = this.dgvSms.CurrentRow;
      this.cmbType.SelectedValue = (object) currentRow.Cells["colType"].Value.ToString();
      this.txtSmsID.Text = currentRow.Cells["colSmsID"].Value.ToString();
      this.numValidTime.Value = (Decimal) Tools.TryConvertToInt32(currentRow.Cells["colValidTime"].Value);
      this.dtpBeginTime.Value = Convert.ToDateTime(currentRow.Cells["colBeginTime"].Value.ToString());
      this.txtContent.Text = currentRow.Cells["colContent"].Value.ToString();
      this.txtUserID.Text = currentRow.Cells["colUserID"].Value.ToString();
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
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.lblModuleName = new Label();
      this.pnlControl = new Panel();
      this.lblMinute = new Label();
      this.numValidTime = new NumericUpDown();
      this.txtContent = new TextBox();
      this.lblContent = new Label();
      this.lblValidTime = new Label();
      this.cmbType = new ComboBox();
      this.lblType = new Label();
      this.btnUpload = new Button();
      this.btnDelete = new Button();
      this.btnSave = new Button();
      this.btnAdd = new Button();
      this.txtSmsID = new TextBox();
      this.lblSmdID = new Label();
      this.dtpBeginTime = new DateTimePicker();
      this.cmbDevSN = new ComboBox();
      this.lblDevSN = new Label();
      this.txtUserID = new TextBox();
      this.lblUserID = new Label();
      this.lblMsg = new Label();
      this.lblBeginTime = new Label();
      this.pnlData = new Panel();
      this.dgvSms = new DataGridView();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewImageColumn1 = new DataGridViewImageColumn();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colSmsID = new DataGridViewTextBoxColumn();
      this.colType = new DataGridViewTextBoxColumn();
      this.colValidTime = new DataGridViewTextBoxColumn();
      this.colBeginTime = new DataGridViewTextBoxColumn();
      this.colContent = new DataGridViewTextBoxColumn();
      this.colUserID = new DataGridViewTextBoxColumn();
      this.pnlTop.SuspendLayout();
      this.pnlControl.SuspendLayout();
      this.numValidTime.BeginInit();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvSms).BeginInit();
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
      this.label1.TabIndex = 5;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(43, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "SMS";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.lblMinute);
      this.pnlControl.Controls.Add((Control) this.numValidTime);
      this.pnlControl.Controls.Add((Control) this.txtContent);
      this.pnlControl.Controls.Add((Control) this.lblContent);
      this.pnlControl.Controls.Add((Control) this.lblValidTime);
      this.pnlControl.Controls.Add((Control) this.cmbType);
      this.pnlControl.Controls.Add((Control) this.lblType);
      this.pnlControl.Controls.Add((Control) this.btnUpload);
      this.pnlControl.Controls.Add((Control) this.btnDelete);
      this.pnlControl.Controls.Add((Control) this.btnSave);
      this.pnlControl.Controls.Add((Control) this.btnAdd);
      this.pnlControl.Controls.Add((Control) this.txtSmsID);
      this.pnlControl.Controls.Add((Control) this.lblSmdID);
      this.pnlControl.Controls.Add((Control) this.dtpBeginTime);
      this.pnlControl.Controls.Add((Control) this.cmbDevSN);
      this.pnlControl.Controls.Add((Control) this.lblDevSN);
      this.pnlControl.Controls.Add((Control) this.txtUserID);
      this.pnlControl.Controls.Add((Control) this.lblUserID);
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Controls.Add((Control) this.lblBeginTime);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Location = new Point(533, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(352, 533);
      this.pnlControl.TabIndex = 1;
      this.lblMinute.AutoSize = true;
      this.lblMinute.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMinute.Location = new Point(156, 112);
      this.lblMinute.Name = "lblMinute";
      this.lblMinute.Size = new Size(53, 12);
      this.lblMinute.TabIndex = 79;
      this.lblMinute.Text = "(Minute)";
      this.numValidTime.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.numValidTime.Location = new Point(96, 108);
      this.numValidTime.Name = "numValidTime";
      this.numValidTime.Size = new Size(54, 21);
      this.numValidTime.TabIndex = 78;
      this.txtContent.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtContent.Location = new Point(96, 168);
      this.txtContent.MaxLength = 320;
      this.txtContent.Multiline = true;
      this.txtContent.Name = "txtContent";
      this.txtContent.Size = new Size(178, 98);
      this.txtContent.TabIndex = 77;
      this.lblContent.AutoSize = true;
      this.lblContent.Font = new Font("Arial", 10f);
      this.lblContent.Location = new Point(18, 168);
      this.lblContent.Name = "lblContent";
      this.lblContent.Size = new Size(58, 16);
      this.lblContent.TabIndex = 76;
      this.lblContent.Text = "Content";
      this.lblValidTime.AutoSize = true;
      this.lblValidTime.Font = new Font("Arial", 10f);
      this.lblValidTime.Location = new Point(18, 110);
      this.lblValidTime.Name = "lblValidTime";
      this.lblValidTime.Size = new Size(68, 16);
      this.lblValidTime.TabIndex = 74;
      this.lblValidTime.Text = "ValidTime";
      this.cmbType.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbType.FormattingEnabled = true;
      this.cmbType.Items.AddRange(new object[2]
      {
        (object) "User",
        (object) "Administrator"
      });
      this.cmbType.Location = new Point(96, 46);
      this.cmbType.Name = "cmbType";
      this.cmbType.Size = new Size(134, 20);
      this.cmbType.TabIndex = 73;
      this.cmbType.SelectedIndexChanged += new EventHandler(this.cmbType_SelectedIndexChanged);
      this.lblType.AutoSize = true;
      this.lblType.Font = new Font("Arial", 10f);
      this.lblType.Location = new Point(18, 48);
      this.lblType.Name = "lblType";
      this.lblType.Size = new Size(39, 16);
      this.lblType.TabIndex = 72;
      this.lblType.Text = "Type";
      this.btnUpload.BackColor = Color.FromArgb(37, 190, 167);
      this.btnUpload.Cursor = Cursors.Hand;
      this.btnUpload.FlatAppearance.BorderColor = Color.White;
      this.btnUpload.FlatStyle = FlatStyle.Flat;
      this.btnUpload.Font = new Font("Arial", 12f);
      this.btnUpload.ForeColor = Color.White;
      this.btnUpload.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnUpload.ImageIndex = 2;
      this.btnUpload.Location = new Point(176, 334);
      this.btnUpload.Name = "btnUpload";
      this.btnUpload.Size = new Size(70, 30);
      this.btnUpload.TabIndex = 71;
      this.btnUpload.Text = "Upload";
      this.btnUpload.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnUpload.UseVisualStyleBackColor = false;
      this.btnUpload.Click += new EventHandler(this.btnUpload_Click);
      this.btnDelete.BackColor = Color.FromArgb(37, 190, 167);
      this.btnDelete.Cursor = Cursors.Hand;
      this.btnDelete.FlatAppearance.BorderColor = Color.White;
      this.btnDelete.FlatStyle = FlatStyle.Flat;
      this.btnDelete.Font = new Font("Arial", 12f);
      this.btnDelete.ForeColor = Color.White;
      this.btnDelete.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnDelete.ImageIndex = 3;
      this.btnDelete.Location = new Point((int) byte.MaxValue, 334);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new Size(70, 30);
      this.btnDelete.TabIndex = 70;
      this.btnDelete.Text = "Delete";
      this.btnDelete.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnDelete.UseVisualStyleBackColor = false;
      this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
      this.btnSave.BackColor = Color.FromArgb(37, 190, 167);
      this.btnSave.Cursor = Cursors.Hand;
      this.btnSave.FlatAppearance.BorderColor = Color.White;
      this.btnSave.FlatStyle = FlatStyle.Flat;
      this.btnSave.Font = new Font("Arial", 12f);
      this.btnSave.ForeColor = Color.White;
      this.btnSave.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnSave.ImageIndex = 1;
      this.btnSave.Location = new Point(97, 334);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(70, 30);
      this.btnSave.TabIndex = 69;
      this.btnSave.Text = "Save";
      this.btnSave.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnSave.UseVisualStyleBackColor = false;
      this.btnSave.Click += new EventHandler(this.btnSave_Click);
      this.btnAdd.BackColor = Color.FromArgb(37, 190, 167);
      this.btnAdd.Cursor = Cursors.Hand;
      this.btnAdd.FlatAppearance.BorderColor = Color.White;
      this.btnAdd.FlatStyle = FlatStyle.Flat;
      this.btnAdd.Font = new Font("Arial", 12f);
      this.btnAdd.ForeColor = Color.White;
      this.btnAdd.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnAdd.ImageIndex = 0;
      this.btnAdd.Location = new Point(18, 334);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new Size(70, 30);
      this.btnAdd.TabIndex = 68;
      this.btnAdd.Text = "New";
      this.btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnAdd.UseVisualStyleBackColor = false;
      this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
      this.txtSmsID.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtSmsID.Location = new Point(96, 78);
      this.txtSmsID.Name = "txtSmsID";
      this.txtSmsID.Size = new Size(134, 21);
      this.txtSmsID.TabIndex = 67;
      this.lblSmdID.AutoSize = true;
      this.lblSmdID.Font = new Font("Arial", 10f);
      this.lblSmdID.Location = new Point(19, 80);
      this.lblSmdID.Name = "lblSmdID";
      this.lblSmdID.Size = new Size(48, 16);
      this.lblSmdID.TabIndex = 66;
      this.lblSmdID.Text = "SmsID";
      this.dtpBeginTime.CalendarFont = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpBeginTime.CustomFormat = "yyyy-MM-dd HH:mm";
      this.dtpBeginTime.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.dtpBeginTime.Format = DateTimePickerFormat.Custom;
      this.dtpBeginTime.Location = new Point(96, 139);
      this.dtpBeginTime.Name = "dtpBeginTime";
      this.dtpBeginTime.RightToLeft = RightToLeft.No;
      this.dtpBeginTime.ShowUpDown = true;
      this.dtpBeginTime.Size = new Size(136, 21);
      this.dtpBeginTime.TabIndex = 64;
      this.cmbDevSN.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbDevSN.FormattingEnabled = true;
      this.cmbDevSN.Location = new Point(96, 300);
      this.cmbDevSN.Name = "cmbDevSN";
      this.cmbDevSN.Size = new Size(136, 20);
      this.cmbDevSN.TabIndex = 63;
      this.lblDevSN.AutoSize = true;
      this.lblDevSN.Font = new Font("Arial", 10f);
      this.lblDevSN.Location = new Point(18, 302);
      this.lblDevSN.Name = "lblDevSN";
      this.lblDevSN.Size = new Size(51, 16);
      this.lblDevSN.TabIndex = 62;
      this.lblDevSN.Text = "DevSN";
      this.txtUserID.Enabled = false;
      this.txtUserID.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtUserID.Location = new Point(96, 270);
      this.txtUserID.Name = "txtUserID";
      this.txtUserID.Size = new Size(134, 21);
      this.txtUserID.TabIndex = 61;
      this.lblUserID.AutoSize = true;
      this.lblUserID.Font = new Font("Arial", 10f);
      this.lblUserID.Location = new Point(19, 272);
      this.lblUserID.Name = "lblUserID";
      this.lblUserID.Size = new Size(50, 16);
      this.lblUserID.TabIndex = 60;
      this.lblUserID.Text = "UserID";
      this.lblMsg.AutoSize = true;
      this.lblMsg.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(18, 13);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(23, 12);
      this.lblMsg.TabIndex = 59;
      this.lblMsg.Text = "msg";
      this.lblMsg.Visible = false;
      this.lblBeginTime.AutoSize = true;
      this.lblBeginTime.Font = new Font("Arial", 10f);
      this.lblBeginTime.Location = new Point(18, 141);
      this.lblBeginTime.Name = "lblBeginTime";
      this.lblBeginTime.Size = new Size(74, 16);
      this.lblBeginTime.TabIndex = 55;
      this.lblBeginTime.Text = "BeginTime";
      this.pnlData.Controls.Add((Control) this.dgvSms);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(533, 533);
      this.pnlData.TabIndex = 2;
      this.dgvSms.AllowUserToAddRows = false;
      this.dgvSms.AllowUserToDeleteRows = false;
      this.dgvSms.AllowUserToResizeRows = false;
      this.dgvSms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvSms.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvSms.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvSms.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colSmsID, (DataGridViewColumn) this.colType, (DataGridViewColumn) this.colValidTime, (DataGridViewColumn) this.colBeginTime, (DataGridViewColumn) this.colContent, (DataGridViewColumn) this.colUserID);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvSms.DefaultCellStyle = gridViewCellStyle2;
      this.dgvSms.Dock = DockStyle.Fill;
      this.dgvSms.Location = new Point(0, 0);
      this.dgvSms.MultiSelect = false;
      this.dgvSms.Name = "dgvSms";
      this.dgvSms.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvSms.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvSms.RowHeadersVisible = false;
      this.dgvSms.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvSms.RowTemplate.Height = 23;
      this.dgvSms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvSms.Size = new Size(533, 533);
      this.dgvSms.TabIndex = 1;
      this.dgvSms.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvSms_CellMouseClick);
      this.dgvSms.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvSms_RowStateChanged);
      this.dataGridViewTextBoxColumn1.HeaderText = "Index";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "SmsID";
      this.dataGridViewTextBoxColumn2.HeaderText = "SmsID";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 40;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "Type";
      this.dataGridViewTextBoxColumn3.HeaderText = "Type";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 40;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "ValidTime";
      this.dataGridViewTextBoxColumn4.HeaderText = "ValidTime";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 70;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "BeginTime";
      gridViewCellStyle4.Format = "yyyy-MM-dd HH:mm:ss";
      gridViewCellStyle4.NullValue = (object) null;
      this.dataGridViewTextBoxColumn5.DefaultCellStyle = gridViewCellStyle4;
      this.dataGridViewTextBoxColumn5.HeaderText = "BeginTime";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.Width = 150;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "Content";
      this.dataGridViewTextBoxColumn6.HeaderText = "Content";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.Width = 75;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "UserID";
      this.dataGridViewTextBoxColumn7.HeaderText = "UserID";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.Width = 80;
      this.dataGridViewImageColumn1.HeaderText = "Status";
      this.dataGridViewImageColumn1.Image = (Image) Resources.imgDevStatus1;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.Width = 50;
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colIndex.DefaultCellStyle = gridViewCellStyle5;
      this.colIndex.FillWeight = 177.665f;
      this.colIndex.Frozen = true;
      this.colIndex.HeaderText = "Index";
      this.colIndex.MinimumWidth = 50;
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.Width = 50;
      this.colSmsID.DataPropertyName = "SmsID";
      this.colSmsID.FillWeight = 87.05584f;
      this.colSmsID.HeaderText = "SmsID";
      this.colSmsID.Name = "colSmsID";
      this.colSmsID.ReadOnly = true;
      this.colType.DataPropertyName = "Type";
      this.colType.FillWeight = 87.05584f;
      this.colType.HeaderText = "Type";
      this.colType.Name = "colType";
      this.colType.ReadOnly = true;
      this.colValidTime.DataPropertyName = "ValidTime";
      this.colValidTime.FillWeight = 87.05584f;
      this.colValidTime.HeaderText = "ValidTime";
      this.colValidTime.Name = "colValidTime";
      this.colValidTime.ReadOnly = true;
      this.colBeginTime.DataPropertyName = "BeginTime";
      gridViewCellStyle6.Format = "yyyy-MM-dd HH:mm:ss";
      gridViewCellStyle6.NullValue = (object) null;
      this.colBeginTime.DefaultCellStyle = gridViewCellStyle6;
      this.colBeginTime.FillWeight = 87.05584f;
      this.colBeginTime.HeaderText = "BeginTime";
      this.colBeginTime.Name = "colBeginTime";
      this.colBeginTime.ReadOnly = true;
      this.colContent.DataPropertyName = "Content";
      this.colContent.FillWeight = 87.05584f;
      this.colContent.HeaderText = "Content";
      this.colContent.Name = "colContent";
      this.colContent.ReadOnly = true;
      this.colUserID.DataPropertyName = "UserID";
      this.colUserID.FillWeight = 87.05584f;
      this.colUserID.HeaderText = "UserID";
      this.colUserID.Name = "colUserID";
      this.colUserID.ReadOnly = true;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCSms);
      this.Size = new Size(885, 563);
      this.Load += new EventHandler(this.UCSms_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      this.numValidTime.EndInit();
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvSms).EndInit();
      this.ResumeLayout(false);
    }
  }
}

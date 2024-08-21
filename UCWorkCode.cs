
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Attendance
{
  public class UCWorkCode : UserControl
  {
    private WorkCodeBll _bll = new WorkCodeBll();
    private DeviceCmdBll _cmdBll = new DeviceCmdBll();
    private DataTable _dt;
    private IContainer components;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private Label lblMsg;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private Button btnDelete;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
    private Button btnUpload;
    private Button btnSave;
    private Button btnAdd;
    private Label lblRequired;
    private Label label_WorkName;
    private TextBox tb_WorkName;
    private TextBox tb_WorkCode;
    private Label label_WorkCode;
    private DataGridView dgvWorkCode;
    private Panel pnlData;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
    private Label lblModuleName;
    private Panel pnlControl;
    private Panel pnlTop;
    private ComboBox cmbDevice;
    private Label lblDevice;
    private Label label1;
    private DataGridViewTextBoxColumn colIndex;
    private DataGridViewTextBoxColumn colWorkCode;
    private DataGridViewTextBoxColumn colWorkName;

    public UCWorkCode() => this.InitializeComponent();

    private void UCWorkCode_Load(object sender, EventArgs e)
    {
      this.dgvWorkCode.AutoGenerateColumns = false;
      this.LoadAllWorkCode();
      this.LoadDeviceSN();
    }

    private void LoadAllWorkCode()
    {
      try
      {
        this._dt = this._bll.GetAll();
        this.dgvWorkCode.DataSource = (object) this._dt;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load workcode info error:" + ex.ToString());
      }
    }

    private void LoadDeviceSN()
    {
      this.cmbDevice.Items.Clear();
      DeviceBll deviceBll = new DeviceBll();
      try
      {
        List<string> allDevSn = deviceBll.GetAllDevSN();
        for (int index = 0; index < allDevSn.Count; ++index)
          this.cmbDevice.Items.Add((object) allDevSn[index]);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Load Device Message error:" + ex.ToString());
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.tb_WorkCode.Text = "";
      this.tb_WorkName.Text = "";
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.tb_WorkCode.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please input workcode.";
      }
      else
      {
        this.lblMsg.Visible = false;
        WorkCodeModel workCode = this._bll.GetByWorkCode(this.tb_WorkCode.Text.Trim()) ?? new WorkCodeModel();
        workCode.WorkCode = this.tb_WorkCode.Text.Trim();
        workCode.WorkName = this.tb_WorkName.Text.Trim();
        try
        {
          if (workCode.ID == 0)
          {
            if (this._bll.Add(workCode) > 0)
            {
              this.LoadAllWorkCode();
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add successful.";
            }
            else
            {
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add fail.";
            }
          }
          else if (this._bll.Update(workCode) > 0)
          {
            this.LoadAllWorkCode();
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
      if (new DeviceBll().Get(this.cmbDevice.Text) == null)
        this.lblMsg.Text = "Please input Device SN.";
      else if (this._bll.GetByWorkCode(this.tb_WorkCode.Text.Trim()) == null)
        this.lblMsg.Text = "Please save workcode first.";
      else if (string.IsNullOrWhiteSpace(this.tb_WorkCode.Text.Trim()))
      {
        this.lblMsg.Text = "Please select workcode item.";
      }
      else
      {
        DeviceCmdModel dCmd = new DeviceCmdModel();
        dCmd.DevSN = this.cmbDevice.Text;
        dCmd.CommitTime = DateTime.Now;
        string str = Encoding.Default.GetString(Encoding.UTF8.GetBytes(this.tb_WorkName.Text));
        dCmd.Content = string.Format("DATA UPDATE WORKCODE PIN={0}\tCODE={1}\tNAME={2}", (object) this._dt.Select("workcode='" + this.tb_WorkCode.Text.Trim() + "'")[0]["ID"].ToString(), (object) this.tb_WorkCode.Text.Trim(), (object) str);
        if (string.IsNullOrEmpty(dCmd.Content))
        {
          this.lblMsg.Text = "The command is error.";
        }
        else
        {
          this.lblMsg.Visible = true;
          try
          {
            if (this._cmdBll.Add(dCmd) >= 0)
              this.lblMsg.Text = "Operate successful.";
            else
              this.lblMsg.Text = "Operate fail.";
          }
          catch
          {
          }
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.tb_WorkCode.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please select workcode item.";
      }
      else
      {
        this.lblMsg.Visible = false;
        if (this._bll.GetByWorkCode(this.tb_WorkCode.Text.Trim()) == null)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = "The workcode is not exist.";
        }
        else
        {
          this.lblMsg.Visible = false;
          if (this._bll.Delete(this.tb_WorkCode.Text.Trim()) > 0)
          {
            this.LoadAllWorkCode();
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

    private void dgvWorkCode_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvWorkCode.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvWorkCode.CurrentRow == null)
        return;
      DataGridViewRow currentRow = this.dgvWorkCode.CurrentRow;
      this.tb_WorkCode.Text = currentRow.Cells["colWorkCode"].Value.ToString();
      this.tb_WorkName.Text = currentRow.Cells["colWorkName"].Value.ToString();
    }

    private void dgvWorkCode_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvWorkCode.Rows[e.Row.Index].Cells["colIndex"].Value = (object) (e.Row.Index + 1);
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
      this.lblMsg = new Label();
      this.btnDelete = new Button();
      this.btnUpload = new Button();
      this.btnSave = new Button();
      this.btnAdd = new Button();
      this.lblRequired = new Label();
      this.label_WorkName = new Label();
      this.tb_WorkName = new TextBox();
      this.tb_WorkCode = new TextBox();
      this.label_WorkCode = new Label();
      this.dgvWorkCode = new DataGridView();
      this.pnlData = new Panel();
      this.lblModuleName = new Label();
      this.pnlControl = new Panel();
      this.cmbDevice = new ComboBox();
      this.lblDevice = new Label();
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.colIndex = new DataGridViewTextBoxColumn();
      this.colWorkCode = new DataGridViewTextBoxColumn();
      this.colWorkName = new DataGridViewTextBoxColumn();
      this.dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.dgvWorkCode).BeginInit();
      this.pnlData.SuspendLayout();
      this.pnlControl.SuspendLayout();
      this.pnlTop.SuspendLayout();
      this.SuspendLayout();
      this.lblMsg.AutoSize = true;
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(25, 89);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(0, 12);
      this.lblMsg.TabIndex = 51;
      this.btnDelete.BackColor = Color.FromArgb(37, 190, 167);
      this.btnDelete.Cursor = Cursors.Hand;
      this.btnDelete.FlatAppearance.BorderColor = Color.White;
      this.btnDelete.FlatStyle = FlatStyle.Flat;
      this.btnDelete.Font = new Font("Arial", 12f);
      this.btnDelete.ForeColor = Color.White;
      this.btnDelete.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnDelete.ImageIndex = 3;
      this.btnDelete.Location = new Point(243, 278);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new Size(65, 30);
      this.btnDelete.TabIndex = 43;
      this.btnDelete.Text = "Delete";
      this.btnDelete.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnDelete.UseVisualStyleBackColor = false;
      this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
      this.btnUpload.BackColor = Color.FromArgb(37, 190, 167);
      this.btnUpload.Cursor = Cursors.Hand;
      this.btnUpload.FlatAppearance.BorderColor = Color.White;
      this.btnUpload.FlatStyle = FlatStyle.Flat;
      this.btnUpload.Font = new Font("Arial", 12f);
      this.btnUpload.ForeColor = Color.White;
      this.btnUpload.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnUpload.ImageIndex = 2;
      this.btnUpload.Location = new Point(165, 278);
      this.btnUpload.Name = "btnUpload";
      this.btnUpload.Size = new Size(70, 30);
      this.btnUpload.TabIndex = 44;
      this.btnUpload.Text = "Upload";
      this.btnUpload.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnUpload.UseVisualStyleBackColor = false;
      this.btnUpload.Click += new EventHandler(this.btnUpload_Click);
      this.btnSave.BackColor = Color.FromArgb(37, 190, 167);
      this.btnSave.Cursor = Cursors.Hand;
      this.btnSave.FlatAppearance.BorderColor = Color.White;
      this.btnSave.FlatStyle = FlatStyle.Flat;
      this.btnSave.Font = new Font("Arial", 12f);
      this.btnSave.ForeColor = Color.White;
      this.btnSave.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnSave.ImageIndex = 1;
      this.btnSave.Location = new Point(87, 278);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new Size(70, 30);
      this.btnSave.TabIndex = 42;
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
      this.btnAdd.Location = new Point(10, 278);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new Size(70, 30);
      this.btnAdd.TabIndex = 41;
      this.btnAdd.Text = "New";
      this.btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnAdd.UseVisualStyleBackColor = false;
      this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
      this.lblRequired.AutoSize = true;
      this.lblRequired.ForeColor = Color.Red;
      this.lblRequired.Location = new Point(246, 135);
      this.lblRequired.Name = "lblRequired";
      this.lblRequired.Size = new Size(11, 12);
      this.lblRequired.TabIndex = 40;
      this.lblRequired.Text = "*";
      this.label_WorkName.AutoSize = true;
      this.label_WorkName.Font = new Font("Arial", 10f);
      this.label_WorkName.Location = new Point(23, 172);
      this.label_WorkName.Name = "label_WorkName";
      this.label_WorkName.Size = new Size(77, 16);
      this.label_WorkName.TabIndex = 36;
      this.label_WorkName.Text = "WorkName";
      this.tb_WorkName.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.tb_WorkName.Location = new Point(106, 170);
      this.tb_WorkName.Name = "tb_WorkName";
      this.tb_WorkName.Size = new Size(134, 21);
      this.tb_WorkName.TabIndex = 35;
      this.tb_WorkCode.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.tb_WorkCode.Location = new Point(106, 133);
      this.tb_WorkCode.Name = "tb_WorkCode";
      this.tb_WorkCode.Size = new Size(134, 21);
      this.tb_WorkCode.TabIndex = 33;
      this.label_WorkCode.AutoSize = true;
      this.label_WorkCode.Font = new Font("Arial", 10f);
      this.label_WorkCode.Location = new Point(23, 135);
      this.label_WorkCode.Name = "label_WorkCode";
      this.label_WorkCode.Size = new Size(75, 16);
      this.label_WorkCode.TabIndex = 32;
      this.label_WorkCode.Text = "WorkCode";
      this.dgvWorkCode.AllowUserToAddRows = false;
      this.dgvWorkCode.AllowUserToDeleteRows = false;
      this.dgvWorkCode.AllowUserToResizeRows = false;
      this.dgvWorkCode.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvWorkCode.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("Arial", 9f);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvWorkCode.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvWorkCode.Columns.AddRange((DataGridViewColumn) this.colIndex, (DataGridViewColumn) this.colWorkCode, (DataGridViewColumn) this.colWorkName);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("Arial", 9f);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvWorkCode.DefaultCellStyle = gridViewCellStyle2;
      this.dgvWorkCode.Dock = DockStyle.Fill;
      this.dgvWorkCode.Location = new Point(0, 0);
      this.dgvWorkCode.MultiSelect = false;
      this.dgvWorkCode.Name = "dgvWorkCode";
      this.dgvWorkCode.ReadOnly = true;
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle3.BackColor = SystemColors.Control;
      gridViewCellStyle3.Font = new Font("Arial", 9f);
      gridViewCellStyle3.ForeColor = SystemColors.WindowText;
      gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
      this.dgvWorkCode.RowHeadersDefaultCellStyle = gridViewCellStyle3;
      this.dgvWorkCode.RowHeadersVisible = false;
      this.dgvWorkCode.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvWorkCode.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dgvWorkCode.RowTemplate.Height = 23;
      this.dgvWorkCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvWorkCode.Size = new Size(516, 667);
      this.dgvWorkCode.TabIndex = 1;
      this.dgvWorkCode.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvWorkCode_CellMouseClick);
      this.dgvWorkCode.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvWorkCode_RowStateChanged);
      this.pnlData.BackColor = Color.White;
      this.pnlData.Controls.Add((Control) this.dgvWorkCode);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(516, 667);
      this.pnlData.TabIndex = 5;
      this.lblModuleName.AutoSize = true;
      this.lblModuleName.Font = new Font("Arial", 12f);
      this.lblModuleName.Location = new Point(12, 8);
      this.lblModuleName.Name = "lblModuleName";
      this.lblModuleName.Size = new Size(84, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "WorkCode";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.cmbDevice);
      this.pnlControl.Controls.Add((Control) this.lblDevice);
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Controls.Add((Control) this.btnUpload);
      this.pnlControl.Controls.Add((Control) this.btnDelete);
      this.pnlControl.Controls.Add((Control) this.btnSave);
      this.pnlControl.Controls.Add((Control) this.btnAdd);
      this.pnlControl.Controls.Add((Control) this.lblRequired);
      this.pnlControl.Controls.Add((Control) this.label_WorkName);
      this.pnlControl.Controls.Add((Control) this.tb_WorkName);
      this.pnlControl.Controls.Add((Control) this.tb_WorkCode);
      this.pnlControl.Controls.Add((Control) this.label_WorkCode);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.Location = new Point(516, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(312, 667);
      this.pnlControl.TabIndex = 4;
      this.cmbDevice.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbDevice.FormattingEnabled = true;
      this.cmbDevice.Location = new Point(106, 207);
      this.cmbDevice.Name = "cmbDevice";
      this.cmbDevice.Size = new Size(134, 20);
      this.cmbDevice.TabIndex = 53;
      this.lblDevice.AutoSize = true;
      this.lblDevice.Font = new Font("Arial", 10f);
      this.lblDevice.Location = new Point(24, 209);
      this.lblDevice.Name = "lblDevice";
      this.lblDevice.Size = new Size(51, 16);
      this.lblDevice.TabIndex = 52;
      this.lblDevice.Text = "Device";
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(828, 30);
      this.pnlTop.TabIndex = 3;
      this.label1.BackColor = Color.FromArgb(37, 190, 167);
      this.label1.Font = new Font("Arial", 9f);
      this.label1.Location = new Point(0, 5);
      this.label1.Name = "label1";
      this.label1.Size = new Size(3, 20);
      this.label1.TabIndex = 3;
      this.label1.Text = " ";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn1.DataPropertyName = "PIN";
      this.dataGridViewTextBoxColumn1.Frozen = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "UserPin";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 72;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Resizable = DataGridViewTriState.True;
      this.dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.dataGridViewTextBoxColumn1.Width = 72;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "UserName";
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn2.DefaultCellStyle = gridViewCellStyle4;
      this.dataGridViewTextBoxColumn2.HeaderText = "UserName";
      this.dataGridViewTextBoxColumn2.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 78;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "IDCard";
      gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn3.DefaultCellStyle = gridViewCellStyle5;
      this.dataGridViewTextBoxColumn3.HeaderText = "CardNumber";
      this.dataGridViewTextBoxColumn3.MinimumWidth = 90;
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 90;
      this.colIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.colIndex.DataPropertyName = "Index";
      this.colIndex.HeaderText = "Index";
      this.colIndex.Name = "colIndex";
      this.colIndex.ReadOnly = true;
      this.colIndex.Resizable = DataGridViewTriState.True;
      this.colIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.colIndex.Width = 50;
      this.colWorkCode.DataPropertyName = "WorkCode";
      gridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colWorkCode.DefaultCellStyle = gridViewCellStyle6;
      this.colWorkCode.HeaderText = "WorkCode";
      this.colWorkCode.MinimumWidth = 72;
      this.colWorkCode.Name = "colWorkCode";
      this.colWorkCode.ReadOnly = true;
      this.colWorkName.DataPropertyName = "WorkName";
      gridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.colWorkName.DefaultCellStyle = gridViewCellStyle7;
      this.colWorkName.HeaderText = "WorkName";
      this.colWorkName.MinimumWidth = 78;
      this.colWorkName.Name = "colWorkName";
      this.colWorkName.ReadOnly = true;
      this.dataGridViewCheckBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewCheckBoxColumn1.DataPropertyName = "checkFlag";
      this.dataGridViewCheckBoxColumn1.Frozen = true;
      this.dataGridViewCheckBoxColumn1.HeaderText = "";
      this.dataGridViewCheckBoxColumn1.MinimumWidth = 50;
      this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
      this.dataGridViewCheckBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "FP9Count";
      this.dataGridViewTextBoxColumn6.HeaderText = "FP9.0";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn5.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "Pri";
      this.dataGridViewTextBoxColumn5.HeaderText = "Privilege";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Passwd";
      this.dataGridViewTextBoxColumn4.HeaderText = "Password";
      this.dataGridViewTextBoxColumn4.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.Width = 78;
      this.dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "FP10Count";
      this.dataGridViewTextBoxColumn7.HeaderText = "FP10.0";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn9.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn9.DataPropertyName = "Grp";
      this.dataGridViewTextBoxColumn9.HeaderText = "Group";
      this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
      this.dataGridViewTextBoxColumn8.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn8.DataPropertyName = "FaceCount";
      this.dataGridViewTextBoxColumn8.HeaderText = "Face";
      this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
      this.dataGridViewTextBoxColumn10.DataPropertyName = "TZ";
      this.dataGridViewTextBoxColumn10.HeaderText = "Timezone";
      this.dataGridViewTextBoxColumn10.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
      this.dataGridViewTextBoxColumn10.Width = 78;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCWorkCode);
      this.Size = new Size(828, 697);
      this.Load += new EventHandler(this.UCWorkCode_Load);
      ((ISupportInitialize) this.dgvWorkCode).EndInit();
      this.pnlData.ResumeLayout(false);
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}

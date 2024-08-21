
using Attendance.Properties;
using BLL;
using Microsoft.CSharp.RuntimeBinder;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Attendance
{
  public class UCUser : UserControl
  {
    private UserInfoBll _bll = new UserInfoBll();
    private List<DataGridViewRow> _CurSelectRows = new List<DataGridViewRow>();
    private DeviceCmdBll _cmdBll = new DeviceCmdBll();
    private TmpFPBll _tmpFPBll = new TmpFPBll();
    private TmpFaceBll _faceBll = new TmpFaceBll();
    private TmpBioPhotoBll _tmpBioPhotoBll = new TmpBioPhotoBll();
    private TmpBioDataBll _tmpBioDataBll = new TmpBioDataBll();
    private string[] labMsg = new string[11];
    private DataTable _dt = new DataTable();
    private DatagridviewCheckboxHeaderCell _headerCheckBox;
    private IContainer components;
    private Panel pnlTop;
    private Panel pnlControl;
    private Panel pnlData;
    private Label lblModuleName;
    private Button btnOpenPic;
    private CheckBox cb_Photo;
    private CheckBox cb_Face;
    private CheckBox cb_FP;
    private Label lblMsg;
    private Label lblUploadSel;
    private PictureBox picUserPhoto;
    private ComboBox cmbDevice;
    private Label lblDevice;
    private ComboBox cmbPrivilege;
    private Label lblPrivilege;
    private Button btnUpload;
    private Button btnDelete;
    private Button btnSave;
    private Button btnAdd;
    private Label lblRequired;
    private TextBox txtPassword;
    private TextBox txtCard;
    private Label lblUserCard;
    private Label lblUserName;
    private TextBox txtUserName;
    private Label lblPassword;
    private TextBox txtPin;
    private Label lbUserPIN;
    private DataGridView dgvUser;
    private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
    private CheckBox cb_Palm;
    private Label label1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
    private DataGridViewCheckBoxColumn cb_Select;
    private DataGridViewTextBoxColumn col_Index;
    private DataGridViewTextBoxColumn colUserPin;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colCardNumber;
    private DataGridViewTextBoxColumn colPassword;
    private DataGridViewTextBoxColumn colPrivilege;
    private DataGridViewTextBoxColumn colFP9Count;
    private DataGridViewTextBoxColumn colFP10Count;
    private DataGridViewTextBoxColumn col_FP12;
    private DataGridViewTextBoxColumn colFaceCount;
    private DataGridViewTextBoxColumn col_Palm;
    private DataGridViewTextBoxColumn colGroup;
    private DataGridViewTextBoxColumn colTimezone;

    public UCUser() => this.InitializeComponent();

    private void UCUser_Load(object sender, EventArgs e)
    {
      Control.CheckForIllegalCrossThreadCalls = false;
      this.dgvUser.AutoGenerateColumns = false;
      this.lblMsg.Visible = false;
      this._headerCheckBox = new DatagridviewCheckboxHeaderCell();
      this._headerCheckBox.OnCheckBoxClicked += new DatagridviewcheckboxHeaderEventHander(this.ch_OnCheckBoxClicked);
      DataGridViewCheckBoxColumn column = this.dgvUser.Columns[0] as DataGridViewCheckBoxColumn;
      column.HeaderCell = (DataGridViewColumnHeaderCell) this._headerCheckBox;
      column.HeaderCell.Value = (object) string.Empty;
      this.InitMsg();
      this.LoadAllUsers();
      this.LoadCmbPrivilage();
      this.LoadDeviceSN();
    }

    public void LoadAllUsers()
    {
      this.dgvUser.DataSource = (object) null;
      this._dt = this._bll.GetAll();
      this.dgvUser.DataSource = (object) this._dt;
      this._headerCheckBox._checked = false;
    }

    private void LoadCmbPrivilage()
    {
      this.cmbPrivilege.DataSource = (object) new ArrayList()
      {
        (object) new DictionaryEntry((object) "0", (object) "User"),
        (object) new DictionaryEntry((object) "14", (object) "Administrator")
      };
      this.cmbPrivilege.DisplayMember = "Value";
      this.cmbPrivilege.ValueMember = "Key";
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

    public void InitMsg()
    {
      try
      {
        this.labMsg[0] = "Please input user id.";
        this.labMsg[1] = "Please input device sn.";
        this.labMsg[2] = "Operate successful.";
        this.labMsg[3] = "Operate fail.";
        this.labMsg[4] = "Update successful.";
        this.labMsg[5] = "Update fail.";
        this.labMsg[6] = "Add successful.";
        this.labMsg[7] = "Add fail.";
        this.labMsg[8] = "The user is not exist.";
        this.labMsg[9] = "The command is error.";
        this.labMsg[10] = "Please save user picture first.";
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.lblMsg.Visible = false;
      this.txtPin.Text = string.Empty;
      this.txtUserName.Text = string.Empty;
      this.txtPassword.Text = string.Empty;
      this.txtCard.Text = string.Empty;
      this.cmbPrivilege.SelectedIndex = 0;
      this.txtPin.Enabled = true;
      this.picUserPhoto.Image = this.picUserPhoto.ErrorImage;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.txtPin.Text))
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please input user id.";
      }
      else
      {
        this.lblMsg.Visible = false;
        UserInfoModel user = this._bll.Get(this.txtPin.Text.Trim()) ?? new UserInfoModel();
        user.PIN = this.txtPin.Text;
        user.UserName = this.txtUserName.Text;
        user.Passwd = this.txtPassword.Text;
        user.IDCard = this.txtCard.Text;
        user.Pri = this.cmbPrivilege.SelectedValue.ToString();
        try
        {
          int num;
          if (user.ID == 0)
          {
            num = this._bll.Add(user);
            if (num > 0)
            {
              this.LoadAllUsers();
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add successful.";
            }
            else
            {
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Add fail.";
            }
          }
          else
          {
            num = this._bll.Update(user);
            if (num > 0)
            {
              this.LoadAllUsers();
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Update successful.";
            }
            else
            {
              this.lblMsg.Visible = true;
              this.lblMsg.Text = "Update fail.";
            }
          }
          if (num <= 0)
            return;
          string destFileName = Environment.CurrentDirectory + "\\Photo\\" + this.txtPin.Text + ".jpg";
          if (this.picUserPhoto.ImageLocation != null)
            File.Copy(this.picUserPhoto.ImageLocation, destFileName, true);
          this.txtPin.Enabled = false;
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
      string empty = string.Empty;
      byte[] numArray = new byte[24];
      if (new DeviceBll().Get(this.cmbDevice.Text) == null)
      {
        this.UserMessageShow(1, "");
      }
      else
      {
        this.lblMsg.Visible = false;
        this.GetCurSelectRows();
        if (this._CurSelectRows.Count == 0)
        {
          this.lblMsg.Visible = true;
          this.lblMsg.Text = "Please select user item";
        }
        else
        {
          foreach (DataGridViewRow curSelectRow in this._CurSelectRows)
          {
            string str1 = !(curSelectRow.Cells["colPrivilege"].Value.ToString() == "Administrator") ? "0" : "14";
            DeviceCmdModel dCmd = new DeviceCmdModel();
            dCmd.DevSN = this.cmbDevice.Text;
            dCmd.CommitTime = DateTime.Now;
            string str2 = Encoding.Default.GetString(Encoding.UTF8.GetBytes(curSelectRow.Cells["colUserName"].Value.ToString()));
            dCmd.Content = string.Format("DATA UPDATE USERINFO PIN={0}\tName={1}\tPri={2}\tPasswd={3}\tCard={4}\tGrp={5}\tTZ={6}", (object) curSelectRow.Cells["colUserPin"].Value.ToString(), (object) str2, (object) str1, (object) curSelectRow.Cells["colPassword"].Value.ToString(), (object) curSelectRow.Cells["colCardNumber"].Value.ToString(), (object) curSelectRow.Cells["colGroup"].Value.ToString(), (object) curSelectRow.Cells["colTimezone"].Value.ToString());
            if (string.IsNullOrEmpty(dCmd.Content))
            {
              this.UserMessageShow(9, "");
              break;
            }
            this.lblMsg.Visible = false;
            try
            {
              if (this._cmdBll.Add(dCmd) >= 0)
              {
                this.UserMessageShow(2, "");
                if (this.cb_FP.Checked)
                  this.uploadUserFingerTemplate(dCmd.DevSN);
                if (this.cb_Face.Checked)
                  this.uploadUserFaceTemplate(dCmd.DevSN);
                if (this.cb_Palm.Checked)
                  this.uploadPalmTemplate(dCmd.DevSN);
                if (this.cb_Photo.Checked)
                  this.uploadUserPhotoID(dCmd.DevSN);
              }
              else
                this.UserMessageShow(3, "");
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }

    private List<DataGridViewRow> GetCurSelectRows()
    {
      this._CurSelectRows = new List<DataGridViewRow>();
      for (int index = 0; index < this.dgvUser.Rows.Count; ++index)
      {
        if (Convert.ToBoolean(this.dgvUser.Rows[index].Cells[0].Value))
          this._CurSelectRows.Add(this.dgvUser.Rows[index]);
      }
      return this._CurSelectRows;
    }

    private void uploadUserFingerTemplate(string sn)
    {
      DeviceModel deviceModel = new DeviceBll().Get(sn);
      if (!deviceModel.IsBioDataSupport(BioType.FingerPrint))
        return;
      List<string> pins = new List<string>();
      foreach (DataGridViewRow curSelectRow in this._CurSelectRows)
        pins.Add(curSelectRow.Cells["colUserPin"].Value.ToString());
      List<TmpFPModel> tmpFpModelList = this._tmpFPBll.Get(pins);
      DeviceCmdModel dCmd = new DeviceCmdModel()
      {
        DevSN = sn
      };
      string str = deviceModel.GetBioVersion(BioType.FingerPrint).Split('.')[0];
      foreach (TmpFPModel tmpFpModel in tmpFpModelList)
      {
        dCmd.CommitTime = DateTime.Now;
        if (!(tmpFpModel.MajorVer != str))
        {
          if (tmpFpModel.MajorVer != "12")
            dCmd.Content = string.Format("DATA UPDATE FINGERTMP PIN={0}\tFID={1}\tSize={2}\tValid={3}\tTMP={4}", (object) tmpFpModel.Pin, (object) tmpFpModel.Fid, (object) tmpFpModel.Size, (object) tmpFpModel.Valid, (object) tmpFpModel.Tmp);
          else
            dCmd.Content = string.Format("DATA UPDATE BIODATA Pin={0}\tNo={1}\tIndex={2}\tValid={3}\tDuress={4}\tType={5}\tMajorVer={6}\tMinorVer ={7}\tFormat={8}\tTmp={9}", (object) tmpFpModel.Pin, (object) tmpFpModel.Fid, (object) "", (object) tmpFpModel.Valid, (object) tmpFpModel.Duress, (object) "1", (object) tmpFpModel.MajorVer, (object) tmpFpModel.MinorVer, (object) "", (object) tmpFpModel.Tmp);
          if (string.IsNullOrEmpty(dCmd.Content))
          {
            this.UserMessageShow(9, "");
            break;
          }
          this.lblMsg.Visible = false;
          try
          {
            if (this._cmdBll.Add(dCmd) >= 0)
              this.UserMessageShow(2, "");
            else
              this.UserMessageShow(3, "");
          }
          catch
          {
          }
        }
      }
    }

    private void uploadUserFaceTemplate(string sn)
    {
      DeviceModel deviceModel = new DeviceBll().Get(sn);
      List<string> pins = new List<string>();
      foreach (DataGridViewRow curSelectRow in this._CurSelectRows)
        pins.Add(curSelectRow.Cells["colUserPin"].Value.ToString());
      DeviceCmdModel dCmd = new DeviceCmdModel();
      dCmd.DevSN = sn;
      if (deviceModel.IsBioDataSupport(BioType.Face))
      {
        foreach (TmpFaceModel tmpFaceModel in this._faceBll.Get(pins))
        {
          dCmd.CommitTime = DateTime.Now;
          dCmd.Content = string.Format("DATA UPDATE FACE PIN={0}\tFID={1}\tValid={2}\tSize={3}\tTMP={4}", (object) tmpFaceModel.Pin, (object) tmpFaceModel.Fid, (object) tmpFaceModel.Valid, (object) tmpFaceModel.Size, (object) tmpFaceModel.Tmp);
          if (string.IsNullOrEmpty(dCmd.Content))
          {
            this.UserMessageShow(9, "");
            return;
          }
          this.lblMsg.Visible = false;
          try
          {
            if (this._cmdBll.Add(dCmd) >= 0)
              this.UserMessageShow(2, "");
            else
              this.UserMessageShow(3, "");
          }
          catch
          {
          }
        }
      }
      if (!deviceModel.IsBioDataSupport(BioType.VisilightFace))
        return;
      foreach (TmpBioPhotoModel tmpBioPhotoModel in this._tmpBioPhotoBll.Get(pins, BioType.VisilightFace.ToString("D")))
      {
        dCmd.CommitTime = DateTime.Now;
        dCmd.Content = string.Format("DATA UPDATE BIOPHOTO PIN={0}\tType={1}\tSize={2}\tContent={3}\tFormat={4}\tUrl={5}\tPostBackTmpFlag={6}", (object) tmpBioPhotoModel.Pin, (object) tmpBioPhotoModel.Type, (object) tmpBioPhotoModel.Size, (object) tmpBioPhotoModel.Content, (object) "0", (object) "", (object) "0");
        if (string.IsNullOrEmpty(dCmd.Content))
        {
          this.UserMessageShow(9, "");
          break;
        }
        this.lblMsg.Visible = false;
        try
        {
          if (this._cmdBll.Add(dCmd) >= 0)
            this.UserMessageShow(2, "");
          else
            this.UserMessageShow(3, "");
        }
        catch
        {
        }
      }
    }

    private void uploadPalmTemplate(string sn)
    {
      DeviceModel deviceModel = new DeviceBll().Get(sn);
      List<string> pinList = new List<string>();
      foreach (DataGridViewRow curSelectRow in this._CurSelectRows)
        pinList.Add(curSelectRow.Cells["colUserPin"].Value.ToString());
      DeviceCmdModel dCmd = new DeviceCmdModel();
      dCmd.DevSN = sn;
      if (!deviceModel.IsBioDataSupport(BioType.Palm))
        return;
      foreach (TmpBioDataModel tmpBioDataModel in this._tmpBioDataBll.Get(pinList, BioType.Palm.ToString("D")))
      {
        dCmd.CommitTime = DateTime.Now;
        dCmd.Content = string.Format("DATA UPDATE BIODATA Pin={0}\tNo={1}\tIndex={2}\tValid={3}\tDuress={4}\tType={5}\tMajorVer={6}\tMinorVer ={7}\tFormat={8}\tTmp={9}", (object) tmpBioDataModel.Pin, (object) tmpBioDataModel.No, (object) tmpBioDataModel.Index, (object) tmpBioDataModel.Valid, (object) tmpBioDataModel.Duress, (object) tmpBioDataModel.Type, (object) tmpBioDataModel.MajorVer, (object) tmpBioDataModel.MinorVer, (object) tmpBioDataModel.Format, (object) tmpBioDataModel.Tmp);
        if (string.IsNullOrEmpty(dCmd.Content))
        {
          this.UserMessageShow(9, "");
          break;
        }
        this.lblMsg.Visible = false;
        try
        {
          if (this._cmdBll.Add(dCmd) >= 0)
            this.UserMessageShow(2, "");
          else
            this.UserMessageShow(3, "");
        }
        catch
        {
        }
      }
    }

    private string ImgToBase64String(string Imagefilename)
    {
      string base64String = "";
      try
      {
        Bitmap bitmap = new Bitmap(Imagefilename);
        FileStream fileStream = new FileStream(Imagefilename + ".txt", FileMode.Create);
        StreamWriter streamWriter = new StreamWriter((Stream) fileStream);
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = memoryStream1;
        ImageFormat jpeg = ImageFormat.Jpeg;
        bitmap.Save((Stream) memoryStream2, jpeg);
        byte[] numArray = new byte[memoryStream1.Length];
        memoryStream1.Position = 0L;
        memoryStream1.Read(numArray, 0, (int) memoryStream1.Length);
        memoryStream1.Close();
        base64String = Convert.ToBase64String(numArray);
        streamWriter.Write(base64String);
        streamWriter.Close();
        fileStream.Close();
        return base64String;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("ImgToBase64String 转换失败/nException:" + ex.Message);
        return base64String;
      }
    }

    private void uploadUserPhotoID(string sn)
    {
      string currentDirectory = Environment.CurrentDirectory;
      string str1 = currentDirectory;
      string str2 = currentDirectory + "\\Photo\\" + this.txtPin.Text + ".jpg";
      if (str1 == null || !(str1 != ""))
        return;
      if (!File.Exists(str2))
      {
        this.UserMessageShow(10, "");
      }
      else
      {
        string base64String = this.ImgToBase64String(str2);
        if (base64String.Length >= 30720)
        {
          int num = (int) MessageBox.Show("please choice photo size less than 30K", "Error");
        }
        else
        {
          DeviceCmdModel dCmd = new DeviceCmdModel();
          dCmd.DevSN = sn;
          dCmd.CommitTime = DateTime.Now;
          dCmd.Content = string.Format("DATA UPDATE USERPIC PIN={0}\tSize={1}\tContent={2}", (object) this.txtPin.Text, (object) base64String.Length.ToString(), (object) base64String);
          if (string.IsNullOrEmpty(dCmd.Content))
          {
            this.UserMessageShow(9, "");
          }
          else
          {
            this.lblMsg.Visible = false;
            try
            {
              if (this._cmdBll.Add(dCmd) >= 0)
                this.UserMessageShow(2, "");
              else
                this.UserMessageShow(3, "");
            }
            catch
            {
            }
          }
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      this.GetCurSelectRows();
      if (this._CurSelectRows.Count == 0)
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please select user item";
      }
      else
      {
        List<string> pins = new List<string>();
        foreach (DataGridViewRow curSelectRow in this._CurSelectRows)
          pins.Add(curSelectRow.Cells["colUserPin"].Value.ToString());
        this.lblMsg.Visible = false;
        if (this._bll.Delete(pins) > 0)
        {
          this.LoadAllUsers();
          string currentDirectory = Environment.CurrentDirectory;
          string empty = string.Empty;
          foreach (string str in pins)
          {
            string path = currentDirectory + "\\Photo\\" + str + ".jpg";
            if (File.Exists(path))
              File.Delete(path);
          }
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

    private void btnOpenPic_Click(object sender, EventArgs e)
    {
      if (this.txtPin.Text == "")
      {
        this.lblMsg.Visible = true;
        this.lblMsg.Text = "Please input user id.";
      }
      else
      {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "选择要转换的图片";
        openFileDialog.Filter = "JPEG Files (*.jpeg);*.jpeg;PNG Files (*.png);*.png;JPG Files (*.jpg)|*.jpg;|AllFiles (*.*)|*.*";
        if (DialogResult.OK != openFileDialog.ShowDialog())
          return;
        string fileName = openFileDialog.FileName;
        this.picUserPhoto.ImageLocation = fileName;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
          this.picUserPhoto.Image = Image.FromStream((Stream) fileStream);
        this.picUserPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
        this.picUserPhoto.Update();
      }
    }

    private void UserMessageShow(int msgindex, string userid)
    {
      this.lblMsg.Visible = true;
      this.lblMsg.Text = this.labMsg[msgindex] + userid;
    }

    public void UpdateUserFP9Info(TmpFPModel fpver9)
    {
      if (this._dt.Select("PIN='" + fpver9.Pin + "'").Length == 0)
        return;
      DataRow dataRow = this._dt.Select("PIN='" + fpver9.Pin + "'")[0];
      dataRow["FP9Count"] = (object) (Convert.ToInt32(dataRow["FP9Count"].ToString()) + 1).ToString();
      this.dgvUser.DataSource = (object) this._dt;
    }

    public void UpdateUserFP10Info(TmpFPModel fpver10)
    {
      if (this._dt.Select("PIN='" + fpver10.Pin + "'").Length == 0)
        return;
      DataRow dataRow = this._dt.Select("PIN='" + fpver10.Pin + "'")[0];
      dataRow["FP10Count"] = (object) (Convert.ToInt32(dataRow["FP10Count"].ToString()) + 1).ToString();
      this.dgvUser.DataSource = (object) this._dt;
    }

    public void UpdateUserFaceInfo(object face)
    {
      DataTable dt1 = this._dt;
      // ISSUE: reference to a compiler-generated field
      if (UCUser.\u003C\u003Eo__30.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        UCUser.\u003C\u003Eo__30.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Pin", typeof (UCUser), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string filterExpression1 = string.Format("PIN='{0}'", UCUser.\u003C\u003Eo__30.\u003C\u003Ep__0.Target((CallSite) UCUser.\u003C\u003Eo__30.\u003C\u003Ep__0, face));
      if (dt1.Select(filterExpression1).Length == 0)
        return;
      DataTable dt2 = this._dt;
      // ISSUE: reference to a compiler-generated field
      if (UCUser.\u003C\u003Eo__30.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        UCUser.\u003C\u003Eo__30.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Pin", typeof (UCUser), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string filterExpression2 = string.Format("PIN='{0}'", UCUser.\u003C\u003Eo__30.\u003C\u003Ep__1.Target((CallSite) UCUser.\u003C\u003Eo__30.\u003C\u003Ep__1, face));
      DataRow dataRow = dt2.Select(filterExpression2)[0];
      dataRow["FaceCount"] = (object) (Convert.ToInt32(dataRow["FaceCount"].ToString()) + 1).ToString();
      this.dgvUser.DataSource = (object) this._dt;
    }

    public void UpdateUserPalmInfo(TmpBioDataModel palm)
    {
      if (this._dt.Select("PIN='" + palm.Pin + "'").Length == 0)
        return;
      DataRow dataRow = this._dt.Select("PIN='" + palm.Pin + "'")[0];
      dataRow["PalmCount"] = (object) (Convert.ToInt32(dataRow["PalmCount"].ToString()) + 1).ToString();
      this.dgvUser.DataSource = (object) this._dt;
    }

    private void dgvUser_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.ColumnIndex < 0 || e.RowIndex < 0)
        return;
      if (e.Button == MouseButtons.Right)
        this.dgvUser.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
      if (this.dgvUser.CurrentRow == null)
        return;
      this.SetCurUserInfo(this.dgvUser.CurrentRow);
    }

    private void SetCurUserInfo(DataGridViewRow row)
    {
      this.txtPin.Text = row.Cells["colUserPin"].Value.ToString();
      this.txtUserName.Text = row.Cells["colUserName"].Value.ToString();
      this.txtCard.Text = row.Cells["colCardNumber"].Value.ToString();
      this.txtPassword.Text = row.Cells["colPassword"].Value.ToString();
      this.cmbPrivilege.SelectedValue = (object) row.Cells["colPrivilege"].Value.ToString();
      string path = Environment.CurrentDirectory + "\\Photo\\" + row.Cells["colUserPin"].Value.ToString() + ".jpg";
      this.picUserPhoto.Image = this.picUserPhoto.ErrorImage;
      if (string.IsNullOrEmpty(path))
        return;
      try
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
          this.picUserPhoto.Image = Image.FromStream((Stream) fileStream);
        this.picUserPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
        this.picUserPhoto.Update();
      }
      catch
      {
      }
    }

    private void dgvUser_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1 || e.ColumnIndex == -1 || this.dgvUser.Columns[e.ColumnIndex].Name != "cb_Select")
        return;
      DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell) this.dgvUser.Rows[e.RowIndex].Cells[e.ColumnIndex];
      if (cell.Value != null && (bool) cell.Value)
        cell.Value = (object) false;
      else
        cell.Value = (object) true;
    }

    private void ch_OnCheckBoxClicked(object sender, DatagridviewCheckboxHeaderEventArgs e)
    {
      if (e.CheckedState && this.dgvUser.Rows.Count > 0)
        this.SetCurUserInfo(this.dgvUser.Rows[0]);
      foreach (DataGridViewRow row in (IEnumerable) this.dgvUser.Rows)
        row.Cells[0].Value = (object) e.CheckedState;
    }

    private void dgvUser_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
    {
      if (e.Row.Index < 0)
        return;
      this.dgvUser.Rows[e.Row.Index].Cells["col_Index"].Value = (object) (e.Row.Index + 1);
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
      this.pnlTop = new Panel();
      this.label1 = new Label();
      this.lblModuleName = new Label();
      this.pnlControl = new Panel();
      this.cb_Palm = new CheckBox();
      this.btnOpenPic = new Button();
      this.cb_Photo = new CheckBox();
      this.cb_Face = new CheckBox();
      this.cb_FP = new CheckBox();
      this.lblMsg = new Label();
      this.lblUploadSel = new Label();
      this.picUserPhoto = new PictureBox();
      this.cmbDevice = new ComboBox();
      this.lblDevice = new Label();
      this.cmbPrivilege = new ComboBox();
      this.lblPrivilege = new Label();
      this.btnUpload = new Button();
      this.btnDelete = new Button();
      this.btnSave = new Button();
      this.btnAdd = new Button();
      this.lblRequired = new Label();
      this.txtPassword = new TextBox();
      this.txtCard = new TextBox();
      this.lblUserCard = new Label();
      this.lblUserName = new Label();
      this.txtUserName = new TextBox();
      this.lblPassword = new Label();
      this.txtPin = new TextBox();
      this.lbUserPIN = new Label();
      this.pnlData = new Panel();
      this.dgvUser = new DataGridView();
      this.dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
      this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn11 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn12 = new DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn13 = new DataGridViewTextBoxColumn();
      this.colTimezone = new DataGridViewTextBoxColumn();
      this.colGroup = new DataGridViewTextBoxColumn();
      this.col_Palm = new DataGridViewTextBoxColumn();
      this.colFaceCount = new DataGridViewTextBoxColumn();
      this.col_FP12 = new DataGridViewTextBoxColumn();
      this.colFP10Count = new DataGridViewTextBoxColumn();
      this.colFP9Count = new DataGridViewTextBoxColumn();
      this.colPrivilege = new DataGridViewTextBoxColumn();
      this.colPassword = new DataGridViewTextBoxColumn();
      this.colCardNumber = new DataGridViewTextBoxColumn();
      this.colUserName = new DataGridViewTextBoxColumn();
      this.colUserPin = new DataGridViewTextBoxColumn();
      this.col_Index = new DataGridViewTextBoxColumn();
      this.cb_Select = new DataGridViewCheckBoxColumn();
      this.pnlTop.SuspendLayout();
      this.pnlControl.SuspendLayout();
      ((ISupportInitialize) this.picUserPhoto).BeginInit();
      this.pnlData.SuspendLayout();
      ((ISupportInitialize) this.dgvUser).BeginInit();
      this.SuspendLayout();
      this.pnlTop.Controls.Add((Control) this.label1);
      this.pnlTop.Controls.Add((Control) this.lblModuleName);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(919, 30);
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
      this.lblModuleName.Size = new Size(41, 18);
      this.lblModuleName.TabIndex = 2;
      this.lblModuleName.Text = "User";
      this.lblModuleName.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlControl.BackColor = Color.White;
      this.pnlControl.BorderStyle = BorderStyle.FixedSingle;
      this.pnlControl.Controls.Add((Control) this.cb_Palm);
      this.pnlControl.Controls.Add((Control) this.btnOpenPic);
      this.pnlControl.Controls.Add((Control) this.cb_Photo);
      this.pnlControl.Controls.Add((Control) this.cb_Face);
      this.pnlControl.Controls.Add((Control) this.cb_FP);
      this.pnlControl.Controls.Add((Control) this.lblMsg);
      this.pnlControl.Controls.Add((Control) this.lblUploadSel);
      this.pnlControl.Controls.Add((Control) this.picUserPhoto);
      this.pnlControl.Controls.Add((Control) this.cmbDevice);
      this.pnlControl.Controls.Add((Control) this.lblDevice);
      this.pnlControl.Controls.Add((Control) this.cmbPrivilege);
      this.pnlControl.Controls.Add((Control) this.lblPrivilege);
      this.pnlControl.Controls.Add((Control) this.btnUpload);
      this.pnlControl.Controls.Add((Control) this.btnDelete);
      this.pnlControl.Controls.Add((Control) this.btnSave);
      this.pnlControl.Controls.Add((Control) this.btnAdd);
      this.pnlControl.Controls.Add((Control) this.lblRequired);
      this.pnlControl.Controls.Add((Control) this.txtPassword);
      this.pnlControl.Controls.Add((Control) this.txtCard);
      this.pnlControl.Controls.Add((Control) this.lblUserCard);
      this.pnlControl.Controls.Add((Control) this.lblUserName);
      this.pnlControl.Controls.Add((Control) this.txtUserName);
      this.pnlControl.Controls.Add((Control) this.lblPassword);
      this.pnlControl.Controls.Add((Control) this.txtPin);
      this.pnlControl.Controls.Add((Control) this.lbUserPIN);
      this.pnlControl.Dock = DockStyle.Right;
      this.pnlControl.ForeColor = SystemColors.ControlText;
      this.pnlControl.Location = new Point(584, 30);
      this.pnlControl.Name = "pnlControl";
      this.pnlControl.Size = new Size(335, 533);
      this.pnlControl.TabIndex = 1;
      this.cb_Palm.AutoSize = true;
      this.cb_Palm.Font = new Font("Arial", 10f);
      this.cb_Palm.Location = new Point(196, 271);
      this.cb_Palm.Name = "cb_Palm";
      this.cb_Palm.Size = new Size(58, 20);
      this.cb_Palm.TabIndex = 56;
      this.cb_Palm.Text = "Palm";
      this.cb_Palm.UseVisualStyleBackColor = true;
      this.btnOpenPic.BackColor = Color.FromArgb(37, 190, 167);
      this.btnOpenPic.Cursor = Cursors.Hand;
      this.btnOpenPic.FlatStyle = FlatStyle.Flat;
      this.btnOpenPic.Font = new Font("Arial", 12f);
      this.btnOpenPic.ForeColor = Color.White;
      this.btnOpenPic.Location = new Point(245, 187);
      this.btnOpenPic.Name = "btnOpenPic";
      this.btnOpenPic.Size = new Size(59, 30);
      this.btnOpenPic.TabIndex = 55;
      this.btnOpenPic.Text = "Open";
      this.btnOpenPic.UseVisualStyleBackColor = false;
      this.btnOpenPic.Click += new EventHandler(this.btnOpenPic_Click);
      this.cb_Photo.AutoSize = true;
      this.cb_Photo.Font = new Font("Arial", 10f);
      this.cb_Photo.Location = new Point(259, 271);
      this.cb_Photo.Name = "cb_Photo";
      this.cb_Photo.Size = new Size(64, 20);
      this.cb_Photo.TabIndex = 54;
      this.cb_Photo.Text = "Photo";
      this.cb_Photo.UseVisualStyleBackColor = true;
      this.cb_Face.AutoSize = true;
      this.cb_Face.Font = new Font("Arial", 10f);
      this.cb_Face.Location = new Point(133, 271);
      this.cb_Face.Name = "cb_Face";
      this.cb_Face.Size = new Size(59, 20);
      this.cb_Face.TabIndex = 53;
      this.cb_Face.Text = "Face";
      this.cb_Face.UseVisualStyleBackColor = true;
      this.cb_FP.AutoSize = true;
      this.cb_FP.Font = new Font("Arial", 10f);
      this.cb_FP.Location = new Point(85, 271);
      this.cb_FP.Name = "cb_FP";
      this.cb_FP.Size = new Size(45, 20);
      this.cb_FP.TabIndex = 52;
      this.cb_FP.Text = "FP";
      this.cb_FP.UseVisualStyleBackColor = true;
      this.lblMsg.AutoSize = true;
      this.lblMsg.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.lblMsg.ForeColor = Color.Red;
      this.lblMsg.Location = new Point(9, 11);
      this.lblMsg.Name = "lblMsg";
      this.lblMsg.Size = new Size(23, 12);
      this.lblMsg.TabIndex = 51;
      this.lblMsg.Text = "msg";
      this.lblUploadSel.AutoSize = true;
      this.lblUploadSel.Font = new Font("Arial", 10f);
      this.lblUploadSel.Location = new Point(8, 273);
      this.lblUploadSel.Name = "lblUploadSel";
      this.lblUploadSel.Size = new Size(72, 16);
      this.lblUploadSel.TabIndex = 50;
      this.lblUploadSel.Text = "UploadSel";
      this.picUserPhoto.ErrorImage = (Image) Resources.imgNoPhoto;
      this.picUserPhoto.Image = (Image) Resources.imgNoPhoto;
      this.picUserPhoto.InitialImage = (Image) null;
      this.picUserPhoto.Location = new Point(221, 45);
      this.picUserPhoto.Name = "picUserPhoto";
      this.picUserPhoto.Size = new Size(107, 136);
      this.picUserPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
      this.picUserPhoto.TabIndex = 49;
      this.picUserPhoto.TabStop = false;
      this.cmbDevice.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbDevice.FormattingEnabled = true;
      this.cmbDevice.Location = new Point(84, 226);
      this.cmbDevice.Name = "cmbDevice";
      this.cmbDevice.Size = new Size(134, 20);
      this.cmbDevice.TabIndex = 48;
      this.lblDevice.AutoSize = true;
      this.lblDevice.Font = new Font("Arial", 10f);
      this.lblDevice.Location = new Point(8, 228);
      this.lblDevice.Name = "lblDevice";
      this.lblDevice.Size = new Size(51, 16);
      this.lblDevice.TabIndex = 47;
      this.lblDevice.Text = "Device";
      this.cmbPrivilege.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.cmbPrivilege.FormattingEnabled = true;
      this.cmbPrivilege.Items.AddRange(new object[2]
      {
        (object) "User",
        (object) "Administrator"
      });
      this.cmbPrivilege.Location = new Point(84, 189);
      this.cmbPrivilege.Name = "cmbPrivilege";
      this.cmbPrivilege.Size = new Size(134, 20);
      this.cmbPrivilege.TabIndex = 46;
      this.lblPrivilege.AutoSize = true;
      this.lblPrivilege.Font = new Font("Arial", 10f);
      this.lblPrivilege.Location = new Point(8, 191);
      this.lblPrivilege.Name = "lblPrivilege";
      this.lblPrivilege.Size = new Size(62, 16);
      this.lblPrivilege.TabIndex = 45;
      this.lblPrivilege.Text = "Privilege";
      this.btnUpload.BackColor = Color.FromArgb(37, 190, 167);
      this.btnUpload.Cursor = Cursors.Hand;
      this.btnUpload.FlatAppearance.BorderColor = Color.White;
      this.btnUpload.FlatStyle = FlatStyle.Flat;
      this.btnUpload.Font = new Font("Arial", 12f);
      this.btnUpload.ForeColor = Color.White;
      this.btnUpload.ImageAlign = ContentAlignment.MiddleLeft;
      this.btnUpload.ImageIndex = 2;
      this.btnUpload.Location = new Point(170, 309);
      this.btnUpload.Name = "btnUpload";
      this.btnUpload.Size = new Size(70, 30);
      this.btnUpload.TabIndex = 44;
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
      this.btnDelete.Location = new Point(248, 309);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new Size(70, 30);
      this.btnDelete.TabIndex = 43;
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
      this.btnSave.Location = new Point(92, 309);
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
      this.btnAdd.Location = new Point(15, 309);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new Size(70, 30);
      this.btnAdd.TabIndex = 41;
      this.btnAdd.Text = "New";
      this.btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
      this.btnAdd.UseVisualStyleBackColor = false;
      this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
      this.lblRequired.AutoSize = true;
      this.lblRequired.ForeColor = Color.Red;
      this.lblRequired.Location = new Point(205, 45);
      this.lblRequired.Name = "lblRequired";
      this.lblRequired.Size = new Size(11, 12);
      this.lblRequired.TabIndex = 40;
      this.lblRequired.Text = "*";
      this.txtPassword.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtPassword.Location = new Point(84, 152);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.Size = new Size(119, 21);
      this.txtPassword.TabIndex = 39;
      this.txtCard.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtCard.Location = new Point(84, 115);
      this.txtCard.Name = "txtCard";
      this.txtCard.Size = new Size(119, 21);
      this.txtCard.TabIndex = 38;
      this.lblUserCard.AutoSize = true;
      this.lblUserCard.Font = new Font("Arial", 10f);
      this.lblUserCard.Location = new Point(8, 117);
      this.lblUserCard.Name = "lblUserCard";
      this.lblUserCard.Size = new Size(39, 16);
      this.lblUserCard.TabIndex = 37;
      this.lblUserCard.Text = "Card";
      this.lblUserName.AutoSize = true;
      this.lblUserName.Font = new Font("Arial", 10f);
      this.lblUserName.Location = new Point(8, 80);
      this.lblUserName.Name = "lblUserName";
      this.lblUserName.Size = new Size(44, 16);
      this.lblUserName.TabIndex = 36;
      this.lblUserName.Text = "Name";
      this.txtUserName.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtUserName.Location = new Point(84, 78);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new Size(119, 21);
      this.txtUserName.TabIndex = 35;
      this.lblPassword.AutoSize = true;
      this.lblPassword.Font = new Font("Arial", 10f);
      this.lblPassword.Location = new Point(8, 154);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new Size(69, 16);
      this.lblPassword.TabIndex = 34;
      this.lblPassword.Text = "Password";
      this.txtPin.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      this.txtPin.Location = new Point(84, 41);
      this.txtPin.Name = "txtPin";
      this.txtPin.Size = new Size(119, 21);
      this.txtPin.TabIndex = 33;
      this.lbUserPIN.AutoSize = true;
      this.lbUserPIN.Font = new Font("Arial", 10f);
      this.lbUserPIN.Location = new Point(8, 43);
      this.lbUserPIN.Name = "lbUserPIN";
      this.lbUserPIN.Size = new Size(28, 16);
      this.lbUserPIN.TabIndex = 32;
      this.lbUserPIN.Text = "Pin";
      this.pnlData.BackColor = Color.White;
      this.pnlData.Controls.Add((Control) this.dgvUser);
      this.pnlData.Dock = DockStyle.Fill;
      this.pnlData.Location = new Point(0, 30);
      this.pnlData.Name = "pnlData";
      this.pnlData.Size = new Size(584, 533);
      this.pnlData.TabIndex = 2;
      this.dgvUser.AllowUserToAddRows = false;
      this.dgvUser.AllowUserToDeleteRows = false;
      this.dgvUser.AllowUserToResizeRows = false;
      this.dgvUser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvUser.BackgroundColor = Color.White;
      gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle1.BackColor = SystemColors.Control;
      gridViewCellStyle1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      gridViewCellStyle1.ForeColor = SystemColors.WindowText;
      gridViewCellStyle1.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle1.WrapMode = DataGridViewTriState.True;
      this.dgvUser.ColumnHeadersDefaultCellStyle = gridViewCellStyle1;
      this.dgvUser.Columns.AddRange((DataGridViewColumn) this.cb_Select, (DataGridViewColumn) this.col_Index, (DataGridViewColumn) this.colUserPin, (DataGridViewColumn) this.colUserName, (DataGridViewColumn) this.colCardNumber, (DataGridViewColumn) this.colPassword, (DataGridViewColumn) this.colPrivilege, (DataGridViewColumn) this.colFP9Count, (DataGridViewColumn) this.colFP10Count, (DataGridViewColumn) this.col_FP12, (DataGridViewColumn) this.colFaceCount, (DataGridViewColumn) this.col_Palm, (DataGridViewColumn) this.colGroup, (DataGridViewColumn) this.colTimezone);
      gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle2.BackColor = SystemColors.Window;
      gridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 134);
      gridViewCellStyle2.ForeColor = SystemColors.ControlText;
      gridViewCellStyle2.SelectionBackColor = Color.FromArgb(229, 253, 250);
      gridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
      gridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      this.dgvUser.DefaultCellStyle = gridViewCellStyle2;
      this.dgvUser.Dock = DockStyle.Fill;
      this.dgvUser.Location = new Point(0, 0);
      this.dgvUser.MultiSelect = false;
      this.dgvUser.Name = "dgvUser";
      this.dgvUser.ReadOnly = true;
      this.dgvUser.RowHeadersVisible = false;
      this.dgvUser.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgvUser.RowTemplate.Height = 23;
      this.dgvUser.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.dgvUser.Size = new Size(584, 533);
      this.dgvUser.TabIndex = 1;
      this.dgvUser.CellClick += new DataGridViewCellEventHandler(this.dgvUser_CellClick);
      this.dgvUser.CellMouseClick += new DataGridViewCellMouseEventHandler(this.dgvUser_CellMouseClick);
      this.dgvUser.RowStateChanged += new DataGridViewRowStateChangedEventHandler(this.dgvUser_RowStateChanged);
      this.dataGridViewCheckBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewCheckBoxColumn1.DataPropertyName = "checkFlag";
      this.dataGridViewCheckBoxColumn1.Frozen = true;
      this.dataGridViewCheckBoxColumn1.HeaderText = "";
      this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
      this.dataGridViewCheckBoxColumn1.ReadOnly = true;
      this.dataGridViewCheckBoxColumn1.Width = 50;
      this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn1.DataPropertyName = "PIN";
      gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.dataGridViewTextBoxColumn1.DefaultCellStyle = gridViewCellStyle3;
      this.dataGridViewTextBoxColumn1.Frozen = true;
      this.dataGridViewTextBoxColumn1.HeaderText = "UserPin";
      this.dataGridViewTextBoxColumn1.MinimumWidth = 72;
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 72;
      this.dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "UserName";
      this.dataGridViewTextBoxColumn2.FillWeight = 2.02495f;
      this.dataGridViewTextBoxColumn2.HeaderText = "UserName";
      this.dataGridViewTextBoxColumn2.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 78;
      this.dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn3.DataPropertyName = "IDCard";
      this.dataGridViewTextBoxColumn3.FillWeight = 3.387189f;
      this.dataGridViewTextBoxColumn3.HeaderText = "CardNumber";
      this.dataGridViewTextBoxColumn3.MinimumWidth = 90;
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      this.dataGridViewTextBoxColumn3.Width = 90;
      this.dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Passwd";
      this.dataGridViewTextBoxColumn4.FillWeight = 0.2410655f;
      this.dataGridViewTextBoxColumn4.HeaderText = "Password";
      this.dataGridViewTextBoxColumn4.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.ReadOnly = true;
      this.dataGridViewTextBoxColumn4.Width = 78;
      this.dataGridViewTextBoxColumn5.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "Pri";
      this.dataGridViewTextBoxColumn5.FillWeight = 5.789683f;
      this.dataGridViewTextBoxColumn5.HeaderText = "Privilege";
      this.dataGridViewTextBoxColumn5.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      this.dataGridViewTextBoxColumn5.ReadOnly = true;
      this.dataGridViewTextBoxColumn5.Width = 89;
      this.dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn6.DataPropertyName = "FP9Count";
      this.dataGridViewTextBoxColumn6.FillWeight = 10.02681f;
      this.dataGridViewTextBoxColumn6.HeaderText = "FP9.0";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      this.dataGridViewTextBoxColumn6.Width = 66;
      this.dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn7.DataPropertyName = "FP10Count";
      this.dataGridViewTextBoxColumn7.FillWeight = 17.49956f;
      this.dataGridViewTextBoxColumn7.HeaderText = "FP10.0";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.Width = 50;
      this.dataGridViewTextBoxColumn8.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn8.DataPropertyName = "FaceCount";
      this.dataGridViewTextBoxColumn8.FillWeight = 30.67878f;
      this.dataGridViewTextBoxColumn8.HeaderText = "Face";
      this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
      this.dataGridViewTextBoxColumn8.ReadOnly = true;
      this.dataGridViewTextBoxColumn8.Width = 50;
      this.dataGridViewTextBoxColumn9.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn9.DataPropertyName = "Grp";
      this.dataGridViewTextBoxColumn9.FillWeight = 53.92212f;
      this.dataGridViewTextBoxColumn9.HeaderText = "Group";
      this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
      this.dataGridViewTextBoxColumn9.ReadOnly = true;
      this.dataGridViewTextBoxColumn9.Width = 50;
      this.dataGridViewTextBoxColumn10.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn10.DataPropertyName = "TZ";
      this.dataGridViewTextBoxColumn10.FillWeight = 94.91494f;
      this.dataGridViewTextBoxColumn10.HeaderText = "Timezone";
      this.dataGridViewTextBoxColumn10.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
      this.dataGridViewTextBoxColumn10.ReadOnly = true;
      this.dataGridViewTextBoxColumn10.Width = 50;
      this.dataGridViewTextBoxColumn11.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn11.DataPropertyName = "TZ";
      this.dataGridViewTextBoxColumn11.FillWeight = 167.2113f;
      this.dataGridViewTextBoxColumn11.HeaderText = "Timezone";
      this.dataGridViewTextBoxColumn11.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
      this.dataGridViewTextBoxColumn11.ReadOnly = true;
      this.dataGridViewTextBoxColumn11.Width = 50;
      this.dataGridViewTextBoxColumn12.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.dataGridViewTextBoxColumn12.DataPropertyName = "Grp";
      this.dataGridViewTextBoxColumn12.FillWeight = 294.7159f;
      this.dataGridViewTextBoxColumn12.HeaderText = "Group";
      this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
      this.dataGridViewTextBoxColumn12.ReadOnly = true;
      this.dataGridViewTextBoxColumn12.Width = 50;
      this.dataGridViewTextBoxColumn13.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewTextBoxColumn13.DataPropertyName = "TZ";
      this.dataGridViewTextBoxColumn13.FillWeight = 519.5876f;
      this.dataGridViewTextBoxColumn13.HeaderText = "Timezone";
      this.dataGridViewTextBoxColumn13.MinimumWidth = 78;
      this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
      this.dataGridViewTextBoxColumn13.ReadOnly = true;
      this.dataGridViewTextBoxColumn13.Width = 78;
      this.colTimezone.DataPropertyName = "TZ";
      this.colTimezone.HeaderText = "Timezone";
      this.colTimezone.Name = "colTimezone";
      this.colTimezone.ReadOnly = true;
      this.colGroup.DataPropertyName = "Grp";
      this.colGroup.HeaderText = "Group";
      this.colGroup.Name = "colGroup";
      this.colGroup.ReadOnly = true;
      this.col_Palm.DataPropertyName = "PalmCount";
      this.col_Palm.HeaderText = "Palm";
      this.col_Palm.Name = "col_Palm";
      this.col_Palm.ReadOnly = true;
      this.colFaceCount.DataPropertyName = "FaceCount";
      this.colFaceCount.HeaderText = "Face";
      this.colFaceCount.Name = "colFaceCount";
      this.colFaceCount.ReadOnly = true;
      this.col_FP12.DataPropertyName = "FP12Count";
      this.col_FP12.HeaderText = "FP12.0";
      this.col_FP12.Name = "col_FP12";
      this.col_FP12.ReadOnly = true;
      this.colFP10Count.DataPropertyName = "FP10Count";
      this.colFP10Count.HeaderText = "FP10.0";
      this.colFP10Count.Name = "colFP10Count";
      this.colFP10Count.ReadOnly = true;
      this.colFP9Count.DataPropertyName = "FP9Count";
      this.colFP9Count.HeaderText = "FP9.0";
      this.colFP9Count.Name = "colFP9Count";
      this.colFP9Count.ReadOnly = true;
      this.colPrivilege.DataPropertyName = "Pri";
      this.colPrivilege.HeaderText = "Privilege";
      this.colPrivilege.Name = "colPrivilege";
      this.colPrivilege.ReadOnly = true;
      this.colPassword.DataPropertyName = "Passwd";
      this.colPassword.HeaderText = "Password";
      this.colPassword.Name = "colPassword";
      this.colPassword.ReadOnly = true;
      this.colCardNumber.DataPropertyName = "IDCard";
      this.colCardNumber.HeaderText = "CardNumber";
      this.colCardNumber.Name = "colCardNumber";
      this.colCardNumber.ReadOnly = true;
      this.colUserName.DataPropertyName = "UserName";
      this.colUserName.HeaderText = "UserName";
      this.colUserName.Name = "colUserName";
      this.colUserName.ReadOnly = true;
      this.colUserPin.DataPropertyName = "PIN";
      this.colUserPin.HeaderText = "UserPin";
      this.colUserPin.Name = "colUserPin";
      this.colUserPin.ReadOnly = true;
      this.col_Index.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
      this.col_Index.DefaultCellStyle = gridViewCellStyle4;
      this.col_Index.Frozen = true;
      this.col_Index.HeaderText = "Index";
      this.col_Index.Name = "col_Index";
      this.col_Index.ReadOnly = true;
      this.col_Index.Width = 50;
      this.cb_Select.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
      this.cb_Select.DataPropertyName = "checkFlag";
      this.cb_Select.Frozen = true;
      this.cb_Select.HeaderText = "";
      this.cb_Select.Name = "cb_Select";
      this.cb_Select.ReadOnly = true;
      this.cb_Select.Width = 50;
      this.AutoScaleDimensions = new SizeF(6f, 12f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.pnlData);
      this.Controls.Add((Control) this.pnlControl);
      this.Controls.Add((Control) this.pnlTop);
      this.Name = nameof (UCUser);
      this.Size = new Size(919, 563);
      this.Load += new EventHandler(this.UCUser_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.pnlControl.ResumeLayout(false);
      this.pnlControl.PerformLayout();
      ((ISupportInitialize) this.picUserPhoto).EndInit();
      this.pnlData.ResumeLayout(false);
      ((ISupportInitialize) this.dgvUser).EndInit();
      this.ResumeLayout(false);
    }
  }
}

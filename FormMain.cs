
using Attendance.Properties;
using BLL;
using Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Utils;

namespace Attendance
{
  public class FormMain : Form
  {
    private int _currentPageId = 1;
    public const int WM_SYSCOMMAND = 274;
    public const int SC_MOVE = 61456;
    public const int HTCAPTION = 2;
    private bool m_pageLoading;
    private DateTime m_lastCollect = Tools.GetDateTimeNow();
    private UserControl m_lastfrm;
    private bool _isStart;
    private ListenClient listenClient;
    private Thread listenClientThread;
    private DeviceBll _deviceBll;
    private AttLogBll _attLogBll;
    private OpLogBll _opLogBll;
    private ErrorLogBll _erLogBll;
    private UserInfoBll _userInfoBll;
    private TmpFPBll _fPBll;
    private TmpFaceBll _faceBll;
    private TmpBioDataBll _bioDataBll;
    private TmpBioPhotoBll _bioPhotoBll;
    private Brush _brush = (Brush) new SolidBrush(Color.FromArgb(37, 190, 167));
    private IContainer components;
    private Panel pnlTop;
    private Label lblTitle;
    private Panel pnlLeft;
    private PictureBox picLogo;
    private Panel pnlWindowsButton;
    private PictureBox picMax;
    private PictureBox picMin;
    private PictureBox picClose;
    private TreeView tvMenu;
    private Panel pnlSetting;
    private Label lblPort;
    private ComboBox cmbIP;
    private Label lblIP;
    private Button btnStart;
    private TextBox txtPort;
    private SplitContainer scMain;
    private UCCommInfo ucCommInfo1;

    public FormMain() => this.InitializeComponent();

    private void picClose_Click(object sender, EventArgs e) => this.Close();

    private void picMax_Click(object sender, EventArgs e)
    {
      if (this.WindowState == FormWindowState.Maximized)
      {
        this.WindowState = FormWindowState.Normal;
      }
      else
      {
        this.FormBorderStyle = FormBorderStyle.None;
        Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
        int width = workingArea.Width;
        workingArea = Screen.PrimaryScreen.WorkingArea;
        int height = workingArea.Height;
        this.MaximumSize = new Size(width, height);
        this.WindowState = FormWindowState.Maximized;
      }
    }

    private void picMin_Click(object sender, EventArgs e)
    {
      this.WindowState = FormWindowState.Minimized;
    }

    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        createParams.Style |= 131072;
        return createParams;
      }
    }

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int IParam);

    private void pnlTop_MouseDown(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
      FormMain.ReleaseCapture();
      FormMain.SendMessage(this.Handle, 274, 61458, 0);
      this.Cursor = Cursors.Default;
    }

    private void FrmMain_Load(object sender, EventArgs e)
    {
      this.tvMenu.ExpandAll();
      this.GetServerIP();
      this.SwitchPage(FormMain.PageIdEnum.Device);
    }

    public void SwitchPage(FormMain.PageIdEnum id)
    {
      if (this.m_pageLoading)
        return;
      this.m_pageLoading = true;
      if (this.InvokeRequired)
      {
        this.Invoke((Delegate) (() => this.SwitchPage(id)));
      }
      else
      {
        this.SetUcCommandVisible(true);
        switch (id)
        {
          case FormMain.PageIdEnum.Device:
            this.LoadPage((UserControl) new UCDevice());
            break;
          case FormMain.PageIdEnum.User:
            this.LoadPage((UserControl) new UCUser());
            break;
          case FormMain.PageIdEnum.Attendance:
            this.LoadPage((UserControl) new UCAttendance());
            break;
          case FormMain.PageIdEnum.DeviceOperationLog:
            this.LoadPage((UserControl) new UCOperateLog());
            break;
          case FormMain.PageIdEnum.DeviceCmd:
            this.LoadPage((UserControl) new UCDeviceCmd());
            break;
          case FormMain.PageIdEnum.CreateCmd:
            this.LoadPage((UserControl) new UCCreateCmd());
            break;
          case FormMain.PageIdEnum.ErrorLog:
            this.LoadPage((UserControl) new UCErrorLog());
            break;
          case FormMain.PageIdEnum.SMS:
            this.LoadPage((UserControl) new UCSms());
            break;
          case FormMain.PageIdEnum.WorkCode:
            this.LoadPage((UserControl) new UCWorkCode());
            break;
        }
        if (id == FormMain.PageIdEnum.User || id == FormMain.PageIdEnum.Attendance || id == FormMain.PageIdEnum.DeviceOperationLog || id == FormMain.PageIdEnum.ErrorLog || id == FormMain.PageIdEnum.SMS || id == FormMain.PageIdEnum.WorkCode)
          this.SetUcCommandVisible(false);
        this._currentPageId = (int) id;
        this.m_pageLoading = false;
      }
    }

    private void SetUcCommandVisible(bool visible)
    {
      this.ucCommInfo1.Visible = visible;
      this.scMain.Panel2Collapsed = !visible;
    }

    private void LoadPage(UserControl frm)
    {
      UserControl lastfrm = this.m_lastfrm;
      this.m_lastfrm = frm;
      this.scMain.Panel1.Controls.Clear();
      this.m_lastfrm.Parent = (Control) this.scMain.Panel1;
      this.m_lastfrm.Dock = DockStyle.Fill;
      this.m_lastfrm.Show();
      if (lastfrm != null)
      {
        try
        {
          try
          {
            foreach (FieldInfo field in lastfrm.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
              if (field.FieldType == typeof (Control))
                this.DisposeUC((Control) field.GetValue((object) lastfrm));
              else if (field.FieldType == typeof (ToolStrip))
                ((Component) field.GetValue((object) lastfrm)).Dispose();
            }
          }
          catch
          {
          }
          lastfrm.Parent = (Control) null;
          lastfrm.Dispose();
        }
        catch
        {
        }
      }
      if (!(DateTime.Now > this.m_lastCollect.AddMinutes(1.0)))
        return;
      this.m_lastCollect = DateTime.Now;
      try
      {
        GC.Collect();
      }
      catch
      {
      }
    }

    private void DisposeUC(Control ctl)
    {
      if (ctl == null)
        return;
      if (ctl.Controls.Count > 0)
      {
        for (int index = 0; index < ctl.Controls.Count; ++index)
          this.DisposeUC(ctl.Controls[index]);
      }
      ctl.Controls.Clear();
      ctl.Dispose();
      ctl = (Control) null;
    }

    private void tvMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
    }

    private void tvMenu_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
    {
    }

    private void tvMenu_AfterSelect(object sender, TreeViewEventArgs e)
    {
      TreeNode selectedNode = this.tvMenu.SelectedNode;
      if (selectedNode.Level == 0)
      {
        if (selectedNode.IsExpanded)
          return;
        selectedNode.Expand();
      }
      else
      {
        string name = selectedNode.Name;
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name)):
          case 37118603:
            if (!(name == "nodeCreateCmd"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.CreateCmd);
            break;
          case 201891409:
            if (!(name == "nodeDeviceCmd"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.DeviceCmd);
            break;
          case 744344038:
            if (!(name == "nodeDeviceExceptionLog"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.ErrorLog);
            break;
          case 1027069530:
            if (!(name == "nodeUser"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.User);
            break;
          case 2088420909:
            if (!(name == "nodeWorkCode"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.WorkCode);
            break;
          case 2155387146:
            if (!(name == "nodeDeviceOperationLog"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.DeviceOperationLog);
            break;
          case 2276901131:
            if (!(name == "nodeDevice"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.Device);
            break;
          case 2822501170:
            if (!(name == "nodeSMS"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.SMS);
            break;
          case 3587455146:
            if (!(name == "nodeAttendance"))
              break;
            this.SwitchPage(FormMain.PageIdEnum.Attendance);
            break;
        }
      }
    }

    private void tvMenu_BeforeSelect(object sender, TreeViewCancelEventArgs e)
    {
      if (!this.m_pageLoading)
        return;
      e.Cancel = true;
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      if (this._isStart)
      {
        this.StopListenling();
        this.btnStart.Text = "Start";
        this.btnStart.ForeColor = Color.FromArgb(37, 190, 167);
        this.ucCommInfo1.AddCommInfo("", 4);
      }
      else
      {
        this.StartListenling(this.cmbIP.Text, this.txtPort.Text);
        this.btnStart.Text = "Stop";
        this.btnStart.ForeColor = Color.Red;
        this.ucCommInfo1.AddCommInfo("", 3);
      }
      this._isStart = !this._isStart;
    }

        public void GetServerIP()
    {
      IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
      this.cmbIP.Text = "";
      foreach (IPAddress address in hostEntry.AddressList)
      {
        if (Regex.IsMatch(address.ToString(), "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$"))
          this.cmbIP.Items.Add((object) address.ToString());
      }
      this.cmbIP.SelectedIndex = 0;
    }

    private void StartListenling(string serverIP, string Port)
    {
      int num = string.IsNullOrEmpty(Port) ? 8080 : int.Parse(Port);
      this.listenClient = new ListenClient();
      this.listenClient.ServerIP = serverIP;
      this.listenClient.Port = num;
      this.listenClientThread = new Thread(new ThreadStart(this.listenClient.StartListening));
      this.listenClient.OnError += new Action<string>(this.listenClient_OnError);
      this.listenClient.OnNewAttLog += new Action<AttLogModel>(this.listenClient_OnNewAttLog);
      this.listenClient.OnNewUser += new Action<UserInfoModel>(this.listenClient_OnNewUser);
      this.listenClient.OnNewFP += new Action<TmpFPModel>(this.listenClient_OnNewFP);
      this.listenClient.OnNewFace += new Action<TmpFaceModel>(this.listenClient_OnNewFace);
      this.listenClient.OnNewPalm += new Action<TmpBioDataModel>(this.listenClient_OnNewPalm);
      this.listenClient.OnNewBioPhoto += new Action<TmpBioPhotoModel>(this.listenClient_OnNewBioPhoto);
      this.listenClient.OnNewOpLog += new Action<OpLogModel>(this.listenClient_OnNewOpLog);
      this.listenClient.OnNewErrorLog += new Action<ErrorLogModel>(this.listenClient_OnNewErrorLog);
      this.listenClient.OnDeviceSync += new Action<DeviceModel>(this.listenClient_OnDeviceSync);
      this.listenClient.OnSendDataEvent += new Action<string>(this.listenClient_OnSendDataEvent);
      this.listenClient.OnReceiveDataEvent += new Action<string>(this.listenClient_OnReceiveDataEvent);
      this.listenClientThread.IsBackground = true;
      this.listenClientThread.Start();
    }

    private void StopListenling()
    {
      if (this.listenClient == null || !this.listenClient.Listening)
        return;
      this.listenClient.StopListening();
    }

    private DeviceBll DeviceBll
    {
      get
      {
        if (this._deviceBll == null)
          this._deviceBll = new DeviceBll();
        return this._deviceBll;
      }
    }

    private AttLogBll AttLogBll
    {
      get
      {
        if (this._attLogBll == null)
          this._attLogBll = new AttLogBll();
        return this._attLogBll;
      }
    }

    private OpLogBll OpLogBll
    {
      get
      {
        if (this._opLogBll == null)
          this._opLogBll = new OpLogBll();
        return this._opLogBll;
      }
    }

    private ErrorLogBll ErLogBll
    {
      get
      {
        if (this._erLogBll == null)
          this._erLogBll = new ErrorLogBll();
        return this._erLogBll;
      }
    }

    private UserInfoBll UserInfoBll
    {
      get
      {
        if (this._userInfoBll == null)
          this._userInfoBll = new UserInfoBll();
        return this._userInfoBll;
      }
    }

    private TmpFPBll FPBll
    {
      get
      {
        if (this._fPBll == null)
          this._fPBll = new TmpFPBll();
        return this._fPBll;
      }
    }

    private TmpFaceBll FaceBll
    {
      get
      {
        if (this._faceBll == null)
          this._faceBll = new TmpFaceBll();
        return this._faceBll;
      }
    }

    private TmpBioDataBll BioDataBll
    {
      get
      {
        if (this._bioDataBll == null)
          this._bioDataBll = new TmpBioDataBll();
        return this._bioDataBll;
      }
    }

    private TmpBioPhotoBll BioPhotoBll
    {
      get
      {
        if (this._bioPhotoBll == null)
          this._bioPhotoBll = new TmpBioPhotoBll();
        return this._bioPhotoBll;
      }
    }

    private void listenClient_OnNewMachine(string sn)
    {
      this.DeviceBll.Add(new DeviceModel()
      {
        DevSN = sn,
        TimeZone = "34"
      });
    }

    private void listenClient_OnNewAttLog(AttLogModel attlog)
    {
      if (this.AttLogBll.Add(attlog) <= 0)
        return;
      if (this._currentPageId == 3 && this.m_lastfrm != null)
      {
        ((UCAttendance) this.m_lastfrm).AddNewRow(attlog);
      }
      else
      {
        if (this._currentPageId != 1 || this.m_lastfrm == null)
          return;
        UCDevice lastfrm = (UCDevice) this.m_lastfrm;
        lastfrm.UpdateDeviceMask(attlog.DeviceID, attlog.MaskFlag);
        lastfrm.UpdateDeviceTemp(attlog.DeviceID, attlog.Temperature);
      }
    }

    private void listenClient_OnNewOpLog(OpLogModel oplog)
    {
      if (this.OpLogBll.Add(oplog) <= 0 || this._currentPageId != 4 || this.m_lastfrm == null)
        return;
      ((UCOperateLog) this.m_lastfrm).RefreshData();
    }

    private void listenClient_OnNewErrorLog(ErrorLogModel erlog)
    {
      if (this.ErLogBll.Add(erlog) <= 0 || this._currentPageId != 7 || this.m_lastfrm == null)
        return;
      ((UCErrorLog) this.m_lastfrm).AddNewRow(erlog);
    }

    private void listenClient_OnNewUser(UserInfoModel user)
    {
      if (this.UserInfoBll.Get(user.PIN) != null || this._userInfoBll.Add(user) <= 0 || this._currentPageId != 2 || this.m_lastfrm == null)
        return;
      ((UCUser) this.m_lastfrm).LoadAllUsers();
    }

    private void listenClient_OnNewFP(TmpFPModel fp)
    {
      if (this.FPBll.Add(fp) <= 0 || this._currentPageId != 2 || this.m_lastfrm == null)
        return;
      if (fp.MajorVer == "9")
      {
        ((UCUser) this.m_lastfrm).UpdateUserFP9Info(fp);
      }
      else
      {
        if (!(fp.MajorVer == "10"))
          return;
        ((UCUser) this.m_lastfrm).UpdateUserFP10Info(fp);
      }
    }

    private void listenClient_OnNewFace(TmpFaceModel face)
    {
      if (this.FaceBll.Add(face) <= 0 || this._currentPageId != 2 || this.m_lastfrm == null)
        return;
      ((UCUser) this.m_lastfrm).UpdateUserFaceInfo((object) face);
    }

    private void listenClient_OnNewPalm(TmpBioDataModel palm)
    {
      if (this.BioDataBll.Add(palm) <= 0 || this._currentPageId != 2 || this.m_lastfrm == null)
        return;
      ((UCUser) this.m_lastfrm).UpdateUserPalmInfo(palm);
    }

    private void listenClient_OnNewBioPhoto(TmpBioPhotoModel bioPhoto)
    {
      if (this.BioPhotoBll.Add(bioPhoto) <= 0 || this._currentPageId != 2 || this.m_lastfrm == null)
        return;
      ((UCUser) this.m_lastfrm).UpdateUserFaceInfo((object) bioPhoto);
    }

    private void listenClient_OnGetVendorName(string sn, string vendorName)
    {
      this.DeviceBll.UpdateVendorName(sn, vendorName);
    }

    private void listenClient_OnError(string errMessage)
    {
      int num = (int) MessageBox.Show(errMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    private void listenClient_OnSendDataEvent(string Data) => this.ucCommInfo1.AddCommInfo(Data, 1);

    private void listenClient_OnReceiveDataEvent(string Data)
    {
      this.ucCommInfo1.AddCommInfo(Data, 0);
    }

    private void listenClient_OnDeviceSync(DeviceModel device)
    {
      if (this._currentPageId != 1 || this.m_lastfrm == null)
        return;
      ((UCDevice) this.m_lastfrm).UpdateDevice(device);
    }

    private void tvMenu_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
      if ((e.State & TreeNodeStates.Selected) != (TreeNodeStates) 0)
      {
        Rectangle rect;
        ref Rectangle local = ref rect;
        Rectangle bounds = e.Node.Bounds;
        int x = bounds.X;
        bounds = e.Node.Bounds;
        int y = bounds.Y;
        bounds = e.Node.Bounds;
        int width = bounds.Width + 5;
        bounds = e.Node.Bounds;
        int height = bounds.Height;
        local = new Rectangle(x, y, width, height);
        e.Graphics.FillRectangle(this._brush, rect);
        Font font = e.Node.NodeFont ?? ((Control) sender).Font;
        e.Graphics.DrawString(e.Node.Text, font, Brushes.White, (RectangleF) Rectangle.Inflate(rect, 2, -6));
      }
      else
        e.DrawDefault = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      TreeNode treeNode1 = new TreeNode("Device");
      TreeNode treeNode2 = new TreeNode("DeviceCmd");
      TreeNode treeNode3 = new TreeNode("DeviceOperationLog");
      TreeNode treeNode4 = new TreeNode("CreateCmd");
      TreeNode treeNode5 = new TreeNode("SMS");
      TreeNode treeNode6 = new TreeNode("DeviceGoup", new TreeNode[5]
      {
        treeNode1,
        treeNode2,
        treeNode3,
        treeNode4,
        treeNode5
      });
      TreeNode treeNode7 = new TreeNode("User");
      TreeNode treeNode8 = new TreeNode("UserGroup", new TreeNode[1]
      {
        treeNode7
      });
      TreeNode treeNode9 = new TreeNode("Attendance");
      TreeNode treeNode10 = new TreeNode("WorkCode");
      TreeNode treeNode11 = new TreeNode("AttendanceGroup", new TreeNode[2]
      {
        treeNode9,
        treeNode10
      });
      TreeNode treeNode12 = new TreeNode("DeviceExceptionLog");
      TreeNode treeNode13 = new TreeNode("LogGroup", new TreeNode[1]
      {
        treeNode12
      });
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (FormMain));
      this.pnlTop = new Panel();
      this.pnlWindowsButton = new Panel();
      this.picMax = new PictureBox();
      this.picMin = new PictureBox();
      this.picClose = new PictureBox();
      this.picLogo = new PictureBox();
      this.lblTitle = new Label();
      this.pnlLeft = new Panel();
      this.pnlSetting = new Panel();
      this.btnStart = new Button();
      this.txtPort = new TextBox();
      this.lblPort = new Label();
      this.cmbIP = new ComboBox();
      this.lblIP = new Label();
      this.tvMenu = new TreeView();
      this.scMain = new SplitContainer();
      this.ucCommInfo1 = new UCCommInfo();
      this.pnlTop.SuspendLayout();
      this.pnlWindowsButton.SuspendLayout();
      ((ISupportInitialize) this.picMax).BeginInit();
      ((ISupportInitialize) this.picMin).BeginInit();
      ((ISupportInitialize) this.picClose).BeginInit();
      ((ISupportInitialize) this.picLogo).BeginInit();
      this.pnlLeft.SuspendLayout();
      this.pnlSetting.SuspendLayout();
      this.scMain.BeginInit();
      this.scMain.Panel2.SuspendLayout();
      this.scMain.SuspendLayout();
      this.SuspendLayout();
      this.pnlTop.BackColor = Color.White;
      this.pnlTop.Controls.Add((Control) this.pnlWindowsButton);
      this.pnlTop.Controls.Add((Control) this.picLogo);
      this.pnlTop.Controls.Add((Control) this.lblTitle);
      this.pnlTop.Dock = DockStyle.Top;
      this.pnlTop.Location = new Point(0, 0);
      this.pnlTop.Margin = new Padding(3, 0, 3, 3);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new Size(1350, 58);
      this.pnlTop.TabIndex = 0;
      this.pnlTop.MouseDown += new MouseEventHandler(this.pnlTop_MouseDown);
      this.pnlWindowsButton.Controls.Add((Control) this.picMax);
      this.pnlWindowsButton.Controls.Add((Control) this.picMin);
      this.pnlWindowsButton.Controls.Add((Control) this.picClose);
      this.pnlWindowsButton.Dock = DockStyle.Right;
      this.pnlWindowsButton.Location = new Point(1150, 0);
      this.pnlWindowsButton.Name = "pnlWindowsButton";
      this.pnlWindowsButton.Size = new Size(200, 58);
      this.pnlWindowsButton.TabIndex = 5;
      this.picMax.Cursor = Cursors.Hand;
      this.picMax.Image = (Image) Resources.btn_maximize2;
      this.picMax.Location = new Point(83, 12);
      this.picMax.Name = "picMax";
      this.picMax.Size = new Size(44, 39);
      this.picMax.TabIndex = 7;
      this.picMax.TabStop = false;
      this.picMax.Click += new EventHandler(this.picMax_Click);
      this.picMin.Cursor = Cursors.Hand;
      this.picMin.Image = (Image) Resources.btn_minimize2;
      this.picMin.Location = new Point(12, 12);
      this.picMin.Name = "picMin";
      this.picMin.Size = new Size(44, 39);
      this.picMin.TabIndex = 6;
      this.picMin.TabStop = false;
      this.picMin.Click += new EventHandler(this.picMin_Click);
      this.picClose.Cursor = Cursors.Hand;
      this.picClose.Image = (Image) Resources.btn_close2;
      this.picClose.Location = new Point(144, 12);
      this.picClose.Name = "picClose";
      this.picClose.Size = new Size(44, 39);
      this.picClose.TabIndex = 5;
      this.picClose.TabStop = false;
      this.picClose.Click += new EventHandler(this.picClose_Click);
      this.picLogo.Image = (Image) Resources.ZKTeco;
      this.picLogo.Location = new Point(0, 0);
      this.picLogo.Name = "picLogo";
      this.picLogo.Size = new Size(190, 58);
      this.picLogo.SizeMode = PictureBoxSizeMode.CenterImage;
      this.picLogo.TabIndex = 2;
      this.picLogo.TabStop = false;
      this.lblTitle.AutoSize = true;
      this.lblTitle.Font = new Font("Arial", 16f);
      this.lblTitle.ForeColor = Color.FromArgb(37, 190, 167);
      this.lblTitle.Location = new Point(248, 17);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new Size(239, 25);
      this.lblTitle.TabIndex = 0;
      this.lblTitle.Text = "Attendance Push Demo";
      this.lblTitle.TextAlign = ContentAlignment.MiddleLeft;
      this.lblTitle.MouseDown += new MouseEventHandler(this.pnlTop_MouseDown);
      this.pnlLeft.BackColor = SystemColors.InactiveCaption;
      this.pnlLeft.Controls.Add((Control) this.pnlSetting);
      this.pnlLeft.Controls.Add((Control) this.tvMenu);
      this.pnlLeft.Dock = DockStyle.Left;
      this.pnlLeft.Location = new Point(0, 58);
      this.pnlLeft.Name = "pnlLeft";
      this.pnlLeft.Padding = new Padding(0, 3, 0, 0);
      this.pnlLeft.Size = new Size(244, 694);
      this.pnlLeft.TabIndex = 1;
      this.pnlSetting.BackColor = Color.FromArgb(37, 190, 167);
      this.pnlSetting.Controls.Add((Control) this.btnStart);
      this.pnlSetting.Controls.Add((Control) this.txtPort);
      this.pnlSetting.Controls.Add((Control) this.lblPort);
      this.pnlSetting.Controls.Add((Control) this.cmbIP);
      this.pnlSetting.Controls.Add((Control) this.lblIP);
      this.pnlSetting.Dock = DockStyle.Bottom;
      this.pnlSetting.Location = new Point(0, 581);
      this.pnlSetting.Name = "pnlSetting";
      this.pnlSetting.Size = new Size(244, 113);
      this.pnlSetting.TabIndex = 1;
      this.btnStart.BackColor = Color.White;
      this.btnStart.Cursor = Cursors.Hand;
      this.btnStart.FlatStyle = FlatStyle.Flat;
      this.btnStart.Font = new Font("Arial", 12f);
      this.btnStart.ForeColor = Color.FromArgb(37, 190, 167);
      this.btnStart.Location = new Point(70, 73);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new Size(120, 28);
      this.btnStart.TabIndex = 4;
      this.btnStart.Text = "Start";
      this.btnStart.UseVisualStyleBackColor = false;
      this.btnStart.Click += new EventHandler(this.btnStart_Click);
      this.txtPort.Font = new Font("Arial", 9f);
      this.txtPort.Location = new Point(70, 42);
      this.txtPort.Name = "txtPort";
      this.txtPort.Size = new Size(121, 21);
      this.txtPort.TabIndex = 3;
      this.txtPort.Text = "8080";
      this.lblPort.AutoSize = true;
      this.lblPort.Font = new Font("Arial", 10f);
      this.lblPort.ForeColor = Color.White;
      this.lblPort.Location = new Point(34, 44);
      this.lblPort.Name = "lblPort";
      this.lblPort.Size = new Size(34, 16);
      this.lblPort.TabIndex = 2;
      this.lblPort.Text = "Port";
      this.cmbIP.Font = new Font("Arial", 9f);
      this.cmbIP.FormattingEnabled = true;
      this.cmbIP.Location = new Point(70, 15);
      this.cmbIP.Name = "cmbIP";
      this.cmbIP.Size = new Size(121, 23);
      this.cmbIP.TabIndex = 1;
      this.lblIP.AutoSize = true;
      this.lblIP.Font = new Font("Arial", 10f);
      this.lblIP.ForeColor = Color.White;
      this.lblIP.Location = new Point(34, 18);
      this.lblIP.Name = "lblIP";
      this.lblIP.Size = new Size(20, 16);
      this.lblIP.TabIndex = 0;
      this.lblIP.Text = "IP";
      this.tvMenu.BackColor = Color.FromArgb(64, 64, 64);
      this.tvMenu.Cursor = Cursors.Hand;
      this.tvMenu.Dock = DockStyle.Fill;
      this.tvMenu.DrawMode = TreeViewDrawMode.OwnerDrawText;
      this.tvMenu.Font = new Font("Arial", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.tvMenu.ForeColor = Color.White;
      this.tvMenu.HideSelection = false;
      this.tvMenu.ItemHeight = 35;
      this.tvMenu.Location = new Point(0, 3);
      this.tvMenu.Margin = new Padding(3, 0, 3, 3);
      this.tvMenu.Name = "tvMenu";
      treeNode1.Name = "nodeDevice";
      treeNode1.Text = "Device";
      treeNode2.Name = "nodeDeviceCmd";
      treeNode2.Text = "DeviceCmd";
      treeNode3.Name = "nodeDeviceOperationLog";
      treeNode3.Text = "DeviceOperationLog";
      treeNode4.Name = "nodeCreateCmd";
      treeNode4.Text = "CreateCmd";
      treeNode5.Name = "nodeSMS";
      treeNode5.Text = "SMS";
      treeNode6.Name = "nodeDeviceGoup";
      treeNode6.Text = "DeviceGoup";
      treeNode7.Name = "nodeUser";
      treeNode7.Text = "User";
      treeNode8.Name = "nodeUserGroup";
      treeNode8.Text = "UserGroup";
      treeNode9.Name = "nodeAttendance";
      treeNode9.Text = "Attendance";
      treeNode10.Name = "nodeWorkCode";
      treeNode10.Text = "WorkCode";
      treeNode11.Name = "nodeAttendanceGroup";
      treeNode11.Text = "AttendanceGroup";
      treeNode12.Name = "nodeDeviceExceptionLog";
      treeNode12.Text = "DeviceExceptionLog";
      treeNode13.Name = "nodeLogGroup";
      treeNode13.Text = "LogGroup";
      this.tvMenu.Nodes.AddRange(new TreeNode[4]
      {
        treeNode6,
        treeNode8,
        treeNode11,
        treeNode13
      });
      this.tvMenu.Size = new Size(244, 691);
      this.tvMenu.TabIndex = 0;
      this.tvMenu.DrawNode += new DrawTreeNodeEventHandler(this.tvMenu_DrawNode);
      this.tvMenu.NodeMouseHover += new TreeNodeMouseHoverEventHandler(this.tvMenu_NodeMouseHover);
      this.tvMenu.BeforeSelect += new TreeViewCancelEventHandler(this.tvMenu_BeforeSelect);
      this.tvMenu.AfterSelect += new TreeViewEventHandler(this.tvMenu_AfterSelect);
      this.tvMenu.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvMenu_NodeMouseClick);
      this.scMain.BorderStyle = BorderStyle.FixedSingle;
      this.scMain.Dock = DockStyle.Fill;
      this.scMain.Location = new Point(244, 58);
      this.scMain.Name = "scMain";
      this.scMain.Orientation = Orientation.Horizontal;
      this.scMain.Panel2.Controls.Add((Control) this.ucCommInfo1);
      this.scMain.Size = new Size(1106, 694);
      this.scMain.SplitterDistance = 413;
      this.scMain.TabIndex = 2;
      this.ucCommInfo1.Dock = DockStyle.Fill;
      this.ucCommInfo1.Location = new Point(0, 0);
      this.ucCommInfo1.Name = "ucCommInfo1";
      this.ucCommInfo1.Size = new Size(1104, 275);
      this.ucCommInfo1.TabIndex = 0;
      this.AutoScaleMode = AutoScaleMode.None;
      this.ClientSize = new Size(1350, 752);
      this.ControlBox = false;
      this.Controls.Add((Control) this.scMain);
      this.Controls.Add((Control) this.pnlLeft);
      this.Controls.Add((Control) this.pnlTop);
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MinimumSize = new Size(1135, 730);
      this.Name = nameof (FormMain);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Load += new EventHandler(this.FrmMain_Load);
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.pnlWindowsButton.ResumeLayout(false);
      ((ISupportInitialize) this.picMax).EndInit();
      ((ISupportInitialize) this.picMin).EndInit();
      ((ISupportInitialize) this.picClose).EndInit();
      ((ISupportInitialize) this.picLogo).EndInit();
      this.pnlLeft.ResumeLayout(false);
      this.pnlSetting.ResumeLayout(false);
      this.pnlSetting.PerformLayout();
      this.scMain.Panel2.ResumeLayout(false);
      this.scMain.EndInit();
      this.scMain.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    public enum PageIdEnum
    {
      Device = 1,
      User = 2,
      Attendance = 3,
      DeviceOperationLog = 4,
      DeviceCmd = 5,
      CreateCmd = 6,
      ErrorLog = 7,
      SMS = 8,
      WorkCode = 9,
    }
  }
}

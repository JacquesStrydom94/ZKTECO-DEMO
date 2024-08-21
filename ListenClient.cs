
using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Utils;

namespace Attendance
{
  public class ListenClient
  {
    private TcpListener tcp;
    private const int MAXBUFFERSIZE = 2097152;
    private int port = 80;
    private string serverIP = string.Empty;
    private bool listening;
    private DeviceBll _deviceBll;
    private DeviceCmdBll _deviceCmdBll;

    public int Port
    {
      get => this.port;
      set => this.port = value;
    }

    public string ServerIP
    {
      get => this.serverIP;
      set => this.serverIP = value;
    }

    public bool Listening => this.listening;

    public event Action<string> OnError;

    public event Action<AttLogModel> OnNewAttLog;

    public event Action<OpLogModel> OnNewOpLog;

    public event Action<ErrorLogModel> OnNewErrorLog;

    public event Action<UserInfoModel> OnNewUser;

    public event Action<TmpFPModel> OnNewFP;

    public event Action<TmpFaceModel> OnNewFace;

    public event Action<TmpBioDataModel> OnNewPalm;

    public event Action<TmpBioPhotoModel> OnNewBioPhoto;

    public event Action<WorkCodeModel> OnNewWorkcode;

    public event Action<string> OnNewMachine;

    public event Action<DeviceModel> OnDeviceSync;

    public event Action<string> OnReceiveDataEvent;

    public event Action<string> OnSendDataEvent;

    public void StartListening()
    {
      try
      {
        if (this.tcp == null)
          this.tcp = new TcpListener(IPAddress.Parse(this.serverIP), this.port);
        this.tcp.Start();
        this.listening = true;
        while (this.listening)
        {
          try
          {
            Socket endsocket = this.tcp.AcceptSocket();
            endsocket.ReceiveBufferSize = 2097152;
            endsocket.SendBufferSize = 2097152;
            Thread.Sleep(500);
            if (endsocket.Available > 0)
            {
              byte[] numArray = new byte[2097152];
              endsocket.Receive(numArray);
              this.Analysis(numArray, endsocket);
            }
          }
          catch (Exception ex)
          {
          }
        }
        this.tcp.Stop();
      }
      catch
      {
        this.listening = false;
        string str = string.Format("Please be sure that you are listening to the port number of your own PC.And {0} port is not occupied by other application or stopped by firewall.", (object) this.port);
        if (this.OnError == null)
          return;
        this.OnError(str);
      }
    }

    public void StopListening()
    {
      if (!this.listening)
        return;
      this.listening = false;
      this.tcp.Stop();
    }

    private void Analysis(byte[] bReceive, Socket endsocket)
    {
      string str = Encoding.ASCII.GetString(bReceive).TrimEnd().TrimEnd(new char[1]);
      if (this.OnReceiveDataEvent != null)
        this.OnReceiveDataEvent(str);
      if (str.IndexOfEx("cdata?") > 0)
        this.cdataProcess(bReceive, endsocket);
      else if (str.IndexOfEx("getrequest?") > 0)
        this.GetRequestProcess(bReceive, endsocket);
      else if (str.IndexOfEx("devicecmd?") > 0)
      {
        this.DeviceCmdProcess(bReceive, endsocket);
      }
      else
      {
        this.UnknownCmdProcess(endsocket);
        if (this.OnError == null)
          return;
        this.OnError("UnKnown message from device: " + str);
      }
    }

    private void GetTimeNumber(string sBuffer, ref string numberstr)
    {
      numberstr = "";
      for (int index = 0; index < sBuffer.Length && sBuffer[index] > '/' && sBuffer[index] < ':'; ++index)
        numberstr += sBuffer[index].ToString();
    }

    private string GetValueByNameInPushHeader(string buffer, string Name)
    {
      string[] strArray = buffer.Split('&', '?', ' ');
      if (strArray.Length == 0)
        return (string) null;
      foreach (string str in strArray)
      {
        if (str.IndexOfEx(Name + "=") >= 0)
          return str.Substring(str.IndexOfEx(Name + "=") + Name.Length + 1);
      }
      return (string) null;
    }

    private int GetTimeFormTimeZone(string timezone)
    {
      try
      {
        if ('-' == timezone[0])
        {
          timezone = timezone.Substring(1);
          string[] strArray = timezone.Split(':');
          if (strArray.Length == 2)
            return -1 * (Convert.ToInt32(strArray[0]) * 60 + Convert.ToInt32(strArray[1]));
          if (strArray.Length == 1)
            return -1 * (Convert.ToInt32(strArray[0]) * 60);
        }
        else
        {
          string[] strArray = timezone.Split(':');
          if (strArray.Length == 2)
            return Convert.ToInt32(strArray[0]) * 60 + Convert.ToInt32(strArray[1]);
          if (strArray.Length == 1)
            return Convert.ToInt32(strArray[0]) * 60;
        }
      }
      catch
      {
      }
      return 0;
    }

    private string GetDeviceInitInfo(DeviceModel device)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int timeFormTimeZone = this.GetTimeFormTimeZone(device.TimeZone);
      stringBuilder.AppendFormat("GET OPTION FROM:{0}\n", (object) device.DevSN);
      stringBuilder.AppendFormat("Stamp={0}\n", (object) device.AttLogStamp);
      stringBuilder.AppendFormat("OpStamp={0}\n", (object) device.OperLogStamp);
      stringBuilder.AppendFormat("PhotoStamp={0}\n", (object) device.AttPhotoStamp);
      stringBuilder.AppendFormat("TransFlag={0}\n", (object) device.TransFlag);
      stringBuilder.AppendFormat("ErrorDelay={0}\n", (object) device.ErrorDelay);
      stringBuilder.AppendFormat("Delay={0}\n", (object) device.Delay);
      stringBuilder.AppendFormat("TimeZone={0}\n", (object) timeFormTimeZone);
      stringBuilder.AppendFormat("TransTimes={0}\n", (object) device.TransTimes);
      stringBuilder.AppendFormat("TransInterval={0}\n", (object) device.TransInterval);
      stringBuilder.AppendFormat("SyncTime={0}\n", (object) device.SyncTime);
      stringBuilder.AppendFormat("Realtime={0}\n", (object) device.Realtime);
      stringBuilder.AppendFormat("ServerVer={0} {1}\n", (object) "2.2.14", (object) Tools.GetDateTimeNow().ToShortDateString());
      stringBuilder.AppendFormat("PushProtVer={0}\n", (object) "2.4.1");
      stringBuilder.AppendFormat("PushOptionsFlag={0}\n", (object) "1");
      stringBuilder.AppendFormat("ATTLOGStamp={0}\n", (object) device.AttLogStamp);
      stringBuilder.AppendFormat("OPERLOGStamp={0}\n", (object) device.OperLogStamp);
      stringBuilder.AppendFormat("ATTPHOTOStamp={0}\n", (object) device.AttPhotoStamp);
      stringBuilder.AppendFormat("ServerName=Logtime Server\n");
      stringBuilder.AppendFormat("MultiBioDataSupport={0}\n", (object) device.MultiBioDataSupport);
      return stringBuilder.ToString();
    }

    public DeviceBll DeviceBll
    {
      get
      {
        if (this._deviceBll == null)
          this._deviceBll = new DeviceBll();
        return this._deviceBll;
      }
    }

    public DeviceCmdBll DeviceCmdBll
    {
      get
      {
        if (this._deviceCmdBll == null)
          this._deviceCmdBll = new DeviceCmdBll();
        return this._deviceCmdBll;
      }
    }

    private string InitDeviceConnect(string DevSN, ref string RepString)
    {
      DeviceModel device = this.DeviceBll.Get(DevSN);
      if (device == null)
      {
        if (this.OnNewMachine != null)
        {
          this.OnNewMachine(DevSN);
          RepString = "OK\r\n";
          return "200 OK";
        }
        RepString = "Device Unauthorized";
        return "401 Unauthorized";
      }
      RepString = this.GetDeviceInitInfo(device);
      return "200 OK";
    }

    private void cdataProcess(byte[] bReceive, Socket remoteSocket)
    {
      string str = Encoding.ASCII.GetString(bReceive).TrimEnd().TrimEnd(new char[1]);
      string nameInPushHeader = this.GetValueByNameInPushHeader(str, "SN");
      string sStatusCode = "200 OK";
      string RepString = "OK\r\n";
      if (str.Substring(0, 3) == "GET")
      {
        if (str.IndexOfEx("options=all") > 0)
        {
          this.SendDataToDevice(this.InitDeviceConnect(nameInPushHeader, ref RepString), RepString, ref remoteSocket);
          remoteSocket.Close();
        }
        else
        {
          this.SendDataToDevice("400 Bad Request", "Unknow Command", ref remoteSocket);
          remoteSocket.Close();
        }
      }
      else
      {
        if (!(str.Substring(0, 4) == "POST"))
          return;
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("OPERLOG", 1) < 0 && str.IndexOfEx("ATTLOG", 1) > 0 && str.IndexOfEx("OPLOG", 1) < 0)
          this.AttLog(str);
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("OPERLOG", 1) > 0 && str.IndexOfEx("ATTLOG", 1) < 0)
          this.OperLog(str);
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("BIODATA", 1) > 0)
          this.BioData(str);
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("ERRORLOG", 1) > 0 && str.IndexOfEx("ATTLOG", 1) < 0)
          this.Errorlog(str);
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("ATTPHOTO", 1) > 0)
          this.AttPhoto(bReceive);
        if (str.IndexOfEx("Stamp", 1) > 0 && str.IndexOfEx("USERPIC", 1) > 0)
          this.UserPicLog(str);
        if (str.IndexOfEx("table=options", 1) > 0)
          this.Options(str);
        this.SendDataToDevice(sStatusCode, RepString, ref remoteSocket);
        remoteSocket.Close();
      }
    }

    private void UpDateDeviceInfo(DeviceModel device, string strDevInfo)
    {
      string[] strArray = strDevInfo.Split(',');
      try
      {
        device.DevFirmwareVersion = strArray[0].Replace("%20", " ");
        device.UserCount = Convert.ToInt32(strArray[1]);
        device.AttCount = Convert.ToInt32(strArray[3]);
        device.DevIP = strArray[4];
      }
      catch
      {
        if (this.OnError != null)
          this.OnError("Device Info Error:" + strDevInfo);
      }
      if (this.DeviceBll.Update(device) <= 0 || this.OnDeviceSync == null)
        return;
      this.OnDeviceSync(device);
    }

    private void GetRequestProcess(byte[] bReceive, Socket remoteSocket)
    {
      string buffer = Encoding.GetEncoding("gb2312").GetString(bReceive);
      string nameInPushHeader1 = this.GetValueByNameInPushHeader(buffer, "SN");
      string sStatusCode = "200 OK";
      DeviceModel device = this.DeviceBll.Get(nameInPushHeader1);
      string sDataStr;
      if (device == null)
      {
        sStatusCode = "401 Unauthorized";
        sDataStr = "Device Unauthorized";
      }
      else
      {
        if (this.OnDeviceSync != null)
          this.OnDeviceSync(device);
        string nameInPushHeader2 = this.GetValueByNameInPushHeader(buffer, "INFO");
        if (string.IsNullOrEmpty(nameInPushHeader2))
        {
          sDataStr = this.DeviceCmdBll.Send(nameInPushHeader1) + "\r\n";
        }
        else
        {
          this.UpDateDeviceInfo(device, nameInPushHeader2);
          sDataStr = "OK\r\n";
        }
      }
      this.SendDataToDevice(sStatusCode, sDataStr, ref remoteSocket);
      remoteSocket.Close();
    }

    private void DeviceCmdProcess(byte[] bReceive, Socket remoteSocket)
    {
      string str1;
      string str2 = str1 = Encoding.ASCII.GetString(bReceive).TrimEnd(new char[1]);
      string empty = string.Empty;
      string str3 = str1.Substring(str1.IndexOfEx("SN=") + 3).Split('&')[0];
      int startIndex = str2.IndexOfEx("ID=");
      this.SendDataToDevice("200 OK", "OK\r\n", ref remoteSocket);
      remoteSocket.Close();
      this._deviceCmdBll.Update(str2.Substring(startIndex));
    }

    private void UnknownCmdProcess(Socket remoteSocket)
    {
      this.SendDataToDevice("401 Unknown", "Unknown DATA", ref remoteSocket);
      remoteSocket.Close();
    }

    private void SendDataToDevice(string sStatusCode, string sDataStr, ref Socket mySocket)
    {
      byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(sDataStr);
      string str1 = "HTTP/1.1 " + sStatusCode + "\r\n" + "Content-Type: text/plain\r\n" + "Accept-Ranges: bytes\r\n";
      DateTime dateTime = Tools.GetDateTimeNow();
      dateTime = dateTime.ToUniversalTime();
      string str2 = dateTime.ToString("r");
      string s = str1 + "Date: " + str2 + "\r\n" + "Content-Length: " + (object) bytes.Length + "\r\n\r\n";
      this.SendToBrowser(Encoding.GetEncoding("gb2312").GetBytes(s), ref mySocket);
      this.SendToBrowser(bytes, ref mySocket);
    }

    private void SendToBrowser(byte[] bSendData, ref Socket mySocket)
    {
      string str = string.Empty;
      try
      {
        if (mySocket.Connected)
        {
          if (this.OnSendDataEvent != null)
            this.OnSendDataEvent(Encoding.ASCII.GetString(bSendData));
          if (mySocket.Send(bSendData, bSendData.Length, SocketFlags.None) == -1)
            str = "Socket Error: Cannot Send Packet";
        }
        else
          str = "Link Failed...";
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      if (string.IsNullOrEmpty(str) || this.OnError == null)
        return;
      this.OnError(str);
    }

    private OpLogModel CreateOplog(string oplog, string machineSN)
    {
      string[] strArray = oplog.Split('\t');
      return new OpLogModel()
      {
        OpType = strArray[0].Substring(6),
        Operator = strArray[1],
        OpTime = Convert.ToDateTime(strArray[2]),
        Obj1 = strArray[3],
        Obj2 = strArray[4],
        Obj3 = strArray[5],
        Obj4 = strArray[6],
        User = "0",
        DeviceID = machineSN
      };
    }

    private TmpFPModel CreatFP(string template, string SN, bool isBioData)
    {
      TmpFPModel tmpFpModel = new TmpFPModel();
      if (isBioData)
      {
        template = Tools.Replace(template, "BIODATA", "");
        Dictionary<string, string> keyValues = Tools.GetKeyValues(template);
        tmpFpModel.Pin = Tools.GetValueFromDic(keyValues, "PIN");
        tmpFpModel.Fid = Tools.GetValueFromDic(keyValues, "No");
        tmpFpModel.Valid = Tools.GetValueFromDic(keyValues, "Valid");
        tmpFpModel.Duress = Tools.GetValueFromDic(keyValues, "Duress");
        string valueFromDic = Tools.GetValueFromDic(keyValues, "MajorVer");
        if (string.IsNullOrEmpty(valueFromDic))
        {
          DeviceModel deviceModel = this._deviceBll.Get(SN);
          if (deviceModel != null)
            valueFromDic = deviceModel.GetBioVersion(BioType.FingerPrint).Split('.')[0];
        }
        tmpFpModel.MajorVer = valueFromDic;
        tmpFpModel.Tmp = Tools.GetValueFromDic(keyValues, "TMP");
      }
      else
      {
        template = Tools.Replace(template, "FP", "");
        Dictionary<string, string> keyValues = Tools.GetKeyValues(template);
        tmpFpModel.Pin = Tools.GetValueFromDic(keyValues, "PIN");
        tmpFpModel.Fid = Tools.GetValueFromDic(keyValues, "FID");
        tmpFpModel.Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(keyValues, "Size"), 0);
        tmpFpModel.Valid = Tools.GetValueFromDic(keyValues, "Valid");
        tmpFpModel.Tmp = Tools.GetValueFromDic(keyValues, "TMP");
        tmpFpModel.MajorVer = !tmpFpModel.Tmp.StartsWith("oco") ? "10" : "9";
      }
      return tmpFpModel;
    }

    private TmpFaceModel CreatFace(string template, bool isBioData)
    {
      TmpFaceModel tmpFaceModel = new TmpFaceModel();
      if (isBioData)
      {
        template = Tools.Replace(template, "BIODATA", "");
        Dictionary<string, string> keyValues = Tools.GetKeyValues(template);
        tmpFaceModel.Pin = Tools.GetValueFromDic(keyValues, "PIN");
        tmpFaceModel.Fid = Tools.GetValueFromDic(keyValues, "No");
        tmpFaceModel.Valid = Tools.GetValueFromDic(keyValues, "Valid");
        string valueFromDic1 = Tools.GetValueFromDic(keyValues, "MajorVer");
        string valueFromDic2 = Tools.GetValueFromDic(keyValues, "MinorVer");
        tmpFaceModel.Ver = valueFromDic1 + "." + valueFromDic2;
        tmpFaceModel.Tmp = Tools.GetValueFromDic(keyValues, "TMP");
      }
      else
      {
        template = Tools.Replace(template, "FACE", "");
        Dictionary<string, string> keyValues = Tools.GetKeyValues(template);
        tmpFaceModel.Pin = Tools.GetValueFromDic(keyValues, "PIN");
        tmpFaceModel.Fid = Tools.GetValueFromDic(keyValues, "FID");
        tmpFaceModel.Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(keyValues, "SIZE"), 0);
        tmpFaceModel.Valid = Tools.GetValueFromDic(keyValues, "VALID");
        tmpFaceModel.Tmp = Tools.GetValueFromDic(keyValues, "TMP");
      }
      return tmpFaceModel;
    }

    private TmpBioDataModel CreatPalm(string template)
    {
      template = Tools.Replace(template, "BIODATA", "");
      Dictionary<string, string> keyValues = Tools.GetKeyValues(template);
      return new TmpBioDataModel()
      {
        Pin = Tools.GetValueFromDic(keyValues, "PIN"),
        No = Tools.GetValueFromDic(keyValues, "No"),
        Index = Tools.GetValueFromDic(keyValues, "Index"),
        Valid = Tools.GetValueFromDic(keyValues, "Valid"),
        Duress = Tools.GetValueFromDic(keyValues, "Duress"),
        Type = Tools.GetValueFromDic(keyValues, "Type"),
        MajorVer = Tools.GetValueFromDic(keyValues, "MajorVer"),
        MinorVer = Tools.GetValueFromDic(keyValues, "MinorVer"),
        Format = Tools.GetValueFromDic(keyValues, "Format"),
        Tmp = Tools.GetValueFromDic(keyValues, "TMP")
      };
    }

    private TmpBioPhotoModel CreatBioPhoto(string bioPhotoString)
    {
      bioPhotoString = Tools.Replace(bioPhotoString, "BIOPHOTO", "");
      Dictionary<string, string> keyValues = Tools.GetKeyValues(bioPhotoString);
      return new TmpBioPhotoModel()
      {
        Pin = Tools.GetValueFromDic(keyValues, "PIN"),
        FileName = Tools.GetValueFromDic(keyValues, "FileName"),
        Type = Tools.GetValueFromDic(keyValues, "Type"),
        Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(keyValues, "Size"), 0),
        Content = Tools.GetValueFromDic(keyValues, "Content")
      };
    }

    private UserInfoModel CreatUserInfo(string userstring)
    {
      userstring = Tools.Replace(userstring, "USER", "");
      Dictionary<string, string> keyValues = Tools.GetKeyValues(userstring);
      return new UserInfoModel()
      {
        PIN = Tools.GetValueFromDic(keyValues, "PIN"),
        UserName = Tools.GetValueFromDic(keyValues, "Name"),
        Pri = Tools.GetValueFromDic(keyValues, "Pri"),
        Passwd = Tools.GetValueFromDic(keyValues, "Passwd"),
        IDCard = Tools.GetValueFromDic(keyValues, "Card"),
        Grp = Tools.GetValueFromDic(keyValues, "Grp"),
        TZ = Tools.GetValueFromDic(keyValues, "TZ")
      };
    }

    private void SaveOperLog(string oplog, string machineSN)
    {
      if (this.OnNewOpLog == null)
        return;
      this.OnNewOpLog(this.CreateOplog(oplog, machineSN));
    }

    private void SaveFP(string enfplog, string SN, bool isBioData)
    {
      if (enfplog.IndexOfEx("PIN") < 0 || this.OnNewFP == null)
        return;
      this.OnNewFP(this.CreatFP(enfplog, SN, isBioData));
    }

    private void SaveFace(string enfacelog, bool isBioData)
    {
      if (enfacelog.IndexOfEx("PIN") < 0 || this.OnNewFace == null)
        return;
      this.OnNewFace(this.CreatFace(enfacelog, isBioData));
    }

    private void SavePalm(string enPalmlog)
    {
      if (enPalmlog.IndexOfEx("PIN") < 0 || this.OnNewPalm == null)
        return;
      this.OnNewPalm(this.CreatPalm(enPalmlog));
    }

    private void SaveUserinfo(string usinlog)
    {
      if (usinlog.IndexOfEx("PIN") < 0 || this.OnNewUser == null)
        return;
      this.OnNewUser(this.CreatUserInfo(usinlog));
    }

    private void SaveBioPhoto(string bioPhoto)
    {
      if (bioPhoto.IndexOfEx("PIN") < 0 || this.OnNewBioPhoto == null)
        return;
      this.OnNewBioPhoto(this.CreatBioPhoto(bioPhoto));
    }

    private void SeparateOPERLOGData(string datastr, string SN)
    {
      try
      {
        string str1 = datastr;
        char[] chArray = new char[1]{ '\n' };
        foreach (object obj in str1.Split(chArray))
        {
          string str2 = obj.ToString();
          if (str2.IndexOfEx("OPLOG ") >= 0)
            this.SaveOperLog(str2, SN);
          else if (str2.Split(' ')[0] == "USER")
            this.SaveUserinfo(str2);
          else if (str2.Split(' ')[0] == "FP")
            this.SaveFP(str2, SN, false);
          else if (str2.Split(' ')[0] == "FACE")
            this.SaveFace(str2, false);
          else if (str2.Split(' ')[0] == "BIOPHOTO")
            this.SaveBioPhoto(str2);
        }
      }
      catch (Exception ex)
      {
        if (this.OnError == null)
          return;
        this.OnError(ex.Message);
      }
    }

    private void SeparateBioData(string datastr, string SN)
    {
      try
      {
        string str1 = datastr;
        char[] chArray = new char[1]{ '\n' };
        foreach (object obj in str1.Split(chArray))
        {
          string str2 = obj.ToString();
          if (!string.IsNullOrEmpty(str2))
          {
            switch ((BioType) System.Enum.Parse(typeof (BioType), str2.Split('\t')[5].Split('=')[1]))
            {
              case BioType.FingerPrint:
                this.SaveFP(str2, SN, true);
                continue;
              case BioType.Face:
                this.SaveFace(str2, true);
                continue;
              case BioType.Palm:
                this.SavePalm(str2);
                continue;
              default:
                continue;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (this.OnError == null)
          return;
        this.OnError(ex.Message);
      }
    }

    private void OperLog(string sBuffer)
    {
      string str = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      if (string.IsNullOrEmpty(str))
        return;
      string sBuffer1 = sBuffer.Substring(sBuffer.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer1, ref numberstr);
      this.DeviceBll.UpdateOperLogStamp(numberstr, str);
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.SeparateOPERLOGData(sBuffer.Substring(num + 4), str);
    }

    private void BioData(string sBuffer)
    {
      string SN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      string sBuffer1 = sBuffer.Substring(sBuffer.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer1, ref numberstr);
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.SeparateBioData(sBuffer.Substring(num + 4), SN);
    }

    public void Errorlog(string sBuffer)
    {
      string str = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      string sBuffer1 = sBuffer.Substring(sBuffer.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer1, ref numberstr);
      this.DeviceBll.UpdateErrorLogStamp(numberstr, str);
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.SeparateErrorData(sBuffer.Substring(num + 4), str);
    }

    private void SeparateErrorData(string datastr, string SN)
    {
      try
      {
        string str1 = datastr;
        char[] chArray = new char[1]{ '\n' };
        foreach (object obj in str1.Split(chArray))
        {
          string str2 = obj.ToString();
          if (str2.IndexOfEx("ERRORLOG ") >= 0)
            this.SaveErrorLog(str2, SN);
        }
      }
      catch (Exception ex)
      {
        if (this.OnError == null)
          return;
        this.OnError(ex.Message);
      }
    }

    private void SaveErrorLog(string erlog, string machineSN)
    {
      if (this.OnNewErrorLog == null)
        return;
      this.OnNewErrorLog(this.CreateErrorlog(erlog, machineSN));
    }

    private ErrorLogModel CreateErrorlog(string erlog, string machineSN)
    {
      string[] strArray = erlog.Split('\t');
      ErrorLogModel errorlog = new ErrorLogModel();
      string str = strArray[0];
      errorlog.ErrCode = str.Substring(8).Split('=')[1];
      errorlog.ErrMsg = strArray[1].Split('=')[1];
      errorlog.DataOrigin = strArray[2].Split('=')[1];
      errorlog.CmdId = strArray[3].Split('=')[1];
      errorlog.Additional = strArray[4].Split('=')[1];
      errorlog.DeviceID = machineSN;
      return errorlog;
    }

    private void AttLog(string sBuffer)
    {
      string str = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      string sBuffer1 = sBuffer.Substring(sBuffer.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer1, ref numberstr);
      this.DeviceBll.UpdateAttLogStamp(numberstr, str);
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.AttLogProcess(sBuffer.Substring(num + 4), str);
    }

    private void AttLogProcess(string attstr, string machineSN)
    {
      try
      {
        string str1 = attstr;
        char[] chArray = new char[1]{ '\n' };
        foreach (string str2 in str1.Split(chArray))
        {
          if (!string.IsNullOrEmpty(str2))
            this.SaveAttLog(str2.ToString(), machineSN);
        }
      }
      catch (Exception ex)
      {
        if (this.OnError == null)
          return;
        this.OnError(ex.Message);
      }
    }

    private AttLogModel CreateAttlog(string attlog, string machineSN)
    {
      string[] strArray = attlog.Split('\t');
      AttLogModel attlog1 = new AttLogModel();
      attlog1.PIN = strArray[0];
      attlog1.AttTime = Convert.ToDateTime(strArray[1]);
      attlog1.Status = strArray[2];
      attlog1.Verify = strArray[3];
      attlog1.DeviceID = machineSN;
      try
      {
        attlog1.WorkCode = strArray[4];
      }
      catch
      {
        attlog1.WorkCode = "0";
      }
      if (strArray.Length > 8)
      {
        int result;
        if (int.TryParse(strArray[7], out result))
          attlog1.MaskFlag = result;
        attlog1.Temperature = strArray[8];
      }
      return attlog1;
    }

    private void SaveAttLog(string attlog, string machineSN)
    {
      if (this.OnNewAttLog == null)
        return;
      this.OnNewAttLog(this.CreateAttlog(attlog, machineSN));
    }

    private void AttPhoto(byte[] bReceive)
    {
      string str1 = Encoding.ASCII.GetString(bReceive);
      byte[] numArray = new byte[bReceive.Length];
      string[] strArray = str1.Split('\n');
      string str2 = "";
      foreach (string str3 in strArray)
      {
        if (str3.IndexOfEx("PIN=") >= 0)
        {
          str2 = str3;
          break;
        }
      }
      string devSN = str1.Substring(str1.IndexOfEx("SN=") + 3).Split('&')[0];
      string sBuffer = str1.Substring(str1.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer, ref numberstr);
      this.DeviceBll.UpdateAttPhotoStamp(numberstr, devSN);
      int sourceIndex = str1.IndexOfEx("uploadphoto") + 12;
      Array.Copy((Array) bReceive, sourceIndex, (Array) numArray, 0, bReceive.Length - sourceIndex);
      System.IO.File.WriteAllBytes(Environment.CurrentDirectory + "\\Capture" + "\\" + str2.Replace("PIN=", ""), numArray);
    }

    private void WorkcodeLog(string sBuffer)
    {
      string str = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.WorkcodeLogProcess(sBuffer.Substring(num + 4));
    }

    private void WorkcodeLogProcess(string sBuffer)
    {
      try
      {
        if (sBuffer.Length == 0)
          return;
        int length = sBuffer.IndexOfEx("\n", 1);
        string sBuffer1 = string.Empty;
        if (length > 0)
        {
          string str = sBuffer.Substring(0, length);
          if (str.IndexOfEx("WORKCODE") >= 0)
            this.SaveWorkcode(str);
          sBuffer1 = sBuffer.Substring(length + 1);
        }
        if (string.IsNullOrEmpty(sBuffer1))
          return;
        this.WorkcodeLogProcess(sBuffer1);
      }
      catch (Exception ex)
      {
        if (this.OnError == null)
          return;
        this.OnError(ex.Message);
      }
    }

    private void SaveWorkcode(string usinlog)
    {
      if (usinlog.IndexOfEx("Code") < 0)
        return;
      string str1 = usinlog.Substring(0, usinlog.IndexOfEx("\t"));
      string str2 = usinlog.Substring(usinlog.IndexOfEx("\t") + 1);
      WorkCodeModel workCodeModel = new WorkCodeModel();
      workCodeModel.WorkCode = str1.Replace("WORKCODE Code=", "");
      workCodeModel.WorkName = str2.Replace("Name=", "");
      if (this.OnNewWorkcode == null)
        return;
      this.OnNewWorkcode(workCodeModel);
    }

    public void UserPicLog(string sBuffer)
    {
      string str = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      string sBuffer1 = sBuffer.Substring(sBuffer.IndexOfEx("Stamp=") + 6);
      string numberstr = "";
      this.GetTimeNumber(sBuffer1, ref numberstr);
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      this.userpiclogprocess(sBuffer.Substring(num + 4));
    }

    public void userpiclogprocess(string sBuffer)
    {
      try
      {
        int length = sBuffer.IndexOfEx("\n", 1);
        string usinlog = "";
        if (length > 0)
          usinlog = sBuffer.Substring(0, length);
        this.saveuserpic(usinlog);
        string sBuffer1 = sBuffer.Substring(length + 1);
        if (!(sBuffer1 != ""))
          return;
        this.userpiclogprocess(sBuffer1);
      }
      catch
      {
      }
    }

    private void Base64StringToImage(string txtFileName)
    {
      try
      {
        FileStream fileStream = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
        StreamReader streamReader = new StreamReader((Stream) fileStream);
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(streamReader.ReadToEnd()));
        new Bitmap((Stream) memoryStream).Save(txtFileName.Substring(0, txtFileName.Length - 4) + ".jpg", ImageFormat.Jpeg);
        memoryStream.Close();
        streamReader.Close();
        fileStream.Close();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Base64StringToImage 转换失败/nException：" + ex.Message);
      }
    }

    public void saveuserpic(string usinlog)
    {
      if (usinlog.IndexOfEx("PIN") <= 0 || usinlog.IndexOfEx("FileName") <= 0)
        return;
      usinlog.Substring(0, usinlog.IndexOfEx("\t"));
      string str1 = usinlog.Substring(usinlog.IndexOfEx("\t") + 1);
      string str2 = str1.Substring(0, str1.IndexOfEx("\t"));
      string str3 = str1.Substring(str1.IndexOfEx("\t") + 1);
      str3.Substring(0, str3.IndexOfEx("\t"));
      string str4 = str3.Substring(str3.IndexOfEx("\t") + 1).Substring(8);
      string str5 = Environment.CurrentDirectory + "\\Photo";
      string str6 = str2.Replace("FileName=", "");
      string str7 = str6.Substring(0, str6.Length - 4);
      string path = str5 + "\\" + str7 + ".txt";
      StreamWriter streamWriter = new StreamWriter(path);
      streamWriter.WriteLine(str4);
      streamWriter.Close();
      this.Base64StringToImage("Photo\\" + str7 + ".txt");
      if (!System.IO.File.Exists(path))
        return;
      System.IO.File.Delete(path);
    }

    private void Options(string sBuffer)
    {
      string devSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3).Split('&')[0];
      if (string.IsNullOrEmpty(devSN))
        return;
      int num = sBuffer.IndexOfEx("\r\n\r\n", 1);
      string strOptions = sBuffer.Substring(num + 4);
      if (string.IsNullOrEmpty(strOptions))
        return;
      this.DeviceBll.Update(this.GetDeviceModelByOptions(devSN, strOptions));
    }

    private DeviceModel GetDeviceModelByOptions(string devSN, string strOptions)
    {
      DeviceModel model = this.DeviceBll.Get(devSN);
      this.FormatBioData(ref strOptions);
      Tools.InitModel((object) model, strOptions);
      return model;
    }

    private void FormatBioData(ref string options)
    {
      if (string.IsNullOrEmpty(options))
        return;
      string vals1 = "0:0:0:0:0:0:0:0:0:0";
      string str1 = "0:0:0:0:0:0:0:0:0:0";
      string vals2 = "0:0:0:0:0:0:0:0:0:0";
      string vals3 = "0:0:0:0:0:0:0:0:0:0";
      string vals4 = "0:0:0:0:0:0:0:0:0:0";
      string str2 = "0:0:0:0:0:0:0:0:0:0";
      options = options.Replace("~", "");
      foreach (string str3 in options.Split(",\t".ToCharArray()))
      {
        char[] chArray = new char[1]{ '=' };
        string[] strArray = str3.Split(chArray);
        if (strArray.Length == 2)
        {
          string a = strArray[0].Trim();
          string val = strArray[1].Trim();
          if (!(val == "") && !(val == "0"))
          {
            if (string.Equals(a, "FingerFunOn", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerPrint, val, ref vals1);
            else if (string.Equals(a, "FPVersion", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerPrint, val, ref vals2);
            else if (string.Equals(a, "FPCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerPrint, val, ref vals3);
            else if (string.Equals(a, "MaxFingerCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerPrint, val, ref vals4);
            else if (string.Equals(a, "FaceFunOn", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Face, val, ref vals1);
            else if (string.Equals(a, "FaceVersion", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Face, val, ref vals2);
            else if (string.Equals(a, "FaceCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Face, val, ref vals3);
            else if (string.Equals(a, "MaxFaceCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Face, val, ref vals4);
            else if (string.Equals(a, "FvFunOn", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerVein, val, ref vals1);
            else if (string.Equals(a, "FvVersion", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerVein, val, ref vals2);
            else if (string.Equals(a, "FvCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerVein, val, ref vals3);
            else if (string.Equals(a, "MaxFvCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.FingerVein, val, ref vals4);
            else if (string.Equals(a, "PvFunOn", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Palm, val, ref vals1);
            else if (string.Equals(a, "PvVersion", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Palm, val, ref vals2);
            else if (string.Equals(a, "PvCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Palm, val, ref vals3);
            else if (string.Equals(a, "MaxPvCount", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.Palm, val, ref vals4);
            else if (string.Equals(a, "VisilightFun", StringComparison.OrdinalIgnoreCase))
              this.UpdateOptionVal(BioType.VisilightFace, val, ref vals1);
          }
        }
      }
      options = options + ",MultiBioDataSupport=" + vals1 + ",MultiBioPhotoSupport=" + str1 + ",MultiBioVersion=" + vals2 + ",MultiBioCount=" + vals3 + ",MaxMultiBioDataCount=" + vals4 + ",MaxMultiBioPhotoCount=" + str2;
    }

    private string UpdateOptionVal(BioType BioType, string val, ref string vals)
    {
      string[] strArray = vals.Split(':');
      int index = (int) BioType;
      if (index >= strArray.Length)
        return vals;
      strArray[index] = val;
      vals = string.Join(":", strArray);
      return vals;
    }
  }
}

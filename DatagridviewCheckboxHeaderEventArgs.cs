
using System;

namespace Attendance
{
  public class DatagridviewCheckboxHeaderEventArgs : EventArgs
  {
    public DatagridviewCheckboxHeaderEventArgs() => this.CheckedState = false;

    public bool CheckedState { get; set; }
  }
}

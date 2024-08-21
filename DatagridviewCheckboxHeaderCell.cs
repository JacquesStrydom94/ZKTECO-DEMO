using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


namespace Attendance
{
  internal class DatagridviewCheckboxHeaderCell : DataGridViewColumnHeaderCell
  {
    private Point checkBoxLocation;
    private Size checkBoxSize;
    private Point _cellLocation;
    private CheckBoxState _cbState = CheckBoxState.UncheckedNormal;

    public bool _checked { get; set; }

    public event DatagridviewcheckboxHeaderEventHander OnCheckBoxClicked;

    protected override void Paint(
      Graphics graphics,
      Rectangle clipBounds,
      Rectangle cellBounds,
      int rowIndex,
      DataGridViewElementStates dataGridViewElementState,
      object value,
      object formattedValue,
      string errorText,
      DataGridViewCellStyle cellStyle,
      DataGridViewAdvancedBorderStyle advancedBorderStyle,
      DataGridViewPaintParts paintParts)
    {
      base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
      Point point = new Point();
      Size glyphSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
      point.X = cellBounds.Location.X + cellBounds.Width / 2 - glyphSize.Width / 2 - 1;
      point.Y = cellBounds.Location.Y + cellBounds.Height / 2 - glyphSize.Height / 2;
      this._cellLocation = cellBounds.Location;
      this.checkBoxLocation = point;
      this.checkBoxSize = glyphSize;
      this._cbState = !this._checked ? CheckBoxState.UncheckedNormal : CheckBoxState.CheckedNormal;
      CheckBoxRenderer.DrawCheckBox(graphics, this.checkBoxLocation, this._cbState);
    }

    protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
    {
      Point point = new Point(e.X + this._cellLocation.X, e.Y + this._cellLocation.Y);
      if (point.X >= this.checkBoxLocation.X && point.X <= this.checkBoxLocation.X + this.checkBoxSize.Width && point.Y >= this.checkBoxLocation.Y && point.Y <= this.checkBoxLocation.Y + this.checkBoxSize.Height)
      {
        this._checked = !this._checked;
        DatagridviewCheckboxHeaderEventArgs e1 = new DatagridviewCheckboxHeaderEventArgs()
        {
          CheckedState = this._checked
        };
        object sender = new object();
        if (this.OnCheckBoxClicked != null)
        {
          this.OnCheckBoxClicked(sender, e1);
          this.DataGridView.InvalidateCell((DataGridViewCell) this);
        }
      }
      base.OnMouseClick(e);
    }
  }
}

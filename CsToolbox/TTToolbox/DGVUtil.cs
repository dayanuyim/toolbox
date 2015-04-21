using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace TTToolbox
{
    public class DGVUtil
    {
        /// <summary>
        /// once click, enter edit mode immediately
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="col_name"></param>
        public static void QucikEditCombo(DataGridView dgv, string col_name)
        {
            dgv.BeginEdit(true);
            if (dgv.EditingControl != null){
                ((ComboBox)dgv.EditingControl).DroppedDown = true;
            }
        }

        //No use more :)  //2011/03/29
        //chekc a column's name and its index
        public static bool IndexMatch(DataGridView dgv, int idx, string name)
        {
            //return (dgv.Columns.Contains(name) && dgv.Columns[name].Index == idx);
            return (dgv.Columns[idx].Name == name);
        }

        //get binding data row
        public static DataRow GetBoundRow(DataGridView dgv, int idx)
        {
            DataRowView view = (DataRowView) dgv.Rows[idx].DataBoundItem;
            return view.Row;
        }

        //get 'deleted' data
        public static T GetDeletedData<T>(DataRow row, string name)
        {
            return (T) row[name, DataRowVersion.Original];
        }

        //Auto size
        /* //no more use
         * use:
         *  dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
         *  
        public static void AutoSize(DataGridView dgv)
        {
            foreach(DataGridViewColumn col in dgv.Columns)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }*/

        public static void PrintTableCount(DataGridView dgv, Control show, string temp)
        {
            int cc = dgv.Rows.Count;

            show.Text = temp.Replace("0", cc.ToString());
            if(cc > 1) show.Text += "s";

            ResizeAux.AlignOutCorner(dgv, show, ResizeAlign.RightUpper);
        }

        //Set Row Color
        public static void SetRowColor(DataGridView dgv, int idx, System.Drawing.Color? fg, System.Drawing.Color? bg)
        {
            SetRowColor(dgv.Rows[idx], fg, bg);
        }

        public static void SetRowColor(DataGridViewRow row, System.Drawing.Color? fg, System.Drawing.Color? bg)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (fg != null) cell.Style.ForeColor = (Color)fg;
                if (bg != null) cell.Style.BackColor = (Color)bg;
            }
        }

        public static void ExtLastCol(DataGridView dgv)
        {

            DataGridViewColumn col = dgv.Columns.GetLastColumn(
                DataGridViewElementStates.Visible, DataGridViewElementStates.None);

            if(col != null)
                AverageColumns(dgv, new DataGridViewColumn[] { col });

        }

        public static void AverageColumns(DataGridView dgv)
        {
            AverageColumns(dgv, dgv.Columns);
        }

        public static void AverageColumnsRev(DataGridView dgv, System.Collections.IList ex_avg_cols)
        {
            List<DataGridViewColumn> avg_cols = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in dgv.Columns)
                if (!ex_avg_cols.Contains(col))
                    avg_cols.Add(col);

            AverageColumns(dgv, avg_cols);
        }

        public static void AverageColumns(DataGridView dgv, System.Collections.IList average_cols)
        {
            //get all visiblew cols
            List<DataGridViewColumn> cols = new List<DataGridViewColumn>();
            List<DataGridViewColumn> avg_cols = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                    cols.Add(col);

                if(col.Visible && average_cols.Contains(col))
                    avg_cols.Add(col);
            }
            
            //non-sense
            if (cols.Count == 0 || avg_cols.Count == 0)
                return;

            int w = dgv.Width -3;   //why 3?

            //subtract other col width
            foreach (DataGridViewColumn col in cols)
            {
                if (!avg_cols.Contains(col))
                    w -= col.Width;
            }

            /*
            //subtract sperated vertical 
            switch (dgv.CellBorderStyle)
            {
                case DataGridViewCellBorderStyle.None:
                case DataGridViewCellBorderStyle.RaisedHorizontal:
                case DataGridViewCellBorderStyle.SingleHorizontal:
                case DataGridViewCellBorderStyle.SunkenHorizontal:
                    break;
                default:
                    w -= cols.Count - 1;
                    break;
            }
            */

            //substract row header
            if(dgv.RowHeadersVisible)
                w -= dgv.RowHeadersWidth;

            //substract vscrollbar with
            if(dgv.Controls.OfType<VScrollBar>().First().Visible)
                w -= SystemInformation.VerticalScrollBarWidth;

            //set
            int avg = w / avg_cols.Count;
            foreach (DataGridViewColumn col in avg_cols)
                col.Width = avg;
            avg_cols[avg_cols.Count - 1].Width += w % avg_cols.Count;

        }

    }
}

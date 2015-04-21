using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TTToolbox
{
    public enum BoxLocation{ ScreenCenter, Mouse};

    public partial class RMessageBox : Form
    {
        private  RMessageBox()
        {
            InitializeComponent();
        }

        private void RMessageBox_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;

            this.KeyDown += FormUtil.KeyDownHandler(button_ok, button_ok);
            button_ok.KeyDown += FormUtil.KeyDownHandler(button_ok, button_ok);
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private  void SetInfo(string msg)
        {
            label_info.Text = msg;
            //TTToolbox.ResizeAux.AlignCenter(label_info, this);
            TTToolbox.ResizeAux.AlignCenter(button_ok, this);
        }

        #region Static Method

        public static DialogResult Show(string msg)
        {
            return Show(msg, Form.MousePosition.X, Form.MousePosition.Y - 50);
        }

        public static DialogResult Show(string msg, Point location)
        {
            return Show(msg, location.X, location.Y);
        }
        public static DialogResult Show(string msg, int x, int y)
        {
            RMessageBox box = new RMessageBox();
            box.SetInfo(msg);
            box.StartPosition = FormStartPosition.Manual;
            box.Location = new Point(x, y);
            return box.ShowDialog();
        }

        #endregion
    }

}

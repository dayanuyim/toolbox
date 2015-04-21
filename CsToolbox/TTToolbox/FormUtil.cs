using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using System.ComponentModel;
using System.Data;

namespace TTToolbox
{
    public class FormUtil
    {
        /// <summary>
        /// Exit? return Yes|No (True|False)
        /// </summary>
        /// <returns></returns>
        public static bool ExitPrompt()
        {
            if (MessageBox.Show("確定離開？", "離開", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                return true;
            return false;
        }

        public static void SetDialogByMouse(Form dialog)
        {
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = new Point(Form.MousePosition.X, Form.MousePosition.Y - 50);
        }

        public static void ControlMessage(Control ctrl, string msg)
        {
            /*
            ToolTip tip = new ToolTip();
            tip.ToolTipIcon = icon;
            tip.ForeColor = Color.Red;
            tip.BackColor = Color.LightYellow;
            //tip.AutoPopDelay = 3000;
            tip.ReshowDelay = 3000;
            //tip.ShowAlways = true;
            //tip.SetToolTip(ctrl, msg);
            tip.Show(msg, ctrl);
             */

            Point pt = FormUtil.ScreenLocation(ctrl);
            TTToolbox.RMessageBox.Show(msg, pt.X + ctrl.Width, pt.Y);
        }



        public static bool MdiChildFocus(Form child)
        {
            if (child != null && child.Visible)
            {
                child.Focus();
                if (child.WindowState == FormWindowState.Minimized) child.WindowState = FormWindowState.Normal;
                return true;
            }

            return false;
        }

        public static void MdiChildInit(Form parent, Form child, ToolStripMenuItem menu)
        {
            child.MdiParent = parent;
            child.FormClosed += new FormClosedEventHandler(
               delegate(object obj, FormClosedEventArgs ex)
               {
                   menu.Checked = false;
               });
            menu.Checked = true;
            child.Show();
        }

        public static Point ScreenLocation(Control ctrl)
        {
            int x = 0, y = 0;

            for(;ctrl != null; ctrl = ctrl.Parent)
            {
                x += ctrl.Location.X;
                y += ctrl.Location.Y;
            }

            return new Point(x, y);
        }

        public static void SetComboAutoComplete(ComboBox combo, List<string> list)
        {
            //ste item
            combo.Items.Clear();
            combo.Items.AddRange(list.ToArray());
            if(combo.Items.Count > 0)
                combo.SelectedIndex = 0;

            //set auto complate
            combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            combo.AutoCompleteSource = AutoCompleteSource.ListItems;

        }




        #region ShowErrorMessage

        public static void ConsoleErrMsg(string msg, Exception ex)
        {
            Console.WriteLine(msg + ": " + ex.ToString());
        }
        public static void ShowErrMsg(string msg)
        {
            MessageBox.Show(msg);
        }
        public static void ShowErrMsg(string msg, Exception ex)
        {
            ShowErrMsg(msg, ex.ToString());
        }
        public static void ShowErrMsg(string msg, string desc)
        {
            if(msg != string.Empty && desc != string.Empty)
                msg += ": " + desc;
            MessageBox.Show(msg);
        }


        public static KeyEventHandler KeyDownHandler(Button ok, Button cancel)
        {
            KeyEventHandler handler = new KeyEventHandler(
                delegate(object sender, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        if(ok != null) ok.PerformClick();
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        if(cancel != null) cancel.PerformClick();
                    }
                });

            return handler;

        }

        public static void SetKeyDownHandler(Form form, Button ok, Button cancel)
        {
            form.KeyPreview = true;
            //SetKeyDownHandler(form, ok, cancel); //stack overflow!
            form.KeyDown += KeyDownHandler(ok, cancel);
        }

        public static void SetKeyDownHandler(Control ctrl, Button ok, Button cancel)
        {
            /*
            Form form = ctrl as Form;
            if (form != null)
                form.KeyPreview = true;
             */

            ctrl.KeyDown += KeyDownHandler(ok, cancel);
        }

        #endregion


        public static void RotateBmpByOrient(Bitmap bmp, int orient)
        {
            switch (orient)
            {
                case 1:
                    break;
                case 3:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 6:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 8:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);

            return result;
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, double scale)
        {
            return ResizeBitmap(bmp,
                (int)(bmp.Width * scale),
                (int)(bmp.Height * scale));
        }

    }
}

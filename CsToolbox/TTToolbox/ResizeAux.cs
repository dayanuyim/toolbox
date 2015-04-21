//2011/03/28
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TTToolbox
{
    public enum ResizeAlign { RightUpper, RightDown };

    public class ResizeAux
    {

        #region Align
        public static void AlignOutCorner(Control ctrl, Control norm, ResizeAlign align)
        {
            AlignOutCorner(ctrl, norm, align, 0, 0);
        }
        public static void AlignOutCorner(Control ctrl, Control norm, ResizeAlign align, int xpad, int ypad)
        {
            switch (align)
            {
                case ResizeAlign.RightUpper:
                    SetLocation(ctrl,
                        norm.Location.X + norm.Width - ctrl.Width - xpad,
                        norm.Location.Y - ctrl.Height - ypad);
                    break;
                case ResizeAlign.RightDown:
                    SetLocation(ctrl,
                        norm.Location.X + norm.Width - ctrl.Width - xpad,
                        norm.Location.Y + norm.Height + ypad);
                    break;
                default:
                    break;
            }
        }


        [System.Obsolete("The Methond is needed?", false)]
        public static void AlignInner(Control ctrl, Control norm, ResizeAlign align)
        {
            AlignInner(norm, ctrl, align, 0, 0);
        }

        [System.Obsolete("The Methond is needed?", false)]
        public static void AlignInner(Control ctrl, Control norm, ResizeAlign align, int xpad, int ypad)
        {
            switch(align)
            {
                case ResizeAlign.RightUpper:
                    SetLocation(ctrl,
                        norm.Width - ctrl.Width - xpad,
                        ypad);
                    break;
                case ResizeAlign.RightDown:
                    SetLocation(ctrl,
                        norm.Width - ctrl.Width - xpad,
                        norm.Height - ctrl.Height - ypad);
                    break;
                default:
                    break;
            }
        }

        public static void AlignRight(Control ctrl, Control norm)
        {
            SetLocationX(ctrl, norm.Location.X + norm.Width - ctrl.Width);
        }

        public static void AlignCenter(Control ctrl, Control norm)
        {
            SetLocationX(ctrl, norm.Location.X + (norm.Width - ctrl.Width)/2);
        }

        #endregion

        public static void MoveLocationX(Control ctrl, int x)
        {
            SetLocationX(ctrl, ctrl.Location.X + x);
        }
        public static void MoveLocationY(Control ctrl, int y)
        {
            SetLocationY(ctrl, ctrl.Location.Y + y);
        }
        public static void MoveLocation(Control ctrl, int x, int y)
        {
            SetLocation(ctrl,
                ctrl.Location.X + x,
                ctrl.Location.Y + y);
        }

        public static void SetLocationX(Control ctrl, int x)
        {
            ctrl.Location = new System.Drawing.Point(
                x,
                ctrl.Location.Y);

        }
        public static void SetLocationY(Control ctrl, int y)
        {
            ctrl.Location = new System.Drawing.Point(
                ctrl.Location.X,
                y);
        }
        public static void SetLocation(Control ctrl, int x, int y)
        {
            ctrl.Location = new System.Drawing.Point(x, y);
        }



    }
}

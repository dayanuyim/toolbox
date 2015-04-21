//version 2011/03/27

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TTToolbox
{
    [Flags]
    public enum ResizerType
    {
        None = 0x00,
        Width = 0x01,
        Height = 0x02,
        Size = 0x03,
        X = 0x04,
        Y = 0x08,
        Location = 0x0C
    };

    public enum TuneType
    {
        None = 0,
        Size = 1,
        Location = 2
    }

    public class Resizer
    {
        private TuneType tune_type_;

        EventHandler resize_handler_ = null;

        private bool enabled_ = false;
        public bool Enabled
        {
            get { return enabled_; }
            set {
                enabled_ = value;
                if (enabled_)
                    norm_ctrl_.Resize += resize_handler_;
                else
                    norm_ctrl_.Resize -= resize_handler_;
            }
        }

        private Control norm_ctrl_;
        public Control NormControl
        {
            get { return norm_ctrl_; }
        }
        
        /*
        private Dictionary<Control, RSize> virtual_size_ = new Dictionary<Control, RSize>();
        private Dictionary<Control, RPoint> virtual_loc_ = new Dictionary<Control, RPoint>();
        private Dictionary<Control, ResizerType> rz_type_ = new Dictionary<Control, ResizerType>();
         */
        protected Dictionary<Control, ResizeInfo> rz_info_ = new Dictionary<Control, ResizeInfo>();

        //private int w_ = 0;
        //private int h_ = 0;
        
        public delegate void AfterTuneHandler();
        public event AfterTuneHandler AfterTune;
        private void OnAfterTune()
        {
            if (AfterTune != null) AfterTune();
        }

        public Resizer(Control base_ctrl)
            : this(base_ctrl, TuneType.Size)
        { }

        public Resizer(Control base_ctrl, TuneType type)
        {
            norm_ctrl_ = base_ctrl;
            tune_type_ = type;

            //init
            //AddInfo(norm_ctrl_);

            //default resize handler
            resize_handler_ += new EventHandler(delegate(object sender, EventArgs e){
                Tune();
                OnAfterTune();
            });
        }

        private void AddInfo(Control ctrl)
        {
            rz_info_[ctrl] = new ResizeInfo(
                ctrl,
                norm_ctrl_,
             new RSize(ctrl.Width, ctrl.Height),
              new RPoint(ctrl.Location.X, ctrl.Location.Y),
                ResizerType.None);
        }

        private void SetInfo(Control ctrl)
        {
            rz_info_[ctrl].Width = ctrl.Width;
            rz_info_[ctrl].Height = ctrl.Height;
            rz_info_[ctrl].X  = ctrl.Location.X;
            rz_info_[ctrl].Y = ctrl.Location.Y;
        }


        public void Add(Control ctrl)
        {
            Add(ctrl, ResizerType.None, 1.0);
        }

        public void Add(Control ctrl, ResizerType type)
        {
            Add(ctrl, type, 1.0);
        }

        public void Add(Control ctrl, ResizerType type, double scale)
        {
            if (!Contains(ctrl))
            {
                AddInfo(ctrl);
            }

            rz_info_[ctrl].Type |= type;

            if ((type & ResizerType.Width) == ResizerType.Width)
                rz_info_[ctrl].WidthScale = scale;

            if ((type & ResizerType.Height) == ResizerType.Height)
                rz_info_[ctrl].HeightScale = scale;

            if ((type & ResizerType.X) == ResizerType.X)
                rz_info_[ctrl].XScale = scale;

            if ((type & ResizerType.Y) == ResizerType.Y)
                rz_info_[ctrl].YScale = scale;

        }

        public int Count
        {
            get { return rz_info_.Count; }
        }


        public bool Contains(Control crtl)
        {
            return rz_info_.ContainsKey(crtl);
        }

        /*
        private void ContainsCheck(Control ctrl)
        {
            if (!Contains(ctrl))
                throw new ArgumentException("The Control '" + ctrl.Name + "' does not exist.");
        }
        */
        protected void Tune()
        {
            //get Tuner
            Tuner tuner =
                (tune_type_ == TuneType.Size) ? new Tuner(new SizeNormist(norm_ctrl_.Size)) :
                (tune_type_ == TuneType.Location) ? new Tuner(new LocationNormist(norm_ctrl_.Location)) :
                null;

            if (tuner == null)
                return;

            //tune
            foreach (KeyValuePair<Control, ResizeInfo> kvp in rz_info_)
                tuner.Tune(kvp.Value);
        }

        /// <summary>
        /// Tune Control by Custom Norm, the function is used for pre-tune a control before Resize Eevent
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="norm"></param>
        protected void TuneControl(Control ctrl, Size norm)
        {
            ResizeInfo info = rz_info_[ctrl];
            (new Tuner(new SizeNormist(norm))).Tune(info);
        }
    }

    public class Tuner
    {
        private Normist norm_;

        public Tuner(Normist norm){
            this.norm_ = norm;
        }

        public void Tune(ResizeInfo info)
        {
            //tune size
            if ((info.Type & ResizerType.Size) == ResizerType.Size)
            {
                TuneSize(info);
            }
            else if ((info.Type & ResizerType.Width) == ResizerType.Width)
            {
                TuneWidth(info);
            }
            else if ((info.Type & ResizerType.Height) == ResizerType.Height)
            {
                TuneHeight(info);
            }

            //tune location
            if ((info.Type & ResizerType.Location) == ResizerType.Location)
            {
                TuneLocation(info);
            }
            else if ((info.Type & ResizerType.X) == ResizerType.X)
            {
                TuneX(info);
            }
            else if ((info.Type & ResizerType.Y) == ResizerType.Y)
            {
                TuneY(info);
            }
        }

        #region Tune<XXX>: Set <XXX> of Control

        private void TuneWidth(ResizeInfo info)
        {
            info.Control.Width = GetTuneWidth(info);
        }

        private void TuneHeight(ResizeInfo info)
        {
            info.Control.Height = GetTuneHeight(info);
        }

        private void TuneSize(ResizeInfo info)
        {
            info.Control.Size = new System.Drawing.Size(
                GetTuneWidth(info),
                GetTuneHeight(info));
        }


        private void TuneX(ResizeInfo info)
        {
            info.Control.Location = new System.Drawing.Point(
                GetTuneX(info),
                info.Control.Location.Y);
        }


        private void TuneY(ResizeInfo info)
        {
            info.Control.Location = new System.Drawing.Point(
                info.Control.Location.X,
                GetTuneY(info));
        }

        private void TuneLocation(ResizeInfo info)
        {
            info.Control.Location = new System.Drawing.Point(
                GetTuneX(info),
                GetTuneY(info));
        }

#endregion

        #region GetTune<XXX>: Get diff between <XXX>

        private int GetTuneWidth(ResizeInfo info) {
            return info.Width + (int)(norm_.GetDiffX(info) * info.WidthScale); }

        private int GetTuneHeight(ResizeInfo info) {
            return info.Height + (int)(norm_.GetDiffY(info) * info.HeightScale);}


        private int GetTuneX(ResizeInfo info) {
            return info.X + (int)(norm_.GetDiffX(info) * info.XScale); }

        private int GetTuneY(ResizeInfo info) {
            return info.Y + (int)(norm_.GetDiffY(info) * info.YScale); }

        #endregion GetTune<XXX>: Get diff between <XXX>
    }

    public interface Normist
    {
        int GetDiffX(ResizeInfo info);
        int GetDiffY(ResizeInfo info);
    }

    public class SizeNormist : Normist
    {
        Size norm_;

        public SizeNormist(Size sz)
        {
            norm_ = sz;
        }

        public int GetDiffX(ResizeInfo info)
        {
            return norm_.Width - info.NormWidth;
        }

        public int GetDiffY(ResizeInfo info)
        {
            return norm_.Height - info.NormHeight;
        }
    }

    public class LocationNormist : Normist
    {
        Point norm_;

        public LocationNormist(Point loc)
        {
            norm_ = loc;
        }

        public int GetDiffX(ResizeInfo info)
        {
            return norm_.X - info.NormX;
        }

        public int GetDiffY(ResizeInfo info)
        {
            return norm_.Y - info.NormY;
        }
    }


    public class ResizeInfo
    {
        public Control Control { get; private set; }
        //public System.Drawing.Size NormSize { get; set; }
        //public System.Drawing.Point NormLocation { get; set; }
        public int NormWidth { get; set; }
        public int NormHeight { get; set; }
        public int NormX { get; set; }
        public int NormY { get; set; }

        public ResizerType Type { get; set; }
        public RSize Size { get; set; }
        public RPoint Point { get; set; }

        public int Width { get { return Size.Width; } set { Size.Width = value; } }
        public int Height { get { return Size.Height; } set { Size.Height = value; } }
        public double WidthScale { get { return Size.WidthScale; } set { Size.WidthScale = value; } }
        public double HeightScale { get { return Size.HeightScale; } set { Size.HeightScale = value; } }

        public int X { get { return Point.X; } set { Point.X = value; } }
        public int Y { get { return Point.Y; } set { Point.Y = value; } }
        public double XScale { get { return Point.XScale; } set { Point.XScale = value; } }
        public double YScale { get { return Point.YScale; } set { Point.YScale = value; } }

        public ResizeInfo(Control ctrl, Control norm, RSize sz, RPoint pt, ResizerType type)
        {
            Control = ctrl;
            NormWidth = norm.Width;
            NormHeight = norm.Height;
            NormX = norm.Location.X;
            NormY = norm.Location.Y;

            Size = sz;
            Point = pt;
            Type = type;
        }
    }

    public class RSize
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public double WidthScale { get; set; }
        public double HeightScale { get; set; }

        public RSize(int width, int height) : this(width, height, 1.0, 1.0) { }

        public RSize(int width, int height, double width_scale, double height_scale)
        {
            Width = width;
            Height = height;
            WidthScale = width_scale;
            HeightScale = height_scale;
        }
    }

    public class RPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double XScale { get; set; }
        public double YScale { get; set; }

        public RPoint(int x, int y) : this(x, y, 1.0, 1.0) { }

        public RPoint(int x, int y, double x_scale, double y_scale)
        {
            X = x;
            Y = y;
            XScale = x_scale;
            YScale = y_scale;
        }
    }
}

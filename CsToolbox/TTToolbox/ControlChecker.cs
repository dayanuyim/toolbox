using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TTToolbox
{
    //return: null is ok, otherwise error message
    public delegate string ValueChecker(string val);
    public delegate string ValuesChecker(string[] vals);
    public delegate void MessageHandler(string msg);

    /*
     * 1. Keep ckeck result (Get result by Valid Property)
     * 2. Check through handler mechanism, unless ckeck is never invoked (saving unnecessary check)
     * 3. Pop up message box near control, or customed handler 
     */
    public class ControlChecker
    {
        private enum ValidStatus{ Indetermin, True, False};

        private Control ctrl_;
        private ValueChecker checker_;
        private EventHandler handler_;
        private MessageHandler msg_handler_ = null;

        private ValidStatus valid_ = ValidStatus.Indetermin;

        public EventHandler Handler
        {
            get { return handler_; }
        }

        public bool Valid
        {
            get
            {
                if (valid_ == ValidStatus.Indetermin)
                    Check();
                return (valid_ == ValidStatus.True);
            }
        }

        public Control Control
        {
            get { return ctrl_; }
        }

        public string Value
        {
            get{ return ctrl_.Text;}
        }


        public ControlChecker(Control ctrl, ValueChecker checker) : this(ctrl, checker, null) { }

        public ControlChecker(Control ctrl, ValueChecker checker, MessageHandler msg_handler)
        {
            if(ctrl == null) throw new ArgumentNullException("Control can't be null");
            if(checker == null) throw new ArgumentNullException("ValueChecker can't be null");

            ctrl_ = ctrl;
            checker_ = checker;
            msg_handler_ = msg_handler;

            handler_ = new EventHandler(
                delegate(object sender, EventArgs e)
                {
                    Check();
                });
        }


        public void Check()
        {
            string msg = GetCheckMessage();

            valid_ = (msg == null) ? ValidStatus.True : ValidStatus.False;

            if (msg != null)
            {
                if (msg_handler_ != null)
                    msg_handler_(msg);
                else
                    FormUtil.ControlMessage(ctrl_, msg);
            }
        }

        private string GetCheckMessage()
        {
            try
            {
                return checker_(ctrl_.Text);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}

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
    public enum FieldType{Info, Text, Passwd, Combo, CheckBox, Line, Padding}

    public partial class FieldsDialog : Form
    {
        private Label label_info_;
        private List<Field> fields_ = new List<Field>();
            private KeyEventHandler key_handler_ = null;


        private Resizer rz_ = null;

        public event ValuesChecker ValuesCheck;

        #region Properties

        public string this[string name]
        {
            get
            {
                foreach (Field field in fields_)
                    if (field.Name == name)
                        return field.Value;

                throw new ArgumentException("No field name is " + name);
            }
        }

        public string Info
        {
            get{ return label_info_.Visible? label_info_.Text: string.Empty;}
            set{ label_info_.Text = value; label_info_.Visible = true;}
        }

    #endregion

        public FieldsDialog()
        {
            InitializeComponent();
            InitailizeDynamicCompnent();
        }

        private void InitailizeDynamicCompnent()
        {
            label_info_ = new Label();
            label_info_.AutoSize = true;
            label_info_.ForeColor = Color.Red;
            //label_info.Font = Regular10;
            //label_info.Text = (type_ == FieldType.CheckBox) ? "" : Name;

            key_handler_ = FormUtil.KeyDownHandler(button_ok, button_cancel);
            this.KeyDown += key_handler_;
            button_ok.KeyDown += key_handler_;
            button_cancel.KeyDown += key_handler_;
        }

        private void ModifyDialog_Load(object sender, EventArgs e)
        {
            InitResize();
            InitFields();
        }

        private void InitResize()
        {
            WindowState = FormWindowState.Normal;
            rz_ = new Resizer(grpbox);
            rz_.Add(this, ResizerType.Size);
            rz_.Add(button_ok, ResizerType.Location);
            rz_.Add(button_cancel, ResizerType.Location);
            rz_.Enabled = true;
        }

        private void InitFields()
        {
            const int y_base = 30;
            const int x_padding = 5, y_padding = 10;
            int x_loc_info = Field.TitleWidth;
            int y_loc_info = 10;

            this.SuspendLayout();

            //info
            grpbox.Controls.Add(label_info_);
            label_info_.Location = new Point(x_loc_info, y_loc_info);


            //fields
            int fields_height = y_base;
            for (int i = 0; i < fields_.Count; ++i)  
            {
                Field fd = fields_[i];
                fd.Location = new Point(x_padding, fields_height);
                fd.KeyDown += key_handler_;
                fd.TabIndex = i;
                grpbox.Controls.Add(fd);
                fields_height += fd.Height;
            }

            //size
            grpbox.Height = fields_height + y_padding;
            grpbox.Width = Field.FWidth + 2 * x_padding;

            this.Controls.Add(grpbox);

            this.ResumeLayout();
            this.PerformLayout();
        }

        public new void Show() {
            this.ShowDialog();
        }

        #region Add Field Methods

        public Field Add(string name, FieldType type)
        {
            Field f = new Field(name, type);
            fields_.Add(f);
            return f;
        }

        public Field Add(string name, FieldType type, string[] items)
        {
            Field f = new Field(name, type, items);
            fields_.Add(f);
            return f;
        }

        /*
        public void Add(string name, FieldType rz_type_, string[] items, int sel_idx)
        {
            Field f = new Field(name, rz_type_, items);
            f.SelectedIndex = sel_idx;
            fields_.Add(f);
        }
         */

        public Field Add(string name, FieldType type, string item)
        {
            Field f = new Field(name, type, item);
            fields_.Add(f);
            return f;
        }

        public Field Add(string name, FieldType type, ValueChecker valid)
        {
            Field f = new Field(name, type, valid);
            fields_.Add(f);
            return f;
        }

        public Field Add(string name, FieldType type, string item, ValueChecker valid)
        {
            Field f = new Field(name, type, item, valid);
            fields_.Add(f);
            return f;
        }

        /*
        public void Add(string name, FieldType rz_type_, string[] items, int selected_idx, ValueChecker valid)
        {
            Field f = new Field(name, rz_type_, items, valid);
            f.SelectedIndex = selected_idx;
            fields_.Add(f);
        }
         */

        public Field Add(string name, FieldType type, string[] items, ValueChecker valid)
        {
            Field f = new Field(name, type, items, valid);
            fields_.Add(f);
            return f;
        }

        #endregion

        #region Event

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            try
            {
                if (Valid()){
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Validation error: " + ex.Message);
            }
        }



        #endregion

        #region validation

        public bool Valid()
        {
            bool valid = true;

            try
            {
                //each field
                foreach (Field field in fields_)    //trigger check of each field
                    if (!field.Valid)
                        valid = false;

                //combine
                List<string> vals = new List<string>();
                foreach (Field field in fields_)
                    vals.Add(field.Value);

                string msg = OnValuesCheck(vals.ToArray());
                if (msg != null)
                {
                    valid = false;
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //status_msg.Text = msg;
                }
                //status_msg.Visible = (msg != null);
            }
            catch (Exception ex)
            {
                valid = false;
                FormUtil.ShowErrMsg("validation check error", ex);
            }

            return valid;
        }

        private string OnValuesCheck(string[] vals)
        {
            if(ValuesCheck == null)
                return null;

            return ValuesCheck(vals);
        }

        #endregion


    }

    public class Field: Panel
    {
        public static int FWidth{ get{ return 300;}}
        public static int FHeight{ get{ return 50;}}
        public static int TitleWidth{ get{ return 120;}}
        public static int TitleHeight{ get{ return 20;}}
        public static int ValueWidth{ get{ return 150;}}
        public static int ValueHeight{ get{ return 20;}}
        
        public static Font Regular10 = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
        public static Font ErrFont =  new Font("Microsoft Sans Serif", 8.25F, ((FontStyle)((FontStyle.Bold | FontStyle.Italic))), GraphicsUnit.Point, ((byte)(136)));

        //private static Font passwd_normal_font;
        //private static Font passwd_italic_font;
        public static string DefaultValueChecker(string name)
        {

            return (name != string.Empty)? null: "The field can't be empty";
        }

        private bool is_checked_ = false;
        private bool valid_; 

        private Label title_;
        private Label err_msg_;
        private ValueChecker checker_;
        private FieldType type_;
        private Control field_ = null;
        private string[] items_;

#region Properties


        public string ErrorMessage
        {
            get{ return err_msg_.Text;}
        }

        public FieldType Type
        {
            get{ return type_; }
        }

        //no exception is required, set error message to err_msg_ label
        public bool Valid
        {
            get{
                if (!is_checked_) CheckValid();
                return valid_;
            }
        }

        public string Value
        {
            get{
                return (type_ == FieldType.CheckBox)?
                    ((CheckBox)field_).Checked.ToString(): field_.Text;
            }
        }

        public new event KeyEventHandler KeyDown
        {
            add{ field_.KeyDown += value;}
            remove{ field_.KeyDown -= value;}
        }

        public int SelectedIndex
        {
            get{
                return (type_ == FieldType.Combo)? ((ComboBox)field_).SelectedIndex: -1;
            }

            set{
                if(type_ == FieldType.Combo && value >= 0 && value < items_.Length){
                    ((ComboBox)field_).SelectedIndex = value;
                }
            }
        }


#endregion


        #region ctor

        //nu-combo rz_type_ & default check
        public Field(string name, FieldType type)
        {
            Init(name, type, null, DefaultValueChecker);
        }

        //nu-combo rz_type_ & customed check
        public Field(string name, FieldType type, ValueChecker valid)
        {
            Init(name, type, null, valid);
        }

        //combo rz_type_ & default check
        public Field(string name, FieldType type, String[]items)
        {
            Init(name, type, items, DefaultValueChecker);
        }
        public Field(string name, FieldType type, String item)
        {
            string[] items = (item != null) ? new string[] { item } : null;
            Init(name, type, items, DefaultValueChecker);
        }

        //general (combo rz_type_ & customed check)
        public Field(string name, FieldType type, String[]items, ValueChecker checker)
        {
            Init(name, type, items, checker);
        }
        public Field(string name, FieldType type, String item, ValueChecker checker)
        {
            string[] items = (item != null) ? new string[] { item } : null;
            Init(name, type, items, checker);
        }

        #endregion


        // ctor init ----------------------------
        private void Init(string name, FieldType type, String[]items, ValueChecker checker)
        {
            if(checker == null)
                throw new ArgumentNullException("ValueValid can't be null");

            if (type == FieldType.Combo && (items == null || items.Length <= 0))
                throw new ArgumentException("Bad Field Constructor argument");

            checker_ = checker;
            type_ = type;
            items_ = items;
            
            InitPanel(name, type);
        }

        #region Component Create

        //Controls
        private Label CreateTitleLabel()
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = Regular10;
            label.Text = (type_ == FieldType.CheckBox)? "": Name;
            label.Location = new Point(TitleWidth - label.PreferredWidth - 5, 0);
            return label;
        }
        private Label CreateErrLabel()
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = ErrFont;
            label.ForeColor = Color.Red;
            label.Text = "Invalid " + Name.ToLower();
            //label.Size = new Size(FIELD_WIDTH, FIELD_HEIGHT);
            label.Location = new Point(TitleWidth, TitleHeight);
            label.Visible = false;

            return label;
        }

        private Control GetUnderlyField()
        {
            string def_val = (items_ != null && items_.Length > 0 && items_[0] != null) ? items_[0] : "";

            switch(type_)
            {
                case FieldType.Text:
                    TextBox txt = new TextBox();
                    txt.Text = def_val;
                    return txt;

                case FieldType.Passwd:
                    TextBox pass = new TextBox();
                    pass.PasswordChar = '*';
                    pass.Text = def_val;
                    return pass;

                case FieldType.Combo:
                    ComboBox combo = new ComboBox();
                    combo.Items.AddRange(items_);
                    //combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    combo.AutoCompleteSource = AutoCompleteSource.ListItems;
                    combo.SelectedIndex = 0;
                    return combo;

                case FieldType.CheckBox:
                    CheckBox ckbox = new CheckBox();
                    ckbox.AutoSize = true;
                    ckbox.UseVisualStyleBackColor = true;
                    ckbox.Checked = Convert.ToBoolean(def_val);
                    ckbox.Text = Name;
                    ckbox.Font = Regular10;

                    return ckbox;

                default:
                    return null;
            }
        }

        private Control CreateField()
        {
            EventHandler handler = new EventHandler(
                 delegate(object sender, EventArgs e){
                     CheckValid();
                 });

            Control ctrl = GetUnderlyField();
            ctrl.Size = new Size(ValueWidth, ValueHeight) ;
            ctrl.Location = new Point(TitleWidth, 0);
            ctrl.Leave += handler;


            return ctrl;
        }

        private void InitPanel(string name, FieldType type)
        {
            this.SuspendLayout();
            this.Location = new Point(0, 0);
            this.Size = new Size(FWidth, FHeight);
            this.Name = name;

            //component
            title_ = CreateTitleLabel();
            field_ = CreateField();
            err_msg_ = CreateErrLabel();

            this.Controls.Add(title_);
            this.Controls.Add(field_);
            this.Controls.Add(err_msg_);

            this.ResumeLayout();
            this.PerformLayout();
            /*
            rz_ = new Resizer(name_);
            rz_.Add(field_, ResizerType.Location);
            rz_.Add(err_msg_, ResizerType.Location);
            rz_.Enabled = true;
             */
        }

        #endregion

        private void CheckValid()
        {
            valid_ = false;

            try{
                string msg = checker_(Value);
                if (msg == null)
                    valid_ = true;
                else
                    err_msg_.Text = msg;
            }
            catch(Exception ex)
            {
                err_msg_.Text = "Invalid: " + ex.Message;
            }

            err_msg_.Visible = !valid_;
            is_checked_ = true;
        }
                
        
    }

}


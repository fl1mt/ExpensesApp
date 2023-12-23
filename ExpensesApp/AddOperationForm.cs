using System;
using System.Linq;
using System.Windows.Forms;
namespace ExpensesApp
{
    public partial class AddOperationForm : Form
    {
        DateTime selectedDate;
        ExpenseTrackerDatabase database;
        Form1 formMain;
        public AddOperationForm(ExpenseTrackerDatabase db, Form1 form)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.database = db;
            this.formMain = form;
        }

        private void buttonAddOpeartion_Click(object sender, EventArgs e)
        {
            if (selectedDate != null && !string.IsNullOrEmpty(textBoxNameOp.Text) && !string.IsNullOrEmpty(textBoxCommentOp.Text) 
                && !string.IsNullOrEmpty(textBoxSumOp.Text) && !string.IsNullOrEmpty(comboBox1.Text))
            {
                if (!textBoxSumOp.Text.Any(char.IsLetter) && !textBoxSumOp.Text.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
                {
                    string name = textBoxNameOp.Text;
                    double sum = Convert.ToDouble(textBoxSumOp.Text);
                    string category = comboBox1.Text;
                    string comment = textBoxCommentOp.Text;
                    database.InsertExpense(name, sum, category, selectedDate, comment);
                    MessageBox.Show("Успешно");
                    formMain.CountExpenses();
                    this.Close();
                } else
                {
                    MessageBox.Show("Введите корректную сумму!");
                }
            }
            else
            {
                MessageBox.Show("Заполните все данные!");
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            selectedDate = dateTimePicker1.Value;
        }
    }
}

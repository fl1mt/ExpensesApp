using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpensesApp
{
    public partial class Form1 : Form
    {
        ExpenseTrackerDatabase db;
        public Form1()
        {
            InitializeComponent();
            string projectPath = "E:\\E\\apps\\ExpensesApp\\ExpensesApp\\ExpensesDatabase.db";
            db = new ExpenseTrackerDatabase(projectPath);
            db.CreateExpenseTable();
            CountExpenses();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddOperationForm add = new AddOperationForm(db, this);
            add.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SearchOperationsForm search = new SearchOperationsForm(db);
            search.Show();
        }

        public void CountExpenses()
        {
            flowLayoutPanel1.Controls.Clear();
            label2.Text = db.CalculateExpensesLast30Days().ToString();
            List<string> lastNExpenses = db.GetLastNExpenses(7);

            foreach (string expense in lastNExpenses)
            {
                Debug.WriteLine(expense);
                Label label = new Label
                {
                    AutoSize = true,
                    ForeColor = Color.Green,
                    Font = label3.Font,
                    Text = expense
                };
                flowLayoutPanel1.Controls.Add(label);
            }
        }
    }
}

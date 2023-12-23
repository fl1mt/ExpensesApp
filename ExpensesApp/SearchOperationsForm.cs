using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpensesApp
{
    public partial class SearchOperationsForm : Form
    {
        ExpenseTrackerDatabase expenseTrackerDatabase;
        public SearchOperationsForm(ExpenseTrackerDatabase db)
        {
            InitializeComponent();
            LoadData();
            this.expenseTrackerDatabase = db;
        }

        private void LoadData()
        {
            //Создаем datatable с названиями столбцов
           DataTable dataTable = new DataTable("Расходы");
            dataTable.Columns.Add("Название", typeof(string));
            dataTable.Columns.Add("Сумма", typeof(string));
            dataTable.Columns.Add("Категория", typeof(string));
            dataTable.Columns.Add("Дата", typeof(string));
            dataTable.Columns.Add("Комментарий", typeof(string));
            dataGridView1.DataSource = dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = filterName.Text;
            DateTime startDate = filterDateStart.Value;
            DateTime endDate= filterDateEnd.Value;

            double max = 0;
            double min = 0;

            if (!string.IsNullOrWhiteSpace(filterMax.Text))
            {
                max = Convert.ToDouble(filterMax.Text);
            }
            if (!string.IsNullOrWhiteSpace(filterMin.Text))
            {
                min = Convert.ToDouble(filterMin.Text);
            }
            dataGridView1.DataSource = expenseTrackerDatabase.GetDataGridWithFilters(name, min, max, startDate,
                endDate);
        }
    }
}

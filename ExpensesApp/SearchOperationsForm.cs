using System;
using System.Data;
using System.Linq;
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
            if (!string.IsNullOrWhiteSpace(filterMax.Text) && !filterMax.Text.Any(char.IsLetter) && 
                !filterMax.Text.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
            {
                max = Convert.ToDouble(filterMax.Text);
            }
            if (!string.IsNullOrWhiteSpace(filterMin.Text) && !filterMin.Text.Any(char.IsLetter) && 
                !filterMin.Text.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
            {
                min = Convert.ToDouble(filterMin.Text);
            }
            dataGridView1.DataSource = expenseTrackerDatabase.GetDataTableWithFilters(name, min, max, startDate,
                endDate);
        }
    }
}

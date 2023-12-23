using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Forms;

public class ExpenseTrackerDatabase
{
    private string connectionString;

    public ExpenseTrackerDatabase(string dbFilePath)
    {
        // Путь к файлу базы данных SQLite  
        connectionString = $"Data Source={dbFilePath};Version=3;";
    }

    public void CreateExpenseTable()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Запрос на создание таблицы расходов
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Category TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    Comment TEXT
                );";
            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertExpense(string name, double amount, string category, DateTime date, string comment)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Запрос на вставку новой записи
            string insertQuery = @"
            INSERT INTO Expenses (Name, Amount, Category, Date, Comment)
            VALUES (@Name, @Amount, @Category, @Date, @Comment);";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                // Параметры
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Date", date.ToString("dd-MM-yyyy HH:mm:ss")); // Формат даты
                command.Parameters.AddWithValue("@Comment", comment);
                // Выполняем
                // коммит 123
                command.ExecuteNonQuery();
            }
        }
    }

    public DataTable GetDataGridWithFilters(string nameFilter, double? minAmountFilter, double? maxAmountFilter, DateTime? startDateFilter, DateTime? endDateFilter)
    {
        DataTable dataTable = new DataTable();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // SQL-запрос на выбор всех столбцов и строк из таблицы Expenses
            string selectAllQuery = "SELECT * FROM Expenses WHERE";

            // Добавление условий фильтрации в SQL-запрос
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                selectAllQuery += " Name = @nameFilter AND";
            }

            if (minAmountFilter.HasValue && minAmountFilter != 0)
            {
                selectAllQuery += " Amount >= @minAmountFilter AND";
            }

            if (maxAmountFilter.HasValue && maxAmountFilter != 0)
            {
                selectAllQuery += " Amount <= @maxAmountFilter AND";
            }

            selectAllQuery += " Date BETWEEN @startDateFilter AND @endDateFilter";

            // Удаление последнего "AND" из SQL-запроса, если он есть
            selectAllQuery = selectAllQuery.TrimEnd(' ', 'A', 'N', 'D');

            // столбцы
            dataTable.Columns.Add("Название", typeof(string));
            dataTable.Columns.Add("Сумма", typeof(string));
            dataTable.Columns.Add("Категория", typeof(string));
            dataTable.Columns.Add("Дата", typeof(string));
            dataTable.Columns.Add("Комментарий", typeof(string));

            Debug.WriteLine(selectAllQuery);

            using (SQLiteCommand command = new SQLiteCommand(selectAllQuery, connection))
            {
                // Добавление параметров к запросу в соответствии с условиями фильтрации
                if (!string.IsNullOrWhiteSpace(nameFilter))
                {
                    command.Parameters.AddWithValue("@nameFilter", nameFilter);
                }
                if (minAmountFilter.HasValue)
                {
                    command.Parameters.AddWithValue("@minAmountFilter", minAmountFilter.Value);
                }
                if(maxAmountFilter.HasValue && maxAmountFilter != 0)
                {
                    command.Parameters.AddWithValue("@maxAmountFilter", maxAmountFilter.Value);
                }
                
                command.Parameters.AddWithValue("@startDateFilter", startDateFilter.Value.ToString(("dd-MM-yyyy HH:mm:ss")));
                command.Parameters.AddWithValue("@endDateFilter", endDateFilter.Value.ToString(("dd-MM-yyyy HH:mm:ss")));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // Читаем запрос, выводим данные в дату таблицы
                    while (reader.Read())
                    {
                        dataTable.Rows.Add(
                            reader["Name"].ToString(),
                            reader["Amount"].ToString(),
                            reader["Category"].ToString(),
                            reader["Date"].ToString(),
                            reader["Comment"].ToString()
                        );
                    }
                }
            }
        }
        return dataTable;
    }

    public double CalculateExpensesLast30Days()
    {
        double totalExpenses = 0;

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Expenses";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime local = Convert.ToDateTime(reader["Date"]);
                        DateTime past = local.AddDays(30);

                        DateTime now = DateTime.Now;
                        // Формируем строку и добавляем в список
                        if (now > local && now < past)
                        {
                            double expenseString = Convert.ToDouble(reader["Amount"]);
                            totalExpenses += expenseString;
                        }
                    }
                }
            }

            connection.Close();
        }
        return totalExpenses;
    }

    public List<string> GetLastNExpenses(int n)
    {
        List<string> expensesList = new List<string>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // SQL-запрос для выбора последних N записей
            string query = "SELECT Name, Amount FROM Expenses ORDER BY Id DESC LIMIT @N;";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@N", n);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Формируем строку и добавляем в список
                        string expenseString = $"{reader["Name"]} - {reader["Amount"]} руб.";
                        expensesList.Add(expenseString);
                    }
                }
            }

            connection.Close();
        }

        return expensesList;
    }

}

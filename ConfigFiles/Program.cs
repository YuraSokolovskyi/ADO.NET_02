using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ConfigFiles
{
    class Program
    {
        private static string ConString => ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        
        private static void printHeader()
        {
            Console.WriteLine("View table: W\n" +
                              "View types: T\n" +
                              "View senders: S\n" +
                              "View goods with max quantity: M\n" +
                              "View goods with min quantity: L\n" +
                              "View goods with max cost: C\n" +
                              "View goods with min cost: F\n" +
                              "Add data: A\n" +
                              "Remove data: R\n" +
                              "Update data: U\n" +
                              "View sender with max amount of goods: 1\n" +
                              "View sender with min amount of goods: 2\n" +
                              "View type with max amount of goods(quantity): 3\n" +
                              "View type with min amount of goods(quantity): 4\n" +
                              "View goods that were added N day ago: 5\n"
            );
        }
        
        private static void printTable(SqlDataAdapter adapter, List<string> columns, DataSet dataSet)
        {
            dataSet.Clear();
            adapter.Fill(dataSet);
            
            foreach (string dataColumn in columns)
            {
                Console.Write("{0,-10} ",  dataColumn);
            }
            Console.WriteLine("\n");
                            
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                foreach (string column in columns)
                {
                    Console.Write("{0,-10} ",  dataRow[column].ToString().Trim());
                }
                Console.WriteLine();
            }
        }

        private static void addData(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Add new product: 1\n" +
                              "Add new type: 2\n" +
                              "Add new sender: 3\n");
            ConsoleKey key = Console.ReadKey().Key;
            string input = "";
            string name = "";
            int typeId = 0;
            int senderId = 0;
            int quantity = 0;
            double cost = 0;
            string date_in = "";
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            DataSet dataSet = new DataSet();
            switch (key)
            {
                case ConsoleKey.D1:
                    // name
                    Console.Clear();
                    Console.Write("Enter name: ");
                    name = Console.ReadLine();
                    // type
                    Console.Clear();
                    printTable(new SqlDataAdapter("SELECT id, type FROM Type", connection), 
                        new List<string>(){"id", "type"}, dataSet);
                    Console.Write("\nEnter id: ");
                    typeId = Int32.Parse(Console.ReadLine());
                    // sender
                    Console.Clear();
                    printTable(new SqlDataAdapter("SELECT id, sender FROM Sender", connection), 
                        new List<string>(){"id", "sender"}, dataSet);
                    Console.Write("\nEnter id: ");
                    senderId = Int32.Parse(Console.ReadLine());
                    // quantity
                    Console.Clear();
                    Console.Write("Enter quantity: ");
                    quantity = Int32.Parse(Console.ReadLine());
                    // cost
                    Console.Clear();
                    Console.Write("Enter cost: ");
                    cost = double.Parse(Console.ReadLine());
                    // date
                    Console.Clear();
                    Console.Write("Enter date(yyyy-mm-dd): ");
                    date_in = Console.ReadLine();
                    
                    // adding
                    command.CommandText = $"INSERT INTO Goods (name, type, sender, quantity, cost, date_in) " +
                                          $"VALUES ('{name}', {typeId}, {senderId}, {quantity}, {cost}, '{date_in}')";
                    command.ExecuteNonQuery();
                    break;
                case ConsoleKey.D2:
                    Console.Clear();    
                    Console.Write("Enter new type: ");
                    input = Console.ReadLine();
                    command.CommandText = $"INSERT INTO Type (type) VALUES ('{input}')";
                    command.ExecuteNonQuery();
                    break;
                case ConsoleKey.D3:
                    Console.Clear();    
                    Console.Write("Enter new sender: ");
                    input = Console.ReadLine();
                    command.CommandText = $"INSERT INTO Sender (sender) VALUES ('{input}')";
                    command.ExecuteNonQuery();
                    break;
            }
        }

        private static void removeData(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Remove product: 1\n" +
                              "Remove type: 2\n" +
                              "Remove sender: 3");
            ConsoleKey key = Console.ReadKey().Key;
            Console.Clear();
            DataSet dataSet = new DataSet();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            List<string> columns = new List<string>();
            int inputId = 0;

            switch (key)
            {
                case ConsoleKey.D1:
                    columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                    printTable(new SqlDataAdapter("SELECT Goods.id, name, Type.type, Sender.sender, quantity, cost, date_in FROM Goods " +
                                                  "JOIN Sender on Goods.sender = Sender.id JOIN Type on Goods.type = Type.id", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    command.CommandText = $"DELETE FROM Goods WHERE id={inputId}";
                    command.ExecuteNonQuery();
                    break;
                case ConsoleKey.D2:
                    columns = new List<string>() { "id", "type" };
                    printTable(new SqlDataAdapter("SELECT id, type FROM Type", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    command.CommandText = $"DELETE FROM Type WHERE id={inputId}";
                    command.ExecuteNonQuery();
                    break;
                case ConsoleKey.D3:
                    columns = new List<string>() { "id", "sender" };
                    printTable(new SqlDataAdapter("SELECT id, sender FROM Sender", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    command.CommandText = $"DELETE FROM Sender WHERE id={inputId}";
                    command.ExecuteNonQuery();
                    break;
            }
        }

        private static void updateData(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Update product: 1\n" +
                              "Update type: 2\n" +
                              "Update sender: 3");
            ConsoleKey key = Console.ReadKey().Key;
            Console.Clear();
            DataSet dataSet = new DataSet();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            List<string> columns = new List<string>();
            int inputId = 0;
            string input = "";
            int inputInt = 0;
            double inputDouble = 0;

            switch (key)
            {
                case ConsoleKey.D1:
                    columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                    printTable(new SqlDataAdapter("SELECT Goods.id, name, Type.type, Sender.sender, quantity, cost, date_in FROM Goods " +
                                                  "JOIN Sender on Goods.sender = Sender.id JOIN Type on Goods.type = Type.id", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    Console.Clear();
                    Console.WriteLine("Update name: 1\n" +
                                      "Update type: 2\n" +
                                      "Update sender: 3\n" +
                                      "Update quantity: 4\n" +
                                      "Update cost: 5\n" +
                                      "Update date_in: 6");
                    key = Console.ReadKey().Key;
                    Console.Clear();
                    switch (key)
                    {
                        case ConsoleKey.D1:
                            Console.Write("Enter new name: ");
                            input = Console.ReadLine();
                            command.CommandText = $"UPDATE Goods SET name='{input}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                        case ConsoleKey.D2:
                            Console.Write("Enter new type: ");
                            inputInt = Int32.Parse(Console.ReadLine());
                            command.CommandText = $"UPDATE Goods SET type='{inputInt}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                        case ConsoleKey.D3:
                            Console.Write("Enter new sender: ");
                            inputInt = Int32.Parse(Console.ReadLine());
                            command.CommandText = $"UPDATE Goods SET sender='{inputInt}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                        case ConsoleKey.D4:
                            Console.Write("Enter new quantity: ");
                            inputInt = Int32.Parse(Console.ReadLine());
                            command.CommandText = $"UPDATE Goods SET quantity='{inputInt}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                        case ConsoleKey.D5:
                            Console.Write("Enter new cost: ");
                            inputDouble = Double.Parse(Console.ReadLine());
                            command.CommandText = $"UPDATE Goods SET cost='{inputDouble}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                        case ConsoleKey.D6:
                            Console.Write("Enter new date(yyyy-mm-dd): ");
                            input = Console.ReadLine();
                            command.CommandText = $"UPDATE Goods SET date_in='{input}' WHERE id={inputId}";
                            command.ExecuteNonQuery();
                            break;
                    }
                    
                    break;
                case ConsoleKey.D2:
                    columns = new List<string>() { "id", "type" };
                    printTable(new SqlDataAdapter("SELECT id, type FROM Type", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    Console.Write("Enter new type: ");
                    input = Console.ReadLine();
                    command.CommandText = $"UPDATE Type SET type='{input}' WHERE id={inputId}";
                    command.ExecuteNonQuery();
                    break;
                case ConsoleKey.D3:
                    columns = new List<string>() { "id", "sender" };
                    printTable(new SqlDataAdapter("SELECT id, sender FROM Sender", connection), columns, dataSet);
                    Console.Write("\nEnter id: ");
                    inputId = Int32.Parse(Console.ReadLine());
                    Console.Write("Enter new type: ");
                    input = Console.ReadLine();
                    command.CommandText = $"UPDATE Sender SET sender='{input}' WHERE id={inputId}";
                    command.ExecuteNonQuery();
                    break;
            }
        }
        
        static void Main(string[] args)
        {
            using (var connection = new SqlConnection(ConString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                DataSet dataSet = new DataSet();
                
                ConsoleKey key = ConsoleKey.NoName;
                List<string> columns = new List<string>();
                int input = 0;
                while (key != ConsoleKey.Q)
                {
                    printHeader();
                    
                    switch (key)
                    {
                        case ConsoleKey.W:
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter("SELECT Goods.id, name, Type.type, Sender.sender, quantity, cost, date_in FROM Goods " +
                                                          "JOIN Sender on Goods.sender = Sender.id JOIN Type on Goods.type = Type.id", connection), columns, dataSet);
                            break;
                        case ConsoleKey.T:
                            columns = new List<string>() { "id", "type" };
                            printTable(new SqlDataAdapter("SELECT id, type FROM Type", connection), columns, dataSet);
                            break;
                        case ConsoleKey.S:
                            columns = new List<string>() { "id", "sender" };
                            printTable(new SqlDataAdapter("SELECT id, sender FROM Sender", connection), columns, dataSet);
                            break;
                        case ConsoleKey.M:
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter("SELECT id, name, type, sender, quantity, cost, date_in FROM Goods " +
                                                          "WHERE (quantity=(SELECT MAX(quantity) FROM Goods))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.L:
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter("SELECT id, name, type, sender, quantity, cost, date_in FROM Goods " +
                                                          "WHERE (quantity=(SELECT MIN(quantity) FROM Goods))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.C:
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter("SELECT id, name, type, sender, quantity, cost, date_in FROM Goods " +
                                                          "WHERE (cost=(SELECT MAX(cost) FROM Goods))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.F:
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter("SELECT id, name, type, sender, quantity, cost, date_in FROM Goods " +
                                                          "WHERE (cost=(SELECT MIN(cost) FROM Goods))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.A:
                            addData(connection);
                            Console.Clear();
                            printHeader();
                            break;
                        case ConsoleKey.R:
                            removeData(connection);
                            Console.Clear();
                            printHeader();
                            break;
                        case ConsoleKey.U:
                            updateData(connection);
                            Console.Clear();
                            printHeader();
                            break;
                        case ConsoleKey.D1:
                            columns = new List<string>() { "id", "sender" };
                            printTable(new SqlDataAdapter("SELECT id, sender FROM Sender WHERE id=" +
                                                          "(SELECT TOP 1 sender FROM Goods GROUP BY sender ORDER BY COUNT(*) DESC)", connection), columns, dataSet);
                            break;
                        case ConsoleKey.D2:
                            columns = new List<string>() { "id", "sender" };
                            printTable(new SqlDataAdapter("SELECT id, sender FROM Sender WHERE id=" +
                                                          "(SELECT TOP 1 sender FROM Goods GROUP BY sender ORDER BY COUNT(*))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.D3:
                            columns = new List<string>() { "id", "type" };
                            printTable(new SqlDataAdapter("SELECT id, type FROM Type WHERE " +
                                                          "id=(SELECT TOP 1 type FROM Goods GROUP BY type ORDER BY SUM(quantity) DESC)", connection), columns, dataSet);
                            break;
                        case ConsoleKey.D4:
                            columns = new List<string>() { "id", "type" };
                            printTable(new SqlDataAdapter("SELECT id, type FROM Type WHERE " +
                                                          "id=(SELECT TOP 1 type FROM Goods GROUP BY type ORDER BY SUM(quantity))", connection), columns, dataSet);
                            break;
                        case ConsoleKey.D5:
                            Console.Write("Enter number of days: ");
                            input = Int32.Parse(Console.ReadLine());
                            Console.WriteLine();
                            columns = new List<string>() { "id", "name", "type", "sender", "quantity", "cost", "date_in" };
                            printTable(new SqlDataAdapter($"SELECT id, name, type, sender, quantity, cost, date_in FROM Goods WHERE date_in<(SELECT DATEADD(day, -{input}, GETDATE()))", connection), columns, dataSet);
                            break;
                    }
                    
                    key = Console.ReadKey().Key;
                    Console.Clear();
                }

                connection.Close();
                Console.WriteLine("Disconnected");
            }
        }
    }
}
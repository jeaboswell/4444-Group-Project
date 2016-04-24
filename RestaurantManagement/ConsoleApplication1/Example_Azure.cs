using System;  // C#
using D = System.Data;            // System.Data.dll
using C = System.Data.SqlClient;  // System.Data.dll


namespace ConnectAndQuery_Example
{
    class Program
    {
        static void Main()
        {
            string SQLConnectionString;

            // Get most of the connection string from ConnectAndQuery_Example.exe.config
            // file, in the same directory where ConnectAndQuery_Example.exe resides.


            SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";

            // Create an SqlConnection from the provided connection string.
            using (C.SqlConnection connection = new C.SqlConnection(SQLConnectionString))
            {
                // Formulate the command.
                C.SqlCommand command = new C.SqlCommand();
                command.Connection = connection;

                // Specify the query to be executed.
                command.CommandType = D.CommandType.Text;
                command.CommandText = @"
                    SELECT * FROM dbo.Menu
                    WHERE Available=1
                    ";
                // Open a connection to database.
                connection.Open();
                // Read data returned for the query.
                C.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int num = (int)reader[0]; // look you can even store the stuff by simply casting it to the correct data type
                    string name = (string)reader[1];
                    Console.WriteLine(num+ " " + name +" " + reader[2]  + " ayylmao");
                } // here where it says reader[0] this is printing the first column of that row, so just dereference it with whatever column you want
            }
            Console.WriteLine("View the results here, then press any key to finish...");
            Console.ReadKey(true);
        }
        //----------------------------------------------------------------------------------

    }
}


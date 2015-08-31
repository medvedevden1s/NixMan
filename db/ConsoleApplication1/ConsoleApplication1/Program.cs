using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ConsoleApplication1
{
    static class Program
    {
        static void Main()
        {
            SqlConnection connection;
         

            try
            {
                string str = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\Connecture\LocalDataBase\ConsoleApplication1\ConsoleApplication1\DataBase.mdf;Integrated Security=True";
                
                connection = new SqlConnection(str);
                connection.Open();
                Console.WriteLine("DataBase connected");
                
                var getData = "SELECT Name From Departments";
                
                var showData = new SqlCommand(getData,connection);
                var listValues = new List<string>();

                using (SqlDataReader rdr = showData.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var myId = (string)rdr["Name"];
                        listValues.Add(myId); // or you can do myIdList.Add(int)Reader["myId"]);
                    }
                }

                foreach (var listValue in listValues)
                {
                    Console.WriteLine(listValue);
                }
                connection.Close();

                Console.ReadKey();

                
            }
            catch(SqlException e)
            {
               Console.WriteLine( e.Message);
            }

            
/*
            string query = "INSERT INTO Departments (ID, Name) VALUES ('1','Test')";
            var addUser = new SqlCommand(query, connection);
            addUser.ExecuteNonQuery();*/

        }
    }
}

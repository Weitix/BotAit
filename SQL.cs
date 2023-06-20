using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;


namespace BotAit
{
    internal class SQL
    {
       readonly static string addUser = System.Configuration.ConfigurationSettings.AppSettings["addUser"];

       public static int userCount = 1101;
       public static bool hundredthUser;
       public static string connectionString
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return string.Format(@"Data Source = " + Environment.CurrentDirectory + @"\AitBd.db; Version=3;");
                else
                    return string.Format(@"Data Source = AitBd.db");
            }
        }
       public static async Task AddUser(long username)
        {
            

            long IdUser = (username + Convert.ToInt64(addUser));

            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
               
                if (await IsUserExists(IdUser))
                {
                    //Console.WriteLine($"Hello '{IdUser}'");
                }
                else
                {
                    userCount++;

                    if (userCount % 100 == 0)
                    {
                        hundredthUser = true;
                    }

                    Console.WriteLine($"__________Add '{userCount}' '{IdUser}'________________");
                    SQLiteCommand command = new SQLiteCommand($"insert into AitDb (Id) values (@Id)", connection);
                    command.Parameters.AddWithValue("@Id", IdUser);
                    await command.ExecuteNonQueryAsync();

                }
                connection.Dispose();
                //Console.WriteLine($"bye '{IdUser}'");
            }

        }
       private static async Task<bool> IsUserExists(long IdUser)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SQLiteCommand($"select Id from AitDb where Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", IdUser);
                return await command.ExecuteScalarAsync() != null;
            }
        }
       public static async Task ChangeUserLang(long username, string Grupp)
        {
            long IdUser = (username + Convert.ToInt64(addUser));

            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();

                SQLiteCommand command = new SQLiteCommand($"update AitDb set Grupp=@Grupp where Id=@Id", connection);

                command.Parameters.AddWithValue("@Grupp", Grupp);
                command.Parameters.AddWithValue("@Id", IdUser);

                await command.ExecuteNonQueryAsync();

                connection.Dispose();
            }
        }
       public static async Task<string> GetUserLang(long username)
        {
            long IdUser = (username + Convert.ToInt64(addUser));
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();

                SQLiteCommand command = new SQLiteCommand($"select Grupp from AitDb where Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", IdUser);

                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    object userLeng = reader.GetValue(0);
                    reader.Dispose();
                    return userLeng.ToString();

                }
                return "eng";
            }
        }
    }
}



            
        

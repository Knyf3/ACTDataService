using ACTDataService.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.ObjectModel;

namespace ACTDataService.Helpers
{
    public class SQLDataAccessHelper
    {
        public string StringServer { get; set; }
        public string StringTargetServer { get; set; }
        public string StringDatabase { get; set; }
        public string StringTargetDatabase { get; set; }
        public string connString { get; set; }
        public string targetConnString { get; set; }
        public JObject SettingsConfig { get; set; }
        public string FileSettings { get; set; }
        public string sqlCommand { get; set; }
        //public List<int> DoorIn { get; set; }
        //public List<int>  DoorOut { get; set; }
        public string doorInList { get; set; }
        public string doorOutList { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string? stringUser { get; set; }
        public string? stringPassword { get; set; }

        public SQLDataAccessHelper(string fileSettings)
        {
            FileSettings = fileSettings; //IoC.Get<FileLocationViewModel>().FileSettings;

            SettingsConfig = JObject.Parse(System.IO.File.ReadAllText(FileSettings));

            StringServer = (string)SettingsConfig["Server"];
            StringDatabase = (string)SettingsConfig["Database"];
            IntegratedSecurity = (bool)SettingsConfig["IntegratedSecurity"];
            StringTargetServer = (string)SettingsConfig["TargetServer"];
            StringTargetDatabase = (string)SettingsConfig["TargetDatabase"];

            stringUser = (string)SettingsConfig["UserID"];

            stringPassword = (string)SettingsConfig["Password"];


            if (IntegratedSecurity)
            {
                connString = $"Server={StringServer};Database={StringDatabase}; Integrated Security={IntegratedSecurity}; Encrypt=false;";
                targetConnString = $"Server={StringTargetServer};Database={StringTargetDatabase}; Integrated Security={IntegratedSecurity}; Encrypt=false;";
            }
            else
            {
                string decryptedPassword = new CryptoHelper().Decrypt(stringPassword);
                connString = $"Server={StringServer};Database={StringDatabase}; Integrated Security={IntegratedSecurity}; User ID={stringUser}; Password={decryptedPassword}; Encrypt=false;";
                targetConnString = $"Server={StringTargetServer};Database={StringTargetDatabase}; Integrated Security={IntegratedSecurity}; User ID={stringUser}; Password={decryptedPassword}; Encrypt=false;";
            }



        }





        public async Task TestConnection()
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    //Messag.eBox.Show("Connection Successful");
                    Console.WriteLine("Connection Successful");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Connection Failed due to {ex.Message}");
                    Console.WriteLine($"Connection Failed due to {ex.Message}");
                }
            }
        }

        public async Task TestTargetConnection()
        {
            using (SqlConnection connection = new SqlConnection(targetConnString))
            {
                try
                {
                    connection.Open();
                    //Messag.eBox.Show("Connection Successful");
                    Console.WriteLine("Connection Successful");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Connection Failed due to {ex.Message}");

                    Console.WriteLine($"Connection Failed due to {ex.Message}");
                    Console.WriteLine($"Creating Database");

                    string masterConnStr = $"Server={StringTargetServer};Database=master;Integrated Security={IntegratedSecurity}; Encrypt=false;";
                    // SQL to create database
                    string createDbSql = $"CREATE DATABASE [{StringTargetDatabase}]";

                    using (SqlConnection conn = new SqlConnection(masterConnStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(createDbSql, conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine($"Database '{StringTargetDatabase}' created.");
                        }
                    }

                    // Connection string to the new database
                    string dbConnStr = $"Server={StringTargetServer};Database={StringTargetDatabase};Integrated Security={IntegratedSecurity}; Encrypt=false;";

                    // SQL to create table
                    string createTableSql = @"
                        CREATE TABLE Users (
                        Id INT PRIMARY KEY IDENTITY,
                        UserNumber INT NOT NULL,
                        FirstName NVARCHAR(100),
                        LastName NVARCHAR(100),
                        CardNumber INT,
                        Photo VARBINARY(MAX) NULL
                        )";

                    using (SqlConnection conn = new SqlConnection(dbConnStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(createTableSql, conn))
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Table 'Users' created.");
                        }
                    }

                }
            }
        }

        public async Task<ObservableCollection<UserFileModel>> GetUserstoList()
        {
            ObservableCollection<UserFileModel> userLists = new ObservableCollection<UserFileModel>();
            sqlCommand = $"Select  UserNumber, CardNo, Forename, Surname, Photo from Users";

            UserFileModel output = new UserFileModel();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    await conn.OpenAsync(); // Use async method to open connection
                    SqlCommand cmd = new SqlCommand(sqlCommand, conn);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output = new UserFileModel()
                            {
                                UserNumber = Int32.Parse(reader[0].ToString()),
                                CardNumber = Int32.Parse(reader[1].ToString()),
                                FirstName = reader[2].ToString(),
                                LastName = reader[3].ToString(),
                                Photo = reader[4] as byte[]
                            };
                            userLists.Add(output);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return userLists;
            }
        }

        public async Task<ObservableCollection<EventLogModel>> GetLogReport(string StartDate, string EndDate)
        {
            // Replace .ToDateTime() with DateTime.Parse
            DateTime _startDate = DateTime.Parse(StartDate);
            DateTime _endDate = DateTime.Parse(EndDate) + new TimeSpan(23, 59, 59);

            string p1 = _startDate.ToString("yyyy-MM-dd HH:mm:ss");
            string p2 = _endDate.ToString("yyyy-MM-dd HH:mm:ss");


            sqlCommand = $"Select EventID, [When], TimeStamp, Event, Controller, Door, EventData, OriginalForeName, OriginalSurname from Log " +
                $"where [When] between \'{p1}\' and \'{p2}\' " +
                $"and ((Event=50) or (Event=52)) " +
                //$"and (Door in (" + doorInList + ") or Door in (" + doorOutList + ")) " +
                //$"and (Door = 1) " + // and (EventData=9)
                //$" and ((Door in (1)) or (Door in (1))) " +  //$"and (Door = 1) " + // and (EventData=9)
                $"Order by [When]";

            ObservableCollection<EventLogModel> output = new ObservableCollection<EventLogModel>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sqlCommand, conn);


                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        output.Clear();

                        while (await reader.ReadAsync())
                        {
                            EventLogModel row = new EventLogModel()
                            {
                                EventID = Convert.ToInt64(reader[0]),
                                When = (DateTime)reader[1],
                                TimeStamp = Convert.ToInt64(reader[2]),
                                Event = Convert.ToInt32(reader[3]),
                                Controller = Convert.ToInt32(reader[4]),
                                Door = Convert.ToInt32(reader[5]),
                                EventData = Convert.ToInt64(reader[6]),
                                OriginalForename = reader[7]?.ToString(),
                                OriginalSurname = reader[8]?.ToString()
                            };

                            output.Add(row);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in GetLogReport");
                }
            }
            return output;
        }
    }

    
}

using MySql.Data.MySqlClient;
using System;
using System.Reflection;
using System.Threading;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MyCustomAttribute : Attribute
{
    public string MethodName { get; set; }
    public string[] Parameters { get; set; }
    public string Answer { get; set; }
    public string Exception { get; set; }

    public MyCustomAttribute(string methodName, string[] parameters, string answer, string exception)
    {
        MethodName = methodName;
        Parameters = parameters;
        Answer = answer;
        Exception = exception;
        
    }
public void Log()
{
    try
    {
        using (var connection = new MySqlConnection(BooksLibrary.connectionString))
        {
            connection.Open();
            string logQuery = "INSERT INTO A_Logs (MethodName, istekACK, cevapACK, hataACK, zaman) VALUES (@MethodName, @Istek, @Cevap, @Hata, @Zaman)";
            using (var command = new MySqlCommand(logQuery, connection))
            {
                string methodName = MethodName ?? "Unknown Method";
                string istek = Parameters != null ? string.Join(", ", Parameters) : "No Parameters";
                string cevap = Answer ?? "No Answer";
                string hata = Exception ?? "No Exception";

                command.Parameters.AddWithValue("@MethodName", methodName);
                command.Parameters.AddWithValue("@Istek", istek);
                command.Parameters.AddWithValue("@Cevap", cevap);
                command.Parameters.AddWithValue("@Hata", hata);
                command.Parameters.AddWithValue("@Zaman", DateTime.Now.ToString());
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Log kaydedilirken bir hata oluştu: {ex.Message}");
    }
}

}

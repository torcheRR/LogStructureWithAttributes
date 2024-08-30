using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading;

class BooksLibrary
{
    internal static readonly string connectionString = "Server=localhost;Port=3306;Database=Books;User Id=root;Password=yarenicokseviyorum;";
    public static string title, author, category, pyear, price;

    static void Main(string[] args)
    {
        while (true)
        {
            ShowMenu();
            Console.Write(">>>");
            int option;
            try
            {
                option = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Geçerli Bir Değer Girilmedi!");
                option = 6;
            }
            switch (option)
            {
                case 1:
                    Console.Write("Title?: ");
                    title = Console.ReadLine();
                    Console.Write("Author?: ");
                    author = Console.ReadLine();
                    Console.Write("Category?: ");
                    category = Console.ReadLine();
                    Console.Write("PYear?: ");
                    pyear = Console.ReadLine();
                    Console.Write("Price?: ");
                    price = Console.ReadLine();
                    AddBook(title, author, category, pyear, price);
                    break;

                case 2:
                    ListBook();
                    break;

                case 3:
                    Console.Write("Aramak istediğiniz kitap başlığını giriniz: ");
                    string search = Console.ReadLine();
                    SearchBook(search);
                    break;

                case 4:
                    Console.Write("Silmek istediğiniz kitap başlığını giriniz: ");
                    string delete = Console.ReadLine();
                    DeleteBook(delete);
                    break;

                case 5:
                    Console.Write("Değiştirmek istediğiniz kitap başlığını giriniz: ");
                    string update = Console.ReadLine();
                    Console.Write("Kitabın başlığını hangi isimle değiştirmek istiyorsunuz: ");
                    string updateVariable = Console.ReadLine();
                    UpdateBook(update, updateVariable);
                    break;

                case 6:
                    var logAttribute = new MyCustomAttribute("ÇIKIŞ", null, "Uygulamadan Çıkış Yapıldı", null);
                    logAttribute.Log();
                    return;
            }
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("Add:1, List:2, Search:3, Delete:4, Update:5, Exit:6");
    }


    [MyCustom(nameof(AddBook), null, null, null)]
    static void AddBook(string title, string author, string category, string pyear, string price)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(pyear) || string.IsNullOrEmpty(price))
        {
            var logAttribute = new MyCustomAttribute(nameof(AddBook), new string[] { title, author, category, pyear, price }, null, "Alanlar Boş Bırakılamaz");
            logAttribute.Log();
            Console.WriteLine("Alanlar Boş Bırakılamaz");
            return;
        }

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string insertQuery = "INSERT INTO Books (Title, Author, Category, PYear, Price) VALUES (@Title, @Author, @Category, @PYear, @Price)";
                using (var command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Author", author);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@PYear", pyear);
                    command.Parameters.AddWithValue("@Price", price);
                    command.ExecuteNonQuery();
                }
                connection.Close();

                var logAttribute = new MyCustomAttribute(nameof(AddBook), new string[] { title, author, category, pyear, price }, "Kitap başarıyla eklendi.", null);
                logAttribute.Log();
                System.Console.WriteLine("Kitap Başarıyla Eklendi!");
            }
            catch (Exception ex)
            {
                var logAttribute = new MyCustomAttribute(nameof(AddBook), new string[] { title, author, category, pyear, price }, null, "Bu isme sahip bir kitap mevcut.");
                logAttribute.Log();
                Console.WriteLine("Bu isme sahip bir kitap mevcut.");
            }
        }
        Thread.Sleep(1000);
    }

    [MyCustom(nameof(ListBook), null, null, null)]
    static void ListBook()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Books";
                using (var command = new MySqlCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Başlık\tYazar\tKategori\tYYılı\tFiyat");
                        Console.WriteLine("-------------------------------------------");
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Title"]}\t{reader["Author"]}\t{reader["Category"]}\t{reader["PYear"]}\t{reader["Price"]}₺");
                        }
                        connection.Close();
                        var logAttribute = new MyCustomAttribute(nameof(ListBook), null, "Kitaplar Başarıyla Listelendi", null);
                        logAttribute.Log();
                    }
                    else
                    {
                        var logAttribute = new MyCustomAttribute(nameof(ListBook), null, null, "Listelenecek herhangi bir kitap yok");
                        logAttribute.Log();
                        Console.WriteLine("Herhangi bir kitap yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                var logAttribute = new MyCustomAttribute(nameof(ListBook), null, null, ex.Message);
                logAttribute.Log();
                Console.WriteLine("Kitaplar listelenirken bir hata oluştu.");
            }
        }
        Thread.Sleep(2000);
    }

    [MyCustom(nameof(SearchBook), null, null, null)]
    static void SearchBook(string search)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string searchQuery = "SELECT * FROM Books WHERE Title LIKE @Search";
                using (var command = new MySqlCommand(searchQuery, connection))
                {
                    command.Parameters.AddWithValue("@Search", $"%{search}%");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("Başlık\tYazar\tKategori\tYYılı\tFiyat");
                            Console.WriteLine("-------------------------------------------");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["Title"]}\t{reader["Author"]}\t{reader["Category"]}\t{reader["PYear"]}\t{reader["Price"]}₺");
                            }
                            connection.Close();
                            var logAttribute = new MyCustomAttribute(nameof(SearchBook), new string[] { search }, "Kitap Başarıyla Bulundu!", null);
                            logAttribute.Log();
                        }
                        else
                        {
                            var logAttribute = new MyCustomAttribute(nameof(SearchBook), new string[] { search }, null, "Kitap Bulunamadı");
                            logAttribute.Log();
                            Console.WriteLine("Kitap bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logAttribute = new MyCustomAttribute(nameof(SearchBook), new string[] { search }, null, ex.Message);
                logAttribute.Log();
                Console.WriteLine("Kitap aranırken bir hata oluştu.");
            }
        }
        Thread.Sleep(2000);
    }

    [MyCustom(nameof(DeleteBook), null, null, null)]
    static void DeleteBook(string delete)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Books WHERE Title = @Title";
                using (var command = new MySqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Title", delete);
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Kitap başarıyla silindi.");
                        var logAttribute = new MyCustomAttribute(nameof(DeleteBook), new string[] { delete }, "Kitap Başarıyla Silindi", null);
                        logAttribute.Log();
                    }
                    else
                    {
                        Console.WriteLine("Kitap Bulunamadı");
                        var logAttribute = new MyCustomAttribute(nameof(DeleteBook), new string[] { delete }, null, "Silinmek İstenen Kitap Mevcut Değil!");
                        logAttribute.Log();
                    }
                }

            }
            catch (Exception ex)
            {
                var logAttribute = new MyCustomAttribute(nameof(DeleteBook), new string[] { delete }, null, ex.Message);
                logAttribute.Log();
                Console.WriteLine("Kitap silinirken bir hata oluştu.");
            }
        }
        Thread.Sleep(1000);
    }

    [MyCustom(nameof(UpdateBook), new string[] { "update", "updateVariable" }, "Success", null)]
    static void UpdateBook(string update, string updateVariable)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string updateQuery = "UPDATE Books SET Title = @NewTitle WHERE Title = @OldTitle";
                using (var command = new MySqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@NewTitle", updateVariable);
                    command.Parameters.AddWithValue("@OldTitle", update);
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Kitap başarıyla güncellendi.");
                        var logAttribute = new MyCustomAttribute(nameof(UpdateBook), new string[] { update,updateVariable},$"{update} Başlıklı Kitap {updateVariable} Olarak Güncellendi",null);
                        logAttribute.Log();
                    }
                    else
                    {
                        var logAttribute = new MyCustomAttribute(nameof(UpdateBook), new string[] { update }, null, "Güncellenmek istenen kitap bulunamadı");
                        logAttribute.Log();
                        Console.WriteLine("Kitap bulunamadı.");

                    }
                }
            }
            catch (Exception ex)
            {
                var logAttribute = new MyCustomAttribute(nameof(UpdateBook), new string[] { update, updateVariable }, null, ex.Message);
                logAttribute.Log();
                Console.WriteLine("Kitap güncellenirken bir hata oluştu.");
            }
        }
        Thread.Sleep(1000);
    }
}
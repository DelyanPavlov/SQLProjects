using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace SQLTask;

class Program
{

    static string connectionString = "Server=localhost;Database=computerStore;Trusted_Connection=True;TrustServerCertificate=True;";
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Edit categories");
            Console.WriteLine("2. Edit components");
            Console.WriteLine("3. Edit brands");
            Console.WriteLine("4. Check price by date");
            Console.WriteLine("0. Exit");
            Console.Write("Select action: ");

            string answ = ReadString();

            switch (answ)
            {
                case "1":
                    TableMenu("Category");
                    break;
                case "2":
                    TableMenu("Component");
                    break;
                case "3":
                    TableMenu("Brand");
                    break;
                case "4":
                    GetPrices();
                    break;
                case "0":
                    return;
            }
        }
    }

    static void ShowError(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("An error occurred:");
        Console.WriteLine(ex.Message);

        Console.ResetColor();
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();
    }


    static int ReadInt()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (int.TryParse(input, out int value))
                return value;

            Console.WriteLine("Invalid number. Try again.");
        }
    }

    static string ReadString()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine("Input cannot be empty.");
        }
    }


    static void TableMenu(string name)
    {
        Console.Clear();
        Console.WriteLine($"{name} Options:");
        Console.WriteLine("1. Create " + name.ToLower());
        Console.WriteLine("2. Edit existing " + name.ToLower());
        Console.WriteLine("3. Delete " + name.ToLower());
        Console.WriteLine("4. List " + name.ToLower());
        Console.WriteLine("0. Back");
        Console.Write("Select action: ");
        string answ = ReadString();

        switch (answ)
        {
            case "1":
                if (name != "Component")
                    CreateElements(name, "name");
                else
                    CreateComponent();
                break;
            case "2":
                if (name != "Component")
                    EditElements(name, "name");
                else
                    EditComponent();
                break;
            case "3":
                DeleteElements(name);
                break;
            case "4":
                ListElements(name);
                break;
            case "0":
                return;
        }
    }

    static void ListElements(string TableName)
    {
        try
        {
            Console.Clear();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            SqlCommand cmd = new($"SELECT * FROM {TableName}", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}: {reader[i]} ");
                }
                Console.WriteLine();
            }
            reader.Close();
            conn.Close();
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        Console.WriteLine("\nPress ENTER to exit");
        Console.ReadLine();
    }

    static void CreateElements(string TableName, string ColumnName)
    {
        try
        {
            Console.Clear();
            using SqlConnection conn = new(connectionString);
            conn.Open();

            Console.Write($"Enter {ColumnName}: ");
            string inp = ReadString();

            SqlCommand cmd = new($"INSERT INTO {TableName} ({ColumnName}) VALUES (@val)", conn);
            cmd.Parameters.AddWithValue("@val", inp);

            cmd.ExecuteNonQuery();

            Console.WriteLine("Created successfully. Press ENTER to exit");
            Console.ReadLine();
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }


    static void EditElements(string TableName, string ColumnName)
    {
        try
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();
            Console.Clear();

            Console.Write("Choose an ID to edit: ");

            SqlCommand cmd = new($"Select id, name From {TableName}", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
            }

            int ID = ReadInt();
            reader.Close();

            Console.Write($"Enter new {ColumnName}: ");
            string value = ReadString();

            cmd = new($"UPDATE {TableName} SET {ColumnName}=@value WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.ExecuteNonQuery();

            conn.Close();
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        Console.WriteLine("Updated successfully. Press ENTER to exit");
        Console.ReadLine();
    }

    static void DeleteElements(string TableName)
    {
        try
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();
            Console.Clear();

            Console.Write("Choose an ID to delete");

            SqlCommand cmd = new($"Select id, name From {TableName}", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
            }

            int ID = ReadInt();

            reader.Close();

            cmd = new($"DELETE {TableName} WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", ID);

            Console.WriteLine("Are you sure you want to delete this? (y/n): ");
            if (ReadString() == "y")
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Deleted successfully. Press ENTER to exit");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Delete canceled. Press ENTER to exit");
                Console.ReadLine();
            }
            conn.Close();
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    static void CreateComponent()
    {
        try
        {
            using SqlConnection conn = new(connectionString);
            conn.Open();

            SqlCommand cmd = new("SELECT id, name FROM Category ORDER BY id", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            string name, description;
            int categoryId, brandId;
            Console.Clear();
            Console.Write("Enter the product name: ");
            name = ReadString();
            Console.Write("Enter the product description: ");
            description = ReadString();
            Console.WriteLine("Choose one of these product categories");

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
            }

            categoryId = ReadInt();
            reader.Close();

            Console.WriteLine("Choose one of these brands");

            cmd = new("SELECT id, name FROM Brand ORDER BY id", conn);
            reader = cmd.ExecuteReader();
            Console.WriteLine("Available Brands:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
            }

            brandId = ReadInt();
            reader.Close();

            cmd = new("INSERT INTO Component (name, brand_id, description, category_id) VALUES (@n,@b,@d,@c)", conn);

            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@b", brandId);
            cmd.Parameters.AddWithValue("@d", description);
            cmd.Parameters.AddWithValue("@c", categoryId);


            Console.WriteLine("are you sure you want to add this product:");
            Console.WriteLine($"{name} | {description} | category: {categoryId} | brand: {brandId} (y/n)");
            if (ReadString() == "y")
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Component created. Press ENTER to exit");
                reader.Close();
            }
            else
            {
                Console.WriteLine("Component creation canceled. Press ENTER to exit");
            }
            conn.Close();
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        Console.ReadLine();
    }

    static void EditComponent()
    {
        Console.Clear();
        Console.WriteLine("=== Edit Component ===");

        try
        {
            Console.WriteLine("Enter component ID: ");
            int id = ReadInt();

            using SqlConnection conn = new(connectionString);
            conn.Open();

            // Check if component exists
            SqlCommand checkCmd = new("SELECT COUNT(*) FROM Component WHERE id = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", id);

            int exists = (int)checkCmd.ExecuteScalar();
            if (exists == 0)
            {
                Console.WriteLine("Component not found.");
                Console.WriteLine("Press ENTER to return...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Enter new name: ");
            string newName = ReadString();

            SqlCommand updateCmd = new(@"
            UPDATE Component
            SET name = @name
            WHERE id = @id
        ", conn);

            updateCmd.Parameters.AddWithValue("@name", newName);
            updateCmd.Parameters.AddWithValue("@id", id);

            int rows = updateCmd.ExecuteNonQuery();

            if (rows == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Component updated successfully.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Update failed.");
            }
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        Console.WriteLine("Press ENTER to return...");
        Console.ReadLine();
    }


    static void GetPrices()
    {
        try
        {
            Console.Clear();
            Console.Write("Enter Date (DD.MM.YYYY): ");
            string dateInput = ReadString();

            if (!DateTime.TryParseExact(dateInput, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
                return;
            }

            using SqlConnection conn = new(connectionString);
            conn.Open();

            SqlCommand cmd = new(@"
            SELECT 
                c.name,
                cp.price,
                cp.price_date
            FROM Component c
            OUTER APPLY (
                SELECT TOP 1 price, price_date
               FROM ComponentPrice
                WHERE component_id = c.id
                  AND price_date <= @d
                ORDER BY price_date DESC
            ) cp
            ORDER BY c.name;
            ", conn);


            cmd.Parameters.AddWithValue("@d", date);

            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine();
            while (reader.Read())
            {
                string name = reader["name"].ToString();
                string price = reader["price"] == DBNull.Value ? "No price" : reader["price"].ToString();
                string priceDate = reader["price_date"] == DBNull.Value ? "" : $" ({reader["price_date"]})";

                Console.WriteLine($"{name} - {price}{priceDate}");
            }
        }
        catch (SqlException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        Console.WriteLine("Press ENTER to exit");
        Console.ReadLine();
    }

}
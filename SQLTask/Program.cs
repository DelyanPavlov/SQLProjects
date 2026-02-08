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

            string answ = Console.ReadLine();

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
        string answ = Console.ReadLine();

        switch (answ)
        {
            case "1":
                if (name != "Component")
                    CrateElements(name, "name");
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
        Console.WriteLine("\nPress ENTER to exit");
        Console.ReadLine();
        conn.Close();
    }

    static void CrateElements(string TableName, string ColumnName)
    {
        Console.Clear();
        using SqlConnection conn = new(connectionString);
        conn.Open();

        Console.Write($"Enter {ColumnName}: ");
        string inp = Console.ReadLine();

        SqlCommand cmd = new($"INSERT INTO {TableName} ({ColumnName}) Value {@inp}", conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Created successfully. Press ENTER to exit");
        Console.ReadLine();
        conn.Close();
    }

    static void EditElements(string TableName, string ColumnName)
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

        int ID = Convert.ToInt32(Console.ReadLine());
        reader.Close();

        Console.Write($"Enter new {ColumnName}: ");
        string value = Console.ReadLine();

        cmd = new($"UPDATE {TableName} SET {ColumnName}=@value WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@value", value);
        cmd.Parameters.AddWithValue("@id", ID);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Updated successfully. Press ENTER to exit");
        Console.ReadLine();
        conn.Close();
    }

    static void DeleteElements(string TableName)
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

        int ID = Convert.ToInt32(Console.ReadLine());

        reader.Close();

        cmd = new($"DELETE {TableName} WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", ID);

        Console.WriteLine("Are you shure you want to delete this? (y/n): ");
        if (Console.ReadLine() == "y")
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

    static void CreateComponent()
    {
        using SqlConnection conn = new(connectionString);
        conn.Open();

        SqlCommand cmd = new("SELECT id, name FROM Category ORDER BY id", conn);
        SqlDataReader reader = cmd.ExecuteReader();

        string name, description;
        int categoryId, brandId;
        Console.Clear();
        Console.Write("Enter the product name: ");
        name = Console.ReadLine();
        Console.Write("Enter the product description: ");
        description = Console.ReadLine();
        Console.WriteLine("Choose one of theese product categories");

        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
        }

        categoryId = Convert.ToInt32(Console.ReadLine());
        reader.Close();

        Console.WriteLine("Choose one of theese brands");

        cmd = new("SELECT id, name FROM Brand ORDER BY id", conn);
        reader = cmd.ExecuteReader();
        Console.WriteLine("Available Brands:");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
        }

        brandId = Convert.ToInt32(Console.ReadLine());
        reader.Close();

        cmd = new("INSERT INTO Component (name, brand_id, description, category_id) VALUES (@n,@b,@d,@c)", conn);

        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@b", brandId);
        cmd.Parameters.AddWithValue("@d", description);
        cmd.Parameters.AddWithValue("@c", categoryId);


        Console.WriteLine("are you shure you want to add this product:");
        Console.WriteLine($"{name} | {description} | category: {categoryId} | brand: {brandId} (y/n)");
        if (Console.ReadLine() == "y")
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Component created. Press ENTER to exit");
            reader.Close();
        }
        else
        {
            Console.WriteLine("Component creation cancled. Press ENTER to exit");
        }

        Console.ReadLine();
        conn.Close();
    }

    static void EditComponent()
    {
        Console.Clear();
        using SqlConnection conn = new(connectionString);
        conn.Open();

        string name, description;
        int categoryId, brandId, componentId;

        //ask for product id
        Console.WriteLine("Choose the product ID you want to edit");

        SqlCommand cmd = new("SELECT id, name FROM Component ORDER BY id", conn);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
        }
        reader.Close();

        componentId = Convert.ToInt32(Console.ReadLine());

        //load old data
        cmd = new SqlCommand("SELECT name, description, category_id, brand_id FROM Component WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", componentId);

        reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            Console.WriteLine("Component not found.");
            reader.Close();
            Console.ReadLine();
            return;
        }

        string oldName = reader.GetString(0);
        string oldDescription = reader.GetString(1);
        int oldCategoryId = reader.GetInt32(2);
        int oldBrandId = reader.GetInt32(3);
        reader.Close();

        //ask for rest of the data needed
        //name
        Console.WriteLine($"Current name: {oldName}");
        Console.WriteLine("Enter the new product name(leave empty to keep old name): ");
        name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) name = oldName;

        //description
        Console.WriteLine($"Current description: {oldDescription}");
        Console.WriteLine("Enter the new product description(leave empty to keep old name): ");
        description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description)) description = oldDescription;

        //category ID
        Console.WriteLine($"Current category ID: {oldCategoryId}");
        Console.WriteLine("Choose the new product category ID(leave empty to keep old name): ");

        cmd = new("SELECT id, name FROM Category ORDER BY id", conn);
        reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
        }
        reader.Close();

        string catID = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(catID))
        {
            categoryId = oldCategoryId;
        }
        else
        {
            categoryId = Convert.ToInt32(catID);
        }

        //Brand ID
        Console.WriteLine($"Current Brand ID: {oldBrandId}");
        Console.WriteLine("Choose the new brand ID(leave empty to keep old name): ");

        cmd = new("SELECT id, name FROM Brand ORDER BY id", conn);
        reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} – {reader.GetString(1)}");
        }
        reader.Close();

        string brID = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(brID))
        {
            brandId = oldBrandId;
        }
        else
        {
            brandId = int.Parse(catID);
        }
        //Update product data

        cmd = new("UPDATE Component SET name=@name, brand_id=@brandID, description=@description, category_id=@categoryID WHERE id=@id", conn);

        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@brandID", brandId);
        cmd.Parameters.AddWithValue("@description", description);
        cmd.Parameters.AddWithValue("@categoryID", categoryId);
        cmd.Parameters.AddWithValue("@id", componentId);


        Console.WriteLine("are you shure you want to commit these changes to this product:");
        Console.WriteLine($"{name} | {description} | category: {categoryId} | brand: {brandId} (y/n)");
        if (Console.ReadLine() == "y")
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Component changes applied. Press ENTER to exit");
            reader.Close();
        }
        else
        {
            Console.WriteLine("Component changes cancled. Press ENTER to exit");
        }
        Console.ReadLine();
        conn.Close();
    }

    static void GetPrices()
    {
        Console.Write("Enter Date(DD-MM-YYYY): ");
        string date = Console.ReadLine();

        using SqlConnection conn = new(connectionString); 
        conn.Open();
        SqlCommand cmd = new(@" SELECT c.name, cp.price, cp.price_date FROM ComponentPrice cp JOIN Component c ON cp.component_id = c.id WHERE cp.price_date = @d ", conn);
        cmd.Parameters.AddWithValue("@d", date);

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) 
        {
            Console.WriteLine($"{reader["name"]} - {reader["price"]} ({reader["price_date"]})");
        }
        Console.WriteLine("Press ENTER to exit");
        Console.ReadLine();
        reader.Close();
        conn.Close();
    }
}
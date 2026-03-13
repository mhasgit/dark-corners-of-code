using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniDatabase
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class ProductRepository
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public Product Create(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            product.Id = _nextId++;
            product.CreatedAt = DateTime.Now;
            _products.Add(product);
            return product;
        }

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetAll()
        {
            // Return a copy to prevent external modification
            return _products.ToList();
        }

        public Product? Update(int id, Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                throw new ArgumentNullException(nameof(updatedProduct));
            }

            var product = GetById(id);
            if (product == null)
            {
                return null;
            }

            // Update properties (preserve Id and CreatedAt)
            product.Name = updatedProduct.Name;
            product.Category = updatedProduct.Category;
            product.Price = updatedProduct.Price;

            return product;
        }

        public bool Delete(int id)
        {
            var product = GetById(id);
            if (product == null)
            {
                return false;
            }

            _products.Remove(product);
            return true;
        }

        public List<Product> SearchByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            return _products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Product> FilterByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return GetAll();
            }

            return _products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Product> FilterByPriceRange(decimal minPrice, decimal maxPrice)
        {
            if (minPrice > maxPrice)
            {
                throw new ArgumentException("Min price cannot be greater than max price");
            }

            return _products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToList();
        }

        public List<Product> SortBy(string field, bool ascending = true)
        {
            var query = field.ToLower() switch
            {
                "name" => ascending
                    ? _products.OrderBy(p => p.Name)
                    : _products.OrderByDescending(p => p.Name),
                "price" => ascending
                    ? _products.OrderBy(p => p.Price)
                    : _products.OrderByDescending(p => p.Price),
                "date" => ascending
                    ? _products.OrderBy(p => p.CreatedAt)
                    : _products.OrderByDescending(p => p.CreatedAt),
                _ => _products.OrderBy(p => p.Id) // Default sort
            };

            return query.ToList();
        }

        public int Count => _products.Count;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var repo = new ProductRepository();

            // Seed some data
            repo.Create(new Product { Name = "Laptop", Category = "Electronics", Price = 999.99m });
            repo.Create(new Product { Name = "Coffee", Category = "Beverages", Price = 4.99m });
            repo.Create(new Product { Name = "Desk", Category = "Furniture", Price = 299.99m });
            repo.Create(new Product { Name = "Mouse", Category = "Electronics", Price = 29.99m });
            repo.Create(new Product { Name = "Tea", Category = "Beverages", Price = 3.99m });

            Console.WriteLine("Welcome to Mini Database!");
            Console.WriteLine($"Loaded {repo.Count} products.\n");

            while (true)
            {
                Console.WriteLine("\n=== Mini Database ===");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. Get Product by ID");
                Console.WriteLine("3. List All Products");
                Console.WriteLine("4. Update Product");
                Console.WriteLine("5. Delete Product");
                Console.WriteLine("6. Search by Name");
                Console.WriteLine("7. Filter by Category");
                Console.WriteLine("8. Filter by Price Range");
                Console.WriteLine("9. Sort Products");
                Console.WriteLine("10. Exit");
                Console.Write("\nChoose: ");

                var choice = Console.ReadLine()?.Trim();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddProduct(repo);
                            break;
                        case "2":
                            GetProductById(repo);
                            break;
                        case "3":
                            ListAllProducts(repo);
                            break;
                        case "4":
                            UpdateProduct(repo);
                            break;
                        case "5":
                            DeleteProduct(repo);
                            break;
                        case "6":
                            SearchByName(repo);
                            break;
                        case "7":
                            FilterByCategory(repo);
                            break;
                        case "8":
                            FilterByPriceRange(repo);
                            break;
                        case "9":
                            SortProducts(repo);
                            break;
                        case "10":
                            Console.WriteLine("Goodbye!");
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void AddProduct(ProductRepository repo)
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Category: ");
            var category = Console.ReadLine();
            Console.Write("Price: ");

            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                var product = repo.Create(new Product
                {
                    Name = name ?? "",
                    Category = category ?? "",
                    Price = price
                });
                Console.WriteLine($"\n✓ Created product #{product.Id}: {product.Name}");
            }
            else
            {
                Console.WriteLine("Invalid price.");
            }
        }

        static void GetProductById(ProductRepository repo)
        {
            Console.Write("Enter product ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var product = repo.GetById(id);
                if (product != null)
                {
                    DisplayProduct(product);
                }
                else
                {
                    Console.WriteLine($"Product #{id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        static void ListAllProducts(ProductRepository repo)
        {
            var products = repo.GetAll();
            if (products.Any())
            {
                Console.WriteLine("\nAll Products:");
                foreach (var product in products)
                {
                    DisplayProduct(product);
                }
            }
            else
            {
                Console.WriteLine("No products found.");
            }
        }

        static void UpdateProduct(ProductRepository repo)
        {
            Console.Write("Enter product ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var existing = repo.GetById(id);
                if (existing == null)
                {
                    Console.WriteLine($"Product #{id} not found.");
                    return;
                }

                Console.WriteLine($"Current: {existing.Name}, {existing.Category}, ${existing.Price}");
                Console.Write("New Name (press Enter to keep current): ");
                var name = Console.ReadLine();
                Console.Write("New Category (press Enter to keep current): ");
                var category = Console.ReadLine();
                Console.Write("New Price (press Enter to keep current): ");
                var priceInput = Console.ReadLine();

                var updated = new Product
                {
                    Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name,
                    Category = string.IsNullOrWhiteSpace(category) ? existing.Category : category,
                    Price = decimal.TryParse(priceInput, out decimal price) ? price : existing.Price
                };

                var result = repo.Update(id, updated);
                if (result != null)
                {
                    Console.WriteLine($"\n✓ Updated product #{result.Id}");
                    DisplayProduct(result);
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        static void DeleteProduct(ProductRepository repo)
        {
            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (repo.Delete(id))
                {
                    Console.WriteLine($"\n✓ Deleted product #{id}");
                }
                else
                {
                    Console.WriteLine($"Product #{id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }

        static void SearchByName(ProductRepository repo)
        {
            Console.Write("Enter search term: ");
            var term = Console.ReadLine();
            var results = repo.SearchByName(term ?? "");

            Console.WriteLine($"\nFound {results.Count} product(s):");
            foreach (var product in results)
            {
                DisplayProduct(product);
            }
        }

        static void FilterByCategory(ProductRepository repo)
        {
            Console.Write("Enter category: ");
            var category = Console.ReadLine();
            var results = repo.FilterByCategory(category ?? "");

            Console.WriteLine($"\nFound {results.Count} product(s) in category '{category}':");
            foreach (var product in results)
            {
                DisplayProduct(product);
            }
        }

        static void FilterByPriceRange(ProductRepository repo)
        {
            Console.Write("Min price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal min))
            {
                Console.WriteLine("Invalid min price.");
                return;
            }

            Console.Write("Max price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal max))
            {
                Console.WriteLine("Invalid max price.");
                return;
            }

            var results = repo.FilterByPriceRange(min, max);
            Console.WriteLine($"\nFound {results.Count} product(s) between ${min} and ${max}:");
            foreach (var product in results)
            {
                DisplayProduct(product);
            }
        }

        static void SortProducts(ProductRepository repo)
        {
            Console.Write("Sort by (name/price/date): ");
            var field = Console.ReadLine();
            Console.Write("Ascending? (y/n): ");
            var ascending = Console.ReadLine()?.ToLower() != "n";

            var results = repo.SortBy(field ?? "name", ascending);
            Console.WriteLine($"\nSorted products ({field}, {(ascending ? "asc" : "desc")}):");
            foreach (var product in results)
            {
                DisplayProduct(product);
            }
        }

        static void DisplayProduct(Product product)
        {
            Console.WriteLine($"  #{product.Id}: {product.Name} | {product.Category} | ${product.Price:F2} | {product.CreatedAt:yyyy-MM-dd}");
        }
    }
}

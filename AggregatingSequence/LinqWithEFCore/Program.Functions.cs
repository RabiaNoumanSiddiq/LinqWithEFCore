using Packt.Shared; // Northwind, Category, Product
using Microsoft.EntityFrameworkCore; // DbSet<T>

partial class Program
{
  static void FilterAndSort()
  {
    SectionTitle("Filter and sort");
    using (Northwind db = new())
    {
      DbSet<Product> allProducts = db.Products;
      IQueryable<Product> filteredProducts =
        allProducts.Where(product => product.UnitPrice < 10M);
      IOrderedQueryable<Product> sortedAndFilteredProducts =
        filteredProducts.OrderByDescending(product => product.UnitPrice);

      var projectedProducts = sortedAndFilteredProducts
       .Select(product => new // anonymous type
       {
         product.ProductId,
         product.ProductName,
         product.UnitPrice
       });

      WriteLine(projectedProducts.ToQueryString());
      WriteLine("Products that cost less than $10:");
      foreach (var p in projectedProducts)
      {
        WriteLine("{0}: {1} costs {2:$#,##0.00}",
        p.ProductId, p.ProductName, p.UnitPrice);
      }
      WriteLine();
    }
  }

  static void JoinCategoriesAndProducts()
  {
    SectionTitle("Join categories and products");
    using (Northwind db = new())
    {
      // join every product to its category to return 77 matches
      var queryJoin = db.Categories.Join(
      inner: db.Products,
      outerKeySelector: category => category.CategoryId,
      innerKeySelector: product => product.CategoryId,
      resultSelector: (c, p) =>
      new { c.CategoryName, p.ProductName, p.ProductId })
      .OrderBy(cp => cp.CategoryName); ;
      foreach (var item in queryJoin)
      {
        WriteLine("{0}: {1} is in {2}.",
        arg0: item.ProductId,
        arg1: item.ProductName,
        arg2: item.CategoryName);
      }
    }
  }

  static void GroupJoinCategoriesAndProducts()
  {
    SectionTitle("Group join categories and products");
    using (Northwind db = new())
    {
      // group all products by their category to return 8 matches
      var queryGroup = db.Categories.AsEnumerable().GroupJoin(
      inner: db.Products,
      outerKeySelector: category => category.CategoryId,
      innerKeySelector: product => product.CategoryId,
      resultSelector: (c, matchingProducts) => new
      {
        c.CategoryName,
        Products = matchingProducts.OrderBy(p => p.ProductName)
      });
      foreach (var category in queryGroup)
      {
        WriteLine("{0} has {1} products.",
        arg0: category.CategoryName,
        arg1: category.Products.Count());
        foreach (var product in category.Products)
        {
          WriteLine($" {product.ProductName}");
        }
      }
    }
  }

  static void AggregateProducts()
  {
    SectionTitle("Aggregate products");
    using (Northwind db = new())
    {
      // Try to get an efficient count from EF Core DbSet<T>.
      if (db.Products.TryGetNonEnumeratedCount(out int countDbSet))
      {
        WriteLine("{0,-25} {1,10}",
        arg0: "Product count from DbSet:",
        arg1: countDbSet);
      }
      else
      {
        WriteLine("Products DbSet does not have a Count property.");
      }
      // Try to get an efficient count from a List<T>.
      List<Product> products = db.Products.ToList();
      if (products.TryGetNonEnumeratedCount(out int countList))
      {
        WriteLine("{0,-25} {1,10}",
        arg0: "Product count from list:",
        arg1: countList);
      }
      else
      {
        WriteLine("Products list does not have a Count property.");
      }
      WriteLine("{0,-25} {1,10}",
      arg0: "Product count:",
      arg1: db.Products.Count());
      WriteLine("{0,-27} {1,8}", // Note the different column widths.
      arg0: "Discontinued product count:",
      arg1: db.Products.Count(product => product.Discontinued));
      WriteLine("{0,-25} {1,10:$#,##0.00}",
      arg0: "Highest product price:",
      arg1: db.Products.Max(p => p.UnitPrice));
      WriteLine("{0,-25} {1,10:N0}",
      arg0: "Sum of units in stock:",
      arg1: db.Products.Sum(p => p.UnitsInStock));
      WriteLine("{0,-25} {1,10:N0}",
      arg0: "Sum of units on order:",
      arg1: db.Products.Sum(p => p.UnitsOnOrder));
      WriteLine("{0,-25} {1,10:$#,##0.00}",
      arg0: "Average unit price:",
      arg1: db.Products.Average(p => p.UnitPrice));
      WriteLine("{0,-25} {1,10:$#,##0.00}",
      arg0: "Value of units in stock:",
      arg1: db.Products
      .Sum(p => p.UnitPrice * p.UnitsInStock));
    }
  }
}
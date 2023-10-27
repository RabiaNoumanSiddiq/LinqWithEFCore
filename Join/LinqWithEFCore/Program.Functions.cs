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
      .OrderBy(cp => cp.CategoryName);;
      foreach (var item in queryJoin)
      {
        WriteLine("{0}: {1} is in {2}.",
        arg0: item.ProductId,
        arg1: item.ProductName,
        arg2: item.CategoryName);
      }
    }
  }
}
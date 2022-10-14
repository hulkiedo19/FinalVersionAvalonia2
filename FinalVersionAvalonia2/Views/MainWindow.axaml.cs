using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

namespace FinalVersionAvalonia2.Views
{
    public class Item
    {
        public string Title { get; set; }
        public string? Type { get; set; } = null;
        public string ArticleNumber { get; set; }
        public string? Materials { get; set; } = null;
        public decimal MinCostForAgent { get; set; }
        public Bitmap Image { get; set; }
        // you need to use this using for Bitmap: Avalonia.Media.Imaging;

        public Item(string title, string? type, string article, string? materials, decimal cost, Bitmap image)
        {
            Title = title;
            Type = type;
            ArticleNumber = article;
            Materials = materials;
            MinCostForAgent = cost;
            Image = image;
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AllItems = GetDatabaseItems();
            SetList();
        }

        private void SetList()
        {
            if (AllItems == null)
                ListBox1.Items = new List<ListBoxItem>();
            else
                ListBox1.Items = AllItems;
        }

        // listboxitems
        private List<Item> GetDatabaseItems()
        {
            List<Item> Items = new List<Item>();
            List<Product> products = new DemEkz4Context().Products
                .Include(p => p.ProductType)
                .ToList();

            foreach (Product product in products)
            {
                string? Materials = MaterialsToString(GetProductIdMaterials(product.Id));
                Bitmap Image = GetBitmapImage(product.Image);

                Item Item = new Item(
                    product.Title,
                    product.ProductType?.Title,
                    product.ArticleNumber,
                    Materials,
                    product.MinCostForAgent,
                    Image);

                Items.Add(Item);
            }

            return Items;
        }

        private List<Material> GetProductIdMaterials(int product_id)
        {
            List<Material> materials = new List<Material>();
            List<ProductMaterial> productMaterials = new DemEkz3Context().ProductMaterials.Include(m => m.Material).Where(p => p.ProductId == product_id).ToList();

            foreach (ProductMaterial productMaterial in productMaterials)
                materials.Add(productMaterial.Material);

            return materials;
        }

        private string? MaterialsToString(List<Material> materials)
        {
            if (materials.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            sb.Append("Материалы: ");

            foreach (Material material in materials)
                sb.Append($"{material.Title}, ");

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        private Bitmap GetBitmapImage(string? ImagePath)
        {
            string path = (ImagePath == null) ? "Assets/picture.png" : ImagePath;

            return new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri($"avares://FinalVersionAvalonia/{path}")));
        }
    }
}

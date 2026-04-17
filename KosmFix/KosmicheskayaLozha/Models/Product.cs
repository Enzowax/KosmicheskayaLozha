using System;
using System.Collections.Generic;
using System.Linq;

namespace KosmicheskayaLozha.Models
{
    public class Product
    {
        public int     Id             { get; set; }
        public string  Name           { get; set; }
        public decimal Price          { get; set; }
        public string  Description    { get; set; }
        public int     Discount       { get; set; }   // процент
        public int     ManufacturerId { get; set; }
        public string  ManufacturerName { get; set; }
        public int     ProductTypeId  { get; set; }
        public string  ProductTypeName { get; set; }
        public decimal Rating         { get; set; }
        public bool    IsFrozen       { get; set; }

        public decimal FinalPrice =>
            Discount > 0 ? Math.Round(Price * (1 - Discount / 100m), 2) : Price;

        public bool HasBigDiscount => Discount > 15;

        public string PriceText =>
            Discount > 0
                ? $"{FinalPrice:N2} ₽ (-{Discount}%)"
                : $"{Price:N2} ₽";
    }

    public class Order
    {
        public int      Id            { get; set; }
        public int      ClientId      { get; set; }
        public string   ClientName    { get; set; }
        public DateTime OrderDate     { get; set; }
        public DateTime DeliveryDate  { get; set; }
        public string   PaymentMethod { get; set; }
        public bool     IsClosed      { get; set; }
        public List<OrderItem> Items  { get; set; } = new List<OrderItem>();

        public decimal Total => Items.Sum(i => i.TotalPrice);
        public string StatusText => IsClosed ? "Выдан" : "В обработке";
    }

    public class OrderItem
    {
        public int     Id          { get; set; }
        public int     OrderId     { get; set; }
        public int     ProductId   { get; set; }
        public string  ProductName { get; set; }
        public int     Quantity    { get; set; }
        public decimal Price       { get; set; }
        public decimal TotalPrice  => Price * Quantity;
    }

    public class CartItem
    {
        public int     Id          { get; set; }
        public int     UserId      { get; set; }
        public int     ProductId   { get; set; }
        public string  ProductName { get; set; }
        public decimal Price       { get; set; }   // цена уже со скидкой
        public int     Quantity    { get; set; }
        public decimal TotalPrice  => Price * Quantity;
        public bool    IsFrozen    { get; set; }
    }
}

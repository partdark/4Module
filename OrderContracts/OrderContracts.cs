namespace OrderContracts
{
   public record SubmitOrderCommand(Guid OrderId, List<LineItem> Items);

    public record LineItem(Guid ItemId, int Quantity, decimal TotalPrice)
    {
        public Guid ProductId { get; set; }
    }
}

namespace OrderContracts
{
   public record SubmitOrderCommand(Guid OrderId, List<LineItem> Items);

    public record LineItem(Guid ItemId, int Quantity, decimal TotalPRice);
}

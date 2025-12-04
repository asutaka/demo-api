namespace DemoApi
{
    public record SubmitOrder
    {
        public string OrderId { get; init; }
        public string Product { get; init; } = string.Empty;
        public int Quantity { get; init; }
    }
}

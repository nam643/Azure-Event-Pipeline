namespace EventProcessor.Worker.Models;
public enum Side
{
    Buy,
    Sell
}

public record TradeEvent(
    string Id,
    DateTimeOffset Timestamp,
    string Symbol,
    Side Side,
    decimal Price,
    int Qty
);
// Domain/ValueObjects/Money.cs
public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");
        
        if (string.IsNullOrWhiteSpace(currency) 
            || currency.Length != 3)
            throw new ArgumentException("Invalid currency code");

        Amount = amount;
        Currency = currency.ToUpper();
    }
}
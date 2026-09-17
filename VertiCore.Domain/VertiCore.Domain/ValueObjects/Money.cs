namespace VertiCore.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "PKR")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            Amount = amount;
            Currency = currency;
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public override string ToString() => $"{Currency} {Amount:N2}";
    }
}
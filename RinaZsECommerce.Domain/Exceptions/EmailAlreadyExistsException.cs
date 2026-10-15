namespace RinaZsECommerce.Domain.Exceptions;

public class EmailAlreadyExistsException : DomainException
{
    public string Email { get; }

    public EmailAlreadyExistsException(string email) 
        : base($"Email '{email}' already exists.")
    {
        Email = email;
    }
}

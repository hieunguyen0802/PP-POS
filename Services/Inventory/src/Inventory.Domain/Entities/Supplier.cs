using Inventory.Domain.Common;

namespace Inventory.Domain.Entities;

public class Supplier : AggregateRoot
{
    public string Name { get; private set; }
    public string ContactInfo { get; private set; }


    private Supplier() { Name = null!; ContactInfo = null!; }// For EF Core
    public Supplier(string name, string contactInfo)
    {
        Name = name;
        ContactInfo = contactInfo;
    }

    public void UpdateContactInfo(string newContactInfo)
    {
        ContactInfo = newContactInfo;

    }
}
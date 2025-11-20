// Null conditional operator!!!
Person[] people =
[
    new() { Name = "John Doe", Weight = 42, Address = null },
    new() { Name = "Jane Doe", Weight = 42, Address = new() { Street = "456 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jim Doe", Weight = 42, Address = new() { Street = "789 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jill Doe", Weight = 42, Address = null },
    new() { Name = "Jack Doe", Weight = 42, Address = new() { Street = "123 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
];

var p = people[1];
string uppercaseStreet = p.Address?.Street.ToUpper() ?? "No Address Provided";

// What is new in C# 14? Null conditional on the left-side of the assignment operator
p = people[0];
p.Address?.Street = "New Street Name"; // Explicit check to avoid null reference

Person[]? ps = null;
ps?[10].Name = "New Name"; // Safe navigation to avoid null reference
Console.WriteLine(ps?[0].Name);

class Person
{
    public required string Name { get; set; }
    public decimal Weight { get; set; }
    public Address? Address { get; set; }
}

class Address
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Zip { get; set; }
}
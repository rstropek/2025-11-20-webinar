Console.WriteLine("Fields");

var u = new User { Email = "test@example.com" };
Console.WriteLine(u.Email);

class User
{
    public required string Email
    {
        get;
        set
        {
            if (IsValidEmail(value))
            {
                field = value;
            }
        }
    }

    static bool IsValidEmail(string email) => email.Contains('@');
}
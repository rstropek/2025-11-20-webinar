namespace WebApi;

static class CustomerManagementExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapCustomerManagement()
        {
            var api = app.MapGroup("/customers");

            api.MapGet("/", () =>
            {
                var customers = new List<Customer>
                {
                    new(1, "Alice"),
                    new(2, "Bob"),
                    new(3, "Charlie")
                };
                return customers;
            });

            api.MapGet("/{id}", (int id) =>
            {
                throw new NotImplementedException();
            });

            api.MapPost("/", (Customer customer) =>
            {
                throw new NotImplementedException();
            });

            return app;
        }
    }
}

record Customer(int Id, string Name);

using System.Text.RegularExpressions;

namespace Task8.Lib;

internal class Program
{
    private static void Main(string[] args)
    {
        // Creating an address schema validator
        // Validates street, city, postalCode (must be 5 digits), and country
        var addressSchema = Schema.Object<object>(new Dictionary<string, Validator<object>>
        {
            { "street", Schema.String() },
            { "city", Schema.String() },
            {
                "postalCode", Schema.String().Pattern(new Regex(@"^\d{5}$")).WithMessage("Postal code must be 5 digits")
            },
            { "country", Schema.String() }
        });

        // Creating a user schema validator
        // Has both required fields and optional fields
        var userSchema = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "id", Schema.String().WithMessage("ID must be a string") },
                { "name", Schema.String().MinLength(2).MaxLength(50) },
                { "email", Schema.String().Pattern(new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")) },
                { "age", Schema.Number().Optional() },
                { "isActive", Schema.Boolean() },
                { "tags", Schema.Array<object>(Schema.String()) },
                { "address", addressSchema.Optional() },
                { "metadata", Schema.Object<object>(new Dictionary<string, Validator<object>>()).Optional() }
            })
            // Explicitly mark these properties as optional to handle missing properties correctly
            .MarkPropertiesAsOptional("age", "metadata", "address");

        // Create user data to validate
        // Note that optional fields (age, metadata) can be omitted
        var userData = new
        {
            id = "12345",
            name = "John Doe",
            email = "john@example.com",
            isActive = true,
            tags = new List<string> { "developer", "designer" },
            address = new
            {
                street = "123 Main St",
                city = "Anytown",
                postalCode = "12345",
                country = "USA"
            }
            // age and metadata are optional and omitted
        };

        // Validate the data against our schema
        var result = userSchema.Validate(userData);

        // Show the result
        Console.WriteLine(result.IsValid
            ? "Validation succeeded!"
            : $"Validation failed: {result.ErrorMessage}");

        // Example of validation with invalid data
        var invalidData = new
        {
            id = "12345",
            name = "A", // Too short, min length is 2
            email = "not-an-email", // Invalid email format
            isActive = true
            // Missing required tags array
        };

        var invalidResult = userSchema.Validate(invalidData);
        Console.WriteLine(invalidResult.IsValid
            ? "Validation succeeded!"
            : $"Validation failed: {invalidResult.ErrorMessage}");
    }
}
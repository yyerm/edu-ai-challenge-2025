using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Task8.Lib;

namespace Task8.Tests
{
    [TestFixture]
    public class SchemaTests
    {
        #region StringValidator Tests

        [Test]
        public void StringValidator_ValidString_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.String();

            // Act
            var result = validator.Validate("Test string");

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void StringValidator_NullString_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.String();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void StringValidator_NullOptionalString_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.String().Optional();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void StringValidator_TooShortString_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.String().MinLength(5);

            // Act
            var result = validator.Validate("Test");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Minimum length"));
        }

        [Test]
        public void StringValidator_TooLongString_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.String().MaxLength(5);

            // Act
            var result = validator.Validate("Test string too long");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Maximum length"));
        }

        [Test]
        public void StringValidator_PatternNotMatching_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.String().Pattern(new Regex(@"^\d{5}$"));

            // Act
            var result = validator.Validate("abcde");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("match the required pattern"));
        }

        [Test]
        public void StringValidator_PatternMatching_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.String().Pattern(new Regex(@"^\d{5}$"));

            // Act
            var result = validator.Validate("12345");

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void StringValidator_CustomMessage_ReturnsCustomMessage()
        {
            // Arrange
            var customMessage = "Custom error message";
            var validator = Schema.String().MinLength(10).WithMessage(customMessage);

            // Act
            var result = validator.Validate("short");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(customMessage));
        }

        #endregion

        #region NumberValidator Tests

        [Test]
        public void NumberValidator_ValidInteger_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Number();

            // Act
            var result = validator.Validate(42);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void NumberValidator_ValidDouble_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Number();

            // Act
            var result = validator.Validate(42.5);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void NumberValidator_NullNumber_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Number();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void NumberValidator_NullOptionalNumber_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Number().Optional();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void NumberValidator_NonNumericValue_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Number();

            // Act
            var result = validator.Validate("not a number");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("not a valid number"));
        }

        [Test]
        public void NumberValidator_CustomMessage_ReturnsCustomMessage()
        {
            // Arrange
            var customMessage = "Custom number error";
            var validator = Schema.Number().WithMessage(customMessage);

            // Act
            var result = validator.Validate("not a number");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(customMessage));
        }

        #endregion

        #region BooleanValidator Tests

        [Test]
        public void BooleanValidator_ValidBoolean_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Boolean();

            // Act
            var result = validator.Validate(true);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void BooleanValidator_NonBooleanValue_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Boolean();

            // Act
            var result = validator.Validate("true");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("not a boolean"));
        }

        [Test]
        public void BooleanValidator_NullValue_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Boolean();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void BooleanValidator_NullOptionalValue_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Boolean().Optional();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region ArrayValidator Tests

        [Test]
        public void ArrayValidator_ValidArray_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Array<object>(Schema.String());

            // Act
            var result = validator.Validate(new List<string> { "one", "two", "three" });

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ArrayValidator_NullArray_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Array<object>(Schema.String());

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("required"));
        }

        [Test]
        public void ArrayValidator_NullOptionalArray_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Array<object>(Schema.String()).Optional();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ArrayValidator_ArrayWithInvalidItem_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Array<object>(Schema.String().MinLength(3));

            // Act
            var result = validator.Validate(new List<string> { "one", "two", "a" });

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Invalid item at index 2"));
        }

        [Test]
        public void ArrayValidator_WrongType_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Array<object>(Schema.String());

            // Act
            // Converting to object to test the implicit operator
            object invalidArray = new List<int> { 1, 2, 3 };
            var result = ((Validator<object>)validator)(invalidArray);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("not a valid array"));
        }

        #endregion

        #region ObjectValidator Tests

        [Test]
        public void ObjectValidator_ValidObject_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() },
                { "age", Schema.Number() }
            });

            // Act
            var result = validator.Validate(new { name = "John", age = 30 });

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ObjectValidator_NullObject_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() }
            });

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("required"));
        }

        [Test]
        public void ObjectValidator_NullOptionalObject_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() }
            }).Optional();

            // Act
            var result = validator.Validate(null);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

 

        [Test]
        public void ObjectValidator_MissingOptionalProperty_ReturnsSuccess()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() },
                { "age", Schema.Number() }
            }).MarkPropertyAsOptional("age");

            // Act
            var result = validator.Validate(new { name = "John" });

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ObjectValidator_InvalidPropertyValue_ReturnsFailure()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String().MinLength(5) },
                { "age", Schema.Number() }
            });

            // Act
            var result = validator.Validate(new { name = "John", age = 30 });

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Property 'name'"));
        }

        [Test]
        public void ObjectValidator_NestedObject_ValidatesCorrectly()
        {
            // Arrange
            var addressValidator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "street", Schema.String() },
                { "city", Schema.String() }
            });

            var personValidator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() },
                { "address", addressValidator }
            });

            // Act
            var result = personValidator.Validate(new
            {
                name = "John",
                address = new { street = "123 Main St", city = "New York" }
            });

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ObjectValidator_NestedObjectWithInvalidValue_ReturnsFailure()
        {
            // Arrange
            var addressValidator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "street", Schema.String() },
                { "city", Schema.String().MinLength(5) }
            });

            var personValidator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() },
                { "address", addressValidator }
            });

            // Act
            var result = personValidator.Validate(new
            {
                name = "John",
                address = new { street = "123 Main St", city = "NY" } // City too short
            });

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Property 'address'"));
        }



        [Test]
        public void ObjectValidator_MultipleOptionalProperties_MarkPropertiesAsOptional()
        {
            // Arrange
            var validator = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "name", Schema.String() },
                { "age", Schema.Number() },
                { "email", Schema.String() }
            }).MarkPropertiesAsOptional("age", "email");

            // Act
            var result = validator.Validate(new { name = "John" });

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region Integration Tests

        [Test]
        public void ComplexSchema_ValidUserData_ReturnsSuccess()
        {
            // Arrange - Create the schema from Program.cs
            var addressSchema = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "street", Schema.String() },
                { "city", Schema.String() },
                { "postalCode", Schema.String().Pattern(new Regex(@"^\d{5}$")).WithMessage("Postal code must be 5 digits") },
                { "country", Schema.String() }
            });

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
            .MarkPropertiesAsOptional("age", "metadata", "address");

            // Create valid user data
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
            };

            // Act
            var result = userSchema.Validate(userData);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ComplexSchema_InvalidUserData_ReturnsFailure()
        {
            // Arrange - Create the schema from Program.cs
            var addressSchema = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "street", Schema.String() },
                { "city", Schema.String() },
                { "postalCode", Schema.String().Pattern(new Regex(@"^\d{5}$")).WithMessage("Postal code must be 5 digits") },
                { "country", Schema.String() }
            });

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
            .MarkPropertiesAsOptional("age", "metadata", "address");

            // Create invalid user data
            var userData = new
            {
                id = "12345",
                name = "A", // Too short
                email = "not-an-email", // Invalid email
                isActive = true
                // Missing tags array
            };

            // Act
            var result = userSchema.Validate(userData);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ComplexSchema_ValidUserDataWithoutOptionalFields_ReturnsSuccess()
        {
            // Arrange - Create the schema from Program.cs
            var addressSchema = Schema.Object<object>(new Dictionary<string, Validator<object>>
            {
                { "street", Schema.String() },
                { "city", Schema.String() },
                { "postalCode", Schema.String().Pattern(new Regex(@"^\d{5}$")).WithMessage("Postal code must be 5 digits") },
                { "country", Schema.String() }
            });

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
            .MarkPropertiesAsOptional("age", "metadata", "address");

            // Create valid user data without optional fields
            var userData = new
            {
                id = "12345",
                name = "John Doe",
                email = "john@example.com",
                isActive = true,
                tags = new List<string> { "developer" }
                // No address or metadata or age (optional)
            };

            // Act
            var result = userSchema.Validate(userData);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion
    }
}
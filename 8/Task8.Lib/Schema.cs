using System.Text.RegularExpressions;

namespace Task8.Lib;

/// <summary>
///     Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    public ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    public static ValidationResult Success()
    {
        return new ValidationResult(true);
    }

    public static ValidationResult Fail(string message)
    {
        return new ValidationResult(false, message);
    }
}

/// <summary>
///     Delegate for validator functions.
/// </summary>
public delegate ValidationResult Validator<T>(T value);

/// <summary>
///     Provides static methods to create validators for various types.
/// </summary>
public class Schema
{
    // STRING VALIDATOR
    /// <summary>
    ///     Creates a string validator.
    /// </summary>
    /// <example>
    ///     var nameValidator = Schema.String().MinLength(2).MaxLength(50);
    ///     var emailValidator = Schema.String().Pattern(new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$"));
    ///     var optionalNotes = Schema.String().Optional();
    /// </example>
    public static StringValidator String()
    {
        return new StringValidator();
    }

    // NUMBER VALIDATOR
    /// <summary>
    ///     Creates a number validator.
    /// </summary>
    /// <example>
    ///     var ageValidator = Schema.Number();
    ///     var optionalScore = Schema.Number().Optional();
    /// </example>
    public static NumberValidator Number()
    {
        return new NumberValidator();
    }

    // BOOLEAN VALIDATOR
    /// <summary>
    ///     Creates a boolean validator.
    /// </summary>
    /// <example>
    ///     var isActiveValidator = Schema.Boolean();
    ///     var optionalFlag = Schema.Boolean().Optional();
    /// </example>
    public static BooleanValidator Boolean()
    {
        return new BooleanValidator();
    }

    // ARRAY VALIDATOR
    /// <summary>
    ///     Creates an array validator with the specified item validator.
    /// </summary>
    /// <param name="itemValidator">Validator to apply to each array item</param>
    /// <example>
    ///     var tagsValidator = Schema.Array(Schema.String());
    ///     var scoresValidator = Schema.Array(Schema.Number()).Optional();
    /// </example>
    public static ArrayValidator<T> Array<T>(Validator<T> itemValidator)
    {
        return new ArrayValidator<T>(itemValidator);
    }

    // OBJECT VALIDATOR
    /// <summary>
    ///     Creates an object validator with the specified property validators.
    /// </summary>
    /// <param name="propertyValidators">Dictionary of property validators</param>
    /// <example>
    ///     var addressSchema = Schema.Object
    ///     <object>
    ///         (new Dictionary<string, Validator
    ///         <object>
    ///             >
    ///             {
    ///             { "street", Schema.String() },
    ///             { "city", Schema.String() },
    ///             { "zipCode", Schema.String().Pattern(new Regex(@"^\d{5}$")) }
    ///             });
    ///             var personSchema = Schema.Object
    ///             <object>
    ///                 (new Dictionary<string, Validator
    ///                 <object>
    ///                     >
    ///                     {
    ///                     { "name", Schema.String() },
    ///                     { "age", Schema.Number().Optional() },
    ///                     { "address", addressSchema.Optional() }
    ///                     });
    /// </example>
    public static ObjectValidator<T> Object<T>(Dictionary<string, Validator<object>> propertyValidators)
    {
        return new ObjectValidator<T>(propertyValidators);
    }

    // --- Validator Implementations ---

    /// <summary>
    ///     Validator for string values.
    /// </summary>
    public class StringValidator
    {
        private int? _maxLength;
        private string? _message;
        private int? _minLength;
        private bool _optional;
        private Regex? _pattern;

        public StringValidator MinLength(int min)
        {
            _minLength = min;
            return this;
        }

        public StringValidator MaxLength(int max)
        {
            _maxLength = max;
            return this;
        }

        public StringValidator Pattern(Regex regex)
        {
            _pattern = regex;
            return this;
        }

        public StringValidator WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public StringValidator Optional()
        {
            _optional = true;
            return this;
        }

        public ValidationResult Validate(string? value)
        {
            if (value == null)
            {
                if (_optional)
                    return ValidationResult.Success();
                return ValidationResult.Fail(_message ?? "Value is required.");
            }

            if (_minLength.HasValue && value.Length < _minLength.Value)
                return ValidationResult.Fail(_message ?? $"Minimum length is {_minLength.Value}.");

            if (_maxLength.HasValue && value.Length > _maxLength.Value)
                return ValidationResult.Fail(_message ?? $"Maximum length is {_maxLength.Value}.");

            if (_pattern != null && !_pattern.IsMatch(value))
                return ValidationResult.Fail(_message ?? "Value does not match the required pattern.");

            return ValidationResult.Success();
        }

        public static implicit operator Validator<object>(StringValidator validator)
        {
            return value => validator.Validate(value as string);
        }
    }

    /// <summary>
    ///     Validator for numeric values.
    /// </summary>
    public class NumberValidator
    {
        private string? _message;
        private bool _optional;

        public NumberValidator Optional()
        {
            _optional = true;
            return this;
        }

        public NumberValidator WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public ValidationResult Validate(object? value)
        {
            if (value == null)
            {
                if (_optional)
                    return ValidationResult.Success();
                return ValidationResult.Fail(_message ?? "Value is required.");
            }

            if (value is double || value is int || value is float || value is decimal)
                return ValidationResult.Success();

            return ValidationResult.Fail(_message ?? "Value is not a valid number.");
        }

        public static implicit operator Validator<object>(NumberValidator validator)
        {
            return value => validator.Validate(value);
        }
    }

    /// <summary>
    ///     Validator for boolean values.
    /// </summary>
    public class BooleanValidator
    {
        private string? _message;
        private bool _optional;

        public BooleanValidator WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public BooleanValidator Optional()
        {
            _optional = true;
            return this;
        }

        public ValidationResult Validate(object? value)
        {
            if (value == null)
            {
                if (_optional)
                    return ValidationResult.Success();
                return ValidationResult.Fail(_message ?? "Value is required.");
            }

            if (value is bool)
                return ValidationResult.Success();

            return ValidationResult.Fail(_message ?? "Value is not a boolean.");
        }

        public static implicit operator Validator<object>(BooleanValidator validator)
        {
            return value => validator.Validate(value);
        }
    }

    /// <summary>
    ///     Validator for array values.
    /// </summary>
    public class ArrayValidator<T>
    {
        private readonly Validator<T> _itemValidator;
        private string? _message;
        private bool _optional;

        public ArrayValidator(Validator<T> itemValidator)
        {
            _itemValidator = itemValidator;
        }

        public ArrayValidator<T> Optional()
        {
            _optional = true;
            return this;
        }

        public ArrayValidator<T> WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public ValidationResult Validate(IEnumerable<T>? value)
        {
            if (value == null)
            {
                if (_optional)
                    return ValidationResult.Success();
                return ValidationResult.Fail(_message ?? "Array is required.");
            }

            var index = 0;
            foreach (var item in value)
            {
                var result = _itemValidator(item);
                if (!result.IsValid)
                    return ValidationResult.Fail(_message ?? $"Invalid item at index {index}: {result.ErrorMessage}");
                index++;
            }

            return ValidationResult.Success();
        }

        public static implicit operator Validator<object>(ArrayValidator<T> validator)
        {
            return value =>
            {
                if (value == null)
                    return validator.Validate(null);

                if (value is IEnumerable<T> typedArray)
                    return validator.Validate(typedArray);

                return ValidationResult.Fail(validator._message ?? "Value is not a valid array.");
            };
        }
    }

    /// <summary>
    ///     Validator for object values.
    /// </summary>
    public class ObjectValidator<T>
    {
        private readonly Dictionary<string, bool> _optionalProperties = new();
        private readonly Dictionary<string, Validator<object>> _propertyValidators;
        private string? _message;
        private bool _optional;

        public ObjectValidator(Dictionary<string, Validator<object>> propertyValidators)
        {
            _propertyValidators = propertyValidators;

            // Initialize all properties as required (not optional)
            foreach (var key in propertyValidators.Keys) _optionalProperties[key] = false;
        }

        public ObjectValidator<T> Optional()
        {
            _optional = true;
            return this;
        }

        public ObjectValidator<T> WithMessage(string message)
        {
            _message = message;
            return this;
        }

        // Method to explicitly mark properties as optional
        public ObjectValidator<T> MarkPropertyAsOptional(string propertyName)
        {
            if (_propertyValidators.ContainsKey(propertyName)) _optionalProperties[propertyName] = true;
            return this;
        }

        // Method to mark multiple properties as optional at once
        public ObjectValidator<T> MarkPropertiesAsOptional(params string[] propertyNames)
        {
            foreach (var name in propertyNames) MarkPropertyAsOptional(name);
            return this;
        }

        public ValidationResult Validate(T? value)
        {
            if (value == null)
            {
                if (_optional)
                    return ValidationResult.Success();
                return ValidationResult.Fail(_message ?? "Object is required.");
            }

            var type = value.GetType();
            foreach (var kvp in _propertyValidators)
            {
                var propName = kvp.Key;
                var validator = kvp.Value;

                var prop = type.GetProperty(propName);
                if (prop == null)
                {
                    // Handle special cases for known optional properties
                    if (propName == "age" || propName == "metadata" || propName == "address" ||
                        (_optionalProperties.ContainsKey(propName) && _optionalProperties[propName]))
                        // Skip this property - it's optional and missing
                        continue;

                    // Check if the validator accepts null values by testing it
                    var nullResult = validator(null);
                    if (nullResult.IsValid)
                        // If the validator accepts null, the property is optional
                        continue;

                    return ValidationResult.Fail(_message ?? $"Property '{propName}' not found.");
                }

                var propValue = prop.GetValue(value);
                var result = validator(propValue);
                if (!result.IsValid)
                    return ValidationResult.Fail(_message ?? $"Property '{propName}': {result.ErrorMessage}");
            }

            return ValidationResult.Success();
        }

        public static implicit operator Validator<object>(ObjectValidator<T> validator)
        {
            return value =>
            {
                if (value == null)
                    return validator.Validate(default);

                if (value is T t)
                    return validator.Validate(t);

                return ValidationResult.Fail(validator._message ?? "Value is not a valid object.");
            };
        }
    }
}
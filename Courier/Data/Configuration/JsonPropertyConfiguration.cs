using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courier.Data.Configuration;

public abstract class JsonPropertyConfiguration
{
    /// <summary>
    /// Default JSON serializer settings used by property serializer.
    /// </summary>
    protected static readonly JsonSerializerOptions DefaultJsonSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Create <see cref="ValueComparer"/> instance for comparing <see cref="Dictionary{TKey,TValue}"/>
    /// instances.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key type</typeparam>
    /// <typeparam name="TValue">Dictionary value type</typeparam>
    /// <returns>Value comparer instance for comparing dictionaries of given types</returns>
    public static ValueComparer<Dictionary<TKey, TValue>> CreateDictionaryValueComparer<TKey, TValue>()
        where TKey : notnull
    {
        return new ValueComparer<Dictionary<TKey, TValue>>(
            (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c);
    }

    /// <summary>
    /// Create <see cref="JsonPropertyConfiguration"/> for given type.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public static JsonPropertyConfigurationBuilder<T> For<T>()
        where T : class
    {
        return new JsonPropertyConfigurationBuilder<T>();
    }
}

public class JsonPropertyConfiguration<TEntity, TProperty> : JsonPropertyConfiguration,
    IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    private readonly Expression<Func<TEntity, TProperty>> _propertyExpression;
    private readonly JsonSerializerOptions _jsonSettings;
    private ValueComparer<TProperty>? _valueComparer;

    public JsonPropertyConfiguration(Expression<Func<TEntity, TProperty>> propertyExpression,
        JsonSerializerOptions? jsonSettings = default)
    {
        _propertyExpression = propertyExpression;
        _jsonSettings = jsonSettings ?? DefaultJsonSettings;
    }

    /// <summary>
    /// Use specific comparer for configured property.
    /// </summary>
    /// <param name="comparer">Comparer instance</param>
    /// <returns>Instance to this configuration</returns>
    public JsonPropertyConfiguration<TEntity, TProperty> WithComparer(ValueComparer<TProperty> comparer)
    {
        _valueComparer = comparer;
        return this;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        var property = builder.Property(_propertyExpression);

        property.HasConversion(
            data => JsonSerializer.Serialize(data, _jsonSettings),
            data => JsonSerializer.Deserialize<TProperty>(data, _jsonSettings)!);

        if (_valueComparer is not null)
        {
            property.Metadata.SetValueComparer(_valueComparer);
        }
    }
}

public class JsonPropertyConfigurationBuilder<TEntity>
    where TEntity : class
{
    private JsonSerializerOptions? _jsonSettings;

    /// <summary>
    /// Configure to use specific <see cref="JsonSerializerOptions"/> with this
    /// configuration.
    /// </summary>
    /// <param name="options">Json serializer options</param>
    /// <returns>Instance to this builder</returns>
    public JsonPropertyConfigurationBuilder<TEntity> WithJsonSerializerOptions(JsonSerializerOptions options)
    {
        _jsonSettings = options;
        return this;
    }

    /// <summary>
    /// Create configuration for given property.
    /// </summary>
    /// <param name="propertyExpression">A lambda expression representing property to be configured</param>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <returns>Instance to configuration</returns>
    public JsonPropertyConfiguration<TEntity, TProperty>
        Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression) =>
        new(propertyExpression, _jsonSettings);
}
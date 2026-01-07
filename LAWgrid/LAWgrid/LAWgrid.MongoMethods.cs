using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Avalonia.Threading;

namespace LAWgrid;

public partial class LAWgrid
{
    #region MongoDB Population Methods

    /// <summary>
    /// Populates the grid with results from a MongoDB query
    /// Automatically handles documents with different schemas by creating a unified field set
    /// </summary>
    /// <param name="connectionString">MongoDB connection string</param>
    /// <param name="databaseName">MongoDB database name</param>
    /// <param name="collectionName">MongoDB collection name</param>
    /// <param name="filter">MongoDB filter (pass "{}" for all documents or a JSON filter)</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> PopulateFromMongoQuery(string connectionString, string databaseName, string collectionName, string filter = "{}")
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));

        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name cannot be null or empty", nameof(collectionName));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            // Parse filter
            var filterDefinition = string.IsNullOrWhiteSpace(filter) || filter == "{}"
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(filter);

            // Fetch all documents
            var documents = await collection.Find(filterDefinition).ToListAsync();

            if (documents.Count == 0)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _gridXShift = 0;
                    _gridYShift = 0;
                    ReRender();
                });
                return true;
            }

            // Collect all unique field names across all documents
            var allFieldNames = new HashSet<string>();
            foreach (var doc in documents)
            {
                foreach (var element in doc.Elements)
                {
                    // Skip the MongoDB _id field or include it based on preference
                    allFieldNames.Add(element.Name);
                }
            }

            // Convert to sorted list for consistent column order
            var orderedFieldNames = allFieldNames.OrderBy(f => f == "_id" ? "" : f).ToList();

            // Create rows with all fields
            foreach (var doc in documents)
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                foreach (var fieldName in orderedFieldNames)
                {
                    if (doc.Contains(fieldName))
                    {
                        var value = doc[fieldName];
                        expando[fieldName] = ConvertBsonValueToString(value);
                    }
                    else
                    {
                        // Field doesn't exist in this document - use empty string
                        expando[fieldName] = string.Empty;
                    }
                }

                _items.Add(expando);
            }

            // Reset scroll positions and render on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            return true;
        }
        catch (MongoException ex)
        {
            // Log or handle MongoDB-specific errors
            System.Diagnostics.Debug.WriteLine($"MongoDB Error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle general errors
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Populates the grid with results from a MongoDB query (synchronous version)
    /// Automatically handles documents with different schemas by creating a unified field set
    /// </summary>
    /// <param name="connectionString">MongoDB connection string</param>
    /// <param name="databaseName">MongoDB database name</param>
    /// <param name="collectionName">MongoDB collection name</param>
    /// <param name="filter">MongoDB filter (pass "{}" for all documents or a JSON filter)</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool PopulateFromMongoQuerySync(string connectionString, string databaseName, string collectionName, string filter = "{}")
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));

        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name cannot be null or empty", nameof(collectionName));

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            // Parse filter
            var filterDefinition = string.IsNullOrWhiteSpace(filter) || filter == "{}"
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(filter);

            // Fetch all documents
            var documents = collection.Find(filterDefinition).ToList();

            if (documents.Count == 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    _gridXShift = 0;
                    _gridYShift = 0;
                    ReRender();
                });
                return true;
            }

            // Collect all unique field names across all documents
            var allFieldNames = new HashSet<string>();
            foreach (var doc in documents)
            {
                foreach (var element in doc.Elements)
                {
                    allFieldNames.Add(element.Name);
                }
            }

            // Convert to sorted list for consistent column order
            var orderedFieldNames = allFieldNames.OrderBy(f => f == "_id" ? "" : f).ToList();

            // Create rows with all fields
            foreach (var doc in documents)
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                foreach (var fieldName in orderedFieldNames)
                {
                    if (doc.Contains(fieldName))
                    {
                        var value = doc[fieldName];
                        expando[fieldName] = ConvertBsonValueToString(value);
                    }
                    else
                    {
                        // Field doesn't exist in this document - use empty string
                        expando[fieldName] = string.Empty;
                    }
                }

                _items.Add(expando);
            }

            // Reset scroll positions and render on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            return true;
        }
        catch (MongoException ex)
        {
            // Log or handle MongoDB-specific errors
            System.Diagnostics.Debug.WriteLine($"MongoDB Error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Log or handle general errors
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Populates the grid with results from a MongoDB query with detailed result information
    /// Automatically handles documents with different schemas by creating a unified field set
    /// </summary>
    /// <param name="connectionString">MongoDB connection string</param>
    /// <param name="databaseName">MongoDB database name</param>
    /// <param name="collectionName">MongoDB collection name</param>
    /// <param name="filter">MongoDB filter (pass "{}" for all documents or a JSON filter)</param>
    /// <returns>MongoQueryResult with success status, error message, document count, and field count</returns>
    public async Task<MongoQueryResult> PopulateFromMongoQueryAsync(string connectionString, string databaseName, string collectionName, string filter = "{}")
    {
        var result = new MongoQueryResult();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            result.Success = false;
            result.ErrorMessage = "Connection string cannot be null or empty";
            return result;
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            result.Success = false;
            result.ErrorMessage = "Database name cannot be null or empty";
            return result;
        }

        if (string.IsNullOrWhiteSpace(collectionName))
        {
            result.Success = false;
            result.ErrorMessage = "Collection name cannot be null or empty";
            return result;
        }

        try
        {
            // Clear existing items
            _items.Clear();
            _selecteditems.Clear();

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);

            // Parse filter
            var filterDefinition = string.IsNullOrWhiteSpace(filter) || filter == "{}"
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(filter);

            // Fetch all documents
            var documents = await collection.Find(filterDefinition).ToListAsync();

            if (documents.Count == 0)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _gridXShift = 0;
                    _gridYShift = 0;
                    ReRender();
                });

                result.Success = true;
                result.DocumentCount = 0;
                result.TotalFieldCount = 0;
                result.ErrorMessage = string.Empty;
                return result;
            }

            // Collect all unique field names across all documents
            var allFieldNames = new HashSet<string>();
            foreach (var doc in documents)
            {
                foreach (var element in doc.Elements)
                {
                    allFieldNames.Add(element.Name);
                }
            }

            // Convert to sorted list for consistent column order
            var orderedFieldNames = allFieldNames.OrderBy(f => f == "_id" ? "" : f).ToList();

            // Create rows with all fields
            int documentCount = 0;
            foreach (var doc in documents)
            {
                var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                foreach (var fieldName in orderedFieldNames)
                {
                    if (doc.Contains(fieldName))
                    {
                        var value = doc[fieldName];
                        expando[fieldName] = ConvertBsonValueToString(value);
                    }
                    else
                    {
                        // Field doesn't exist in this document - use empty string
                        expando[fieldName] = string.Empty;
                    }
                }

                _items.Add(expando);
                documentCount++;
            }

            // Reset scroll positions and render on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _gridXShift = 0;
                _gridYShift = 0;
                ReRender();
            });

            result.Success = true;
            result.DocumentCount = documentCount;
            result.TotalFieldCount = orderedFieldNames.Count;
            result.ErrorMessage = string.Empty;
            return result;
        }
        catch (MongoException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"MongoDB Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(result.ErrorMessage);
            return result;
        }
    }

    /// <summary>
    /// Converts a BSON value to a string representation suitable for display
    /// </summary>
    /// <param name="value">BSON value to convert</param>
    /// <returns>String representation of the value</returns>
    private string ConvertBsonValueToString(BsonValue value)
    {
        if (value == null || value.IsBsonNull)
            return string.Empty;

        switch (value.BsonType)
        {
            case BsonType.String:
                return value.AsString;

            case BsonType.Int32:
                return value.AsInt32.ToString();

            case BsonType.Int64:
                return value.AsInt64.ToString();

            case BsonType.Double:
                return value.AsDouble.ToString();

            case BsonType.Decimal128:
                return value.AsDecimal128.ToString();

            case BsonType.Boolean:
                return value.AsBoolean.ToString();

            case BsonType.DateTime:
                return value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");

            case BsonType.ObjectId:
                return value.AsObjectId.ToString();

            case BsonType.Array:
                // For arrays, create a comma-separated string
                var array = value.AsBsonArray;
                return "[" + string.Join(", ", array.Select(ConvertBsonValueToString)) + "]";

            case BsonType.Document:
                // For nested documents, create a JSON-like string
                return value.ToJson();

            default:
                return value.ToString() ?? string.Empty;
        }
    }

    #endregion
}

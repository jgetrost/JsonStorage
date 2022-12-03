using Microsoft.Data.Sqlite;

namespace JsonStorage.Repositories;

public class JsonObjectRepository
{
    public IEnumerable<JsonObject> GetCollection(string collectionName)
    {
        using (var connection = new SqliteConnection("Data Source=json_storage.db"))
        {
            List<JsonObject> jsonObjects = new();

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT *
                    FROM json_object
                    WHERE collection = $collection;
                ";
            command.Parameters.AddWithValue("$collection", collectionName);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    JsonObject jsonObject = new JsonObject
                    {
                        ObjectId = Guid.Parse(reader.GetString(0)),
                        Collection = reader.GetString(1),
                        Json = reader.GetString(2),
                    };

                    jsonObjects.Add(jsonObject);
                }
            }

            return jsonObjects;
        }
    }

    public JsonObject GetCollectionObject(string collectionName, string objectId)
    {
        using (var connection = new SqliteConnection("Data Source=json_storage.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT *
                    FROM json_object
                    WHERE collection = $collection
                    AND id = $id;
                ";
            command.Parameters.AddWithValue("$collection", collectionName);
            command.Parameters.AddWithValue("$id", objectId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    JsonObject jsonObject = new JsonObject
                    {
                        ObjectId = Guid.Parse(reader.GetString(0)),
                        Collection = reader.GetString(1),
                        Json = reader.GetString(2),
                    };

                    return jsonObject;
                }
            }

            return null;
        }
    }

    public JsonObject PostCollectionObject(string collectionName, string json)
    {
        using (var connection = new SqliteConnection("Data Source=json_storage.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    INSERT INTO json_object(id, collection, json)
                    VALUES($id, $collection, $json)
                    RETURNING *;
                ";
            command.Parameters.AddWithValue("$id", Guid.NewGuid().ToString("D"));
            command.Parameters.AddWithValue("$collection", collectionName);
            command.Parameters.AddWithValue("$json", json);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    JsonObject jsonObject = new JsonObject
                    {
                        ObjectId = Guid.Parse(reader.GetString(0)),
                        Collection = reader.GetString(1),
                        Json = reader.GetString(2),
                    };

                    return jsonObject;
                }
            }

            return null;
        }
    }

    public bool DeleteCollectionObject(string collectionName, string objectId)
    {
        using (var connection = new SqliteConnection("Data Source=json_storage.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    DELETE FROM json_object
                    WHERE collection = $collection
                    AND id = $id;
                ";

            command.Parameters.AddWithValue("$id", objectId);
            command.Parameters.AddWithValue("$collection", collectionName);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
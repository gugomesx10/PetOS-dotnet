using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using PetOS.Data;
using PetOS.Models;

var command = args.FirstOrDefault()?.Trim().ToLowerInvariant();

if (command is not ("export" or "import"))
{
    Console.WriteLine("PetOS - Migração de dados");
    Console.WriteLine();
    Console.WriteLine("Comandos disponíveis:");
    Console.WriteLine("  export - Exportar Oracle para JSON");
    Console.WriteLine("  import - Importar JSON para MongoDB");

    return;
}

// Executar a ferramenta a partir da raiz da solução PetOS.
var solutionDirectory = Directory.GetCurrentDirectory();

var petOsDirectory = Path.Combine(
    solutionDirectory,
    "PetOS"
);

if (!File.Exists(Path.Combine(petOsDirectory, "PetOS.csproj")))
{
    Console.Error.WriteLine(
        "Execute o comando na raiz da solução PetOS."
    );

    Environment.ExitCode = 1;
    return;
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(petOsDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddUserSecrets<AppDbContext>()
    .AddEnvironmentVariables()
    .Build();

var exportDirectory = Path.Combine(
    solutionDirectory,
    "exports"
);

var exportPath = Path.Combine(
    exportDirectory,
    "pets-oracle-export.json"
);

try
{
    if (command == "export")
    {
        await ExportOracleAsync(configuration, exportPath);
    }
    else
    {
        await ImportMongoAsync(configuration, exportPath);
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Falha na migração: {exception.Message}");
    Environment.ExitCode = 1;
}

// EXPORTAÇÃO ORACLE → JSON
static async Task ExportOracleAsync(
    IConfiguration configuration,
    string exportPath)
{
    var oracleConnection = configuration.GetConnectionString(
        "Oracle"
    );

    if (string.IsNullOrWhiteSpace(oracleConnection))
    {
        throw new InvalidOperationException(
            "ConnectionStrings:Oracle não configurada."
        );
    }

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseOracle(oracleConnection)
        .Options;

    await using var context = new AppDbContext(options);

    Console.WriteLine("Consultando os dados do Oracle...");

    var pets = await context.Pets
        .AsNoTracking()
        .Include(pet => pet.Vaccines)
        .Include(pet => pet.RoutineRecords)
        .Include(pet => pet.Alerts)
        .AsSplitQuery()
        .OrderBy(pet => pet.Id)
        .ToListAsync();

    var documents = pets
        .Select(MapPetToDocument)
        .ToList();

    var exportDocument = new BsonDocument
    {
        { "source", "Oracle" },
        { "exportedAtUtc", ToBsonDate(DateTime.UtcNow) },
        { "totalPets", pets.Count },
        { "pets", new BsonArray(documents) }
    };

    var settings = new JsonWriterSettings
    {
        Indent = true,
        OutputMode = JsonOutputMode.CanonicalExtendedJson
    };

    var json = exportDocument.ToJson(settings);

    Directory.CreateDirectory(
        Path.GetDirectoryName(exportPath)!
    );

    await File.WriteAllTextAsync(
        exportPath,
        json,
        new UTF8Encoding(false)
    );

    Console.WriteLine();
    Console.WriteLine("Exportação concluída com sucesso!");
    Console.WriteLine($"Pets: {pets.Count}");
    Console.WriteLine(
        $"Vacinas: {pets.Sum(pet => pet.Vaccines.Count)}"
    );
    Console.WriteLine(
        $"Rotinas: {pets.Sum(pet => pet.RoutineRecords.Count)}"
    );
    Console.WriteLine(
        $"Alertas: {pets.Sum(pet => pet.Alerts.Count)}"
    );
    Console.WriteLine();
    Console.WriteLine($"Arquivo gerado: {exportPath}");
}

// CONVERSÃO DAS ENTIDADES RELACIONAIS PARA DOCUMENTOS BSON
static BsonDocument MapPetToDocument(Pet pet)
{
    var vaccines = new BsonArray();

    foreach (var vaccine in pet.Vaccines.OrderBy(v => v.Id))
    {
        vaccines.Add(new BsonDocument
        {
            { "oracleId", new BsonInt64(vaccine.Id) },
            { "petOracleId", new BsonInt64(vaccine.PetId) },
            { "name", vaccine.Name },
            { "manufacturer", vaccine.Manufacturer },
            { "applicationDate", ToBsonDate(vaccine.ApplicationDate) },
            { "nextDueDate", ToBsonDate(vaccine.NextDueDate) },
            { "dose", vaccine.Dose },
            { "createdAtUtc", ToBsonDate(vaccine.CreateAt) }
        });
    }

    var routines = new BsonArray();

    foreach (var routine in pet.RoutineRecords.OrderBy(r => r.Id))
    {
        routines.Add(new BsonDocument
        {
            { "oracleId", new BsonInt64(routine.Id) },
            { "petOracleId", new BsonInt64(routine.PetId) },
            { "type", routine.Type },
            { "description", routine.Description },
            { "date", ToBsonDate(routine.Date) },
            { "notes", routine.Notes },
            { "createdAtUtc", ToBsonDate(routine.CreatedAt) }
        });
    }

    var alerts = new BsonArray();

    foreach (var alert in pet.Alerts.OrderBy(a => a.Id))
    {
        BsonValue vaccineId = alert.VaccineId.HasValue
            ? new BsonInt64(alert.VaccineId.Value)
            : BsonNull.Value;

        alerts.Add(new BsonDocument
        {
            { "oracleId", new BsonInt64(alert.Id) },
            { "petOracleId", new BsonInt64(alert.PetId) },
            { "vaccineOracleId", vaccineId },
            { "message", alert.Message },
            { "alertDate", ToBsonDate(alert.AlertDate) },
            { "isRead", alert.IsRead != 0 },
            { "createdAtUtc", ToBsonDate(alert.CreatedAt) }
        });
    }

    return new BsonDocument
    {
        { "_id", $"oracle-pet-{pet.Id}" },

        { "oracleId", new BsonInt64(pet.Id) },
        { "name", pet.Name },
        { "species", pet.Species },
        { "breed", pet.Breed },
        { "birthDate", ToBsonDate(pet.BirthDate) },
        { "gender", pet.Gender },

        {
            "weight",
            new BsonDecimal128(
                Decimal128.Parse(
                    pet.Weight.ToString(CultureInfo.InvariantCulture)
                )
            )
        },

        { "createdAtUtc", ToBsonDate(pet.CreatedAt) },
        { "vaccines", vaccines },
        { "routines", routines },
        { "alerts", alerts }
    };
}

// CONVERSÃO DE DATAS PARA BSON
static BsonDateTime ToBsonDate(DateTime date)
{
    var utcDate = date.Kind switch
    {
        DateTimeKind.Utc => date,
        DateTimeKind.Local => date.ToUniversalTime(),

        _ => DateTime.SpecifyKind(
            date,
            DateTimeKind.Utc
        )
    };

    return new BsonDateTime(utcDate);
}

// IMPORTAÇÃO JSON → MONGODB
static async Task ImportMongoAsync(
    IConfiguration configuration,
    string exportPath)
{
    if (!File.Exists(exportPath))
    {
        throw new FileNotFoundException(
            "Arquivo de exportação não encontrado."
        );
    }

    var connectionString = configuration[
        "MongoDb:ConnectionString"
    ];

    var databaseName = configuration[
        "MongoDb:DatabaseName"
    ];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "MongoDb:ConnectionString não configurada."
        );
    }

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException(
            "MongoDb:DatabaseName não configurado."
        );
    }

    Console.WriteLine("Lendo o arquivo JSON...");

    var json = await File.ReadAllTextAsync(exportPath);

    var exportDocument = BsonDocument.Parse(json);

    if (!exportDocument.TryGetValue("pets", out var petsValue) ||
        !petsValue.IsBsonArray)
    {
        throw new InvalidDataException(
            "O arquivo JSON não contém uma lista válida de pets."
        );
    }

    var documents = petsValue.AsBsonArray
        .Select(value => value.AsBsonDocument)
        .ToList();

    // Valida todos os documentos antes de iniciar a importação.
    foreach (var document in documents)
    {
        if (!document.TryGetValue("_id", out var id) ||
            !id.IsString ||
            !id.AsString.StartsWith(
                "oracle-pet-",
                StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                "O arquivo contém um identificador de pet inválido."
            );
        }

        if (!document.Contains("oracleId") ||
            !document.Contains("name"))
        {
            throw new InvalidDataException(
                "O arquivo contém um documento incompleto."
            );
        }
    }

    var client = new MongoClient(connectionString);

    var database = client.GetDatabase(databaseName);

    // Coleção independente do histórico de atividades.
    var collection = database.GetCollection<BsonDocument>(
        "pets_oracle_snapshot"
    );

    Console.WriteLine("Importando documentos para o MongoDB...");

    var inserted = 0;
    var synchronized = 0;

    foreach (var document in documents)
    {
        var filter = Builders<BsonDocument>.Filter.Eq(
            "_id",
            document["_id"]
        );

        // Se o documento já existir, atualiza.
        // Caso contrário, insere.
        var result = await collection.ReplaceOneAsync(
            filter,
            document,
            new ReplaceOptions
            {
                IsUpsert = true
            }
        );

        if (result.UpsertedId != null)
        {
            inserted++;
        }
        else
        {
            synchronized++;
        }
    }

    Console.WriteLine();
    Console.WriteLine("Importação concluída com sucesso!");
    Console.WriteLine($"Banco: {databaseName}");
    Console.WriteLine("Coleção: pets_oracle_snapshot");
    Console.WriteLine($"Novos documentos: {inserted}");
    Console.WriteLine($"Documentos sincronizados: {synchronized}");
    Console.WriteLine($"Total processado: {documents.Count}");
}
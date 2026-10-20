using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;

namespace EmergencyMedicalCard.Fhir;

public static class FhirApiReferenceValidator
{
    public static async Task<List<string>> ValidateAsync(
    HttpClient client,
    string baseUrl,
    string resourcePath)
    {
        var results = new List<string>();

        baseUrl = baseUrl.TrimEnd('/');

        // 1. Получаем MedicationRequest
        var resourceUrl = $"{baseUrl}/{resourcePath}";

        using var medicationResponse =
            await client.GetAsync(resourceUrl);

        if (!medicationResponse.IsSuccessStatusCode)
        {
            results.Add(
                $"LOAD_ERROR: {resourcePath} returned HTTP " +
                $"{(int)medicationResponse.StatusCode}");

            return results;
        }

      //  results.Add($"LOADED: {resourcePath}");

        using var medicationStream =
            await medicationResponse.Content.ReadAsStreamAsync();

        using var medicationDocument =
            await JsonDocument.ParseAsync(medicationStream);

        // -----------------------------------------------------------
        var resourceRoot = medicationDocument.RootElement;
        var resourceParts = resourcePath.Split('/');

        if (resourceParts.Length != 2 ||
            !resourceRoot.TryGetProperty("resourceType", out var resourceType0) ||
            !resourceRoot.TryGetProperty("id", out var resourceId) ||
            resourceType0.GetString() != resourceParts[0] ||
            resourceId.GetString() != resourceParts[1])
        {
            results.Add($"ERROR: {resourcePath} identity mismatch");
            return results;
        }

        results.Add($"LOADED: {resourcePath}");
        // -----------------------------------------------------------

        // 2. Автоматически извлекаем ссылки
        var references = ExtractReferences(
            medicationDocument.RootElement);

        // 3. Проверяем каждую ссылку
        foreach (var reference in references)
        {
            // Пока поддерживаем только относительные ссылки
            var parts = reference.Split('/');

            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                results.Add($"SKIP: {reference}");
                continue;
            }

            var url = $"{baseUrl}/{reference}";

            try
            {
                using var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    results.Add(
                        $"ERROR: {reference} returned HTTP " +
                        $"{(int)response.StatusCode}");

                    continue;
                }

                using var stream =
                    await response.Content.ReadAsStreamAsync();

                using var document =
                    await JsonDocument.ParseAsync(stream);

                var root = document.RootElement;

                var resourceType =
                    root.GetProperty("resourceType").GetString();

                var id = root.GetProperty("id").GetString();

                if (resourceType == parts[0] &&
                    id == parts[1])
                {
                    results.Add($"OK: {reference}");
                }
                else
                {
                    results.Add($"ERROR: {reference} mismatch");
                }
            }
            catch (Exception ex)
            {
                results.Add(
                    $"ERROR: {reference}: {ex.Message}");
            }
        }

        return results;
    }

    public static async Task<Dictionary<string, List<string>>>
        ValidateAllAsync(
            HttpClient client,
            string baseUrl,
            JsonElement bundle)
    {
        var resources = ExtractResourcePaths(bundle);

        var report = new Dictionary<string, List<string>>();

        foreach (var resource in resources)
        {
            var results = await ValidateAsync(
                client,
                baseUrl,
                resource);

            report[resource] = results;
        }

        return report;
    }

    public static List<string> ExtractReferences(
    JsonElement element)
    {
        var references = new List<string>();

        void Visit(JsonElement current)
        {
            if (current.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in current.EnumerateObject())
                {
                    if (property.Name == "reference" &&
                        property.Value.ValueKind ==
                            JsonValueKind.String)
                    {
                        var value = property.Value.GetString();

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            references.Add(value);
                        }
                    }
                    else
                    {
                        Visit(property.Value);
                    }
                }
            }
            else if (current.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in current.EnumerateArray())
                {
                    Visit(item);
                }
            }
        }

        Visit(element);

        return references.Distinct().ToList();
    }

    public static List<string> ExtractResourcePaths(
    JsonElement bundle)
    {
        var paths = new List<string>();

        if (!bundle.TryGetProperty("resourceType", out var type) ||
            type.GetString() != "Bundle")
        {
            return paths;
        }

        if (!bundle.TryGetProperty("entry", out var entries) ||
            entries.ValueKind != JsonValueKind.Array)
        {
            return paths;
        }

        foreach (var entry in entries.EnumerateArray())
        {
            if (!entry.TryGetProperty("resource", out var resource))
                continue;

            if (!resource.TryGetProperty("resourceType", out var rt) ||
                !resource.TryGetProperty("id", out var id))
                continue;

            var resourceType = rt.GetString();
            var resourceId = id.GetString();

            if (!string.IsNullOrWhiteSpace(resourceType) &&
                !string.IsNullOrWhiteSpace(resourceId))
            {
                paths.Add($"{resourceType}/{resourceId}");
            }
        }

        return paths.Distinct().ToList();
    }

    public static FhirValidationSummary CreateSummary(
        Dictionary<string, List<string>> report)
    {
        var allResults = report.Values
            .SelectMany(results => results)
            .ToList();

        return new FhirValidationSummary
        {
            TotalResources = report.Count,

            LoadedResources = allResults.Count(
                r => r.StartsWith("LOADED:")),

            FailedResources = allResults.Count(
                r => r.StartsWith("LOAD_ERROR:")),

            SuccessfulReferences = allResults.Count(
                r => r.StartsWith("OK:")),

            Errors = allResults.Count(
                r => r.StartsWith("ERROR:")),

            Skipped = allResults.Count(
                r => r.StartsWith("SKIP:"))
        };
    }



}
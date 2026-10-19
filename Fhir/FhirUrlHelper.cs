using Microsoft.AspNetCore.Http;

namespace EmergencyMedicalCard.Fhir;

public static class FhirUrlHelper
{
    public static string GetBaseUrl(HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}" +
               $"{request.PathBase}/fhir";
    }

    public static string GetResourceUrl(
        HttpRequest request,
        string resourceType,
        string id)
    {
        var baseUrl = GetBaseUrl(request);

        return $"{baseUrl}/{resourceType}/{id}";
    }
}
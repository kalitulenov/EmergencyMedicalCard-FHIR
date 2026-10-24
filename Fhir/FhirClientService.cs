using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace EmergencyMedicalCard.Fhir;

public class FhirClientService
{
    private readonly HttpClient _httpClient;

    public FhirClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ---------------------------------------------------------------------------

    public async Task<Patient?> GetPatientAsync(
        string baseUrl,
        string patientId)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/Patient/{Uri.EscapeDataString(patientId)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        if (response.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Patient>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> SearchPatientsAsync(
    string baseUrl,
    string name)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/Patient?name={Uri.EscapeDataString(name)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> GetNextPageAsync(
    string nextUrl)
    {
        if (!Uri.TryCreate(
                nextUrl,
                UriKind.Absolute,
                out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            !string.Equals(
                uri.Host,
                "hapi.fhir.org",
                StringComparison.OrdinalIgnoreCase) ||
            !uri.AbsolutePath.StartsWith(
                "/baseR4",
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Unexpected FHIR pagination URL.",
                nameof(nextUrl));
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get, uri);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<List<Patient>> SearchAllPatientsAsync(
    string baseUrl,
    string name,
    int maxPages = 3)
    {
        if (maxPages < 1 || maxPages > 10)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxPages));
        }

        var patients = new List<Patient>();

        var bundle = await SearchPatientsAsync(baseUrl, name);

        for (int page = 1; page <= maxPages; page++)
        {
            foreach (var entry in bundle.Entry)
            {
                if (entry.Resource is Patient patient)
                {
                    patients.Add(patient);
                }
            }

            var nextUrl = bundle.Link?
                .FirstOrDefault(link => link.Relation == "next")?
                .Url;

            if (string.IsNullOrWhiteSpace(nextUrl))
            {
                break;
            }

            if (page == maxPages)
            {
                break;
            }

            bundle = await GetNextPageAsync(nextUrl);
        }

        return patients;
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> SearchConditionsAsync(
        string baseUrl,
        string patientId)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/Condition?patient={Uri.EscapeDataString(patientId)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> SearchAllergiesAsync(
    string baseUrl,
    string patientId)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/AllergyIntolerance?patient={Uri.EscapeDataString(patientId)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> SearchMedicationRequestsAsync(
    string baseUrl,
    string patientId)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/MedicationRequest?patient={Uri.EscapeDataString(patientId)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

    // ---------------------------------------------------------------------------

    public async Task<Bundle> SearchObservationsAsync(
    string baseUrl,
    string patientId)
    {
        var url =
            $"{baseUrl.TrimEnd('/')}/Observation?patient={Uri.EscapeDataString(patientId)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get, url);

        request.Headers.Accept.ParseAdd(
            "application/fhir+json");

        using var response =
            await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var parser = new FhirJsonParser();

        return parser.Parse<Bundle>(json);
    }

// ---------------------------------------------------------------------------

    public async Task<PatientMedicalSummary>
    GetPatientMedicalSummaryAsync(
        string baseUrl,
        string patientId)
    {
        var conditions = await SearchConditionsAsync(
            baseUrl, patientId);

        var allergies = await SearchAllergiesAsync(
            baseUrl, patientId);

        var medications = await SearchMedicationRequestsAsync(
            baseUrl, patientId);

        var observations = await SearchObservationsAsync(
            baseUrl, patientId);

        return new PatientMedicalSummary
        {
            PatientId = patientId,

            Conditions = conditions.Entry
                .Select(e => e.Resource)
                .OfType<Condition>()
                .ToList(),

            Allergies = allergies.Entry
                .Select(e => e.Resource)
                .OfType<AllergyIntolerance>()
                .ToList(),

            Medications = medications.Entry
                .Select(e => e.Resource)
                .OfType<MedicationRequest>()
                .ToList(),

            Observations = observations.Entry
                .Select(e => e.Resource)
                .OfType<Observation>()
                .ToList()
        };
    }

    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------


    // ---------------------------------------------------------------------------









}
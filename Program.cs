using EmergencyMedicalCard.Components;
using EmergencyMedicalCard.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//  ---------------------------------------------------------
app.MapGet("/fhir/Patient/{id}", (string id) =>
{
    var patient = PatientFactory.CreatePatient();

    if (patient.Id != id)
    {
        var outcome = OperationOutcomeFactory.NotFound("Patient", id);

        var errorJson = new FhirJsonSerializer()
            .SerializeToString(outcome);

        return Results.Text(
            errorJson,
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    var serializer = new FhirJsonSerializer();

    var json = serializer.SerializeToString(patient);

    return Results.Text(
        json,
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

app.MapGet("/fhir/Observation/{id}", (string id) =>
{
    var observations = new[]
    {
        ObservationFactory.CreateBloodGroup(),
        ObservationFactory.CreateRhFactor()
    };

    var observation = observations
        .FirstOrDefault(o => o.Id == id);

    if (observation == null)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Observation", id);

        var jsonFhir = new FhirJsonSerializer()
            .SerializeToString(outcome);

        return Results.Text(
            jsonFhir,
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    var serializer = new Hl7.Fhir.Serialization.FhirJsonSerializer();

    var json = serializer.SerializeToString(observation);

    return Results.Text(
        json,
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

app.MapGet("/fhir/Observation",
        (string? patient, HttpContext context) =>
        {
            var observations = new List<Observation>
        {
            ObservationFactory.CreateBloodGroup(),
            ObservationFactory.CreateRhFactor()
        };

        // Параметр patient обязателен в нашем учебном API.
        if (string.IsNullOrWhiteSpace(patient))
        {
            var outcome = OperationOutcomeFactory.BadRequest(
                "Missing required search parameter: patient");

            var jsonFhir = new FhirJsonSerializer()
                .SerializeToString(outcome);

            return Results.Text(
                jsonFhir,
                "application/fhir+json",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        // Поддерживаем patient-001 и Patient/patient-001.
        var patientReference = patient.StartsWith("Patient/")
            ? patient
            : $"Patient/{patient}";

        var matchingObservations = observations
            .Where(o => o.Subject?.Reference == patientReference)
            .ToList();

        // Формируем FHIR Search Bundle.
        var searchBundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = matchingObservations.Count,

            Entry = matchingObservations.Select(o =>
                new Bundle.EntryComponent
                {
                    FullUrl =
                        $"https://hospital.example.org/fhir/Observation/{o.Id}",

                    Resource = o,

                    Search = new Bundle.SearchComponent
                    {
                        Mode = Bundle.SearchEntryMode.Match
                    }
                }).ToList()
        };

        var serializer = new FhirJsonSerializer();

        var json = serializer.SerializeToString(searchBundle);

        return Results.Text(
            json,
            "application/fhir+json");
    });

//  ---------------------------------------------------------

app.MapGet("/fhir/metadata", (HttpContext context) =>
{
    var request = context.Request;

    var baseUrl =
        $"{request.Scheme}://{request.Host}{request.PathBase}/fhir";

    var capability = CapabilityStatementFactory.Create(baseUrl);

    var serializer = new FhirJsonSerializer();

    var json = serializer.SerializeToString(capability);

    return Results.Text(
        json,
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

//app.MapGet("/fhir/Organization/{id}", (string id) =>
//{
//    var organization = OrganizationFactory.CreateLaboratory();

//    var serializer = new FhirJsonSerializer();

//    if (organization.Id != id)
//    {
//        var outcome = OperationOutcomeFactory.NotFound(
//            "Organization", id);

//        return Results.Text(
//            serializer.SerializeToString(outcome),
//            "application/fhir+json",
//            statusCode: StatusCodes.Status404NotFound
//        );
//    }

//    return Results.Text(
//        serializer.SerializeToString(organization),
//        "application/fhir+json"
//    );
//});

//  ---------------------------------------------------------

app.MapGet("/fhir/Organization/{id}", (string id) =>
{
    var organization = OrganizationFactory.CreateLaboratory();
    var serializer = new FhirJsonSerializer();

    if (organization.Id != id)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Organization", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(organization),
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------


app.MapGet("/fhir/AllergyIntolerance/{id}", (string id) =>
{
    var allergies = new List<AllergyIntolerance>
    {
        AllergyFactory.CreatePenicillinAllergy(),
        AllergyFactory.CreatePeanutAllergy()
    };

    var allergy = allergies.FirstOrDefault(a => a.Id == id);

    var serializer = new FhirJsonSerializer();

    if (allergy == null)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "AllergyIntolerance", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(allergy),
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

app.MapGet("/fhir/AllergyIntolerance",
    (string? patient) =>
    {
        var serializer = new FhirJsonSerializer();

        if (string.IsNullOrWhiteSpace(patient))
        {
            var outcome = OperationOutcomeFactory.BadRequest(
                "Missing required search parameter: patient");

            return Results.Text(
                serializer.SerializeToString(outcome),
                "application/fhir+json",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        var allergies = new List<AllergyIntolerance>
    {
        AllergyFactory.CreatePenicillinAllergy(),
        AllergyFactory.CreatePeanutAllergy()
    };

        var patientReference = patient.StartsWith("Patient/")
            ? patient
            : $"Patient/{patient}";

        var matches = allergies
            .Where(a => a.Patient?.Reference == patientReference)
            .ToList();

        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = matches.Count,

            Entry = matches.Select(a => new Bundle.EntryComponent
            {
                FullUrl =
                    $"https://hospital.example.org/fhir/AllergyIntolerance/{a.Id}",

                Resource = a,

                Search = new Bundle.SearchComponent
                {
                    Mode = Bundle.SearchEntryMode.Match
                }
            }).ToList()
        };

        return Results.Text(
            serializer.SerializeToString(bundle),
            "application/fhir+json"
        );
    });


//  ---------------------------------------------------------


//  ---------------------------------------------------------


//  ---------------------------------------------------------


//  ---------------------------------------------------------


app.Run();

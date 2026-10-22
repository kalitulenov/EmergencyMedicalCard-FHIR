using EmergencyMedicalCard.Components;
using EmergencyMedicalCard.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient(); // Добавляем IHttpClientFactory для создания HttpClient

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
                    FullUrl = FhirUrlHelper.GetResourceUrl(context.Request,"Observation",o.Id),
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
  //  var request = context.Request;

    var baseUrl = FhirUrlHelper.GetBaseUrl(context.Request);

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

app.MapGet("/fhir/AllergyIntolerance",(string? patient, HttpContext context) =>
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
                FullUrl = FhirUrlHelper.GetResourceUrl(context.Request, "AllergyIntolerance",a.Id),
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

app.MapGet("/fhir/Condition/{id}", (string id) =>
{
    var conditions = new List<Condition>
    {
        ConditionFactory.CreateHypertension(),
        ConditionFactory.CreateDiabetes()
    };

    var condition = conditions.FirstOrDefault(c => c.Id == id);

    var serializer = new FhirJsonSerializer();

    if (condition == null)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Condition", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(condition),
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

app.MapGet("/fhir/Condition", (string? patient, HttpContext context) => {
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

    var conditions = new List<Condition>
    {
        ConditionFactory.CreateHypertension(),
        ConditionFactory.CreateDiabetes()
    };

    var patientReference = patient.StartsWith("Patient/")
        ? patient
        : $"Patient/{patient}";

    var matches = conditions
        .Where(c => c.Subject?.Reference == patientReference)
        .ToList();

    var bundle = new Bundle
    {
        Type = Bundle.BundleType.Searchset,
        Total = matches.Count,

        Entry = matches.Select(c => new Bundle.EntryComponent
        {
            FullUrl = FhirUrlHelper.GetResourceUrl(context.Request,"Condition",c.Id),

            Resource = c,

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
app.MapGet("/fhir/MedicationStatement/{id}", (string id) =>
{
    var medications = new List<MedicationStatement>
    {
        MedicationFactory.CreateLisinopril(),
        MedicationFactory.CreateAtorvastatin()
    };

    var medication = medications.FirstOrDefault(m => m.Id == id);

    var serializer = new FhirJsonSerializer();

    if (medication == null)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "MedicationStatement", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(medication),
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------
app.MapGet("/fhir/MedicationStatement",(string? patient, HttpContext context) => {
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

    var medications = new List<MedicationStatement>
    {
        MedicationFactory.CreateLisinopril(),
        MedicationFactory.CreateAtorvastatin()
    };

    var patientReference = patient.StartsWith("Patient/")
        ? patient
        : $"Patient/{patient}";

    var matches = medications
        .Where(m => m.Subject?.Reference == patientReference)
        .ToList();

    var bundle = new Bundle
    {
        Type = Bundle.BundleType.Searchset,
        Total = matches.Count,

        Entry = matches.Select(m => new Bundle.EntryComponent
        {
            FullUrl = FhirUrlHelper.GetResourceUrl(context.Request, "MedicationStatement",m.Id),
            Resource = m,

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

app.MapGet("/fhir/MedicationRequest/{id}", (string id) =>
{
    var medicationRequest =
        MedicationRequestFactory.CreateLisinoprilRequest();

    var serializer = new FhirJsonSerializer();

    if (medicationRequest.Id != id)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "MedicationRequest", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(medicationRequest),
        "application/fhir+json"
    );
});

//  ---------------------------------------------------------

app.MapGet("/fhir/MedicationRequest",(string? patient, HttpContext context) => {
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

    var requests = new List<MedicationRequest>
    {
        MedicationRequestFactory.CreateLisinoprilRequest()
    };

    var patientReference = patient.StartsWith("Patient/")
        ? patient
        : $"Patient/{patient}";

    var matches = requests
        .Where(r => r.Subject?.Reference == patientReference)
        .ToList();

    var bundle = new Bundle
    {
        Type = Bundle.BundleType.Searchset,
        Total = matches.Count,

        Entry = matches.Select(r => new Bundle.EntryComponent
        {
            FullUrl = FhirUrlHelper.GetResourceUrl(context.Request, "MedicationRequest", r.Id),
            Resource = r,

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
app.MapGet("/fhir/Practitioner/{id}", (string id) =>
{
    var practitioner = PractitionerFactory.CreateDrBrown();

    var serializer = new FhirJsonSerializer();

    if (practitioner.Id != id)
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Practitioner", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    return Results.Text(
        serializer.SerializeToString(practitioner),
        "application/fhir+json"
    );
});

//  --------------------------------------------------------- TEST 
app.MapGet("/fhir/test-references",
    async (
        string? resource,
        HttpContext context,
        IHttpClientFactory clientFactory) =>
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            return Results.BadRequest(
                "Resource parameter is required.");
        }

        var parts = resource.Split('/');

        if (parts.Length != 2 ||
            string.IsNullOrWhiteSpace(parts[0]) ||
            string.IsNullOrWhiteSpace(parts[1]) ||
            parts.Any(p => p is "." or ".." ||
                           p.Contains('?') || p.Contains('#')))
        {
            return Results.BadRequest(
                "Expected format: ResourceType/id");
        }

        var client = clientFactory.CreateClient();

        var baseUrl =
            FhirUrlHelper.GetBaseUrl(context.Request);

        var results =
            await FhirApiReferenceValidator.ValidateAsync(
                client,
                baseUrl,
                resource);

        return Results.Json(results);
    });

//  ------------------------------ TEST все ресурсы, которые ссылаются на другие ресурсы в FHIR API

app.MapGet("/fhir/test-all-references",
    async (
        HttpContext context,
        IHttpClientFactory clientFactory) =>
    {
        var client = clientFactory.CreateClient();

        var baseUrl =
            FhirUrlHelper.GetBaseUrl(context.Request);

        // Создаём ресурсы медицинской карты
        var patient = PatientFactory.CreatePatient();

        var allergies = new List<AllergyIntolerance>
            {
                AllergyFactory.CreatePenicillinAllergy(),
                AllergyFactory.CreatePeanutAllergy()
            };

        var conditions = new List<Condition>
            {
                ConditionFactory.CreateHypertension(),
                ConditionFactory.CreateDiabetes()
            };

        var medications = new List<MedicationStatement>
            {
                MedicationFactory.CreateLisinopril(),
                MedicationFactory.CreateAtorvastatin()
            };

        var observations = new List<Observation>
            {
                ObservationFactory.CreateBloodGroup(),
                ObservationFactory.CreateRhFactor()
            };

        var practitioners = new List<Practitioner>
            {
                PractitionerFactory.CreateDrBrown()
            };

        var medicationRequests = new List<MedicationRequest>
            {
                MedicationRequestFactory.CreateLisinoprilRequest()
            };

        // Формируем Bundle
        var bundle = BundleFactory.CreateEmergencyBundle(
            patient,
            allergies,
            conditions,
            medications,
            observations,
            practitioners,
            medicationRequests);

        // Преобразуем Bundle в JSON
        var serializer = new FhirJsonSerializer();

        var json = serializer.SerializeToString(bundle);

        using var document = JsonDocument.Parse(json);

        // Автоматически проверяем все ресурсы
        var report =
            await FhirApiReferenceValidator.ValidateAllAsync(
                client,
                baseUrl,
                document.RootElement);

        var summary =
            FhirApiReferenceValidator.CreateSummary(report);

        return Results.Json(new
        {
            Summary = summary,
            Resources = report
        });
    });

//  ---------------------------------------------------------



app.MapGet("/fhir/test-extract-references", () =>
{
    var medicationRequest =
        MedicationRequestFactory.CreateLisinoprilRequest();

    var serializer = new FhirJsonSerializer();

    var json = serializer.SerializeToString(
        medicationRequest);

    using var document = JsonDocument.Parse(json);

    var references =
        FhirApiReferenceValidator.ExtractReferences(
            document.RootElement);

    return Results.Json(references);
});

//  ---------------------------------------------------------
app.MapGet("/fhir/Bundle/{id}", (string id) =>
{
    var serializer = new FhirJsonSerializer();

    if (id != "emergency-bundle-001")
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Bundle", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound);
    }

    var patient = PatientFactory.CreatePatient();

    var allergies = new List<AllergyIntolerance>
    {
        AllergyFactory.CreatePenicillinAllergy(),
        AllergyFactory.CreatePeanutAllergy()
    };

    var conditions = new List<Condition>
    {
        ConditionFactory.CreateHypertension(),
        ConditionFactory.CreateDiabetes()
    };

    var medications = new List<MedicationStatement>
    {
        MedicationFactory.CreateLisinopril(),
        MedicationFactory.CreateAtorvastatin()
    };

    var observations = new List<Observation>
    {
        ObservationFactory.CreateBloodGroup(),
        ObservationFactory.CreateRhFactor()
    };

    var practitioners = new List<Practitioner>
    {
        PractitionerFactory.CreateDrBrown()
    };

    var medicationRequests = new List<MedicationRequest>
    {
        MedicationRequestFactory.CreateLisinoprilRequest()
    };

    var bundle = BundleFactory.CreateEmergencyBundle(
        patient,
        allergies,
        conditions,
        medications,
        observations,
        practitioners,
        medicationRequests);

    return Results.Text(
        serializer.SerializeToString(bundle),
        "application/fhir+json");
});

//  ---------------------------------------------------------
app.MapGet("/fhir/Patient/{id}/$everything", (string id, HttpRequest request) =>
{
    var serializer = new FhirJsonSerializer();

    if (id != "patient-001")
    {
        var outcome = OperationOutcomeFactory.NotFound(
            "Patient", id);

        return Results.Text(
            serializer.SerializeToString(outcome),
            "application/fhir+json",
            statusCode: StatusCodes.Status404NotFound);
    }

    var patient = PatientFactory.CreatePatient();

    var bundle = BundleFactory.CreateEmergencyBundle(
        patient,
        new List<AllergyIntolerance>
        {
            AllergyFactory.CreatePenicillinAllergy(),
            AllergyFactory.CreatePeanutAllergy()
        },
        new List<Condition>
        {
            ConditionFactory.CreateHypertension(),
            ConditionFactory.CreateDiabetes()
        },
        new List<MedicationStatement>
        {
            MedicationFactory.CreateLisinopril(),
            MedicationFactory.CreateAtorvastatin()
        },
        new List<Observation>
        {
            ObservationFactory.CreateBloodGroup(),
            ObservationFactory.CreateRhFactor()
        },
        new List<Practitioner>
        {
            PractitionerFactory.CreateDrBrown()
        },
        new List<MedicationRequest>
        {
            MedicationRequestFactory.CreateLisinoprilRequest()
        });

    // The Patient/$everything operation returns a searchset Bundle.
    bundle.Type = Bundle.BundleType.Searchset;
    bundle.Total = bundle.Entry.Count;

    // Give the response Bundle its own identity.
    bundle.Id = Guid.NewGuid().ToString();

    var baseUrl = FhirUrlHelper.GetBaseUrl(request);

    bundle.Link = new List<Bundle.LinkComponent>
    {
        new Bundle.LinkComponent
        {
            Relation = "self",
            Url = $"{baseUrl}/Patient/{id}/$everything"
        }
    };

    const string oldBaseUrl = "https://hospital.example.org/fhir/";

    foreach (var entry in bundle.Entry)
    {
        if (entry.Resource == null)
            continue;

        entry.Search = new Bundle.SearchComponent
        {
            Mode = Bundle.SearchEntryMode.Match
        };

        var resourceType = entry.Resource.TypeName;
        var resourceId = entry.Resource.Id;

        if (string.IsNullOrWhiteSpace(resourceId))
            continue;

        entry.FullUrl = $"{baseUrl}/{resourceType}/{resourceId}";

        // Update references in known resource types
        switch (entry.Resource)
        {
            case AllergyIntolerance allergy:
                UpdateReference(allergy.Patient);
                break;

            case Condition condition:
                UpdateReference(condition.Subject);
                break;

            case MedicationStatement medication:
                UpdateReference(medication.Subject);
                break;

            case MedicationRequest requestResource:
                UpdateReference(requestResource.Subject);
                UpdateReference(requestResource.Requester);

                foreach (var reason in requestResource.ReasonReference ?? [])
                    UpdateReference(reason);
                break;

            case Observation observation:
                UpdateReference(observation.Subject);

                foreach (var performer in observation.Performer ?? [])
                    UpdateReference(performer);
                break;
        }
    }

    void UpdateReference(ResourceReference? reference)
    {
        if (reference?.Reference?.StartsWith(
            oldBaseUrl, StringComparison.Ordinal) == true)
        {
            reference.Reference =
                baseUrl + "/" +
                reference.Reference.Substring(oldBaseUrl.Length);
        }
    }


    return Results.Text(
        serializer.SerializeToString(bundle),
        "application/fhir+json");
});

//  ---------------------------------------------------------
app.MapGet("/fhir/test-everything", async (
    HttpRequest request,
    IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var baseUrl = FhirUrlHelper.GetBaseUrl(request);

    using var metadataResponse =
        await client.GetAsync($"{baseUrl}/metadata");

    using var everythingResponse =
        await client.GetAsync(
            $"{baseUrl}/Patient/patient-001/$everything");

    if (!metadataResponse.IsSuccessStatusCode ||
        !everythingResponse.IsSuccessStatusCode)
    {
        return Results.Json(new
        {
            success = false,
            metadataStatus = (int)metadataResponse.StatusCode,
            everythingStatus = (int)everythingResponse.StatusCode
        });
    }

    using var metadata = JsonDocument.Parse(
        await metadataResponse.Content.ReadAsStringAsync());

    using var everything = JsonDocument.Parse(
        await everythingResponse.Content.ReadAsStringAsync());

    var patient = metadata.RootElement
        .GetProperty("rest")[0]
        .GetProperty("resource")
        .EnumerateArray()
        .First(r => r.GetProperty("type").GetString() == "Patient");

    var operationDeclared =
        patient.TryGetProperty("operation", out var operations) &&
        operations.EnumerateArray().Any(op =>
            op.GetProperty("name").GetString() == "everything");

    var root = everything.RootElement;

    var bundleType = root.GetProperty("type").GetString();
    var total = root.GetProperty("total").GetInt32();
    var entryCount = root.GetProperty("entry").GetArrayLength();

    return Results.Json(new
    {
        success = operationDeclared &&
                  bundleType == "searchset" &&
                  total == 12 &&
                  entryCount == 12,

        operationDeclared,
        bundleType,
        total,
        entryCount
    });
});

//  ---------------------------------------------------------


//  ---------------------------------------------------------


//  ---------------------------------------------------------


//  ---------------------------------------------------------


app.Run();

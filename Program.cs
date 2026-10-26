using EmergencyMedicalCard.Components;
using EmergencyMedicalCard.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient(); // Добавляем IHttpClientFactory для создания HttpClient

builder.Services.AddHttpClient<FhirClientService>();

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
app.MapGet("/fhir/test-client", async (
    FhirClientService fhirClient,
    HttpRequest request) =>
{
    var baseUrl = FhirUrlHelper.GetBaseUrl(request);

    var patient = await fhirClient.GetPatientAsync(
        baseUrl,
        "patient-001");

    if (patient == null)
    {
        return Results.NotFound();
    }

    return Results.Json(new
    {
        patient.Id,
        Name = patient.Name?.FirstOrDefault()?.ToString(),
        Gender = patient.Gender?.ToString()
    });
});

//  --------------------------------------------------------- ПОЛУЧАЕМ ДАННЫЕ ИЗ ВНЕШНЕГО СЕРВЕРА
app.MapGet("/fhir/test-external-client", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    try
    {
        var patient = await fhirClient.GetPatientAsync(
            baseUrl,
            "example");

        if (patient == null)
        {
            return Results.Json(new
            {
                success = false,
                message = "Patient/example not found"
            });
        }

        return Results.Json(new
        {
            success = true,
            source = baseUrl,
            patient.Id,
            Name = patient.Name?.FirstOrDefault()?.ToString(),
            Gender = patient.Gender?.ToString()
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-conditions", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var bundle = await fhirClient.SearchConditionsAsync(
            baseUrl, patientId);

        var conditions = bundle.Entry
            .Where(e => e.Resource is Condition)
            .Select(e =>
            {
                var condition = (Condition)e.Resource;

                return new
                {
                    condition.Id,
                    Code = condition.Code?.Coding?
                        .FirstOrDefault()?.Code,
                    Display = condition.Code?.Coding?
                        .FirstOrDefault()?.Display,
                    ClinicalStatus = condition.ClinicalStatus?
                        .Coding?.FirstOrDefault()?.Code
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = true,
            patientId,
            returned = conditions.Count,
            conditions
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------
app.MapGet("/fhir/test-external-search", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    try
    {

        // Асинхронно ищет пациентов по строке "Smith" через FHIR-клиент.
        // Из полученного Bundle берёт только записи, чей Resource — это Patient.
        // Проецирует каждого в анонимный объект { Id, Name, Gender }.
        // Материализует результат в List.

        var bundle = await fhirClient.SearchPatientsAsync(
            baseUrl,
            "Smith");

        var patients = bundle.Entry
            .Where(e => e.Resource is Patient)
            .Select(e =>
            {
                var patient = (Patient)e.Resource;

                return new
                {
                    patient.Id,
                    Name = patient.Name?
                        .FirstOrDefault()?
                        .ToString(),
                    Gender = patient.Gender?.ToString()
                };
            })
            .ToList();

        var nextUrl = bundle.Link?
            .FirstOrDefault(link => link.Relation == "next")?
            .Url;

        return Results.Json(new
        {
            success = true,
            bundleType = bundle.Type?.ToString(),
            total = bundle.Total,
            returned = patients.Count,
            hasNextPage = !string.IsNullOrWhiteSpace(nextUrl),
            nextUrl,
            patients
        });

    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------
app.MapGet("/fhir/test-external-page2", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    try
    {
        var firstPage = await fhirClient.SearchPatientsAsync(
            baseUrl, "Smith");

        var nextUrl = firstPage.Link?
            .FirstOrDefault(link => link.Relation == "next")?
            .Url;

        if (string.IsNullOrWhiteSpace(nextUrl))
        {
            return Results.Json(new
            {
                success = true,
                message = "No next page available"
            });
        }

        var secondPage =
            await fhirClient.GetNextPageAsync(nextUrl);

        var patients = secondPage.Entry
            .Where(e => e.Resource is Patient)
            .Select(e =>
            {
                var patient = (Patient)e.Resource;

                return new
                {
                    patient.Id,
                    Name = patient.Name?
                        .FirstOrDefault()?
                        .ToString()
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = true,
            page = 2,
            returned = patients.Count,
            firstPatient = patients.FirstOrDefault(),
            hasNextPage = secondPage.Link?
                .Any(link => link.Relation == "next") == true
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------
app.MapGet("/fhir/test-external-all-pages", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    try
    {
        var patients =
            await fhirClient.SearchAllPatientsAsync(
                baseUrl,
                "Smith",
                maxPages: 3);

        return Results.Json(new
        {
            success = true,
            totalLoaded = patients.Count,
            firstPatient = patients.FirstOrDefault()?.Id,
            lastPatient = patients.LastOrDefault()?.Id
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-allergies", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var bundle = await fhirClient.SearchAllergiesAsync(
            baseUrl, patientId);

        var allergies = bundle.Entry
            .Where(e => e.Resource is AllergyIntolerance)
            .Select(e =>
            {
                var allergy = (AllergyIntolerance)e.Resource;

                return new
                {
                    allergy.Id,
                    Code = allergy.Code?.Coding?
                        .FirstOrDefault()?.Code,
                    Display = allergy.Code?.Text
                        ?? allergy.Code?.Coding?
                            .FirstOrDefault()?.Display,
                    ClinicalStatus = allergy.ClinicalStatus?
                        .Coding?.FirstOrDefault()?.Code
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = true,
            patientId,
            returned = allergies.Count,
            allergies
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-medications", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var bundle =
            await fhirClient.SearchMedicationRequestsAsync(
                baseUrl, patientId);

        var medications = bundle.Entry
            .Where(e => e.Resource is MedicationRequest)
            .Select(e =>
            {
                var medication = (MedicationRequest)e.Resource;

                var medicationName =
                    (medication.Medication as CodeableConcept)?
                        .Text
                    ?? (medication.Medication as CodeableConcept)?
                        .Coding?.FirstOrDefault()?.Display
                    ?? (medication.Medication as ResourceReference)?
                        .Reference;

                return new
                {
                    medication.Id,
                    Medication = medicationName,
                    Status = medication.Status?.ToString(),
                    Intent = medication.Intent?.ToString()
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = true,
            patientId,
            returned = medications.Count,
            medications
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-observations", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var bundle =
            await fhirClient.SearchObservationsAsync(
                baseUrl, patientId);

        var observations = bundle.Entry
            .Where(e => e.Resource is Observation)
            .Select(e =>
            {
                var observation = (Observation)e.Resource;

                var value = observation.Value switch
                {
                    Quantity q =>
                        $"{q.Value} {q.Unit}",

                    CodeableConcept c =>
                        c.Text ??
                        c.Coding?.FirstOrDefault()?.Display,

                    FhirString s => s.Value,

                    _ => observation.Value?.ToString()
                };

                return new
                {
                    observation.Id,
                    Code = observation.Code?.Coding?
                        .FirstOrDefault()?.Code,
                    Display = observation.Code?.Text ??
                        observation.Code?.Coding?
                            .FirstOrDefault()?.Display,
                    Value = value,
                    Status = observation.Status.ToString()
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = true,
            patientId,
            returned = observations.Count,
            observations
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-medical-summary", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var summary =
            await fhirClient.GetPatientMedicalSummaryAsync(
                baseUrl, patientId);

        return Results.Json(new
        {
            success = true,
            summary.PatientId,
            conditions = summary.Conditions.Count,
            allergies = summary.Allergies.Count,
            medications = summary.Medications.Count,
            observations = summary.Observations.Count
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-bundle", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var summary =
            await fhirClient.GetPatientMedicalSummaryAsync(
                baseUrl, patientId);

        var patient = await fhirClient.GetPatientAsync(
            baseUrl, patientId);

        if (patient == null)
        {
            return Results.NotFound(
                $"Patient/{patientId} not found");
        }

        var bundle = BundleFactory.CreateMedicalBundle(
            patient, summary, baseUrl);

        return Results.Json(new
        {
            success = true,
            bundleId = bundle.Id,
            bundleType = bundle.Type?.ToString(),
            entryCount = bundle.Entry.Count,
            resources = bundle.Entry.Select(e => new
            {
                resourceType = e.Resource?.TypeName,
                id = e.Resource?.Id,
                fullUrl = e.FullUrl
            })
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-external-references", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var patient = await fhirClient.GetPatientAsync(
            baseUrl, patientId);

        if (patient == null)
        {
            return Results.NotFound();
        }

        var summary =
            await fhirClient.GetPatientMedicalSummaryAsync(
                baseUrl, patientId);

        var bundle = BundleFactory.CreateMedicalBundle(
            patient, summary, baseUrl);

        var patientFullUrl =
            $"{baseUrl}/Patient/{patient.Id}";

        var results = bundle.Entry
            .Select(e => e.Resource)
            .OfType<Observation>()
            .Select(observation =>
            {
                var reference =
                    observation.Subject?.Reference;

                var valid =
                    reference == $"Patient/{patient.Id}" ||
                    reference == patientFullUrl;

                return new
                {
                    observationId = observation.Id,
                    subjectReference = reference,
                    valid
                };
            })
            .ToList();

        return Results.Json(new
        {
            success = results.All(r => r.valid)
                && results.Count > 0,
            patientId,
            observationsChecked = results.Count,
            validReferences = results.Count(r => r.valid),
            invalidReferences = results.Count(r => !r.valid),
            results
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

//  ---------------------------------------------------------

app.MapGet("/fhir/export-external-bundle", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";
    const string patientId = "36149";

    try
    {
        var patient = await fhirClient.GetPatientAsync(
            baseUrl, patientId);

        if (patient == null)
        {
            return Results.NotFound(
                $"Patient/{patientId} not found");
        }

        patient.Meta ??= new Meta();

        const string usCorePatientProfile =
            "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient";

        patient.Meta.ProfileElement.Add(
            new Canonical(usCorePatientProfile));

        var summary =
            await fhirClient.GetPatientMedicalSummaryAsync(
                baseUrl, patientId);

        Console.WriteLine(
                $"Observations before normalization: {summary.Observations.Count}");

        foreach (var observation in summary.Observations)
        {
            var code = observation.Code?.Coding?
                .FirstOrDefault(c => c.System == "http://loinc.org")?.Code;

            Console.WriteLine(
                $"Observation ID={observation.Id}, LOINC={code}");
        }



        var systolic = summary.Observations
            .First(o => o.Id == "61195");

        var diastolic = summary.Observations
            .First(o => o.Id == "61196");

        var normalizedBp = BloodPressureFactory.Create(
            systolic, diastolic);

        normalizedBp.Subject = new ResourceReference(
                                    $"{baseUrl.TrimEnd('/')}/Patient/{patient.Id}");

        summary.Observations.Remove(systolic);

        summary.Observations.Remove(diastolic);

        summary.Observations.Add(normalizedBp);


        foreach (var observation in summary.Observations)
        {
            if (observation.Id == "61195" ||
                observation.Id == "61196" ||
                observation.Id == "61197" ||
                observation.Id == "61198")
            {
                continue;
            }

            Console.WriteLine(
                $"Observation ID: {observation.Id}");

            Console.WriteLine(
                $"Status: {observation.Status}");

            Console.WriteLine(
                $"Code Text: {observation.Code?.Text}");

            foreach (var coding in
                observation.Code?.Coding ?? new List<Coding>())
            {
                Console.WriteLine(
                    $"System: {coding.System}, " +
                    $"Code: {coding.Code}, " +
                    $"Display: {coding.Display}");
            }

            Console.WriteLine(
                $"Value Type: {observation.Value?.GetType().Name}");

            if (observation.Value is FhirString textValue)
            {
                Console.WriteLine(
                    $"Value: {textValue.Value}");
            }

            Console.WriteLine("----------------------");
        }

        var testObservations = summary.Observations
                .Where(ObservationClassifier.IsTestObservation)
                .ToList();

        var otherObservations = summary.Observations
            .Where(o => !ObservationClassifier.IsTestObservation(o))
            .ToList();

        Console.WriteLine(
            $"Total Observations: {summary.Observations.Count}");

        Console.WriteLine(
            $"Test Observations: {testObservations.Count}");

        Console.WriteLine(
            $"Other Observations: {otherObservations.Count}");

        var patientUrl =
            $"{baseUrl.TrimEnd('/')}/Patient/{patient.Id}";

        var normalizedObservations =
            FhirObservationNormalizer.Normalize(
                summary.Observations,
                patientUrl);

        summary.Observations.Clear();
        summary.Observations.AddRange(normalizedObservations);

        var emergencyObservations =
            EmergencyCardFilter.SelectObservations(
                    summary.Observations);

        Console.WriteLine(
            $"All Observations: {summary.Observations.Count}");

        Console.WriteLine(
            $"Emergency Observations: {emergencyObservations.Count}");

        foreach (var observation in emergencyObservations)
        {
            Console.WriteLine(
                $"Emergency Observation: {observation.Id}");
        }

        Console.WriteLine(
            $"Observations after normalization: {summary.Observations.Count}");

        var bundle = BundleFactory.CreateMedicalBundle(
            patient, summary, baseUrl);

        var serializer = new FhirJsonSerializer();

        var json = serializer.SerializeToString(bundle);

        return Results.Text(
            json,
            "application/fhir+json");
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            error = ex.Message
        });
    }
});

// ----------------------------------------------------------

app.MapGet("/fhir/emergency-card",
    async (FhirClientService fhirClient, bool demo = false) =>
    {
        const string patientId = "36149";
        const string baseUrl = "https://hapi.fhir.org/baseR4";

        var patient = await fhirClient.GetPatientAsync(baseUrl, patientId);

        if (patient == null)
        {
            return Results.NotFound("Patient not found.");
        }

        var summary = await fhirClient.GetPatientMedicalSummaryAsync(
                baseUrl, patientId);

        var patientUrl =
            $"{baseUrl}/Patient/{patient.Id}";

        var normalizedObservations =
            FhirObservationNormalizer.Normalize(
                summary.Observations,
                patientUrl);

        var emergencyObservations =
            EmergencyCardFilter.SelectObservations(
                normalizedObservations);

        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Collection,
            Timestamp = DateTimeOffset.UtcNow
        };

        bundle.Entry.Add(new Bundle.EntryComponent
        {
            FullUrl = patientUrl,
            Resource = patient
        });

        foreach (var condition in summary.Conditions)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"{baseUrl}/Condition/{condition.Id}",
                Resource = condition
            });
        }

        foreach (var allergy in summary.Allergies)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"{baseUrl}/AllergyIntolerance/{allergy.Id}",
                Resource = allergy
            });
        }

        if (demo)
        {
            var demoAllergy =
                DemoAllergyFactory.Create(patientId);

            demoAllergy.Patient =
                new ResourceReference(patientUrl);

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{Guid.NewGuid()}",
                Resource = demoAllergy
            });
        }

        if (demo)
        {
            var demoCondition =
                DemoConditionFactory.Create(patientId);

            demoCondition.Subject =
                new ResourceReference(patientUrl);

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{Guid.NewGuid()}",
                Resource = demoCondition
            });
        }

        if (demo)
        {
            var demoMedication =
                DemoMedicationRequestFactory.Create(patientId);

            Console.WriteLine(
                $"Medication requester: {demoMedication.Requester?.Reference}");

            demoMedication.Subject =
                new ResourceReference(patientUrl);

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{Guid.NewGuid()}",
                Resource = demoMedication
            });


        }



        foreach (var medication in summary.Medications)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"{baseUrl}/MedicationRequest/{medication.Id}",
                Resource = medication
            });
        }

        foreach (var observation in emergencyObservations)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl =
                    $"{baseUrl}/Observation/{observation.Id}",
                Resource = observation
            });
        }
        // Add other resource types (Practitioner, MedicationRequest, etc.) as needed
        Console.WriteLine("=== Emergency Medical Card ===");

        Console.WriteLine(
            $"Conditions: {summary.Conditions.Count}");

        Console.WriteLine(
            $"Allergies: {summary.Allergies.Count}");

        Console.WriteLine(
            $"MedicationRequests: {summary.Medications.Count}");

        Console.WriteLine(
            $"Observations: {emergencyObservations.Count}");

        Console.WriteLine(
            $"Total Bundle Resources: {bundle.Entry.Count}");
        // 

        var serializer = new FhirJsonSerializer();

        return Results.Text(
            serializer.SerializeToString(bundle),
            "application/fhir+json");
    });

//  ---------------------------------------------------------

app.MapGet("/fhir/test-blood-pressure-details", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    var bundle = await fhirClient.SearchObservationsAsync(
        baseUrl, "36149");

    var results = bundle.Entry
        .Select(e => e.Resource)
        .OfType<Observation>()
        .Where(o => o.Id == "61195" || o.Id == "61196")
        .Select(o => new
        {
            o.Id,
            Code = o.Code?.Coding?.FirstOrDefault()?.Code,
            Effective = o.Effective?.ToString(),
            Issued = o.Issued?.ToString(),
            Subject = o.Subject?.Reference,
            Value = (o.Value as Quantity)?.Value,
            Unit = (o.Value as Quantity)?.Unit,
            System = (o.Value as Quantity)?.System,
            UnitCode = (o.Value as Quantity)?.Code,
            Categories = o.Category.SelectMany(
                c => c.Coding.Select(x => new
                {
                    x.System,
                    x.Code
                }))
        });

    return Results.Json(results);
});

//  ---------------------------------------------------------

app.MapGet("/fhir/test-normalized-bp", async (
    FhirClientService fhirClient) =>
{
    const string baseUrl = "https://hapi.fhir.org/baseR4";

    var bundle = await fhirClient.SearchObservationsAsync(
        baseUrl, "36149");

    var observations = bundle.Entry
        .Select(e => e.Resource)
        .OfType<Observation>()
        .ToList();

    var systolic = observations
        .First(o => o.Id == "61195");

    var diastolic = observations
        .First(o => o.Id == "61196");

    var bp = BloodPressureFactory.Create(
        systolic, diastolic);

    var serializer = new FhirJsonSerializer();

    return Results.Text(
        serializer.SerializeToString(bp),
        "application/fhir+json");
});

//  ---------------------------------------------------------


app.Run();

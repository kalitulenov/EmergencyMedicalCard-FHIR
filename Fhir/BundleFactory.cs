
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class BundleFactory
{
    private const string BaseUrl = "https://hospital.example.org/fhir/";

    public static Bundle CreateEmergencyBundle(
        Patient patient,
        List<AllergyIntolerance> allergies,
        List<Condition> conditions,
        List<MedicationStatement> medications,
        List<Observation> observations,
        List<Practitioner> practitioners,
        List<MedicationRequest> medicationRequests)

    {
        var patientUrl = BaseUrl + "Patient/" + patient.Id;

        var bundle = new Bundle
        {
            Id = "emergency-bundle-001",
            Type = Bundle.BundleType.Collection
        };

        bundle.Entry.Add(new Bundle.EntryComponent
        {
            FullUrl = patientUrl,
            Resource = patient
        });

        foreach (var allergy in allergies)
        {
            // Ссылка должна соответствовать
            // адресу пациента в Bundle.
            allergy.Patient = new ResourceReference
            {
                Reference = patientUrl,
                Display = "John Smith"
            };

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "AllergyIntolerance/" + allergy.Id,

                Resource = allergy
            });
        }

        foreach (var condition in conditions)
        {
            condition.Subject = new ResourceReference
            {
                Reference = patientUrl,
                Display = "John Smith"
            };

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "Condition/" + condition.Id,

                Resource = condition
            });
        }

        foreach (var medication in medications)
        {
            medication.Subject = new ResourceReference
            {
                Reference = patientUrl,
                Display = "John Smith"
            };

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "MedicationStatement/" + medication.Id,

                Resource = medication
            });
        }

        foreach (var practitioner in practitioners)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "Practitioner/" + practitioner.Id,

                Resource = practitioner
            });
        }

        foreach (var request in medicationRequests)
        {
            request.Subject = new ResourceReference
            {
                Reference = patientUrl,
                Display = "John Smith"
            };

            request.Requester = new ResourceReference
            {
                Reference =
                    BaseUrl + "Practitioner/practitioner-001",

                Display = "Dr. Robert Brown"
            };

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "MedicationRequest/" + request.Id,

                Resource = request
            });
        }

        foreach (var observation in observations)
        {
            observation.Subject = new ResourceReference
            {
                Reference = patientUrl,
                Display = "John Smith"
            };

            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = BaseUrl +
                    "Observation/" + observation.Id,

                Resource = observation
            });
        }

        return bundle;
    }
}

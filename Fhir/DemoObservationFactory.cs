using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoObservationFactory
{
    public static List<Observation> Create(
        string patientReference,
        string practitionerReference)
    {
        var observations = new List<Observation>
{
            CreateQuantity(
                "demo-heart-rate-001",
                "8867-4",
                "Heart rate",
                65,
                "/min",
                "beats/minute",
                "vital-signs",
                patientReference),

            CreateQuantity(
                "demo-glucose-001",
                "2339-0",
                "Glucose [Mass/volume] in Blood",
                92,
                "mg/dL",
                "mg/dL",
                "laboratory",
                patientReference),

            CreateBloodPressure(patientReference)
        };

        foreach (var observation in observations)
        {
                    observation.Performer = new List<ResourceReference>
            {
                new ResourceReference(practitionerReference)
            };
        }

        var glucose = observations.First(o => o.Id == "demo-glucose-001");

        glucose.Text = new Narrative
        {
            Status = Narrative.NarrativeStatus.Generated,

            Div =
                "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<p>Blood glucose: 92 mg/dL</p>" +
                "<p>Demonstration measurement - synthetic data</p>" +
                "</div>"
        };


        return observations;

        //return new List<Observation>
        //{
        //    CreateQuantity(
        //        "demo-heart-rate-001",
        //        "8867-4",
        //        "Heart rate",
        //        65,
        //        "/min",
        //        "beats/minute",
        //        "vital-signs",
        //        patientReference),

        //    CreateQuantity(
        //        "demo-glucose-001",
        //        "2339-0",
        //        "Glucose [Mass/volume] in Blood",
        //        92,
        //        "mg/dL",
        //        "mg/dL",
        //        "laboratory",
        //        patientReference),

        //    CreateBloodPressure(patientReference)
        //};
    }

    private static Observation CreateQuantity(
        string id,
        string loincCode,
        string display,
        decimal value,
        string unitCode,
        string unitDisplay,
        string category,
        string patientReference)
    {
        return new Observation
        {
            Id = id,
            Status = ObservationStatus.Final,

            Effective = new FhirDateTime("2026-10-01T10:00:00Z"),

            Performer = new List<ResourceReference>
                {
                    new ResourceReference("Practitioner/demo-doctor-001")
                },

            Category = new List<CodeableConcept>
            {
                new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/observation-category",
                    category)
            },

            Code = new CodeableConcept(
                "http://loinc.org",
                loincCode,
                display),

            Subject = new ResourceReference(patientReference),

            Value = new Quantity
            {
                Value = value,
                Unit = unitDisplay,
                System = "http://unitsofmeasure.org",
                Code = unitCode
            }
        };
    }

    private static Observation CreateBloodPressure(
        string patientReference)
    {
        return new Observation
        {
            Id = "demo-bp-001",

            Status = ObservationStatus.Final,

            Effective = new FhirDateTime("2026-10-01T10:00:00Z"),

            Performer = new List<ResourceReference>
                {
                    new ResourceReference("Practitioner/demo-doctor-001")
                },

            Category = new List<CodeableConcept>
            {
                new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/observation-category",
                    "vital-signs")
            },

            Code = new CodeableConcept(
                "http://loinc.org",
                "85354-9",
                "Blood pressure panel"),

            Subject = new ResourceReference(patientReference),

            Component = new List<Observation.ComponentComponent>
            {
                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept(
                        "http://loinc.org",
                        "8480-6",
                        "Systolic blood pressure"),

                    Value = new Quantity
                    {
                        Value = 128,
                        Unit = "mmHg",
                        System = "http://unitsofmeasure.org",
                        Code = "mm[Hg]"
                    }
                },

                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept(
                        "http://loinc.org",
                        "8462-4",
                        "Diastolic blood pressure"),

                    Value = new Quantity
                    {
                        Value = 82,
                        Unit = "mmHg",
                        System = "http://unitsofmeasure.org",
                        Code = "mm[Hg]"
                    }
                }
            }
        };
    }
}
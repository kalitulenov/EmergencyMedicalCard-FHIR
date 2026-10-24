using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class BloodPressureFactory
{
    public static Observation Create(
        Observation systolic,
        Observation diastolic)
    {
        if (systolic.Subject?.Reference !=
            diastolic.Subject?.Reference)
        {
            throw new ArgumentException(
                "Observations belong to different patients.");
        }

        if (systolic.Effective?.ToString() !=
            diastolic.Effective?.ToString())
        {
            throw new ArgumentException(
                "Observation times do not match.");
        }

        if (systolic.Value is not Quantity sys ||
            diastolic.Value is not Quantity dia ||
            sys.Value == null ||
            dia.Value == null ||
            sys.Unit != "mmHg" ||
            dia.Unit != "mmHg")
        {
            throw new ArgumentException(
                "Expected blood pressure values in mmHg.");
        }

        return new Observation
        {
            Id = "bp-36149-001",

            Status = ObservationStatus.Final,

            Category =
            {
                new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/observation-category",
                    "vital-signs",
                    "Vital Signs")
            },

            Code = new CodeableConcept(
                "http://loinc.org",
                "85354-9",
                "Blood pressure panel with all children optional"),

            Subject = new ResourceReference(
                systolic.Subject!.Reference),

            Effective = systolic.Effective?.DeepCopy()
                as DataType,

            Component =
            {
                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept(
                        "http://loinc.org",
                        "8480-6",
                        "Systolic blood pressure"),

                    Value = new Quantity
                    {
                        Value = sys.Value,
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
                        Value = dia.Value,
                        Unit = "mmHg",
                        System = "http://unitsofmeasure.org",
                        Code = "mm[Hg]"
                    }
                }
            }
        };
    }
}
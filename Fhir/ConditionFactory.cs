using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ConditionFactory
{
    public static Condition CreateHypertension()
    {
        return new Condition
        {
            Id = "condition-001",

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-clinical",
                "active",
                "Active"
            ),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                "confirmed",
                "Confirmed"
            ),

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "http://snomed.info/sct",
                        Code = "38341003",
                        Display = "Hypertensive disorder"
                    }
                },
                Text = "Hypertension"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Onset = new FhirDateTime("2020")
        };
    }

    public static Condition CreateDiabetes()
    {
        return new Condition
        {
            Id = "condition-002",

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-clinical",
                "active",
                "Active"
            ),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                "confirmed",
                "Confirmed"
            ),

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "44054006",
                            Display = "Diabetes mellitus type 2"
                        }
                    },
                Text = "Type 2 diabetes mellitus"
            },
            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Onset = new FhirDateTime("2018")
        };
    }

}
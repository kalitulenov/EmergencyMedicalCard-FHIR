
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class AllergyFactory
{
    public static AllergyIntolerance CreatePenicillinAllergy()
    {
        return new AllergyIntolerance
        {
            Id = "allergy-001",

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical",
                "active",
                "Active"
            ),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-verification",
                "confirmed",
                "Confirmed"
            ),

            Type = AllergyIntolerance.AllergyIntoleranceType.Allergy,

            Category = new List<AllergyIntolerance
                .AllergyIntoleranceCategory?>
            {
                AllergyIntolerance
                    .AllergyIntoleranceCategory.Medication
            },

            Criticality =
                AllergyIntolerance.AllergyIntoleranceCriticality.High,

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "764146007",
                            Display = "Penicillin"
                        }
                    },

                Text = "Penicillin"
            },

            Patient = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Reaction = new List<AllergyIntolerance.ReactionComponent>
            {
                new AllergyIntolerance.ReactionComponent
                {
                    Manifestation = new List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Text = "Hives"
                        }
                    },

                    Severity = AllergyIntolerance
                        .AllergyIntoleranceSeverity.Moderate
                }
            }
        };
    }


    public static AllergyIntolerance CreatePeanutAllergy()
    {
        return new AllergyIntolerance
        {
            Id = "allergy-002",

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical",
                "active",
                "Active"
            ),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-verification",
                "confirmed",
                "Confirmed"
            ),

            Type = AllergyIntolerance.AllergyIntoleranceType.Allergy,

            Category = new List<AllergyIntolerance
                .AllergyIntoleranceCategory?>
            {
                AllergyIntolerance
                    .AllergyIntoleranceCategory.Medication
            },

            Criticality =
                AllergyIntolerance.AllergyIntoleranceCriticality.High,

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "762952008",
                            Display = "Peanut"
                        }
                    },

                Text = "Peanut"
            },
            Patient = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Reaction = new List<AllergyIntolerance.ReactionComponent>
            {
                new AllergyIntolerance.ReactionComponent
                {
                    Manifestation = new List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Text = "Hives"
                        }
                    },

                    Severity = AllergyIntolerance
                        .AllergyIntoleranceSeverity.Moderate
                }
            }
        };
    }

}

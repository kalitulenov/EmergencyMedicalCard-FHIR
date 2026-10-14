
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class AllergyFactory
{
    public static AllergyIntolerance CreatePenicillinAllergy()
    {
        return new AllergyIntolerance
        {
            Id = "allergy-001",

            Meta = new Meta
            {
                Profile = new[]
                    {
                        "http://hl7.org/fhir/us/core/StructureDefinition/" +
                        "us-core-allergyintolerance"
                    }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Penicillin allergy</b></p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "<p>Criticality: High</p>" +
                    "<p>Reaction: Hives</p>" +
                    "<p>Reaction severity: Moderate</p>" +
                    "</div>"
            },

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
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "126485001",
                                    Display = "Urticaria"
                                }
                            },
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

            Meta = new Meta
            {
                Profile = new[]
                    {
                        "http://hl7.org/fhir/us/core/StructureDefinition/" +
                        "us-core-allergyintolerance"
                    }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Peanut allergy</b></p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "</div>"
            },

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
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Code = "126485001",
                                    Display = "Urticaria"
                                }
                            },
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

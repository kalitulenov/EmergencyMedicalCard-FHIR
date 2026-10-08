using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ConditionFactory
{
    public static Condition CreateHypertension()
    {
        return new Condition
        {
            Id = "condition-001",

            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/" +
                    "us-core-condition-problems-health-concerns"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Hypertension</b></p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "<p>Onset: 2020</p>" +
                    "</div>"
            },

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

            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-category",

                            Code = "problem-list-item",

                            Display = "Problem List Item"
                        }
                    }
                }
            },

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "38341003",
                            Display = "Hypertensive disorder"
                        },

                        new Coding
                        {
                            System =
                                "http://hl7.org/fhir/sid/icd-10-cm",

                            Code = "I10",
                            Display =
                                "Essential (primary) hypertension"
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

            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/" +
                    "us-core-condition-problems-health-concerns"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Type 2 diabetes mellitus</b></p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "<p>Onset: 2018</p>" +
                    "</div>"
            },

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

            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-category",

                            Code = "problem-list-item",

                            Display = "Problem List Item"
                        }
                    }
                }
            },

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "44054006",
                            Display = "Diabetes mellitus type 2"
                        },
                      
                        new Coding
                        {
                            System = "http://hl7.org/fhir/sid/icd-10-cm",
                            Code = "E11.9",
                            Display = "Type 2 diabetes mellitus without complications"
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
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ObservationFactory
{
    public static Observation CreateBloodGroup()
    {
        return new Observation
        {
            Id = "observation-blood-group-001",

            Meta = new Meta
            {
                Profile = new[]
                    {
                        "http://hl7.org/fhir/us/core/StructureDefinition/us-core-observation-lab"
                    }
            },

            Status = ObservationStatus.Final,

            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://terminology.hl7.org/CodeSystem/observation-category",
                            Code = "laboratory",
                            Display = "Laboratory"
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
                        System = "http://loinc.org",
                        Code = "883-9",
                        Display = "ABO group [Type] in Blood"
                    }
                },

                Text = "ABO blood group"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Value = new CodeableConcept
            {
                Text = "O"
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
            "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
            "<p><b>ABO blood group: O</b></p>" +
            "<p>Status: Final</p>" +
            "</div>"
            },

            Effective = new FhirDateTime("2026-05-15"),

            Performer = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Reference = "Organization/lab-001",
                    Display = "Example Clinical Laboratory"
                }
            }
        };
    }

    public static Observation CreateRhFactor()
    {
        return new Observation
        {
            Id = "observation-rh-factor-001",

            Meta = new Meta
            {
                Profile = new[]
                    {
                        "http://hl7.org/fhir/us/core/StructureDefinition/us-core-observation-lab"
                    }
            },

            Status = ObservationStatus.Final,

            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://terminology.hl7.org/CodeSystem/observation-category",
                            Code = "laboratory",
                            Display = "Laboratory"
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
                        System = "http://loinc.org",
                        Code = "10331-7",
                        Display = "Rh [Type] in Blood"
                    }
                }
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Effective = new FhirDateTime("2026-05-15"),

            Performer = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Reference = "Organization/lab-001",
                    Display = "Example Clinical Laboratory"
                }
            },

            Value = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "LA6576-8",
                            Display = "Positive"
                        }
                    },
                Text = "Positive"
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Rh factor: Positive</b></p>" +
                    "<p>Status: Final</p>" +
                    "<p>Effective date: 2026-05-15</p>" +
                    "</div>"
            },
        };
    }


}
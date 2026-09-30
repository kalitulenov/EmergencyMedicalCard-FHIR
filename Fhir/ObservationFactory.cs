using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ObservationFactory
{
    public static Observation CreateBloodGroup()
    {
        return new Observation
        {
            Id = "observation-blood-group-001",

            Status = ObservationStatus.Final,

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
            }
        };
    }

    public static Observation CreateRhFactor()
    {
        return new Observation
        {
            Id = "observation-rh-factor-001",

            Status = ObservationStatus.Final,

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
            },

                Text = "Rh factor"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
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
            }
        };
    }


}
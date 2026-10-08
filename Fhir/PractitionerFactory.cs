using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class PractitionerFactory
{
    public static Practitioner CreateDrBrown()
    {
        return new Practitioner
        {
            Id = "practitioner-001",

            Active = true,

            Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        System =
                            "https://hospital.example.org/practitioner-id",

                        Value = "DOC-001"
                    }
                },

            Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Use = HumanName.NameUse.Official,
                        Family = "Brown",
                        Given = new[] { "Robert" },
                        Prefix = new[] { "Dr." }
                    }
                }
        };
    }
}
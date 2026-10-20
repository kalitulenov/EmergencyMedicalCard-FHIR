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

            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/us-core-practitioner"
                }
            },

            Identifier = new List<Identifier>
            {
                // Local hospital identifier
                new Identifier
                {
                    System = "urn:oid:2.16.840.1.113883.19.999.2",
                    Value = "DOC-001"
                },

                // Demonstration NPI — not a real provider identity
                new Identifier
                {
                    System = "http://hl7.org/fhir/sid/us-npi",
                    Value = "1234567893"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Dr. Robert Brown</b></p>" +
                    "</div>"
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
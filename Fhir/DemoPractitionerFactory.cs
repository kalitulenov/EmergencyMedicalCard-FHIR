using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoPractitionerFactory
{
    public static Practitioner Create()
    {
        return new Practitioner
        {
            Id = "demo-doctor-001",

            Active = true,

            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = "Johnson",
                    Given = new[] { "Michael" },
                    Prefix = new[] { "Dr." }
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p>Dr. Michael Johnson</p>" +
                    "<p>Demo Physician</p>" +
                    "</div>"
            }
        };
    }
}
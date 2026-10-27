using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoPatientFactory
{
    public static Patient Create()
    {
        return new Patient
        {
            Id = "demo-patient-001",

            Active = true,

            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = "Smith",
                    Given = new[] { "Robert" }
                }
            },

            Gender = AdministrativeGender.Male,

            BirthDate = "1960-05-15",

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p>Robert Smith</p>" +
                    "<p>Male, born 1960-05-15</p>" +
                    "<p>Demonstration patient - synthetic data</p>" +
                    "</div>"
            }
        };
    }
}
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class OrganizationFactory
{
    public static Organization CreateLaboratory()
    {
        return new Organization
        {
            Id = "lab-001",

            Active = true,

            Name = "Example Clinical Laboratory",

            Type = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/organization-type",
                            Code = "dept",
                            Display = "Hospital Department"
                        }
                    },
                    Text = "Clinical Laboratory"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Example Clinical Laboratory</b></p>" +
                    "<p>Active: Yes</p>" +
                    "</div>"
            }
        };
    }
}
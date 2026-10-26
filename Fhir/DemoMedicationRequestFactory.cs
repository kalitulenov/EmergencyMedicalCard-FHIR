using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoMedicationRequestFactory
{
    public static MedicationRequest Create(string patientId)
    {
        return new MedicationRequest
        {
            Id = "demo-medication-001",

            Status = MedicationRequest.MedicationrequestStatus.Active,

            Intent = MedicationRequest.MedicationRequestIntent.Order,

            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://www.nlm.nih.gov/research/umls/rxnorm",
                            //Version = "03022026",
                            Code = "314076",
                            Display = "Lisinopril 10 MG Oral Tablet"
                        }
                    },
                Text = "Lisinopril 10 MG Oral Tablet"
            },

            Subject = new ResourceReference(
                $"Patient/{patientId}"),

            Requester = new ResourceReference
            {
                Reference = "Practitioner/demo-doctor-001",
                Display = "Demo Physician"
            },

            AuthoredOn = "2026-10-01",

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p>Lisinopril 10 mg oral tablet</p>" +
                    "<p>Status: Active</p>" +
                    "<p>Intent: Order</p>" +
                    "</div>"
            }
        };
    }
}
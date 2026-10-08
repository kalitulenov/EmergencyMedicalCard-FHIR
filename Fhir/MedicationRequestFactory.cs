using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class MedicationRequestFactory
{
    public static MedicationRequest CreateLisinoprilRequest()
    {
        return new MedicationRequest
        {
            Id = "medicationrequest-001",

            Status = MedicationRequest.MedicationrequestStatus.Active,

            Intent = MedicationRequest.MedicationRequestIntent.Order,

            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System =
                            "http://www.nlm.nih.gov/research/umls/rxnorm",

                        Code = "314076",

                        Display =
                            "lisinopril 10 MG Oral Tablet"
                    }
                },

                Text = "Lisinopril 10 mg"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Requester = new ResourceReference
            {
                Reference = "Practitioner/practitioner-001",
                Display = "Dr. Robert Brown"
            },

            AuthoredOn = "2026-06-10",

            DosageInstruction = new List<Dosage>
            {
                new Dosage
                {
                    Text = "Take 10 mg by mouth once daily"
                }
            }
        };
    }
}
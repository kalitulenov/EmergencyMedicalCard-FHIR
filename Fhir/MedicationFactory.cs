using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class MedicationFactory
{
    public static MedicationStatement CreateLisinopril()
    {
        return new MedicationStatement
        {
            Id = "medicationstatement-001",

            Status =
                MedicationStatement.MedicationStatusCodes.Active,

            Medication = new CodeableConcept
            {
                Text = "Lisinopril 10 mg"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Dosage = new List<Dosage>
            {
                new Dosage
                {
                    Text = "10 mg once daily by mouth"
                }
            }
        };
    }

    public static MedicationStatement CreateAtorvastatin()
    {
        return new MedicationStatement
        {
            Id = "medicationstatement-002",

            Status =
                MedicationStatement.MedicationStatusCodes.Active,

            Medication = new CodeableConcept
            {
                Text = "Atorvastatin 20 mg"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Dosage = new List<Dosage>
            {
                new Dosage
                {
                    Text = "once daily"
                }
            }
        };
    }

}
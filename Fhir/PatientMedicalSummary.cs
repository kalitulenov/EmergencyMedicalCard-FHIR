using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public class PatientMedicalSummary
{
    public string PatientId { get; set; } = "";

    public List<Condition> Conditions { get; set; } = new();

    public List<AllergyIntolerance> Allergies { get; set; } = new();

    public List<MedicationRequest> Medications { get; set; } = new();

    public List<Observation> Observations { get; set; } = new();
}
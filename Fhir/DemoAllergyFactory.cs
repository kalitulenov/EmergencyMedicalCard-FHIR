using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoAllergyFactory
{
    public static AllergyIntolerance Create(
        string patientId)
    {
        return new AllergyIntolerance
        {
            Id = "demo-allergy-001",

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                      "<p>Allergy to penicillin</p>" +
                      "<p>Clinical status: Active</p>" +
                      "<p>Verification status: Confirmed</p>" +
                      "<p>Criticality: High</p>" +
                      "</div>"
            },

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical",
                "active"),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/allergyintolerance-verification",
                "confirmed"),

            Type = AllergyIntolerance.AllergyIntoleranceType.Allergy,

            Category = new AllergyIntolerance.AllergyIntoleranceCategory?[]
            {
                AllergyIntolerance.AllergyIntoleranceCategory.Medication
            },

            Criticality =
                AllergyIntolerance.AllergyIntoleranceCriticality.High,

            Code = new CodeableConcept(
                "http://snomed.info/sct",
                "91936005",
                "Allergy to penicillin"),

            Patient = new ResourceReference(
                $"Patient/{patientId}")
        };
    }
}
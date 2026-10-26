using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class DemoConditionFactory
{
    public static Condition Create(string patientId)
    {
        return new Condition
        {
            Id = "demo-condition-001",

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-clinical",
                "active"),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                "confirmed"),

            Category = new List<CodeableConcept>
            {
                new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/condition-category",
                    "problem-list-item",
                    "Problem List Item")
            },

            Code = new CodeableConcept(
                "http://snomed.info/sct",
                "59621000",
                "Essential hypertension"),

            Subject = new ResourceReference(
                $"Patient/{patientId}"),

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p>Essential hypertension</p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "</div>"
            }
        };
    }
}
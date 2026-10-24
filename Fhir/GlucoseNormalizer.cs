using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class GlucoseNormalizer
{
    public static Observation Normalize(Observation source)
    {
        var result = (Observation)source.DeepCopy();

        var coding = result.Code?.Coding?
            .FirstOrDefault(c =>
                c.System == "http://loinc.org" &&
                c.Code == "2339-0");

        if (coding == null)
        {
            throw new ArgumentException(
                "Observation is not blood glucose LOINC 2339-0.");
        }

        coding.Display = "Glucose [Mass/volume] in Blood";

        result.Category ??= new List<CodeableConcept>();

        if (!result.Category.Any(category =>
            category.Coding.Any(c =>
                c.System ==
                "http://terminology.hl7.org/CodeSystem/observation-category"
                && c.Code == "laboratory")))
        {
            result.Category.Add(new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/observation-category",
                "laboratory",
                "Laboratory"));
        }

        return result;
    }
}
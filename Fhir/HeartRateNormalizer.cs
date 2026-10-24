using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class HeartRateNormalizer
{
    public static Observation Normalize(Observation source)
    {
        var result = (Observation)source.DeepCopy();

        if (result.Code?.Coding?.Any(c =>
            c.System == "http://loinc.org" &&
            c.Code == "8867-4") != true)
        {
            throw new ArgumentException(
                "Observation is not Heart Rate.");
        }

        if (result.Value is not Quantity quantity ||
            quantity.Value == null)
        {
            throw new ArgumentException(
                "Heart Rate must contain a Quantity.");
        }

        if (quantity.Unit != "beats/min" &&
            !(quantity.System == "http://unitsofmeasure.org"
              && quantity.Code == "/min"))
        {
            throw new ArgumentException(
                "Unexpected Heart Rate unit.");
        }

        result.Category ??= new List<CodeableConcept>();

        if (!result.Category.Any(c =>
            c.Coding.Any(x =>
                x.System ==
                "http://terminology.hl7.org/CodeSystem/observation-category"
                && x.Code == "vital-signs")))
        {
            result.Category.Add(new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/observation-category",
                "vital-signs",
                "Vital Signs"));
        }

        quantity.Unit = "beats/min";
        quantity.System = "http://unitsofmeasure.org";
        quantity.Code = "/min";

        return result;
    }
}
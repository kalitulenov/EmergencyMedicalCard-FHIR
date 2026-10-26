using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class FhirObservationNormalizer
{
    public static List<Observation> Normalize(
        IEnumerable<Observation> observations,
        string patientUrl)
    {
        var source = observations.ToList();
        var result = new List<Observation>();

        var systolic = source.FirstOrDefault(o =>
            HasLoinc(o, "8480-6"));

        var diastolic = source.FirstOrDefault(o =>
            HasLoinc(o, "8462-4"));

        Observation? normalizedBp = null;

        if (systolic != null && diastolic != null)
        {
            normalizedBp = BloodPressureFactory.Create(
                systolic, diastolic);

            normalizedBp.Subject =
                new ResourceReference(patientUrl);
        }

        foreach (var observation in source)
        {
            if (normalizedBp != null &&
                (ReferenceEquals(observation, systolic) ||
                 ReferenceEquals(observation, diastolic)))
            {
                continue;
            }

            if (HasLoinc(observation, "8867-4"))
            {
                result.Add(
                    HeartRateNormalizer.Normalize(observation));
            }
            else if (HasLoinc(observation, "2339-0"))
            {
                result.Add(
                    GlucoseNormalizer.Normalize(observation));
            }
            else
            {
                result.Add(
                    (Observation)observation.DeepCopy());
            }
        }

        if (normalizedBp != null)
        {
            result.Add(normalizedBp);
        }

        return result;
    }

    private static bool HasLoinc(
        Observation observation,
        string code)
    {
        return observation.Code?.Coding?.Any(c =>
            c.System == "http://loinc.org" &&
            c.Code == code) == true;
    }
}
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class EmergencyCardFilter
{
    public static List<Observation> SelectObservations(
        IEnumerable<Observation> observations)
    {
        var importantCodes = new HashSet<string>
        {
            "85354-9", // Blood Pressure
            "8867-4",  // Heart Rate
            "2339-0"   // Glucose
        };

        return observations
            .Where(o =>
                o.Code?.Coding?.Any(c =>
                    c.System == "http://loinc.org" &&
                    c.Code != null &&
                    importantCodes.Contains(c.Code)) == true)
            .Select(o => (Observation)o.DeepCopy())
            .ToList();
    }
}
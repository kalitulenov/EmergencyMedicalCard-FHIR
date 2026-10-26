using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ObservationClassifier
{
    public static bool IsTestObservation(
        Observation observation)
    {
        if (observation.Value is FhirString value)
        {
            return value.Value?.StartsWith(
                "TEST_",
                StringComparison.OrdinalIgnoreCase) == true;
        }

        return false;
    }
}
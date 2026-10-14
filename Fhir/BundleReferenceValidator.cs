using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class BundleReferenceValidator
{
    public static List<string> Validate(Bundle bundle)
    {
        var errors = new List<string>();

        var resourceUrls = bundle.Entry
            .Where(e => !string.IsNullOrWhiteSpace(e.FullUrl))
            .Select(e => e.FullUrl!)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var entry in bundle.Entry)
        {
            if (entry.Resource == null)
                continue;

            var source = entry.FullUrl ?? "Unknown";

            foreach (var reference in GetReferences(entry.Resource))
            {
                if (string.IsNullOrWhiteSpace(reference.Reference))
                    continue;

                var target = reference.Reference;

                if (target.StartsWith("#"))
                    continue;

                // Преобразуем относительную ссылку
                // в абсолютную относительно fullUrl ресурса.
                if (!Uri.TryCreate(
                    target,
                    UriKind.Absolute,
                    out _))
                {
                    if (!Uri.TryCreate(
                        source,
                        UriKind.Absolute,
                        out var sourceUri))
                    {
                        errors.Add(
                            $"Cannot resolve: {source} -> {target}");
                        continue;
                    }

                    target = new Uri(sourceUri, target).ToString();
                }

                if (!resourceUrls.Contains(target))
                {
                    errors.Add(
                        $"Broken reference: {source} -> {target}");
                }
            }
        }

        return errors;
    }

    private static IEnumerable<ResourceReference> GetReferences(
        Resource resource)
    {
        switch (resource)
        {
            case AllergyIntolerance allergy:
                if (allergy.Patient != null)
                    yield return allergy.Patient;
                if (allergy.Recorder != null)
                    yield return allergy.Recorder;
                if (allergy.Asserter != null)
                    yield return allergy.Asserter;
                break;

            case Condition condition:
                if (condition.Subject != null)
                    yield return condition.Subject;
                if (condition.Recorder != null)
                    yield return condition.Recorder;
                if (condition.Asserter != null)
                    yield return condition.Asserter;
                break;

            case MedicationStatement medication:
                if (medication.Subject != null)
                    yield return medication.Subject;
                if (medication.InformationSource != null)
                    yield return medication.InformationSource;
                break;

            case MedicationRequest request:
                if (request.Subject != null)
                    yield return request.Subject;
                if (request.Requester != null)
                    yield return request.Requester;

                if (request.ReasonReference != null)
                {
                    foreach (var reason in request.ReasonReference)
                        yield return reason;
                }
                break;

            case Observation observation:
                if (observation.Subject != null)
                    yield return observation.Subject;

                if (observation.Performer != null)
                {
                    foreach (var performer in observation.Performer)
                        yield return performer;
                }
                break;
        }
    }
}
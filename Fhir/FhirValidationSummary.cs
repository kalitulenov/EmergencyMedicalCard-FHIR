namespace EmergencyMedicalCard.Fhir;

public class FhirValidationSummary
{
    public int TotalResources { get; set; }
    public int SuccessfulReferences { get; set; }
    public int Errors { get; set; }
    public int Skipped { get; set; }

    public bool Success =>
        TotalResources > 0 &&
        LoadedResources == TotalResources &&
        FailedResources == 0 &&
        Errors == 0 &&
        Skipped == 0; public int LoadedResources { get; set; }

    public int FailedResources { get; set; }
}
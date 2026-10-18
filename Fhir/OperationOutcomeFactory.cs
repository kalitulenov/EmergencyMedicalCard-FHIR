using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class OperationOutcomeFactory
{
    public static OperationOutcome NotFound(
        string resourceType,
        string id)
    {
        return new OperationOutcome
        {
            Issue = new List<OperationOutcome.IssueComponent>
            {
                new OperationOutcome.IssueComponent
                {
                    Severity = OperationOutcome.IssueSeverity.Error,
                    Code = OperationOutcome.IssueType.NotFound,
                    Diagnostics = $"{resourceType}/{id} was not found."
                }
            }
        };
    }

    public static OperationOutcome BadRequest(string message)
    {
        return new OperationOutcome
        {
            Issue = new List<OperationOutcome.IssueComponent>
        {
            new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = OperationOutcome.IssueType.Required,
                Diagnostics = message
            }
        }
        };
    }
}
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class MedicationRequestFactory
{
    public static MedicationRequest CreateLisinoprilRequest()
    {
        return new MedicationRequest
        {
            Id = "medicationrequest-001",

            Meta = new Meta
            {
                Profile = new[]
                    {
                        "http://hl7.org/fhir/us/core/StructureDefinition/us-core-medicationrequest"
                    }
            },

            DispenseRequest = new MedicationRequest.DispenseRequestComponent
            {
                Quantity = new Quantity
                {
                    Value = 30,
                    Unit = "tablets"
                },

                NumberOfRepeatsAllowed = 2,

                ExpectedSupplyDuration = new Duration
                {
                    Value = 30,
                    Unit = "days",
                    System = "http://unitsofmeasure.org",
                    Code = "d"
                }
            },

            ReasonReference = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Reference = "Condition/condition-001",
                    Display = "Hypertension"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Medication order: Lisinopril 10 mg oral tablet</b></p>" +
                    "<p>Status: Active</p>" +
                    "<p>Intent: Order</p>" +
                    "<p>Patient: John Smith</p>" +
                    "<p>Prescriber: Dr. Robert Brown</p>" +
                    "<p>Authored: 2026-06-10</p>" +
                    "</div>"
            },

            Status = MedicationRequest.MedicationrequestStatus.Active,

            Intent = MedicationRequest.MedicationRequestIntent.Order,

            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System =
                            "http://www.nlm.nih.gov/research/umls/rxnorm",

                        Code = "314076",

                        Display =
                            "lisinopril 10 MG Oral Tablet"
                    }
                },

                Text = "Lisinopril 10 mg"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Requester = new ResourceReference
            {
                Reference = "Practitioner/practitioner-001",
                Display = "Dr. Robert Brown"
            },

            AuthoredOn = "2026-06-10",

            DosageInstruction = new List<Dosage>
            {
                new Dosage
                {
                    Text = "Take 10 mg by mouth once daily",

                    Timing = new Timing
                    {
                        Repeat = new Timing.RepeatComponent
                        {
                            Frequency = 1,
                            Period = 1,
                            PeriodUnit = Timing.UnitsOfTime.D
                        }
                    },

                    Route = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://snomed.info/sct",
                                Code = "26643006",
                                Display = "Oral route"
                            }
                        },
                        Text = "By mouth"
                    },

                    DoseAndRate = new List<Dosage.DoseAndRateComponent>
                    {
                        new Dosage.DoseAndRateComponent
                        {
                            Dose = new Quantity
                            {
                                Value = 10,
                                Unit = "mg",
                                System = "http://unitsofmeasure.org",
                                Code = "mg"
                            }
                        }
                    }
                }
            }


        };
    }
}
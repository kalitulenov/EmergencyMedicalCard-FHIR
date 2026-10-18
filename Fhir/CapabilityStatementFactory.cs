using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class CapabilityStatementFactory
{
    public static CapabilityStatement Create(string baseUrl)
    {
        return new CapabilityStatement
        {
            Id = "emergency-medical-card-capabilities",

            Status = PublicationStatus.Active,

            Date = "2026-10-15",

            Kind = CapabilityStatementKind.Instance,

            FhirVersion = FHIRVersion.N4_0_1,

            Format = new[] { "json" },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>EmergencyMedicalCard FHIR R4 Server</b></p>" +
                    "<p>Supported resources: Patient, Observation, " +
                    "Organization, AllergyIntolerance, Condition, " +
                    "MedicationStatement, MedicationRequest and Practitioner.</p>" +
                    "<p>Supported operations: read and patient-based searches.</p>" +
                    "</div>"
            },
            Software = new CapabilityStatement.SoftwareComponent
            {
                Name = "EmergencyMedicalCard",
                Version = "1.0.0"
            },

            Implementation = new CapabilityStatement.ImplementationComponent
            {
                Description = "Educational FHIR R4 REST API",
                Url = baseUrl
            },

            Rest = new List<CapabilityStatement.RestComponent>
            {
                new CapabilityStatement.RestComponent
                {
                    Mode = CapabilityStatement.RestfulCapabilityMode.Server,

                    Resource = new List<CapabilityStatement.ResourceComponent>
                    {
                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "Patient",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code =
                                        CapabilityStatement.TypeRestfulInteraction.Read
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "Observation",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code =
                                        CapabilityStatement.TypeRestfulInteraction.Read
                                },

                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code =
                                        CapabilityStatement.TypeRestfulInteraction.SearchType
                                }
                            },

                            SearchParam = new List<
                                CapabilityStatement.SearchParamComponent>
                            {
                                new CapabilityStatement.SearchParamComponent
                                {
                                    Name = "patient",
                                    Type = SearchParamType.Reference,
                                    Documentation =
                                        "Search observations by patient"
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "Organization",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "AllergyIntolerance",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                },
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.SearchType
                                }
                            },

                            SearchParam = new List<
                                CapabilityStatement.SearchParamComponent>
                            {
                                new CapabilityStatement.SearchParamComponent
                                {
                                    Name = "patient",
                                    Type = SearchParamType.Reference,
                                    Documentation = "Search allergies by patient"
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "Condition",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                },
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.SearchType
                                }
                            },

                            SearchParam = new List<
                                CapabilityStatement.SearchParamComponent>
                            {
                                new CapabilityStatement.SearchParamComponent
                                {
                                    Name = "patient",
                                    Type = SearchParamType.Reference,
                                    Documentation = "Search conditions by patient"
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "MedicationStatement",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                },
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.SearchType
                                }
                            },

                            SearchParam = new List<
                                CapabilityStatement.SearchParamComponent>
                            {
                                new CapabilityStatement.SearchParamComponent
                                {
                                    Name = "patient",
                                    Type = SearchParamType.Reference,
                                    Documentation = "Search medication statements by patient"
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "MedicationRequest",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                },
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.SearchType
                                }
                            },

                            SearchParam = new List<
                                CapabilityStatement.SearchParamComponent>
                            {
                                new CapabilityStatement.SearchParamComponent
                                {
                                    Name = "patient",
                                    Type = SearchParamType.Reference,
                                    Documentation = "Search medication requests by patient"
                                }
                            }
                        },

                        new CapabilityStatement.ResourceComponent
                        {
                            Type = "Practitioner",

                            Interaction = new List<
                                CapabilityStatement.ResourceInteractionComponent>
                            {
                                new CapabilityStatement.ResourceInteractionComponent
                                {
                                    Code = CapabilityStatement.TypeRestfulInteraction.Read
                                }
                            }
                        }



                    }
                }
            }
        };
    }
}
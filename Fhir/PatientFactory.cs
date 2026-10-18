
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class PatientFactory
{
    public static Patient CreatePatient()
    {
        return new Patient
        {
            Id = "patient-001",

            // -------------- для соответствия профилю US Core Patient Profile ----------------
            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient"
                }
            },
            // -------------- для соответствия профилю US Core Patient Profile ----------------

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<p><b>John Smith</b></p>" +
                "<p>Male, born 1955-04-12</p>" +
                "<p>Medical Record Number: MRN-2026-001</p>" +
                "</div>"
            },

            Active = true,

            Identifier = new List<Identifier>
            {
                new Identifier
                {
                    Use = Identifier.IdentifierUse.Usual,

                    Type = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System =
                                    "http://terminology.hl7.org/CodeSystem/v2-0203",
                                Code = "MR",
                                Display = "Medical record number"
                            }
                        }
                    },
           //         System = "https:hospital.example.org/mrn",
                    System = "urn:oid:2.16.840.1.113883.19.999.1",
                    Value = "MRN-2026-001"
                }
            },

            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = "Smith",
                    Given = new[] { "John" }
                }
            },

            Gender = AdministrativeGender.Male,

            BirthDate = "1955-04-12",

            Telecom = new List<ContactPoint>
            {
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Phone,
                    Value = "+1-202-555-0142",
                    Use = ContactPoint.ContactPointUse.Mobile
                },

                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Email,
                    Value = "john.smith@example.com"
                }
            },

            Address = new List<Address>
            {
                new Address
                {
                    Use = Address.AddressUse.Home,
                    Type = Address.AddressType.Physical,
                    Line = new[] { "100 Example Street" },
                    City = "Washington",
                    State = "DC",
                    PostalCode = "20001",
                    Country = "USA"
                }
            },

            Contact = new List<Patient.ContactComponent>
            {
                new Patient.ContactComponent
                {
                    Relationship = new List<CodeableConcept>
                        {
                            new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        System =
                                            "http://terminology.hl7.org/CodeSystem/v2-0131",
                                        Code = "C",
                                        Display = "Emergency Contact"
                                    }
                                },
                                Text = "Daughter"
                            }
                        },

                    Name = new HumanName
                    {
                        Family = "Smith",
                        Given = new[] { "Mary" }
                    },

                    Telecom = new List<ContactPoint>
                    {
                        new ContactPoint
                        {
                            System =
                                ContactPoint.ContactPointSystem.Phone,
                            Value = "+1-202-555-0175"
                        }
                    }
                }
            }
        };
    }
}
using Hl7.Fhir.Model;

namespace EmergencyMedicalCard.Fhir;

public static class ConditionFactory
{
    // Объявление публичного статического метода, который возвращает объект типа Condition (Диагноз)
    public static Condition CreateHypertension()
    {
        // Создается и инициализируется новый экземпляр класса Condition
        return new Condition
        {
            // Присваивается уникальный текстовый идентификатор для этой записи диагноза
            Id = "condition-001",

            // Инициализация блока метаданных, описывающих технические параметры ресурса

            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/us-core-condition-problems-health-concerns"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,
                Div =
                "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<p>Hypertension</p>" +
                "</div>"
            },

            // Установка клинического статуса болезни с помощью кодируемого концепта (CodeableConcept)

            ClinicalStatus = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            Code = "active",
                            Display = "Active"
                        }
                    }
            },

            // Установка статуса достоверности диагноза
            VerificationStatus = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                            Code = "confirmed",
                            Display = "Confirmed"
                        }
                    }
            },

            // Инициализация списка категорий, к которым относится данный диагноз
            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-category",
                            Code = "problem-list-item",
                            Display = "Problem List Item"
                        }
                    }
                }
            },

            // Инициализация кодирования самого медицинского диагноза (что за болезнь?)
            Code = new CodeableConcept
            {
                // Заполнение списка кодов из разных международных медицинских справочников
                Coding = new List<Coding>
                {
                    // Первый код — из международной системы клинических терминов SNOMED CT
                    new Coding
                    {
                        // Ссылка на терминологическую систему SNOMED CT
                        System = "http://snomed.info/sct",
                        // Уникальный код гипертонической болезни в этой системе
                        Code = "38341003",
                        // Официальное медицинское наименование понятия в SNOMED CT
                        Display = "Hypertensive disorder"
                    },

                    // Второй код — из американской модификации МКБ-10 (ICD-10-CM)
                    new Coding
                    {
                        // Ссылка на справочник кодов МКБ-10
                        System = "http://hl7.org/fhir/sid/icd-10-cm",

                        // Код эссенциальной (первичной) гипертензии по МКБ-10
                        Code = "I10",
                        // Официальное наименование диагноза по классификатору МКБ
                        Display =
                            "Essential (primary) hypertension"
                    }
                },

                // Обычное текстовое название диагноза, которое будет приоритетным для отображения
                Text = "Hypertension"
            },

            // Заполнение информации о субъекте (пациенте), которому ставится этот диагноз
            Subject = new ResourceReference
            {
                // Относительная ссылка на FHIR-ресурс конкретного пациента в базе данных
                Reference = "Patient/patient-001",
                // Имя пациента, чтобы не делать лишний запрос в базу для простого отображения
                Display = "John Smith"
            },

            // Установка даты или времени начала заболевания с помощью специального FHIR-типа данных
            Onset = new FhirDateTime("2020")
        }; // Завершение инициализации объекта и закрытие блока return
    } // Конец метода CreateHypertension

    public static Condition CreateDiabetes()
    {
        return new Condition
        {
            Id = "condition-002",

            Meta = new Meta
            {
                Profile = new[]
                {
                    "http://hl7.org/fhir/us/core/StructureDefinition/" +
                    "us-core-condition-problems-health-concerns"
                }
            },

            Text = new Narrative
            {
                Status = Narrative.NarrativeStatus.Generated,

                Div =
                    "<div xmlns=\"http://www.w3.org/1999/xhtml\">" +
                    "<p><b>Type 2 diabetes mellitus</b></p>" +
                    "<p>Clinical status: Active</p>" +
                    "<p>Verification status: Confirmed</p>" +
                    "<p>Onset: 2018</p>" +
                    "</div>"
            },

            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-clinical",
                "active",
                "Active"
            ),

            VerificationStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                "confirmed",
                "Confirmed"
            ),

            Category = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System =
                                "http://terminology.hl7.org/CodeSystem/condition-category",

                            Code = "problem-list-item",

                            Display = "Problem List Item"
                        }
                    }
                }
            },

            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://snomed.info/sct",
                            Code = "44054006",
                            Display = "Diabetes mellitus type 2"
                        },
                      
                        new Coding
                        {
                            System = "http://hl7.org/fhir/sid/icd-10-cm",
                            Code = "E11.9",
                            Display = "Type 2 diabetes mellitus without complications"
                        }
                    },
                Text = "Type 2 diabetes mellitus"
            },

            Subject = new ResourceReference
            {
                Reference = "Patient/patient-001",
                Display = "John Smith"
            },

            Onset = new FhirDateTime("2018")
        };
    }

}
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MedicalCenter.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class A_Worker
    {
        public A_Worker()
        {
            this.Gender = false;
            this.JobTitle = 0;
            this.Specialization = 0;
            this.A_Absences = new HashSet<A_Absence>();
            this.A_Users = new HashSet<A_User>();
            this.M_Visits_RegistrarId = new HashSet<M_Visit>();
            this.M_Visits_DoctorId = new HashSet<M_Visit>();
            this.M_MedicalTreatments_DoctorId = new HashSet<M_MedicalTreatment>();
            this.M_MedicalTreatments_DoerId = new HashSet<M_MedicalTreatment>();
            this.A_WorkersRooms = new HashSet<A_WorkersRoom>();
            this.A_Schedules = new HashSet<A_Schedule>();
            this.M_MedicalTreatment_VerifierId = new HashSet<M_MedicalTreatment>();
            this.M_MedicalTreatment_EditorId = new HashSet<M_MedicalTreatment>();
        }
    
        public int Id { get; private set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public System.DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public long Pesel { get; set; }
        public int JobTitle { get; set; }
        public int Specialization { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string Apartment { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Post { get; set; }
    
        public virtual ICollection<A_Absence> A_Absences { get; set; }
        public virtual ICollection<A_User> A_Users { get; set; }
        public virtual A_DictionaryJobTitle A_DictionaryJobTitle { get; set; }
        public virtual A_DictionarySpecialization A_DictionarySpecialization { get; set; }
        public virtual ICollection<M_Visit> M_Visits_RegistrarId { get; set; }
        public virtual ICollection<M_Visit> M_Visits_DoctorId { get; set; }
        public virtual ICollection<M_MedicalTreatment> M_MedicalTreatments_DoctorId { get; set; }
        public virtual ICollection<M_MedicalTreatment> M_MedicalTreatments_DoerId { get; set; }
        public virtual ICollection<A_WorkersRoom> A_WorkersRooms { get; set; }
        public virtual ICollection<A_Schedule> A_Schedules { get; set; }
        public virtual ICollection<M_MedicalTreatment> M_MedicalTreatment_VerifierId { get; set; }
        public virtual ICollection<M_MedicalTreatment> M_MedicalTreatment_EditorId { get; set; }
    }
}

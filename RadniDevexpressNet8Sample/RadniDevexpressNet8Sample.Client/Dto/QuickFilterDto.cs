using Common.DataAccess.Filtering;

namespace CommonBlazor.DynamicData.Abstractions
{
    public class QuickFilterDto
    {
        public int Id { get; set; }

        public bool IsNew => Id == 0;

        public string Key { get; set; } //Invoice
        public string TableName { get; set; } // tblSklIzlazniRacunStavka
        public string DisplayName { get; set; } // Customer
        public string Property { get; set; } // Kupac.Naziv
        public FunctionCriteriaType Operator { get; set; } // Contains
    }
}
